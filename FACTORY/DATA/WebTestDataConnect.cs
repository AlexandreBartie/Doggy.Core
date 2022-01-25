using Dooggy.Lib.Parse;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dooggy.Factory.Data
{
    public class TestDataConnect
    {

        public TestDataPool Pool;

        private DataBaseOracle _Oracle;

        public int command_timeout = 20;

        public DataBaseOracle Oracle { get { if (_Oracle == null) _Oracle = new DataBaseOracle(this); return _Oracle; } }

        public TestDataConnect(TestDataPool prmPool)
        {
            Pool = prmPool;
        }

        public void SetCommandTimeOut(int prmSegundos) => command_timeout = prmSegundos;

    }
    public class DataBaseOracle : DataBaseOracleDefault
    {

        private string model = @"Data Source=(DESCRIPTION =(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(Host = {0})(PORT = {1})))(CONNECT_DATA =(SERVICE_NAME = {2})));User ID={3};Password={4};Connection Timeout={5}";

        public string user;
        public string password;

        public string host;
        public string port;
        public string service;

        public string timeout;

        public DataBaseOracle(TestDataConnect prmConexao)
        {

            Connect = prmConexao;

        }

        public bool Add(string prmTag) => Pool.AddDataBase(prmTag, GetString());

        public string GetString() => String.Format(model, host, port, service, user, password, timeout);

    }
    public class DataBaseOracleDefault
    {

        public TestDataConnect Connect;
        public TestDataPool Pool => Connect.Pool;

        private xJSON Args;

        public void AddJSON(string prmTag, string prmDados)
        {

            Args = new xJSON(prmDados);

            Connect.Oracle.host = Args.GetValor("host", prmPadrao: "10.250.1.35");
            Connect.Oracle.port = Args.GetValor("port", prmPadrao: "1521");

            Connect.Oracle.service = Args.GetValor("service");

            Connect.Oracle.user = Args.GetValor("user", prmPadrao: "desenvolvedor_sia");
            Connect.Oracle.password = Args.GetValor("password", prmPadrao: "asdfg");

            Connect.Oracle.timeout = Args.GetValor("timeout", prmPadrao: "10");

            Connect.Oracle.Add(prmTag.ToUpper());

        }

        private string GetBranch(string prmBranch) => GetStage(prmStage: string.Format("branch_{0}", prmBranch));
        private string GetStage(string prmStage) => prmStage + ".prod01.redelocal.oraclevcn.com";

    }

}
