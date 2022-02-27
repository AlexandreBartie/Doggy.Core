using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dooggy.Lib.Data
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
    }

    public class DataCommandVirtual
    {

        private OracleCommand query;

        public DataCommandVirtual(string prmSQL, DataBaseConnectVirtual prmConnect, int prmTimeOut)
        {
            query = new OracleCommand(prmSQL, prmConnect.GetConnection);

            query.CommandTimeout = prmTimeOut;
        }

        public DataReaderVirtual GetReader() => new DataReaderVirtual(query);

    }

    public class DataReaderVirtual
    {

        private OracleDataReader reader;

        private OracleCommand query;

        public DataReaderVirtual(OracleCommand prmQuery)
        {
            query = prmQuery;

            reader = query.ExecuteReader();
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
