using Dooggy;
using Dooggy.Factory;
using Dooggy.Factory.Trace;
using Dooggy.Lib.Data;
using Dooggy.Lib.Generic;
using Dooggy.Lib.Parse;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Dooggy.Factory.Data
{
    public class TestDataPool
    {

        public TestTraceDataBase Trace;

        private DataBasesConnection Bases;
        private TestDataViews Visoes;
        private TestDataModels Modelos;

        public DataBaseConnection DataBaseCorrente { get => (Bases.Corrente); }

        public TestDataView DataViewCorrente { get => (Visoes.Corrente); }
        public TestDataModel DataModelCorrente { get => (Modelos.Corrente); }

        private bool IsBaseCorrente { get => (DataBaseCorrente != null); }
        private bool IsModelCorrente { get => (DataModelCorrente != null); }

        public TestDataPool(TestTraceDataBase prmTrace)
        {

            Trace = prmTrace;

            Bases = new DataBasesConnection();

            Visoes = new TestDataViews();
            Modelos = new TestDataModels();

        }

        public bool AddDataBase(string prmTag, string prmConexao) => Bases.AddItem(prmTag, prmConexao, this);

        public bool AddDataView(string prmTag, string prmSQL) => AddDataView(prmTag, prmSQL, prmMask: "");

        public bool AddDataView(string prmTag, string prmSQL, string prmMask) => Visoes.AddItem(DataBaseCorrente.CreateView(prmTag, prmSQL, prmMask));

        public bool AddDataModel(string prmTag, string prmModel) => AddDataModel(prmTag, prmModel, prmMask: "");

        public bool AddDataModel(string prmTag, string prmModel, string prmMask)
        {

            if (IsBaseCorrente)
                Modelos.AddItem(DataBaseCorrente.CreateModel(prmTag, prmModel, prmMask));

            return (IsBaseCorrente);

        }
        public bool AddDataVariant(string prmTag) => AddDataVariant(prmTag, prmRegra: "");

        public bool AddDataVariant(string prmTag, string prmRegra) => AddDataVariant(prmTag, prmRegra, prmQtde: 1);

        public bool AddDataVariant(string prmTag, string prmRegra, int prmQtde) => DataModelCorrente.CriarVariacao(prmTag, prmRegra, prmQtde);

        public bool SetView(string prmTag) => Visoes.SetView(prmTag);

        public string json(string prmTag)
        {
            if (SetView(prmTag))
                return DataViewCorrente.json();

            return ("");

        }
        public bool IsON()
        {

            bool retorno = false;

            foreach (DataBaseConnection Base in Bases)
            {
                if (!Base.IsON)
                { return (false); }

                retorno = true;
            }

            return (retorno);

        }

        public string csv() => Visoes.csv();
        public string json() => Visoes.json();

        public string Export(string prmCabecalho) => Export(prmCabecalho, prmColunaExtra: true);
        public string Export(string prmCabecalho, bool prmColunaExtra) => Export(prmCabecalho, prmColunaExtra, prmSeparador: ",");
        public string Export(string prmCabecalho, bool prmColunaExtra, string prmSeparador) => Visoes.Export(prmCabecalho, prmColunaExtra, prmSeparador);

    }
    public class TestDataLocal
    {

        private Object Origem;

        public TestDataPool Pool;

        public TestDataExport Export;

        public TestDataView View
        { get => Pool.DataViewCorrente; }

        public void Setup(Object prmOrigem, TestDataPool prmPool)
        {

            Origem = prmOrigem;

            Pool = prmPool;

            Export = new TestDataExport(this);

        }
        public bool AddDataBase(string prmTag, string prmConexao)
        {
            return (Pool.AddDataBase(prmTag, prmConexao));
        }
        public bool AddDataView(string prmTag, string prmSQL)
        {
            return (Pool.AddDataView(prmTag, prmSQL));
        }
        public void AddDataModel(string prmTag, string prmModelo)
        {
            Pool.AddDataModel(prmTag, prmModelo);
        }
        public void AddDataVariant(string prmTag)
        {
            Pool.AddDataVariant(prmTag);
        }
        public void AddDataVariant(string prmTag, string prmRegra)
        {
            Pool.AddDataVariant(prmTag, prmRegra);
        }
        public bool SetView(string prmTag)
        {
            return (Pool.SetView(prmTag));
        }
        public string GetView(string prmTag)
        {
            return (Pool.json(prmTag));
        }
        public string csv()
        {
            return (Pool.csv());
        }
        public string json()
        {
            return (Pool.json());
        }

    }
    public class ITestDataLocal
    {

        private TestDataLocal _Dados;

        public TestDataLocal Dados
        {
            get
            {
                if (_Dados == null)
                    _Dados = new TestDataLocal();

                return _Dados;

            }

        }

    }
    public class TestDataModel
    {

        public string tag;

        private string mask;

        public xJSON Parametros;

        public DataBaseConnection DataBase;

        private TestDataViews Visoes = new TestDataViews();

        private TestDataVariant Variacao;

        public TestDataPool Pool { get => DataBase.Pool; }

        public TestDataModel(string prmTag, string prmModel, string prmMask, DataBaseConnection prmDataBase)
        {

            tag = prmTag;

            mask = prmMask;

            Parametros = new xJSON(prmModel);

            DataBase = prmDataBase;

            Variacao = new TestDataVariant(this);

        }

        public bool CriarView(string prmTag, string prmSQL)
        {

            if (Pool.AddDataView(prmTag, prmSQL, mask))
            { Visoes.Add(Pool.DataViewCorrente); return (true); }

            return (false);

        }
        public bool CriarVariacao(string prmTag, string prmRegras, int prmQtde) => Variacao.Criar(prmTag, prmRegras, prmQtde);

        public string GetListaTabelas() => (TratarParametros(Parametros.GetValor("#TABELAS#")));

        public string GetListaCampos() => (TratarParametros(Parametros.GetValor("#CAMPOS#", prmPadrao: "*")));

        private string TratarParametros(string prmLista) => new xMemo(prmLista, prmSeparador: "+").memo(", ");

    }
    public class TestDataVariant
    {

        private xJSON Regras;

        public TestDataModel Modelo;

        private string GetTagExtendida(string prmTag) => Modelo.tag + prmTag;

        private bool IsRegraOK { get => (Regras.IsOK); }

        public TestDataVariant(TestDataModel prmModelo)
        {

            Modelo = prmModelo;

        }

        public bool Criar(string prmTag, string prmRegras, int prmQtde)
        {

            Regras = new xJSON(prmRegras);

            return (Modelo.CriarView(GetTagExtendida(prmTag), prmSQL: GetSQL(prmQtde)));

        }

        private string GetSQL(int prmQtde)
        {

            string sql = MontaSELECT(prmQtde);

            if (IsRegraOK)
                sql += " " + MontaEXTENSAO();

            return (MontaSQL(sql, prmQtde));

        }

        private string MontaSQL(string prmSQL, int prmQtde) => (string.Format("SELECT * FROM ({0}) WHERE ROWNUM = {1}", prmSQL, prmQtde));

        private string MontaSELECT(int prmQtde) => (string.Format("SELECT {0} FROM {1}", Modelo.GetListaCampos(), Modelo.GetListaTabelas()));

        private string MontaEXTENSAO() => (string.Format("{0} {1}", MontaWHERE(), MontaORDERBY()).Trim());

        private string MontaWHERE() => (Regras.FindValor("#CONDICAO#", "WHERE {0}"));

        private string MontaORDERBY() => (Regras.FindValor("#ORDEM#", "ORDER BY {0}"));

    }
    public class TestDataView
    {

        public string tag;

        public DataBaseConnection DataBase;

        public DataCursorConnection Cursor;

        public bool IsOK { get => Cursor.IsOK(); }
        public string sql { get => Cursor.sql; }
        public Exception erro { get => Cursor.erro; }



        public TestDataView(string prmTag, string prmSQL, string prmMask, DataBaseConnection prmDataBase)
        {

            tag = prmTag;

            DataBase = prmDataBase;

            Cursor = new DataCursorConnection(prmSQL, prmMask, prmDataBase);

        }

        public bool Next() => Cursor.Next();

        public string GetName(int prmIndice) => Cursor.GetName(prmIndice);

        public string GetValor(int prmIndice) => Cursor.GetValor(prmIndice);

        public string GetValor(string prmNome) => Cursor.GetValor(prmNome);

        public string csv(string prmSeparador) => Cursor.GetCSV(prmSeparador);
        public string csv() => Cursor.GetCSV();
        public string json() => Cursor.GetJSON();

        public bool Fechar() => Cursor.Fechar();

        public string log_sql() => String.Format("VIEW:[{0,25}] SQL: {1}", tag, sql);
        public string log_data() => String.Format("DATA:[{0,25}] Fluxo: {1}", tag, json());

    }
    public class TestDataViews : List<TestDataView>
    {

        public TestDataView Corrente;

        public bool AddItem(TestDataView prmView)
        {

            Corrente = prmView;

            Add(Corrente);

            return (prmView.IsOK);

        }

        public bool SetView(string prmTag)
        {

            foreach (TestDataView Visao in this)
            {

                if (Visao.tag == prmTag)
                {

                    Corrente = Visao;

                    return (true);

                }

            }

            return (false);

        }
        public string Export(string prmCabecalho, bool prmColunaExtra, string prmSeparador)
        {

            string txt = prmCabecalho + Environment.NewLine;

            string linha; string extra = "";

            if (prmColunaExtra)
                extra = prmSeparador;

            foreach (TestDataView Visao in this)
            {

                linha = Visao.csv(prmSeparador);

                if (linha != "")
                    txt += extra + linha;

                txt += Environment.NewLine;

            }

            return (txt);

        }
        public string csv()
        {

            string csv = "";

            foreach (TestDataView Visao in this)
                csv += Visao.csv() + Environment.NewLine;


            return (csv);

        }
        public string json()
        {

            string json = ""; string separador = "";

            foreach (TestDataView Visao in this)
            {

                json += separador + Visao.json(); separador = ", ";

            }

            return (json);

        }

    }
    public class TestDataModels : List<TestDataModel>
    {

        public TestDataModel Corrente;

        public void AddItem(TestDataModel prmModel)
        {

            Corrente = prmModel;

            Add(prmModel);

        }

        public bool SetModel(string prmTag)
        {

            foreach (TestDataModel Model in this)
            {

                if (Model.tag == prmTag)
                {

                    Corrente = Model;

                    return (true);

                }

            }

            return (false);

        }

    }
    public class TestDataExport
    {

        private TestDataLocal Dados;

        public TestDataExport(TestDataLocal prmDados)
        {

            Dados = prmDados;

        }

        public bool SaveJSON(string prmPath, string prmNome) => Save(prmPath, prmNome, prmConteudo: Dados.json(), prmExtensao: "json");

        public bool SaveCSV(string prmPath, string prmNome) => SaveCSV(prmPath, prmNome, prmConteudo: Dados.csv());

        public bool SaveCSV(string prmPath, string prmNome, string prmConteudo) => Save(prmPath, prmNome, prmConteudo, prmExtensao: "csv");

        private bool Save(string prmPath, string prmNome, string prmConteudo, string prmExtensao)
        {

            string nome_completo = String.Format("{0}{1}.{2}", prmPath, prmNome, prmExtensao);

            try
            {
                File.WriteAllText("WriteLines.txt", prmConteudo);

                return (true);
            }
            catch
            { }

            return (false);
        }


    }
}
