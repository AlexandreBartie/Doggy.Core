using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Text;

namespace BlueRocket.CORE.Lib.Data
{
    public class DataBaseConnectVirtual
    {
        private OracleConnection conexao;

        public DataBaseConnectVirtual(string prmConnection)
        {
            conexao = new OracleConnection(prmConnection);
        }

        public OracleConnection GetConnection => conexao;

        public void Open() => conexao.Open();

        public void Close() => conexao.Close();

        public bool Execute(string prmNoSQL, int prmTimeOut)
        {
            DataCommandVirtual Command = new DataCommandVirtual(prmNoSQL, this, prmTimeOut);

            return Command.GetNoResults();
        }

    }

    public class DataCommandVirtual
    {

        private OracleCommand command;

        public DataCommandVirtual(string prmSQL, DataBaseConnectVirtual prmConnect, int prmTimeOut)
        {
            command = new OracleCommand(prmSQL, prmConnect.GetConnection);

            command.CommandTimeout = prmTimeOut;
        }

        public bool GetNoResults()
        {
            int result = command.ExecuteNonQuery();

            return result == -1;
        }

        public DataReaderVirtual GetReader() => new DataReaderVirtual(command);

    }

    public class DataReaderVirtual
    {

        private OracleDataReader reader;

        private OracleCommand command;

        public DataReaderVirtual(OracleCommand prmCommand)
        {
            command = prmCommand;

            reader = command.ExecuteReader();
        }

        public bool IsDBNull(int prmIndice) => reader.IsDBNull(prmIndice);

        public int GetIndex(string prmName) => reader.GetOrdinal(prmName);
        public string GetName(int prmIndice) => reader.GetName(prmIndice);
        public string GetType(int prmIndice) => reader.GetDataTypeName(prmIndice).ToLower();
        public DateTime GetDateTime(int prmIndice) => reader.GetDateTime(prmIndice);
        public Double GetDouble(int prmIndice) => reader.GetDouble(prmIndice);
        public string GetString(int prmIndice) => reader.GetOracleValue(prmIndice).ToString();

        public int GetFieldCount => reader.VisibleFieldCount;


        public bool Next() => reader.Read();
        public void Close() => reader.Close();

    }


}
