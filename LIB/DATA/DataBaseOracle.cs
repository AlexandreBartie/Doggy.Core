using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Text;

namespace BlueRocket.CORE.Lib.Data
{

    public class DataBaseConnectOracle
    {
        public OracleConnection Conexao;
    }
    public class DataCursorBaseOracle
    {
        public OracleDataReader reader;
    }
}
