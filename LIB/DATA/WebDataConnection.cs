using System.Diagnostics;
using System.Data;
using System.Data.OracleClient;
using System.Collections.Generic;
using System;
using Dooggy.KERNEL;
using Oracle.ManagedDataAccess.Client;
using Dooggy.LIB;

namespace Dooggy
{
    public class DataPoolConnection
    {

        public TestTraceDataBase Trace;

        private List<DataBaseConnection> Bases = new List<DataBaseConnection>();
        private List<DataViewConnection> Visoes = new List<DataViewConnection>();

        public DataBaseConnection DataBaseCorrente;
        public DataViewConnection DataViewCorrente;

        public DataModelConnection DataModelCorrente;
        public DataVariantConnection DataVariantCorrente;
       
        public DataPoolConnection(TestTraceDataBase prmTrace)
        {

            Trace = prmTrace;

        }

        public bool AddDataBase(string prmTag, string prmConexao)
        {

            DataBaseCorrente = new DataBaseConnection(prmTag, prmConexao, Trace);

            Bases.Add(DataBaseCorrente);

            return (DataBaseCorrente.IsOK);

        }
        public bool AddDataView(string prmTag, string prmSQL)
        {

            DataViewCorrente = new DataViewConnection(prmTag, prmSQL, DataBaseCorrente);

            Visoes.Add(DataViewCorrente);

            return (DataViewCorrente.IsOK());

        }
        public bool AddDataModel(string prmTag, string prmModel)
        {

            DataModelCorrente = new DataModelConnection(prmTag, prmModel, DataBaseCorrente);

            return (DataBaseCorrente != null);

        }
        public bool AddDataVariant(string prmTag)
        {

            return (AddDataVariant(prmTag, prmRegra: "", prmQtde: 1));

        }
        public bool AddDataVariant(string prmTag, string prmRegra)
        {

            return (AddDataVariant(prmTag, prmRegra, prmQtde: 1));

        }
        public bool AddDataVariant(string prmTag, string prmRegra, int prmQtde)
        {

            DataVariantCorrente = new DataVariantConnection(prmTag, prmRegra, DataModelCorrente);

            string ViewSQL = DataVariantCorrente.CriarSQL(prmQtde);

            return (AddDataView(DataVariantCorrente.tag_extendida, prmSQL: ViewSQL));

        }
        public bool SetView(string prmTag)
        {

            foreach (DataViewConnection Visao in Visoes)
            {

                if (Visao.tag == prmTag)
                {

                    DataViewCorrente = Visao;

                    return (true);

                }

            }

            return (false);

        }

        public string GetView(string prmTag)
        {
            if (SetView(prmTag))
                return DataViewCorrente.GetJSon();

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
        public string memo()
        {

            string memo = "";

            foreach (DataViewConnection Visao in Visoes)
                memo += Visao.memo() + Environment.NewLine;

            
            return (memo);
        
        }

    }
    public class DataBaseConnection
    {

        public string tag;

        public TestTraceDataBase Trace;

        public OracleConnection conexao;

        public Exception erro;

        public DataBaseConnection(string prmTag, string prmConexao, TestTraceDataBase prmTrace)
        {

            tag = prmTag;

            Trace = prmTrace;

            Abrir(prmConexao);

        }


        public bool IsON { get => (conexao != null); }
        public bool IsOK { get { if (IsON) return (conexao.State == ConnectionState.Open); return (false); } }

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

        public xJSON JSON;

        public DataBaseConnection DataBase;

        public DataModelConnection(string prmTag, string prmModel, DataBaseConnection prmDataBase)
        {

            tag = prmTag;

            DataBase = prmDataBase;

            JSON = new xJSON(prmModel);

        }

        public string GetSELECT(int prmQtde) => (string.Format("SELECT {0} FROM {1}", GetListaCampos() , GetListaTabelas()));

        private string GetListaTabelas() => (TratarSQL(JSON.GetValor("#ENTIDADES#")));

        private string GetListaCampos() => (TratarSQL(JSON.GetValor("#ATRIBUTOS#", prmPadrao: "*")));

        public string GetSQL(string prmSQL, int prmQtde) => (string.Format("SELECT * FROM ({0}) WHERE ROWNUM = {1}", prmSQL, prmQtde));

        private string TratarSQL(string prmLista) => new xMemo(prmLista, prmSeparador: "+").memo(", ");

    }
    public class DataVariantConnection
    {

        public string tag;

        private xJSON JSON;

        private string regra;

        public DataModelConnection Modelo;

        public DataVariantConnection(string prmTag, string prmRegra, DataModelConnection prmModelo)
        {

            tag = prmTag;

            Modelo = prmModelo;

            if (prmRegra.Trim() != "")
                JSON = new xJSON(prmRegra);

        }
        public string tag_extendida { get => Modelo.tag + tag; }    
        public bool IsRegraOK { get => JSON != null; }

        public string CriarSQL(int prmQtde)
        {

            string sql = Modelo.GetSELECT(prmQtde);

            if (IsRegraOK)
                sql += " " + GetExtensaoSQL();

            return (Modelo.GetSQL(sql,prmQtde));
            
        }
        private string GetExtensaoSQL()
        {
            return (string.Format("{0} {1}", GetWHERE(), GetORDERBY()).Trim());
        }
        private string GetWHERE()
        {
            return (GetRegras("WHERE {0}"));
        }
        private string GetORDERBY()
        {
            return (GetOrdenacao("ORDER BY {0}"));
        }
        private string GetRegras(string prmFormato)
        {
           
            return (JSON.FindValor("#REGRAS#", prmFormato));
        }
        private string GetOrdenacao(string prmFormato)
        {
            return (JSON.FindValor("#ORDEM#", prmFormato));
        }
    }
    public class DataViewConnection
    {

        public string tag;

        public DataBaseConnection DataBase;

        public DataCursorConnection Cursor;

        public string sql { get => Cursor.sql; }
        public Exception erro { get => Cursor.erro; }

        public DataViewConnection(string prmTag, string prmSQL, DataBaseConnection prmDataBase)
        {

            tag = prmTag;

            DataBase = prmDataBase;

            Cursor = new DataCursorConnection(prmSQL, prmDataBase);

        }

        public bool Next() => Cursor.Next();

        public string GetName(int prmIndice) => Cursor.GetName(prmIndice);

        public string GetValor(int prmIndice) => Cursor.GetValor(prmIndice);

        public string GetValor(string prmNome) => Cursor.GetValor(prmNome);

        public string GetJSon() => Cursor.GetJSON();

        public bool Fechar() => Cursor.Fechar();

        public bool IsOK() => Cursor.IsOK();

        public string memo() => String.Format("VIEW:[{0,25}] SQL: {1}", tag, sql);
        public string data() => String.Format("DATA:[{0,25}] Fluxo: {1}", tag, GetJSon());

    }
    public class DataCursorConnection
    {

        private DataBaseConnection DataBase;

        private OracleDataReader reader;

        public Exception erro;

        private string _sql;

        public string sql { get => _sql; }

        public TestTraceDataBase Trace => DataBase.Trace;


        public DataCursorConnection(string prmSQL, DataBaseConnection prmDataBase)
        {

            DataBase = prmDataBase;

            GetReader(prmSQL);

        }

        public void GetReader(string prmSQL)
        {

            _sql = prmSQL;

            try
            {
                OracleCommand vlSql = new OracleCommand(prmSQL, DataBase.conexao);

                reader = (vlSql.ExecuteReader());

                Start();

                Trace.SQLExecution(DataBase.tag, prmSQL);

            }
            catch (Exception e)
            { Trace.FailSQLConnection(DataBase.tag, prmSQL, e); erro = e; }

        }
        private bool Start() => Next();
        public bool Next() => reader.Read();
        public string GetName(int prmIndice)
        {

            return reader.GetName(prmIndice);

        }
        public string GetValor(int prmIndice)
        {

            return reader.GetOracleValue(prmIndice).ToString();

        }
        public string GetValor(string prmNome)
        {

            return reader.GetString(prmNome);

        }
        public string GetJSON()
        {
            string memo = "";
            string separador = "";

            for (int cont = 0; cont < reader.VisibleFieldCount; cont++)
            {
                memo += separador + GetJSon(cont);

                separador = ", ";
            }

            return ("{ " + memo + " }");
        }
        public string GetJSon(int prmIndice)
        {

            return string.Format("'{0}': '{1}'", GetName(prmIndice), GetValor(prmIndice));

        }
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
            return (Pool.GetView(prmTag));
        }
        public string memo()
        {
            return(Pool.memo());
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
    /*


        public class DataRowConnection
        {

    *//*        public bool Ler()
            { return reader.Read(); }

            public string GetName(int prmIndice)
            {

                return reader.GetName(prmIndice);

            }
            public string GetValor(int prmIndice)
            {

                return reader.GetSqlValue(prmIndice).ToString();

            }
            public string GetValor(string prmNome)
            {

                return reader.GetString(prmNome);

            }
            public string GetJSon(int prmIndice)
            {

                return string.Format("'{0}': '{1}'", GetName(prmIndice), GetValor(prmIndice));

            }*//*
        }*/

    //private CursorDados _cursor;

    //    private _cursor As CursorDados
    //        private _tabelas As colTabelaGenerica
    //        Private ReadOnly Property Tabelas As colTabelaGenerica
    //            Get
    //                If IsNothing(_tabelas) Then
    //                    _tabelas = New colTabelaGenerica(Me)
    //                End If
    //                Return _tabelas
    //            End Get
    //        End Property
    //    public CursorDados Cursor;
    //    {
    //        get
    //        {
    //            if(_cursor == null)
    //                { _cursor = New CursorDados(conexao);
    //}
    //             return _cursor;
    //        }
    //    }


    //private _schema As CursorSchema;

    //            public ReadOnly Property Schema As CursorSchema
    //    Get
    //        If IsNothing(_schema) Then
    //            _schema = New CursorSchema(Me)
    //        End If
    //        Return _schema
    //    End Get
    //End Property
    //Public ReadOnly Property erro As Exception
    //    Get
    //        erro = _erro
    //    End Get
    //End Property
    //
    //

}




