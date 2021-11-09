using System.Diagnostics;
using System.Data.SqlClient;
using System.Data;
using System;

namespace MeuSeleniumCSharp
{
    class WebDataConection
    {

        private SqlDataAdapter adapter = new SqlDataAdapter();

    }

    public class BaseDados
    {

        public SqlConnection conexao;

        public CursorDados cursor;

        public Exception erro;

        public BaseDados(string prmConexao)
        {
            SetConexao(prmConexao);
        }

        public bool SetConexao(string prmConexao)
        {

            conexao = new SqlConnection(prmConexao);

            try
            { conexao.Open(); }

            catch (Exception e)
            { erro = e; }

            return (IsOK());
        }

        public bool Executar(string prmSQL)
        {

            cursor = new CursorDados(conexao);

            return (cursor.Executar(prmSQL)); 
        
        }

        public bool IsOK()
        { return (conexao.State == ConnectionState.Open); }

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
        public class CursorDados
        {

        public SqlConnection conexao;

        public Exception erro;
        public SqlDataReader reader;

        public CursorDados(SqlConnection prmConexao)
        {

            conexao = prmConexao;
        }

        public bool Executar(string prmSQL)
        {

            reader = null; erro = null;

            try
            {
                SqlCommand vlSql = new SqlCommand(prmSQL, conexao);

                reader = vlSql.ExecuteReader();
            }
            catch (Exception e)
            {

                Debug.Assert(false);

                erro = e;
            }

            return (IsOK());

        }
        public bool Ler()
         { return reader.Read(); }

        public Object GetValor(string prmNome)
        {

            return ("xx"); // reader.GetFieldValue(prmNome));

        }
        public Object GetValor(int prmIndice)
        {

            return reader[prmIndice];

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

}




