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

        public string str_conection;

        public OracleConnection Conexao;

        public Exception erro;

        public DataBaseConnection(string prmTag, string prmConexao, TestDataPool prmPool)
        {

            tag = prmTag;

            Pool = prmPool;

            str_conection = prmConexao;

            Abrir();

        }

        public TestTrace Trace => (Pool.Trace);

        public bool IsON => (Conexao != null);
        public bool IsOK { get { if (IsON) return (IsOpen); return (false); } }
        private bool IsOpen => (Conexao.State == ConnectionState.Open);

        public bool Testar() => Abrir();
        private bool Conectar()
        {

            try
            {

                Conexao = new OracleConnection(str_conection);

                return (true);

            }

            catch (Exception e)
            { Trace.LogData.FailDBConnection(tag, str_conection, e); erro = e; }

            return (false);
        }
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

                if (Conectar())
                    Conexao.Open();

                Trace.LogData.DBConnection(tag, "CONECTADO");

                return (true);

            }

            catch (Exception e)
            { Trace.LogData.FailDBConnection(tag, str_conection, e); erro = e; }

            return (false);
        }
        public void Fechar()
        {
            try
            { Conexao.Close(); }

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

        public bool TemDados;

        public TestTrace Trace => DataBase.Trace;


        public DataCursorConnection(string prmSQL, string prmMask, DataBaseConnection prmDataBase)
        {

            DataBase = prmDataBase;

            string sql = GetTratarSQL(prmSQL);

            if (DataBase.IsOK)
            {
                GetReader(sql); SetMask(prmMask);
            }
            else
                { Trace.LogData.FailSQLNoDataBaseConnection(DataBase.tag, sql, DataBase.erro); erro = DataBase.erro; }
        }
        private void SetMask(string prmMask)
        {
            if (prmMask != "")
                Mask = new xMask(prmMask);
        }

        private void GetReader(string prmSQL)
        {

            _sql = prmSQL;

            try
            {
                OracleCommand vlSql = new OracleCommand(prmSQL, DataBase.Conexao);

                reader = (vlSql.ExecuteReader());

                TemDados = Start();

                Trace.LogData.SQLExecution(DataBase.tag, prmSQL, TemDados);

            }
            catch (Exception e)
            { Trace.LogData.FailSQLConnection(DataBase.tag, prmSQL, e); erro = e; }

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

        private string GetTratarSQL(string prmSQL)
        {

            string sql = Bloco.GetBlocoTroca(prmSQL, prmDelimitadorInicial: "<##>", prmDelimitadorFinal: "<##>", prmDelimitadorNovo: "'");
            
            return (sql); 

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

        public bool IsOK => GetIsOK();

        public bool Criar(string prmTag, string prmConexao, TestDataPool prmPool)
        {

            Corrente = new DataBaseConnection(prmTag, prmConexao, prmPool);

            Add(Corrente);

            return (Corrente.IsOK);

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

    }

}




