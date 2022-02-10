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
    public class TestDataView : TestDataSQLBase
    {

        public DataBaseConnection DataBase;

        private string key;

        private string alias;

        public string descricao;
        public string tag => GetTag();

        public TestViewHeader Header;

        public TestDataFlows Flows;

        public TestDataPool Pool { get => DataBase.Pool; }

        private TestTrace Trace { get => Pool.Trace; }

        public TestDataView(string prmTag, DataBaseConnection prmDataBase)
        {

            DataBase = prmDataBase;

            Setup(prmTag);

            Cleanup();

        }
        private void Setup(string prmTag)
        {

            string bloco = Bloco.GetBloco(prmTag, prmDelimitador: "|", prmPreserve: true);

            descricao = Bloco.GetBloco(bloco, prmDelimitador: "|").Trim();

            alias = Bloco.GetBlocoAntes(prmTag, bloco, prmTRIM: true);

            //SetOutput(prmLista: Bloco.GetBlocoDepois(prmTag, bloco, prmTRIM: true));

            key = Pool.GetNextKeyDataView();

        }

        public void SetInput(string prmLista) => Header.Input.Parse(prmLista);
        public void SetOutput(string prmLista) => Header.Output.Parse(prmLista);

        public void Cleanup()
        {
            Header = new TestViewHeader(this);

            Flows = new TestDataFlows(Pool);
        }
        private string GetTag()
        {
            if (alias == "")
                return (key);

            return (alias);
        }

    }
    public class TestDataFlow : TestDataSQL
    {

        public string tag;

        public TestDataView View;

        public myTupla Input;

        private TestDataTratamento Tratamento { get => View.Pool.Tratamento; }

        public DataBaseConnection DataBase { get => View.DataBase; }

        // internal

        private DataCursorConnection _cursor;

        public DataCursorConnection Cursor
        {
            get
            {
                if (_cursor == null)
                    _cursor = new DataCursorConnection(GetSQL(View), GetMask(), DataBase);

                return (_cursor);

            }

        }

        public string tag_view  => View.tag;

        public string header_txt => View.Header.txt;

        public bool IsOK => Cursor.IsOK;
        public Exception erro => Cursor.erro;

        public TestDataFlow(string prmTag, string prmSQL, string prmMask, TestDataView prmDataView)
        {

            tag = prmTag;

            SetInput(prmDataView.Header.Input);

            SetSQL(prmSQL);

            SetMask(prmMask);

            View = prmDataView;

        }

        public bool Next() => Cursor.Next();

        public string GetName(int prmIndice) => Cursor.GetName(prmIndice);
        public string GetValor(int prmIndice) => Cursor.GetValor(prmIndice);
        public string GetValor(string prmNome) => Cursor.GetValor(prmNome);

        public bool Fechar() => Cursor.Fechar();

        public string csv() => csv(prmSeparador: ",");
        public string csv(string prmSeparador) => GetCSV(prmSeparador);
        public string json() => Cursor.GetJSON();

        private string GetMask()
        {

            if (mask == "")
                return (View.mask);

            return mask;

        }

        private string GetCSV(string prmSeparador)
        {
            if (TemSQL() || View.TemEstrutura())
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
    public class TestDataSQL : TestDataSQLBase
    {

        private myTuplas Input;
        
        private string sql;

        private string filtro;

        private string ordem;

        public TestDataSQL()
        {
            Input = new myTuplas();
        }
        public void SetInput(myTuplas prmTuplas) => Input.Parse(prmTuplas);
        public void SetInput(string prmValores) => Input.Parse(prmValores);
        public void SetSQL(string prmSQL) => sql = prmSQL;
        public void SetFiltro(string prmFiltro) => filtro = prmFiltro;
        public void SetOrdem(string prmOrdem) => ordem = prmOrdem;

        public bool TemSQL() => (TemSQLMontada() || TemEstrutura());
        public bool TemSQLMontada() => (myString.IsFull(sql));
        private bool TemCombinacoes() => TemRelacoes() && TemFiltro();
        private bool TemCondicoes() => TemRelacoes() || TemFiltro();
        private bool TemFiltro() => (myString.IsFull(filtro));
        private bool TemOrdem() => (myString.IsFull(ordem));

        public string GetSQL(TestDataView prmView)
        {

            string comando_sql = sql;

            if (!TemSQLMontada())
            {

                tabelas = prmView.tabelas;

                campos = prmView.campos;

                relacoes = prmView.relacoes;

                comando_sql = GetSQL();

            }
            return (prmView.Pool.GetTextoTratado(comando_sql));
        }

        private string GetSQL()
        {

            string condicoes = ""; string ordenacao = ""; string sql = "";

            if (TemEstrutura())
            {

                sql = String.Format("SELECT {0} FROM {1}", campos, tabelas);

                if (TemCombinacoes())
                    condicoes = String.Format("({0}) AND ({1})", relacoes, filtro);

                else if (TemRelacoes())
                    condicoes = relacoes;

                else if (TemFiltro())
                    condicoes = filtro;

                if (TemCondicoes())
                    condicoes = String.Format(" WHERE {0}", condicoes);

                if (TemOrdem())
                    ordenacao = String.Format(" ORDER BY {0}", ordem);

                sql += condicoes + ordenacao;

            }
            return (sql.Trim());
        }

    }
    public class TestDataSQLBase
    {

        public string tabelas;

        public string campos;

        public string relacoes;

        public string mask;

        public void SetTabelas(string prmTabelas) => tabelas = prmTabelas;
        public void SetCampos(string prmCampos) => campos = prmCampos;
        public void SetRelacoes(string prmRelacoes) => relacoes = prmRelacoes;
        public void SetMask(string prmMask) => mask = prmMask;

        public bool TemEstrutura() => TemTabelas() && TemCampos();
        protected bool TemTabelas() => (myString.IsFull(tabelas));
        protected bool TemCampos() => (myString.IsFull(campos));
        protected bool TemRelacoes() => (myString.IsFull(relacoes));

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

                case "descricao":
                    Corrente.descricao = prmInstrucao;
                    break;

                case "tabelas":
                    Corrente.SetTabelas(prmInstrucao);
                    break;

                case "campos":
                    Corrente.SetCampos(prmInstrucao);
                    break;

                case "relacoes":
                    Corrente.SetRelacoes(prmInstrucao);
                    break;
                
                case "mask":
                    Corrente.SetMask(prmInstrucao);
                    break;

                case "entrada":
                    Corrente.SetInput(prmInstrucao);
                    break;

                case "saida":
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

            Trace.LogData.FailNoDataViewDetected(prmTag);

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

                case "input":
                    Corrente.SetInput(prmInstrucao);
                    break;
                
                case "sql":
                    Corrente.SetSQL(prmInstrucao);
                    break;

                case "filtro":
                    Corrente.SetFiltro(prmInstrucao);
                    break;

                case "ordem":
                    Corrente.SetOrdem(prmInstrucao);
                    break;

                case "mask":
                    Corrente.SetMask(prmInstrucao);
                    break;

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

    public class TestViewHeader
    {

        private TestDataView View;

        private myTuplasBox Box;

        private myTuplas _input;
        private myTuplas _output;

        public myTuplas Input => _input;
        public myTuplas Output => _output;
        public string txt => Box.header;

        public TestViewHeader (TestDataView prmView)
        {
            View = prmView;

            Box = new myTuplasBox();

            _input = Box.AddItem(prmKey: "input");

            _output = Box.AddItem(prmKey: "output");
        }
    }
    public class TestDataInput
    {

        private TestDataFlow Flow;

        public myTupla Tupla = new myTupla();

        public TestDataInput(string prmTag, TestDataFlow prmFlow)
        {

            Flow = prmFlow;

            //Tupla.tag = prmTag;

        }

    }

    public class TestInputFlow 
    {

        private TestDataFlow Flow;

        public myTuplas Tuplas = new myTuplas();

        public TestInputFlow(TestDataFlow prmFlow)
        {

            Flow = prmFlow;

        }

        //public void Load(string prmInput) => Tuplas.Load(prmInput);


    }

}
