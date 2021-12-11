using System;
using System.Collections.Generic;
using System.Text;

namespace Dooggy.Factory.Data
{
    public class TestDataConnect
    {

        private TestDataPool Pool;

        private DataBaseOracle _Oracle;

        public DataBaseOracle Oracle { get { if (_Oracle == null) _Oracle = new DataBaseOracle(Pool); return _Oracle; } }

        public TestDataConnect(TestDataPool prmPool)
        {

            Pool = prmPool;

        }


    }
    public class DataBaseOracle
    {

        private TestDataPool Pool;

        private string model = @"Data Source=(DESCRIPTION =(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(Host = {0})(PORT = {1})))(CONNECT_DATA =(SERVICE_NAME = {2})));User ID={3};Password={4}";

        public string user;
        public string password;

        public string host;
        public string port;
        public string service;

        public DataBaseOracle(TestDataPool prmPool)
        {

            Pool = prmPool;

        }

        public DataBaseOracle()
        {
        }

        public bool Add(string prmTag) => Pool.AddDataBase(prmTag, GetString());

        public string GetString() => String.Format(model, host, port, service, user, password);

    }
}
