using Katty;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dooggy
{
    public class TestDataView : TestDataViewSQL
    {

        public DataPool Pool;
        
        public DataBase DataBase;

        private string key;

        private string _tag;
        public string tag => myString.GetFull(_tag, key);

        public TestDataFlows Flows;

        public TestDataTratamento Tratamento { get => Pool.Tratamento; }

        public TestTrace Trace { get => Pool.Trace; }

        public TestDataView(string prmTag, DataBase prmDataBase, DataPool prmPool)
        {
            Pool = prmPool;
            
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
        private TestTrace Trace => View.Trace;

        public DataBase DataBase { get => View.DataBase; }

        private bool IsVazio => (_cursor == null);

        private DataCursor _cursor;

        public DataCursor Cursor
        {
            get
            {
                if (IsVazio)
                {
                    _cursor = DataBase.GetCursor(prmSQL: GetSQL(), prmMasks: GetMasksMerged());

                    CheckResultDataCursor();
                }
                return (_cursor);
            }
        }
        public string tag_view  => View.tag;
        public string header_txt => View.Header.txt;

        public bool IsOK => Cursor.IsOK;
        public Exception Erro => Cursor.Erro;

        public TestDataFlow(string prmTag, string prmSQL, string prmMasks, TestDataView prmDataView)
        {
            View = prmDataView;

            tag = prmTag;

            SetHeader(prmDataView.Header);

            SetSQL(prmSQL);

            SetMasks(prmMasks);
        }

        public bool Next() => Cursor.Next();

        public string GetName(int prmIndice) => Cursor.GetName(prmIndice);
        public string GetValor(int prmIndice) => Cursor.GetValor(prmIndice);
        public string GetValor(string prmNome) => Cursor.GetValor(prmNome);

        public bool Fechar() => Cursor.Fechar();

        public string csv() => csv(prmSeparador: ",");
        public string csv(string prmSeparador) => GetCSV(prmSeparador);
        public string json() => GetJSON();

        private string GetCSV(string prmSeparador)
        {
            if (TemSQL || TemStructure)
            {
                string memo = Cursor.csv(prmSeparador);

                if (memo != "")
                    return memo;
                else
                    return Tratamento.GetZeroItens();
            }
            return Tratamento.GetNoCommand();
        }

        private string GetJSON()
        {
            if (TemSQL || TemStructure)
            {
                string memo = Cursor.json();

                if (memo != "")
                    return memo;
                else
                    return Tratamento.GetZeroItens();
            }
            return Tratamento.GetNoCommand();
        }
        private void CheckResultDataCursor()
        {
            
            if (Cursor.TemDados)
            {
                CheckQtdeInterface(Header.qtdeColumns, Cursor.qtdeColumns);

                CheckFindFieldToMaskFormat();
            }
        }

        private void CheckQtdeInterface(int prmHeaderColumns, int prmSQLColumns)
        {
            if (prmHeaderColumns != prmSQLColumns)
                Trace.LogData.FailSQLFailInterface(prmHeaderColumns, prmSQLColumns);
        }

        private void CheckFindFieldToMaskFormat()
        {
            if (Cursor.HasMasks)
                foreach (myTupla tupla in Cursor.Masks)
                    if (!Cursor.IsFind(prmColumn: tupla.name))
                        Trace.LogData.FailSQLFormatFindField(prmCampo: tupla.name, prmFormat: tupla.value);
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
                sql = String.Format("SELECT {0} FROM {1}", Header.columns_sql, View.tables);

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

        internal myMasks GetMasksMerged()
        {
            myMasks ret = new myMasks();

            ret.Parse(View.Masks);

            ret.Parse(Masks);

            ret.Parse(Header.Masks);

            return ret;
        }

    }
    public class TestDataViewSQL : TestDataMask
    {

        public TestDataHeaderView Header;

        public string tables => Header.Tables.csv;
        public string colums => Header.columns;

        public myArgs links;

        public TestDataViewSQL()
        {
            links = new myArgs();
        }

        public void SetTables(string prmTables) => Header.SetTables(prmTables);
        public void SetLinks(string prmLinks) => links.Add(prmLinks);

        internal bool TemTables => Header.Tables.IsFull;
        internal bool TemColumns => Header.TemColumns;
        internal bool TemLinks => links.IsFull;

    }

    public class TestDataMask
    {
        public myMasks Masks = new myMasks();

        public void SetMasks(string prmLista) => Masks.Parse(prmLista);
    }
    public class TestDataViews : List<TestDataView>
    {

        public DataPool Pool;

        public TestDataView Corrente;

        private TestTrace Trace { get => Pool.Trace; }

        public bool IsHaveData => (this.Count != 0);

        public TestDataViews(DataPool prmPool)
        {

            Pool = prmPool;

        }

        public string Criar(string prmTag, string prmMasks, DataBase prmDataBase)
        {

            if (Find(prmTag))

                Corrente.Cleanup();

            else
            {

                Corrente = new TestDataView(prmTag, prmDataBase, Pool);

                Add(Corrente);

            }

            Corrente.SetMasks(prmMasks);

            return Corrente.tag;

        }
        
        public void SetArgumento(string prmArg, string prmInstrucao)
        {

            switch (prmArg)
            {

                case "name":
                    Corrente.SetName(prmInstrucao);
                    break;

                case "table":
                case "tables":
                    Corrente.SetTables(prmInstrucao);
                    break;

                case "link":
                case "links":
                    Corrente.SetLinks(prmInstrucao);
                    break;
                
                case "mask":
                    Corrente.SetMasks(prmInstrucao);
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

                if (myString.IsEmpty(prmTags) || lista.IsFind(View.tag))
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
                    if (myString.IsMatch(View.tag, tag))
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

                if (myString.IsMatch(view.tag, prmTag))
                {

                    Corrente = view;

                    return (true);

                }
            return (false);
        }

    }
    public class TestDataFlows : List<TestDataFlow>
    {

        private DataPool Pool;

        public TestDataFlow Corrente;

        public TestTrace Trace { get => Pool.Trace; }

        public bool IsFull => (Count != 0);

        public TestDataFlows(DataPool prmPool)
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
                    Corrente.SetMasks(prmInstrucao); break;

                case "index":
                    Corrente.SetIndex(prmInstrucao); break;
            }

        }

        public string output(eTipoFileFormat prmTipo)
        {
            if (IsFull)
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
            return null;
        }

        private string txt() => txt(prmSeparador: ",", prmColunaExtra: true);
        private string txt(string prmSeparador, bool prmColunaExtra)
        {

            string header = ""; string tag_view = ""; int cont = 0;

            string linha; string coluna_extra = ""; xLinhas corpo = new xLinhas();

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

                    corpo.Add(header);

                    //Trace.LogFile.DataFileFormatTXT(header);

                }
                
                //
                // Montar o Corpo do arquivo TXT ...
                //

                linha = Flow.csv(prmSeparador);

                if (linha == "")
                    corpo.Add("");
                else
                    corpo.Add(coluna_extra + linha);

                //Trace.LogFile.DataFileFormatTXT(linha);

            }

            return (corpo.memo);

        }
        private string csv()
        {
            xLinhas corpo = new xLinhas();

            foreach (TestDataFlow Flow in this)
                corpo.Add(Flow.csv());

            return (corpo.memo);
        }
        private string json()
        {

            xLinhas corpo = new xLinhas();

            foreach (TestDataFlow Flow in this)
                corpo.Add(Flow.json());

            return (corpo.txt);

        }

    }
    public class TestDataRaws
    {

        private xLinhas Itens;

        public DataPool Pool;

        private TestTrace Trace { get => Pool.Trace; }

        private string output { get => Itens.memo ; }

        public bool IsON = true;
        public bool IsHaveData => IsON && Itens.IsFull;

        public TestDataRaws(DataPool prmPool)
        {

            Pool = prmPool;

            Itens = new xLinhas();

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
            xLinhas Dados = new xLinhas(prmDados);

            for (int cont = 1; cont <= Dados.qtde; cont++)
                AtualizaLinha(cont, prmText: Dados.Get(cont));

            return (output);
        }

        private void AtualizaLinha(int prmIndice, string prmText)
        {

            string texto = Itens.Get(prmIndice);

            Itens.Add(prmIndice, prmTexto: GetLinhaTratada(prmLinhaNova: prmText, prmLinhaAtual: texto));

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
        internal myBoxMasks Box;

        public myMasks Input;
        public myMasks Output;

        public myMasks Masks => new myMasks(Box.masks);

        public string name;
        public myArgs Tables;

        public int qtdeColumns => Box.Main.qtde;
        public string columns => Box.Main.columns;
        public string columns_sql => Box.Main.columns_sql;
        public string txt => name + "," + columns;

        public bool TemColumns => Input.IsFull || Output.IsFull;

        public TestDataHeader()
        {
            Box = new myBoxMasks();

            Input = Box.AddItem(prmKey: "input", prmGroup: "main");
            Output = Box.AddItem(prmKey: "output", prmGroup: "main");

            Tables = new myArgs();

        }
        public void SetName(string prmName) => name = prmName;
        public void SetTables(string prmTables) => Tables.Add(prmTables);
        public void Parse(TestDataHeader prmHeader) { Input.Parse(prmHeader.Input); Output.Parse(prmHeader.Output); }

    }

}
