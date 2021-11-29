using Dooggy;
using Dooggy.Factory;
using Dooggy.Lib.Data;
using Dooggy.Lib.Files;
using Dooggy.Lib.Generic;
using Dooggy.Lib.Parse;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace Dooggy.Factory.Data
{

    public class TestDataProject : TestFactory
    {

        public TestDataConnect Connect { get => Pool.Connect; }

        public void Start(string prmPathDataFiles)
        {

            Pool.SetPathDestino(prmPathDataFiles);

            Call(this, Parameters.GetDataFactoryBlockCode());

        }

    }

    public class TestDataPool
    {

        public TestDataTrace Trace;

        public TestDataConnect Connect;

        private DataBasesConnection Bases;
        private TestDataViews Visoes;
        private TestDataModels Modelos;

        private string pathDataFiles;

        public DataBaseConnection DataBaseCorrente { get => (Bases.Corrente); }

        public TestDataView DataViewCorrente { get => (Visoes.Corrente); }
        public TestDataModel DataModelCorrente { get => (Modelos.Corrente); }

        private bool IsBaseCorrente { get => (DataBaseCorrente != null); }
        private bool IsModelCorrente { get => (DataModelCorrente != null); }

        public TestDataPool()
        {

            Trace = new TestDataTrace();

            Bases = new DataBasesConnection();

            Visoes = new TestDataViews();
            Modelos = new TestDataModels();

            Connect = new TestDataConnect(this);

        }

        public bool AddDataBase(string prmTag, string prmConexao) => Bases.AddItem(prmTag, prmConexao, this);

        public bool AddDataView(string prmTag, string prmSQL, string prmMask) => Visoes.AddItem(prmTag, prmSQL, prmMask, DataBaseCorrente);

        public bool AddDataModel(string prmTag, string prmModelo, string prmMask) => Modelos.AddItem(prmTag, prmModelo, prmMask, DataBaseCorrente);

        public bool AddDataVariant(string prmTag, string prmRegra) => AddDataVariant(prmTag, prmRegra, prmQtde: 1);

        public bool AddDataVariant(string prmTag, string prmRegra, int prmQtde) => DataModelCorrente.CriarVariacao(prmTag, prmRegra, prmQtde);

        public bool SetView(string prmTag) => Visoes.SetView(prmTag);

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

        public void SetPathDestino(string prmPath)
        {

            pathDataFiles = prmPath;

            Trace.SetPath(prmTitulo: "MassaTestes", prmPath);

        }

        public string GetPath(string prmSubPath)
        {
            if (prmSubPath != "")
                return (pathDataFiles + @"\" + prmSubPath + @"\"); 

            return (pathDataFiles);
        }

        public string json(string prmTag)
        {
            if (SetView(prmTag))
                return DataViewCorrente.json();

            return ("");

        }

        public string csv() => Visoes.csv();
        public string json() => Visoes.json();
        public string txt(string prmCabecalho, string prmSeparador, bool prmColunaExtra) => Visoes.Export(prmCabecalho, prmSeparador, prmColunaExtra);

    }
    public class TestDataLocal
    {

        private Object Origem;

        public TestDataPool Pool;

        public TestDataFile File;

        public TestDataView View { get => Pool.DataViewCorrente; }

         public TestDataLocal()
        {

            File = new TestDataFile(this);

        }

        public void Setup(Object prmOrigem, TestDataPool prmPool)
        {

            Origem = prmOrigem;

            Pool = prmPool;

        }
        public bool AddDataBase(string prmTag, string prmConexao) => (Pool.AddDataBase(prmTag, prmConexao));

        public bool AddDataView(string prmTag, string prmSQL) => (Pool.AddDataView(prmTag, prmSQL, prmMask: ""));
        public bool AddDataView(string prmTag, string prmSQL, string prmMask) => (Pool.AddDataView(prmTag, prmSQL, prmMask));

        public void AddDataModel(string prmTag, string prmModelo) => AddDataModel(prmTag, prmModelo, prmMask: "");
        public void AddDataModel(string prmTag, string prmModelo, string prmMask) => Pool.AddDataModel(prmTag, prmModelo, prmMask);

        public void AddDataVariant(string prmTag) => AddDataVariant(prmTag, prmRegra: "");
        public void AddDataVariant(string prmTag, string prmRegra) => Pool.AddDataVariant(prmTag, prmRegra);

        public string csv() => (Pool.csv());
        public string json() => (Pool.json());
        public string json(string prmTag) => (Pool.json(prmTag));

        public string txt(string prmCabecalho) => txt(prmCabecalho, prmColunaExtra: true);
        public string txt(string prmCabecalho, bool prmColunaExtra) => txt(prmCabecalho, prmSeparador: ",", prmColunaExtra);
        public string txt(string prmCabecalho, string prmSeparador, bool prmColunaExtra) => Pool.txt(prmCabecalho, prmSeparador, prmColunaExtra);

    }
    public class TestDataFile
    {

        private TestDataLocal Dados;

        private TestDataExport Export;

        public TestDataPool Pool { get => Dados.Pool; }
        public TestDataTrace Trace { get => Pool.Trace ; }

        public TestDataFile(TestDataLocal prmDados)
        {

            Dados = prmDados;

            Export = new TestDataExport(Dados);

        }

        public bool SaveJSON(string prmNome) => SaveJSON(prmNome, prmSubPath: "");
        public bool SaveJSON(string prmNome, string prmSubPath) => Export.Save(prmNome, prmSubPath, prmConteudo: Dados.json(), prmExtensao: "json");

        public bool SaveCSV(string prmNome) => SaveCSV(prmNome, prmSubPath: "");
        public bool SaveCSV(string prmNome, string prmSubPath) => Export.Save(prmNome, prmSubPath, prmConteudo: Dados.csv(), prmExtensao: "csv");

        public bool SaveCSV2(string prmNome, string prmCabecalho) => SaveCSV2(prmNome, prmCabecalho, prmSubPath: "");
        public bool SaveCSV2(string prmNome, string prmCabecalho, string prmSubPath) => Export.Save(prmNome, prmSubPath, prmConteudo: Dados.txt(prmCabecalho, prmColunaExtra: true), prmExtensao: "txt");


        public string OpenTXT(string prmNome) => OpenTXT(prmNome, prmSubPath: "");
        public string OpenTXT(string prmNome, string prmSubPath) => Export.Open(prmNome, prmSubPath, prmExtensao: "csv");

        public void SetPathDestino(string prmPath) => Pool.SetPathDestino(prmPath);

    }
    public class TestDataExport
    {

        private TestDataLocal Dados;

        private xFileTXT File = new xFileTXT();

        public TestDataTrace Trace { get => Dados.Pool.Trace; }

        public TestDataExport(TestDataLocal prmDados)
        {

            Dados = prmDados;

        }

        public string Open(string prmNome, string prmSubPath, string prmExtensao)
        {

            string path = GetPath(prmSubPath);

            if (File.Open(path, prmNome, prmExtensao))
                return File.txt();

            return ("");

        }

        public bool Save(string prmNome, string prmSubPath, string prmConteudo, string prmExtensao)
        {

            string path = GetPath(prmSubPath);


            if (File.Save(path, prmNome, prmConteudo, prmExtensao))
            {

                Trace.DataFileExport(prmNome, prmSubPath, prmExtensao);

                return (true);

            }

            Trace.FailDataFileExport(path, prmNome, prmExtensao);

            return (false);
        }

        public string GetPath(string prmSubPath) => Dados.Pool.GetPath(prmSubPath);

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

        public bool AddItem(string prmTag, string prmSQL, string prmMask, DataBaseConnection prmDataBase)
        {

            if (prmDataBase != null)
            {

                Corrente = new TestDataView(prmTag, prmSQL, prmMask, prmDataBase);

                Add(Corrente);

                return (true);
            }

            return (false);

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
        public string Export(string prmCabecalho, string prmSeparador, bool prmColunaExtra)
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

        public bool AddItem(string prmTag, string prmModel, string prmMask, DataBaseConnection prmDataBase)
        {

            if (prmDataBase != null)
            {

                Corrente = new TestDataModel(prmTag, prmModel, prmMask, prmDataBase);

                Add(Corrente);

                return (true);
            }

            return (false);

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
    public class TestDataConnect
    {

        private TestDataPool Pool;

        private DataBaseOracle _Oracle;

        public DataBaseOracle Oracle { get { if (_Oracle == null) _Oracle = new DataBaseOracle(Pool); return _Oracle; } }

        public TestDataConnect(TestDataPool prmPool)
        {

            Pool = prmPool;

        }


    }

    public class DataBaseOracle
    {

        private TestDataPool Pool;

        private string model = @"Data Source=(DESCRIPTION =(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(Host = {0})(PORT = {1})))(CONNECT_DATA =(SERVICE_NAME = {2})));User ID={3};Password={4}";

        public string user;// = "desenvolvedor_sia";
        public string password;// = "asdfg";

        public string host;// = "10.250.1.35";
        public string port;// = "1521";
        public string service;// = "branch_1084.prod01.redelocal.oraclevcn.com";

        public DataBaseOracle(TestDataPool prmPool)
        {

            Pool = prmPool;

        }

        public DataBaseOracle()
        {
        }

        public bool Add(string prmTag) => Pool.AddDataBase(prmTag, GetString());

        private string GetString() => String.Format(model, host, port, service, user, password);

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
}
