using Dooggy.Factory.Data;
using Dooggy.Lib.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dooggy.Factory.Console
{
    public class TestDataVar
    {

        private DataBaseConnection DataBase;

        public string tag;

        public string valor;

        public string sql;

        public TestDataVar(string prmTag, string prmValor, DataBaseConnection prmDataBase)
        {

            DataBase = prmDataBase;

            tag = prmTag;

            valor = prmValor;

        }

    }

    public class TestDataVars : List<TestDataVar>
    {

        public TestDataPool Pool;

        public TestDataVar Corrente;

        private TestTrace Trace { get => Pool.Trace; }

        public TestDataVars(TestDataPool prmPool)
        {

            Pool = prmPool;

        }

        public string Criar(string prmTag, string prmValor, DataBaseConnection prmDataBase)
        {

            if (!Find(prmTag))
            {

                Corrente = new TestDataVar(prmTag, prmValor, prmDataBase);

                Add(Corrente);

            }

            return Corrente.tag;

        }

        public void SetArgumento(string prmArg, string prmInstrucao)
        {

            switch (prmArg)
            {

                case "sql":
                    Corrente.sql = prmInstrucao;
                    break;

            }

        }
        private bool Find(string prmTag)
        {

            foreach (TestDataVar var in this)

                if (xString.IsEqual(var.tag, prmTag))
                {

                    Corrente = var;

                    return (true);

                }

            return (false);

        }


    }

}


