using System.Diagnostics;
using System.Data.SqlClient;
using System.Data;
using System.Collections.Generic;
using System;

namespace MeuSeleniumCSharp
{
    public class DataPoolConnection
    {

        private List<DataBaseConnection> Bases = new List<DataBaseConnection>();
        private List<DataViewConnection> Visoes = new List<DataViewConnection>();

        public DataBaseConnection DataBaseCorrente;
        public DataViewConnection DataViewCorrente;

        public DataModelConnection DataModelCorrente;
        public DataVariantConnection DataVariantCorrente;

        public bool AddDataBase(string prmTag, string prmConexao)
        {

            DataBaseCorrente = new DataBaseConnection(prmTag, prmConexao);

            Bases.Add(DataBaseCorrente);

            return (DataBaseCorrente.IsOK());

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
        public bool AddDataVariant(string prmTag, string prmVariacao)
        {

            return (AddDataVariant(prmTag, prmVariacao, prmQtde: 1));

        }
        public bool AddDataVariant(string prmTag, string prmVariacao, int prmQtde)
        {

            DataVariantCorrente = new DataVariantConnection(prmTag, prmVariacao, DataModelCorrente);

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

            Debug.Assert(false);

            return (false);

        }
        public bool IsOk()
        {

            bool retorno = false;

            foreach (DataBaseConnection Base in Bases)
            {
                if (!Base.IsOK())
                { return (false); }

                retorno = true;
            }

            return (retorno);

        }
        public string memo()
        {

            string memo = "";

            foreach (DataBaseConnection Base in Bases)
                memo += Base.memo() + Environment.NewLine;

            foreach (DataViewConnection Visao in Visoes)
                memo += Visao.memo() + Environment.NewLine;

            
            return (memo);
        
        }

    }
    public class DataBaseConnection
    {

        public string tag;

        public SqlConnection conexao;

        public Exception erro;

        public DataBaseConnection(string prmTag, string prmConexao)
        {

            tag = prmTag;

            Abrir(prmConexao);
        }

        public bool Abrir(string prmConexao)
        {

            conexao = new SqlConnection(prmConexao);

            try
            { conexao.Open(); }

            catch (Exception e)
            { erro = e; }

            return (IsOK());
        }

        //public DataCursorConnection GetCursor(string prmTag, string prmSQL)
        //{

        //    return (new DataViewConnection(prmTag, prmSQL, this));

        //}

        //public DataViewConnection GetView(string prmTag)
        //{

        //    foreach (DataViewConnection Visao in Visoes)
        //    {
        //        if (Visao.tag == prmTag)
        //        { return (Visao); }
        //    }

        //    return (null);

        //}

        public void Fechar()
        {
            try
            { conexao.Close(); }

            catch (Exception e)
            { erro = e; }

        }

        public bool IsOK() => (conexao.State == ConnectionState.Open);

        public string memo() => (String.Format("BASE:[{0,25}] CONEXÃO: {1}", tag, conexao));

    }
    public class DataModelConnection
    {

        public string tag;

        public xJSON JSON;

        public DataBaseConnection DataBase;

        public DataModelConnection(string prmTag, string prmModelo, DataBaseConnection prmDataBase)
        {

            tag = prmTag;

            DataBase = prmDataBase;

            JSON = new xJSON(prmModelo);

        }

        public string GetSQL(int prmQtde) => (GetSELECT(prmQtde) + " " + GetFROM());

        private string GetSELECT(int prmQtde) => (string.Format("SELECT {0} {1}", GetLinhas(prmQtde), GetListaCampos()));

        private string GetFROM() => (string.Format("FROM {0}", GetListaTabelas()));

        private string GetListaTabelas() => (TratarSQL(JSON.GetValor("#ENTIDADES#")));

        private string GetListaCampos() => (TratarSQL(JSON.GetValor("#ATRIBUTOS#", prmPadrao: "*")));

        private string GetLinhas(int prmQtde) => (string.Format("TOP {0}", prmQtde));

        private string TratarSQL(string prmLista) => (new xLista(prmLista, prmSeparador: "+").memo(", "));

    }
    public class DataVariantConnection
    {

        public string tag;

        public xJSON JSON;

        public DataModelConnection Modelo;

        public DataVariantConnection(string prmTag, string prmVariacao, DataModelConnection prmModelo)
        {

            tag = prmTag;

            Modelo = prmModelo;

            JSON = new xJSON(prmVariacao);

        }
        public string tag_extendida
        { get => Modelo.tag + tag; }

        public string CriarSQL(int prmQtde)
        {
            return (string.Format("{0} {1}", Modelo.GetSQL(prmQtde), GetExtensaoSQL()));
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

    }
    public class DataCursorConnection
    {

        private DataBaseConnection DataBase;

        private SqlDataReader reader;

        public Exception erro;

        private string _sql;

        public string sql
        { get => _sql; }

        public DataCursorConnection(string prmSQL, DataBaseConnection prmDataBase)
        {

            DataBase = prmDataBase;

            _sql = prmSQL; 

            Executar();

        }

        private bool Executar()
        {

            erro = null;

            try
            {
                reader = GetReader(sql);

                Next();
            }
            catch (Exception e)
            {

                erro = e;
            }

            return (IsOK());

        }
        public SqlDataReader GetReader(string prmSQL)
        {

            try
            {
                SqlCommand vlSql = new SqlCommand(prmSQL, DataBase.conexao);

                return (vlSql.ExecuteReader());

            }
            catch (Exception e)
            { erro = e; }

            return (null);

        }

        public bool Next()
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
            AddDataVariant(prmTag, prmVariacao: "");
        }
        public void AddDataVariant(string prmTag, string prmVariacao)
        {
            Pool.AddDataVariant(prmTag, prmVariacao);
        }
        public bool SetView(string prmTag)
        { 
            return (Pool.SetView(prmTag)); 
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




