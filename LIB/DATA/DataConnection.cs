using System.Diagnostics;
using System.Data;
using System.Data.OracleClient;
using System.Collections.Generic;
using System;
using Oracle.ManagedDataAccess.Client;
using System.IO;
using Dooggy.Factory.Data;
using Dooggy;
using Dooggy.Lib.Parse;
using Dooggy.Factory;
using Dooggy.Lib.Generic;

namespace Dooggy.Lib.Data
{

    public class DataCursorConnection : DataCursorDados
    {

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
        
        public DataBaseConnection DataBase;

        public TestDataTratamento Tratamento => DataBase.Pool.Tratamento;

        private DataTypesField DataTypes => DataBase.DataTypes;

        public OracleDataReader reader;

        private xMask Mask;
        public bool IsMask { get => (Mask != null); }

        public bool TemDados;

        public void SetMask(string prmMask)
        {
            if (prmMask != "")
                Mask = new xMask(prmMask);
        }

        public bool IsDBNull(int prmIndice) => reader.IsDBNull(prmIndice);
        public string GetName(int prmIndice) => reader.GetName(prmIndice);
        public string GetType(int prmIndice) => reader.GetDataTypeName(prmIndice).ToLower();
        public string GetValor(int prmIndice) => GetValorTratado(prmIndice);
        public string GetValor(string prmCampo) => GetValorTratado(prmCampo);

        private string GetValorTratado(string prmCampo) => GetValorTratado(prmIndice: reader.GetOrdinal(prmCampo));
        private string GetValorTratado(int prmIndice)
        {
            string tipo = GetType(prmIndice);

            string campo = GetName(prmIndice);

            if (DataTypes.IsTypeDate(tipo))
                return GetMaskDate(campo, prmDate: reader.GetDateTime(prmIndice));

            if (DataTypes.IsTypeDouble(tipo))
               return GetMaskDouble(campo, prmNumber: reader.GetDouble(prmIndice));

            return GetMask(campo, prmText: reader.GetOracleValue(prmIndice).ToString());
        }

        private string GetMask(string prmCampo, string prmText)
        {
            if (IsMask)
                return Tratamento.GetTextFormat(prmText, Mask.GetFormat(prmCampo));

            return (prmText);
        }
        private string GetMaskDate(string prmCampo, DateTime prmDate)
        {
            if (IsMask)
                return Tratamento.GetDateFormat(prmDate, Mask.GetFormat(prmCampo)); 

            return (Tratamento.GetDateFormat(prmDate));
        }
        private string GetMaskDouble(string prmCampo, Double prmNumber)
        {
            if (IsMask)
                return Tratamento.GetDoubleFormat(prmNumber, Mask.GetFormat(prmCampo));

            return (Tratamento.GetDoubleFormat(prmNumber));
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

        private string status;

        public string stringConnection;

        private string baseConnection;


        public OracleConnection Conexao;

        public Exception erro;

        private bool _isOpen;

        public TestTrace Trace => (Pool.Trace);
        public DataTypesField DataTypes => Pool.DataTypes;

        public DataBaseConnection(string prmTag, string prmConexao, TestDataPool prmPool)
        {
            tag = prmTag;

            Pool = prmPool;

            baseConnection = prmConexao;
        }

        public bool IsOK => _isOpen;
        public string log => string.Format("-db[{0}]: {1}", tag, status);

        private string SetStatus(string prmStatus) { status = prmStatus; _isOpen = (prmStatus == "CONECTADO");  return prmStatus; }

        public string GetStatus() { if (status == "") Connect(); return status; }

        public bool Connect()
        {

            stringConnection = Pool.Connect.GetFullConnection(baseConnection);

            // used to do unit-testing ...
            if (Pool.IsDbBlocked)
            {
                Trace.LogData.FailDBBlocked(tag, stringConnection);

                return (false);
            }

            try
            {

                int x = Pool.Connect.command_timeout;

                Conexao = new OracleConnection(stringConnection);

                Conexao.Open();

                Trace.LogData.DBConnection(tag, SetStatus("CONECTADO"));

                return (true);

            }

            catch (Exception e)
            { Trace.LogData.FailDBConnection(tag, stringConnection, e); erro = e; SetStatus("ERRO"); }

            return (false);
        }
        public void Close()
        {
            try
            { Conexao.Close(); SetStatus("FECHADO");  }

            catch (Exception e)
            { erro = e; }

        }

    }
    public class DataBasesConnection : List<DataBaseConnection>
    {

        public DataBaseConnection Corrente;

        private bool IsConnected;

        public DataTypesField DataTypes;

        public bool IsOK => GetIsOK();

        public DataBasesConnection(DataTypesField prmDataTypes)
        {
            DataTypes = prmDataTypes;
        }
        public bool Criar(string prmTag, string prmConexao, TestDataPool prmPool)
        {
            Corrente = new DataBaseConnection(prmTag, prmConexao, prmPool);

            Add(Corrente);

            return (Corrente.IsOK);
        }
        public bool DoConnect()
        {
            IsConnected = true;

            foreach (DataBaseConnection db in this)
                if (!db.Connect())
                    return false;

            return IsConnected;
        }

        private bool GetIsOK()
        {

            if (!IsConnected)
                DoConnect();

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
            xMemo lista = new xMemo(prmSeparador: ", ");

                foreach (DataBaseConnection db in this)
                lista.Add(db.log);

            return ">dbase: " + lista.txt();
        }

    }

    public class DataTypesField
    {

        private myDominio TypesDate;
        private myDominio TypesDouble;

        public DataTypesField()
        {
            TypesDate = new myDominio(prmLista: "date");
            TypesDouble = new myDominio(prmLista: "double");
        }

        public void SetTypesDate(string prmTypes)
        {
            myDominio TypesDate = new myDominio(prmTypes);
        }
        public void SetTypesNumber(string prmTypes)
        {
            myDominio TypesNumber = new myDominio(prmTypes);
        }

        public bool IsTypeDate(string prmType) => TypesDate.IsContem(prmType);
        public bool IsTypeDouble(string prmType) => TypesDouble.IsContem(prmType);

    }
}




