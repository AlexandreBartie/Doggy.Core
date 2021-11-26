using System.Diagnostics;
using System.Data;
using System.Data.OracleClient;
using System.Collections.Generic;
using System;
using Dooggy.KERNEL;
using Oracle.ManagedDataAccess.Client;
using Dooggy.LIB.PARSE;
using System.IO;

namespace Dooggy
{
    public class DataPoolConnection
    {

        public TestTraceDataBase Trace;

        private DataBasesConnection Bases;
        private DataViewsConnection Visoes;
        private DataModelsConnection Modelos;

        public DataBaseConnection DataBaseCorrente { get => (Bases.Corrente); }
        public DataViewConnection DataViewCorrente { get => (Visoes.Corrente); }
        public DataModelConnection DataModelCorrente { get => (Modelos.Corrente); }

        private bool IsBaseCorrente { get => (DataBaseCorrente != null) ; }
        private bool IsModelCorrente { get => (DataModelCorrente != null); }

        public DataPoolConnection(TestTraceDataBase prmTrace)
        {

            Trace = prmTrace;

            Bases = new DataBasesConnection();
            Visoes = new DataViewsConnection();
            Modelos = new DataModelsConnection();

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
    public class DataBaseConnection
    {

        public DataPoolConnection Pool;

        public string tag;

        public OracleConnection conexao;

        public Exception erro;

        public DataBaseConnection(string prmTag, string prmConexao, DataPoolConnection prmPool)
        {

            tag = prmTag;

            Pool = prmPool;

            Abrir(prmConexao);

        }

        public TestTraceDataBase Trace { get => (Pool.Trace); }
        public bool IsON { get => (conexao != null); }
        public bool IsOK { get { if (IsON) return (conexao.State == ConnectionState.Open); return (false); } }

        public DataViewConnection CreateView(string prmTag, string prmSQL, string prmMask) => new DataViewConnection(prmTag, prmSQL, prmMask, this);

        public DataModelConnection CreateModel(string prmTag, string prmModel, string prmMask) => new DataModelConnection(prmTag, prmModel, prmMask, this);

        private bool Criar(string prmConexao)
        {

            try
            {

                conexao = new OracleConnection(prmConexao);

                Trace.StatusConnection(tag, "CONECTADO");

                return (true);

            }

            catch (Exception e)
            { Trace.FailDataConnection(tag, prmConexao, e); erro = e; }

            return (false);
        }
        public bool Abrir(string prmConexao)
        {

            try
            {

                if (Criar(prmConexao))
                    conexao.Open();

                Trace.StatusConnection(tag, "ABERTO");

                return (true);

            }

            catch (Exception e)
            { Trace.FailDataConnection(tag, prmConexao, e); erro = e; }

            return (false);
        }

        public void Fechar()
        {
            try
            { conexao.Close(); }

            catch (Exception e)
            { erro = e; }

        }

    }
    public class DataModelConnection
    {

        public string tag;

        private string mask;

        public xJSON Parametros;

        public DataBaseConnection DataBase;

        private DataViewsConnection Visoes = new DataViewsConnection();

        private DataVariantConnection Variacao;

        public DataPoolConnection Pool { get => DataBase.Pool; }

        public DataModelConnection(string prmTag, string prmModel, string prmMask, DataBaseConnection prmDataBase)
        {

            tag = prmTag;

            mask = prmMask;

            Parametros = new xJSON(prmModel);

            DataBase = prmDataBase;

            Variacao = new DataVariantConnection(this);

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
    public class DataVariantConnection
    {

        private xJSON Regras;

        public DataModelConnection Modelo;

        private string GetTagExtendida(string prmTag) => Modelo.tag + prmTag;

        private bool IsRegraOK { get => (Regras.IsOK); }

        public DataVariantConnection(DataModelConnection prmModelo)
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

            return (MontaSQL(sql,prmQtde));
            
        }

        private string MontaSQL(string prmSQL, int prmQtde) => (string.Format("SELECT * FROM ({0}) WHERE ROWNUM = {1}", prmSQL, prmQtde));

        private string MontaSELECT(int prmQtde) => (string.Format("SELECT {0} FROM {1}", Modelo.GetListaCampos(), Modelo.GetListaTabelas()));

        private string MontaEXTENSAO() => (string.Format("{0} {1}", MontaWHERE(), MontaORDERBY()).Trim());
  
        private string MontaWHERE() => (Regras.FindValor("#CONDICAO#", "WHERE {0}"));

        private string MontaORDERBY() => (Regras.FindValor("#ORDEM#", "ORDER BY {0}"));

    }
    public class DataViewConnection
    {

        public string tag;

        public DataBaseConnection DataBase;

        public DataCursorConnection Cursor;

        public bool IsOK { get => Cursor.IsOK(); }
        public string sql { get => Cursor.sql; }
        public Exception erro { get => Cursor.erro; }



        public DataViewConnection(string prmTag, string prmSQL, string prmMask, DataBaseConnection prmDataBase)
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
    public class DataCursorConnection
    {

        private DataBaseConnection DataBase;

        private OracleDataReader reader;

        private xMask Mask;

        public Exception erro;

        private string _sql;

        public string sql { get => _sql; }

        public bool IsMask { get => (Mask != null); }

        public bool IsResult;

        public TestTraceDataBase Trace => DataBase.Trace;


        public DataCursorConnection(string prmSQL, string prmMask, DataBaseConnection prmDataBase)
        {

            DataBase = prmDataBase;

            if (DataBase.IsOK)
            {
                GetReader(prmSQL); SetMask(prmMask);
            }
            else
                { Trace.FailSQLNoDataBaseConnection(DataBase.tag, prmSQL, DataBase.erro); erro = DataBase.erro; }
        }
        private void SetMask(string prmMask)
        {
            if (prmMask != "")
                Mask = new xMask(prmMask);
        }

        public void GetReader(string prmSQL)
        {

            _sql = prmSQL;

            try
            {
                OracleCommand vlSql = new OracleCommand(prmSQL, DataBase.conexao);

                reader = (vlSql.ExecuteReader());

                IsResult = Start();

                Trace.SQLExecution(DataBase.tag, prmSQL);

            }
            catch (Exception e)
            { Trace.FailSQLConnection(DataBase.tag, prmSQL, e); erro = e; }

        }
        private bool Start() => Next();
        public bool Next() => reader.Read();
        public bool IsDBNull(int prmIndice)
        {

            return reader.IsDBNull(prmIndice);

        }
        public string GetName(int prmIndice)
        {

            return reader.GetName(prmIndice);

        }
        public string GetValor(int prmIndice)
        {

            string valor = reader.GetOracleValue(prmIndice).ToString();

            return GetMask(valor, prmName: GetName(prmIndice));

        }
        public string GetValor(string prmNome)
        {

            string valor = reader.GetString(prmNome);

            return GetMask(valor, prmNome);

        }

        private string GetMask(string prmValor, string prmName)
        {

            if (IsMask)
                return Mask.GetValor(prmValor, prmName);

            return (prmValor);

        }

        public string GetCSV() => GetCSV(prmSeparador: ",");
        public string GetCSV(string prmSeparador)
        {
            string memo = "";
            string texto = "";
            string separador = "";

            if (IsResult)
            {
                for (int cont = 0; cont < reader.VisibleFieldCount; cont++)
                {
                    if (IsDBNull(cont))
                        texto = "";
                    else
                        texto = GetValor(cont);

                    memo += separador + texto;

                    separador = prmSeparador;

                }

            }

            return (memo);
        }
        public string GetJSON()
        {
            string memo = "";
            string separador = "";

            if (IsResult)
            {
                for (int cont = 0; cont < reader.VisibleFieldCount; cont++)
                {
                    if (!IsDBNull(cont))
                    {

                        memo += separador + GetTupla(cont);

                        separador = ", ";

                    }

                }

                return ("{ " + memo + " }");
            }

            return ("{ }");
        }
        public string GetTupla(int prmIndice) => string.Format("'{0}': '{1}'", GetName(prmIndice), GetValor(prmIndice));

        public bool Fechar()
        {
            if (IsOK())
            { reader.Close(); }

            return (IsOK());
        }

        public bool IsOK()
            { return (erro == null); }


    }
    public class DataLocalConnection
    {

        private Object Origem;

        public DataPoolConnection Pool;
        
        public DataViewConnection View
        { get => Pool.DataViewCorrente; }

        public void Setup(Object prmOrigem, DataPoolConnection prmPool)
        {
            
            Origem = prmOrigem;

            Pool = prmPool;

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
            return(Pool.csv());
        }

        public string json()
        {
            return (Pool.json());
        }
    }
    public class IDataLocalConnection
    {

        private DataLocalConnection _Dados;

        public DataLocalConnection Dados
        {
            get
            {
                if (_Dados == null)
                    _Dados = new DataLocalConnection();

                return _Dados;

            }

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
    public class DataBaseOracle
    {

        private string modelo = @"Data Source=(DESCRIPTION =(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(Host = {0})(PORT = {1})))(CONNECT_DATA =(SERVICE_NAME = {2})));User ID={3};Password={4}";

        public string user = "desenvolvedor_sia";
        public string password = "asdfg";

        public string host = "10.250.1.35";
        public string port = "1521";
        public string service = "branch_1084.prod01.redelocal.oraclevcn.com";


        public string GetString()  => String.Format(modelo, host, port, service, user, password);

    }

    public class DataBasesConnection : List<DataBaseConnection>
    {

        public DataBaseConnection Corrente;

        public bool AddItem(string prmTag, string prmConexao, DataPoolConnection prmPool)
        {

            Corrente = new DataBaseConnection(prmTag, prmConexao, prmPool);

            Add(Corrente);

            return (Corrente.IsOK);

        }

    }
    public class DataViewsConnection : List<DataViewConnection>
    {

        public DataViewConnection Corrente;

        public bool AddItem(DataViewConnection prmView)
        {

            Corrente = prmView;

            Add(Corrente);

            return (prmView.IsOK);

        }

        public bool SetView(string prmTag)
        {

            foreach (DataViewConnection Visao in this)
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

            foreach (DataViewConnection Visao in this)
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

            foreach (DataViewConnection Visao in this)
                csv += Visao.csv() + Environment.NewLine;


            return (csv);

        }
        public string json()
        {

            string json = ""; string separador = "";

            foreach (DataViewConnection Visao in this)
            {

                json += separador + Visao.json(); separador = ", ";

            }

            return (json);

        }

    }

    public class DataModelsConnection : List<DataModelConnection>
    {

        public DataModelConnection Corrente;

        public void AddItem(DataModelConnection prmModel)
        {

            Corrente = prmModel;

            Add(prmModel);

        }

        public bool SetModel(string prmTag)
        {

            foreach (DataModelConnection Model in this)
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

}




