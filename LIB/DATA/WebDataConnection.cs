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

    public class DataCursorConnection : DataCursorDados
    {

        private DataBaseConnection DataBase;

        public Exception erro;

        private string _sql;

        public string sql => _sql;

        public bool IsOK => (erro == null);

        public TestTrace Trace => DataBase.Trace;

        public DataCursorConnection(string prmSQL, string prmMask, DataBaseConnection prmDataBase)
        {

            DataBase = prmDataBase;

            string sql = GetTratarSQL(prmSQL);

            if (DataBase.IsOK)
            {
                SetQuery(sql); SetMask(prmMask);
            }
            else
            { Trace.LogData.FailSQLNoDataBaseConnection(DataBase.tag, sql, DataBase.erro); erro = DataBase.erro; }
        }

        private void SetQuery(string prmSQL)
        {
            _sql = prmSQL;

            if (GetReader(prmTimeOut: DataBase.Pool.Connect.command_timeout))
            {
                TemDados = Next();

                Trace.LogData.SQLExecution(DataBase.tag, sql, TemDados);
            }
            else
                Trace.LogData.FailSQLConnection(DataBase.tag, sql, erro);
        }
        
        private bool GetReader(int prmTimeOut)
        {
            try
            {
                OracleCommand query = new OracleCommand(sql, DataBase.Conexao);

                query.CommandTimeout = prmTimeOut;

                reader = query.ExecuteReader();

                return (true);
            }
            catch (Exception e)
            { erro = e; }

            return (false);
        }

        public bool Next() => reader.Read();

        public bool Fechar()
        {
            if (IsOK)
            { reader.Close(); }

            return (IsOK);
        }

        private string GetTratarSQL(string prmSQL) => Bloco.GetBlocoTroca(prmSQL, prmDelimitadorInicial: "<##>", prmDelimitadorFinal: "<##>", prmDelimitadorNovo: "'");

    }

    public class DataCursorDados
    {

        public OracleDataReader reader;

        private xMask Mask;
        public bool IsMask { get => (Mask != null); }

        public bool TemDados;

        public void SetMask(string prmMask)
        {
            if (prmMask != "")
                Mask = new xMask(prmMask);
        }

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
        public string GetCSV(string prmSeparador)
        {
            string memo = "";
            string texto = "";
            string separador = "";

            if (TemDados)
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

            return memo;

        }
        public string GetJSON()
        {
            string memo = "";
            string separador = "";

            if (TemDados)
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

    }
    public class DataBaseConnection
    {

        public TestDataPool Pool;

        public string tag;

        public string status;

        public string str_conection;

        public OracleConnection Conexao;

        public Exception erro;

        private bool _isOpen;

        public TestTrace Trace => (Pool.Trace);

        public DataBaseConnection(string prmTag, string prmConexao, TestDataPool prmPool)
        {

            tag = prmTag;

            Pool = prmPool;

            str_conection = prmConexao;

            Abrir();

        }

        public bool IsOK => _isOpen;
        public int command_timeout => Pool.Connect.command_timeout;
        public string log => string.Format("{0}: {1}", tag, status);

        private string SetStatus(string prmStatus) { status = prmStatus; _isOpen = (prmStatus == "CONECTADO");  return prmStatus; }

        public bool Testar() => Abrir();

        public bool Abrir()
        {

            // used to do unit-testing ...
            if (Pool.IsDbBlocked)
            {
                Trace.LogData.FailDBBlocked(tag, str_conection);

                return (false);
            }

            try
            {

                Conexao = new OracleConnection(str_conection);

                Conexao.Open();

                Trace.LogData.DBConnection(tag, SetStatus("CONECTADO"));

                return (true);

            }

            catch (Exception e)
            { Trace.LogData.FailDBConnection(tag, str_conection, e); erro = e; SetStatus("ERRO"); }

            return (false);
        }
        public void Fechar()
        {
            try
            { Conexao.Close(); status = "FECHADO";  }

            catch (Exception e)
            { erro = e; }

        }

    }
    public class DataBasesConnection : List<DataBaseConnection>
    {

        public DataBaseConnection Corrente;

        public bool IsOK => GetIsOK();

        public bool Criar(string prmTag, string prmConexao, TestDataPool prmPool)
        {

            Corrente = new DataBaseConnection(prmTag, prmConexao, prmPool);

            Add(Corrente);

            return (Corrente.IsOK);

        }
        public bool DoConnect()
        {

            foreach (DataBaseConnection db in this)
                if (!db.Abrir())
                    return false;

            return true;

        }
        public bool Testar()
        {

            foreach (DataBaseConnection db in this)
                if (!db.Testar())
                    return false;

            return true;

        }

        private bool GetIsOK()
        {

            bool ok = false;

            foreach (DataBaseConnection db in this)
                if (db.IsOK)
                    ok = true;
                else
                    break;

            return ok;

        }
        public string log()
        {

            string memo = "";

            foreach (DataBaseConnection db in this)
                memo += db.log + Environment.NewLine;

            return memo;

        }

    }



}




