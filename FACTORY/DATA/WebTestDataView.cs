using Dooggy;
using Dooggy.Factory;
using Dooggy.Factory.Data;
using Dooggy.Lib.Data;
using Dooggy.Lib.Generic;
using Dooggy.Lib.Parse;
using Dooggy.Lib.Vars;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dooggy.Factory.Data
{
    public class TestDataView : TestDataViewSQL
    {

        public DataBaseConnection DataBase;

        private string key;

        private string _tag;
        public string tag => myString.GetFull(_tag, key);

        public TestDataFlows Flows;

        public TestDataPool Pool { get => DataBase.Pool; }
        public TestDataTratamento Tratamento { get => Pool.Tratamento; }

        public TestTrace Trace { get => Pool.Trace; }

        public TestDataView(string prmTag, DataBaseConnection prmDataBase)
        {
            DataBase = prmDataBase;

            Cleanup();

            Setup(prmTag);
        }
        private void Setup(string prmTag)
        {
            string bloco = Bloco.GetBloco(prmTag, prmDelimitador: "|", prmPreserve: true);

            SetName(prmName: Bloco.GetBloco(bloco, prmDelimitador: "|").Trim());

            _tag = Bloco.GetBlocoAntes(prmTag, bloco, prmTRIM: true);

            SetOutput(prmLista: Bloco.GetBlocoDepois(prmTag, bloco, prmTRIM: true));

            key = Pool.GetNextKeyDataView();
        }

        public void SetName(string prmName) => Header.SetName(prmName);
        public void SetInput(string prmLista) => Header.Input.Parse(prmLista);
        public void SetOutput(string prmLista) => Header.Output.Parse(prmLista);
        public void SetAlias(string prmLista) => Header.Alias.Parse(prmLista);

        public void Cleanup()
        {
            Header = new TestDataHeaderView();

            Flows = new TestDataFlows(Pool);
        }
    }
    public class TestDataFlow : TestDataFlowSQL
    {

        public string tag;

        private TestDataTratamento Tratamento { get => View.Pool.Tratamento; }

        public DataBaseConnection DataBase { get => View.DataBase; }

        // internal

        private DataCursorConnection _cursor;

        public DataCursorConnection Cursor
        {
            get
            {
                if (_cursor == null)
                    _cursor = new DataCursorConnection(GetSQL(), GetMaskMerged(), DataBase);

                return (_cursor);
            }
        }
        public string tag_view  => View.tag;
        public string header_txt => View.Header.txt;

        public bool IsOK => Cursor.IsOK;
        public Exception erro => Cursor.erro;

        public TestDataFlow(string prmTag, string prmSQL, string prmMask, TestDataView prmDataView)
        {
            View = prmDataView;

            tag = prmTag;

            SetHeader(prmDataView.Header);

            SetSQL(prmSQL);

            SetMask(prmMask);
        }

        public bool Next() => Cursor.Next();

        public string GetName(int prmIndice) => Cursor.GetName(prmIndice);
        public string GetValor(int prmIndice) => Cursor.GetValor(prmIndice);
        public string GetValor(string prmNome) => Cursor.GetValor(prmNome);

        public bool Fechar() => Cursor.Fechar();

        public string csv() => csv(prmSeparador: ",");
        public string csv(string prmSeparador) => GetCSV(prmSeparador);
        public string json() => Cursor.GetJSON();

        private string GetCSV(string prmSeparador)
        {
            if (TemSQL || TemStructure)
            {
                string memo = Cursor.GetCSV(prmSeparador);

                if (memo != "")
                    return memo;
                else
                    return Tratamento.GetZeroItens();
            }
            return Tratamento.GetNoCommand();
        }
    }
    public class TestDataFlowSQL : TestDataMask
    {
        public TestDataView View;

        public TestDataHeader Header;

        private myTuplas Input => Header.Input;
        private myTuplas Output => Header.Output;
        private TestTrace Trace => View.Trace;

        private string sql;

        public myArgs filter;

        private string order;

        private int index;

        public TestDataFlowSQL()
        {
            Header = new TestDataHeader();

            filter = new myArgs();
        }

        public void SetHeader(TestDataHeader prmHeader) => Header.Parse(prmHeader);
        public void SetEnter(string prmLista) => SetInput(new myTuplas(prmLista));
        public void SetCheck(string prmLista) => SetOutput(new myTuplas(prmLista));

        public void SetSQL(string prmSQL) => sql = prmSQL;
        public void SetFilter(string prmFilter) => filter.Add(prmFilter);
        public void SetOrder(string prmOrder) => order = prmOrder;
        public void SetIndex(string prmIndex) => index = myInt.GetNumero(prmIndex);

        public bool TemSQL => (TemSQLMontada || TemStructure);

        public bool TemStructure => View.TemTables && View.TemColumns;
        public bool TemSQLMontada => (myString.IsFull(sql));
        private bool TemLinks => View.TemLinks;
        private bool TemCombinacoes => TemLinks && TemFiltro;
        private bool TemCondicoes => TemLinks || TemFiltro;
        private bool TemFiltro => filter.IsFull;
        private bool TemOrdem => (myString.IsFull(order));

        public string GetSQL()
        {
            string comando_sql = sql;

            if (!TemSQLMontada)
                comando_sql = MontarSQL();

            return (View.Tratamento.GetSQLTratado(comando_sql, Header));
        }

        private string MontarSQL()
        {
            string condicoes = ""; string ordenacao = ""; string indexado = "";  string sql = "";

            if (TemStructure)
            {
                sql = String.Format("SELECT {0} FROM {1}", Header.columns_sql, View.select);

                if (TemCombinacoes)
                    condicoes = String.Format("({0}) AND ({1})", View.links.sql, filter.sql);

                else if (TemLinks)
                    condicoes = View.links.sql;

                else if (TemFiltro)
                    condicoes = filter.sql;

                if (TemCondicoes)
                    condicoes = String.Format(" WHERE {0}", condicoes);

                if (TemOrdem)
                    ordenacao = String.Format(" ORDER BY {0}", order);

                indexado = String.Format(" offset {0} row fetch first 1 row only", index);

                sql += condicoes + ordenacao + indexado;
            }
            return (sql.Trim());
        }

        private void SetInput(myTuplas prmTuplas)
        {
            foreach (myTupla tupla in prmTuplas)
                if (!Input.SetValue(tupla))
                    Trace.LogConsole.FailEnterVariable(tupla);
        }

        private void SetOutput(myTuplas prmTuplas)
        {
            foreach (myTupla tupla in prmTuplas)
                if (!Output.SetValue(tupla))
                    Trace.LogConsole.FailCheckVariable(tupla);
        }

        public myTuplas GetMaskMerged()
        {
            myTuplas mask = new myTuplas();

            mask.Parse(View.Mask);

            mask.Parse(Mask);

            mask.Parse(Header.Mask);

            return mask;
        }

    }
    public class TestDataViewSQL : TestDataMask
    {

        public TestDataHeaderView Header;

        public string select => Header.Select.csv;
        public string colums => Header.columns;

        public myArgs links;

        public TestDataViewSQL()
        {
            links = new myArgs();
        }

        public void SetSelect(string prmSelect) => Header.SetSelect(prmSelect);
        public void SetLinks(string prmLinks) => links.Add(prmLinks);

        internal bool TemTables => Header.Select.IsFull;
        internal bool TemColumns => Header.TemColumns;
        internal bool TemLinks => links.IsFull;

    }

    public class TestDataMask
    {
        public myTuplas Mask = new myTuplas();

        public void SetMask(string prmLista) => Mask.Parse(prmLista);
    }
    public class TestDataViews : List<TestDataView>
    {

        public TestDataPool Pool;

        public TestDataView Corrente;

        private TestTrace Trace { get => Pool.Trace; }

        public bool IsHaveData => (this.Count != 0);

        public TestDataViews(TestDataPool prmPool)
        {

            Pool = prmPool;

        }

        public string Criar(string prmTag, string prmMask, DataBaseConnection prmDataBase)
        {

            if (Find(prmTag))

                Corrente.Cleanup();

            else
            {

                Corrente = new TestDataView(prmTag, prmDataBase);

                Add(Corrente);

            }

            Corrente.SetMask(prmMask);

            return Corrente.tag;

        }
        
        public void SetArgumento(string prmArg, string prmInstrucao)
        {

            switch (prmArg)
            {

                case "name":
                    Corrente.SetName(prmInstrucao);
                    break;

                case "select":
                    Corrente.SetSelect(prmInstrucao);
                    break;

                case "links":
                    Corrente.SetLinks(prmInstrucao);
                    break;
                
                case "mask":
                    Corrente.SetMask(prmInstrucao);
                    break;

                case "input":
                    Corrente.SetInput(prmInstrucao);
                    break;

                case "output":
                    Corrente.SetOutput(prmInstrucao);
                    break;

                case "alias":
                    Corrente.SetOutput(prmInstrucao);
                    break;
            }

        }
        public string output(string prmTags, eTipoFileFormat prmTipo) 
        {

            TestDataFlows filtro = new TestDataFlows(Pool);

            xLista lista = new xLista(prmTags, prmSeparador: "+");

            foreach (TestDataView View in this)
            {

                if (myString.IsEmpty(prmTags) || lista.IsEqual(View.tag))
                {
                    
                    filtro.AddItens(View.Flows);

                    Trace.LogData.SQLViewsSelection(View.tag, prmQtde: View.Flows.Count);

                }

            }

            return filtro.output(prmTipo);

        }
        private TestDataViews GetFiltro(string prmLista)
        {
            TestDataViews filtro = new TestDataViews(Pool);

            foreach (string tag in new xLista(prmLista, prmSeparador: "+"))
            {
                int cont = 0;

                foreach (TestDataView View in this)
                {
                    if (myString.IsEqual(View.tag, tag))
                    {
                        filtro.Add(View); cont++;
                    }
                }
                Trace.LogData.SQLViewsSelection(tag, prmQtde: cont);
            }
            return filtro;
        }
        private bool Find(string prmTag)
        {

            foreach (TestDataView view in this)

                if (myString.IsEqual(view.tag, prmTag))
                {

                    Corrente = view;

                    return (true);

                }

            return (false);

        }

    }
    public class TestDataFlows : List<TestDataFlow>
    {

        private TestDataPool Pool;

        public TestDataFlow Corrente;

        public TestTrace Trace { get => Pool.Trace; }

        public TestDataFlows(TestDataPool prmPool)
        {

            Pool = prmPool;

        }

        public bool Criar(string prmTag, string prmSQL, string prmMask, TestDataView prmDataView)
        {

            if (prmDataView != null)
            {

                Corrente = new TestDataFlow(prmTag, prmSQL, prmMask, prmDataView);

                Add(Corrente);

                return (true);
            }

            Trace.LogData.FailFindDataView(prmTag);

            return (false);

        }

        public void AddItens(TestDataFlows prmFlows)
        {

            foreach (TestDataFlow Flow in prmFlows)
                Add(Flow);
        }

        public void SetArgumento(string prmArg, string prmInstrucao)
        {

            switch (prmArg)
            {
                case "enter":
                    Corrente.SetEnter(prmInstrucao); break;

                case "check":
                    Corrente.SetCheck(prmInstrucao); break;

                case "sql":
                    Corrente.SetSQL(prmInstrucao); break;

                case "filter":
                    Corrente.SetFilter(prmInstrucao); break;

                case "order":
                    Corrente.SetOrder(prmInstrucao); break;

                case "mask":
                    Corrente.SetMask(prmInstrucao); break;

                case "index":
                    Corrente.SetIndex(prmInstrucao); break;
            }

        }

        public string output(eTipoFileFormat prmTipo)
        {

            switch (prmTipo)
            {

                case eTipoFileFormat.txt:
                    return txt();

                case eTipoFileFormat.csv:
                    return csv();

                case eTipoFileFormat.json:
                    return json();

            }

            return json();

        }

        private string txt() => txt(prmSeparador: ",", prmColunaExtra: true);
        private string txt(string prmSeparador, bool prmColunaExtra)
        {

            string header = ""; string tag_view = ""; int cont = 0;

            string corpo = ""; string linha; string coluna_extra = "";

            if (prmColunaExtra)
                coluna_extra = prmSeparador;

            foreach (TestDataFlow Flow in this)
            {

                cont++;

                //
                // Adicionar o Cabeçalho (Header) sempre que o layout da visão for diferente ...
                //

                if (tag_view != Flow.tag_view)
                {

                    tag_view = Flow.tag_view;

                    header = Flow.header_txt;

                    corpo += header + Environment.NewLine;

                    //Trace.LogFile.DataFileFormatTXT(header);

                }

                //
                // Montar o Corpo do arquivo TXT ...
                //

                linha = Flow.csv(prmSeparador);

                if (linha != "")
                    corpo += coluna_extra + linha;

                corpo += Environment.NewLine;

                //Trace.LogFile.DataFileFormatTXT(linha);

            }

            return (corpo);

        }
        private string csv()
        {

            string csv = "";

            foreach (TestDataFlow Flow in this)
                csv += Flow.csv() + Environment.NewLine;


            return (csv);

        }
        private string json()
        {

            string json = ""; string separador = "";

            foreach (TestDataFlow Flow in this)
            {

                json += separador + Flow.json(); separador = ", ";

            }

            return (json);

        }

    }
    public class TestDataRaws
    {

        private xMemo Itens;

        public TestDataPool Pool;

        private TestTrace Trace { get => Pool.Trace; }

        private string output { get => Itens.memo_ext ; }

        public bool IsON = true;
        public bool IsHaveData => IsON && Itens.IsFull;

        public TestDataRaws(TestDataPool prmPool)
        {

            Pool = prmPool;

            Itens = new xMemo();

        }
        public void SetOptions(string prmOptions)
        {
            switch (prmOptions)
            {
                case "off":
                    IsON = false;
                    break;
            }
        }
        public void SetArgumento(string prmArg, string prmInstrucao)
        {
            switch (prmArg)
            {
                case "null":
                    Itens.Add(prmInstrucao);
                    break;
                case "*":
                    break;
            }
        }
        public string GetOutput(string prmDados, eTipoFileFormat prmTipo)
        {

            if (prmTipo == eTipoFileFormat.json)
                return(prmDados);

            return Pool.GetTextoTratado(Merge(prmDados));

        }

        private string Merge(string prmDados)
        {

            int cont = 0;

            xLista Dados = new xLista(prmDados, prmSeparador: Environment.NewLine);

            foreach (string dado in Dados)
            {

                cont++;

                if (Dados.qtde > cont)
                    AtualizaLinha(cont, prmLinhaNova: dado);

            }

            return (output);

        }

        private void AtualizaLinha(int prmIndice, string prmLinhaNova)
        {

            string texto = Itens.Get(prmIndice);

            Itens.Add(prmIndice, prmTexto: GetLinhaTratada(prmLinhaNova, prmLinhaAtual: texto));

        }

        private string GetLinhaTratada(string prmLinhaNova, string prmLinhaAtual)
        {

            if (Pool.IsSQLDataException(prmLinhaNova))
                return prmLinhaAtual;

            return prmLinhaNova;

        }

    }

    public class TestDataHeaderView : TestDataHeader
    {
        public myTuplas Alias;

        public TestDataHeaderView()
        {
            Alias = Box.AddItem(prmKey: "alias", prmGroup: "auxiliar");
        }

    }

    public class TestDataHeader
    {
        internal myTuplasBox Box;

        public myTuplas Input;
        public myTuplas Output;

        public myTuplas Mask => new myTuplas(Box.mask);

        public string name;
        public myArgs Select;

        public string columns => Box.Main.columns;
        public string columns_sql => Box.Main.columns_sql;
        public string txt => name + "," + columns;

        public bool TemColumns => Input.IsFull || Output.IsFull;

        public TestDataHeader()
        {
            Box = new myTuplasBox();

            Input = Box.AddItem(prmKey: "input", prmGroup: "main");
            Output = Box.AddItem(prmKey: "output", prmGroup: "main");

            Select = new myArgs();

        }
        public void SetName(string prmName) => name = prmName;
        public void SetSelect(string prmSelect) => Select.Add(prmSelect);
        public void Parse(TestDataHeader prmHeader) { Input.Parse(prmHeader.Input); Output.Parse(prmHeader.Output); }

    }

}
