using System.Diagnostics;
using System.Data;
using System.Data.OracleClient;
using System.Collections.Generic;
using System;
using Oracle.ManagedDataAccess.Client;
using System.IO;
using Dooggy.Factory.Data;
using Dooggy.Lib.Generic;
using Dooggy.Lib.Parse;
using Dooggy.Factory;

namespace Dooggy.Lib.Data
{
    public class DataBaseConnection
    {

        public TestDataPool Pool;

        public string tag;

        public OracleConnection conexao;

        public Exception erro;

        public DataBaseConnection(string prmTag, string prmConexao, TestDataPool prmPool)
        {

            tag = prmTag;

            Pool = prmPool;

            Abrir(prmConexao);

        }

        public TestDataTrace Trace { get => (Pool.Trace); }
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

        public TestDataTrace Trace => DataBase.Trace;


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
    public class DataBasesConnection : List<DataBaseConnection>
    {

        public DataBaseConnection Corrente;

        public bool AddItem(string prmTag, string prmConexao, TestDataPool prmPool)
        {

            Corrente = new DataBaseConnection(prmTag, prmConexao, prmPool);

            Add(Corrente);

            return (Corrente.IsOK);

        }

    }

}




