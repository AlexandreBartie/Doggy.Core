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

        private DataBaseConnection DataBaseCorrente;
        private DataViewConnection DataViewCorrente;

        public DataViewConnection View
        { get => DataViewCorrente; }

        public bool AddDataBase(string prmTag, string prmConexao)
        {

            DataBaseCorrente = new DataBaseConnection(prmTag, prmConexao);

            Bases.Add(DataBaseCorrente);

            return (DataBaseCorrente.IsOK());

        }
        public bool AddDataView(string prmTag, string prmSql)
        {

            DataViewCorrente = new DataViewConnection(prmTag, prmSql, DataBaseCorrente);

            Visoes.Add(DataViewCorrente);

            return (DataViewCorrente.IsOK());

        }
        public bool SetView(string prmTag)
        {

            foreach (DataViewConnection Visao in Visoes)
            { 
                if (Visao.tag == prmTag)
                {

                    DataViewCorrente = Visao;

                    DataViewCorrente.Next();

                    return (true);

                }

            }

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

        public bool IsOK()
        { return (conexao.State == ConnectionState.Open); }

    }
    public class DataViewConnection
    {

        public string tag;

        public DataBaseConnection DataBase;

        public DataCursorConnection Cursor;

        public Exception erro
        { get => Cursor.erro; }

        public DataViewConnection(string prmTag, string prmSQL, DataBaseConnection prmDataBase)
        {

            tag = prmTag;

            DataBase = prmDataBase;

            Cursor = new DataCursorConnection(prmSQL, prmDataBase);

        }

        public bool Next()
        { return Cursor.Next(); }

        public string GetName(int prmIndice)
        { return Cursor.GetName(prmIndice); }
        public string GetValor(int prmIndice)
        { return Cursor.GetValor(prmIndice); }

        public string GetValor(string prmNome)
        { return Cursor.GetValor(prmNome); }

        public string GetJSon()
        { return Cursor.GetJSON(); }

        public bool Fechar()
        { return (Cursor.Fechar()); }

        public bool IsOK()
        { return (Cursor.IsOK()); }

    }

    public class DataCursorConnection
    {

        private DataBaseConnection DataBase;

        private SqlDataReader reader;

        public Exception erro;

        public DataCursorConnection(string prmSQL, DataBaseConnection prmDataBase)
        {

            DataBase = prmDataBase;

            Executar(prmSQL);

        }

        private bool Executar(string prmSQL)
        {

            erro = null;

            try
            {
                reader = GetReader(prmSQL);
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



    public class DataRowConnection
    {

/*        public bool Ler()
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

        }*/
    }

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




