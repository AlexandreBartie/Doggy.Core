using BlueRocket.CORE.Factory.Data;
using BlueRocket.CORE.Lib.Data;
using BlueRocket.CORE.Lib.Generic;
using BlueRocket.CORE.Lib.Vars;
using System;
using System.Collections.Generic;
using System.Text;

namespace BlueRocket.CORE.Factory.Data
{

    public class TestDataTag : myDominio
    {

        public TestDataTag(string prmKey, string prmLista) : base(prmKey, prmLista)
        { }

    }

    public class TestDataTags : myDominios
    {

        private string key_start = "[";
        private string key_finish = "]";
        public new void AddItem(string prmTag)
        {
            string key = Bloco.GetBloco(prmTag, key_start, key_finish);
            string lista = Bloco.GetBlocoDepois(prmTag, key_finish,prmTRIM: true);

            if (myString.IsFull(key) && myString.IsFull(lista))
                AddItem(key, lista);
        }
    
    }

    public class TestDataVar
    {

        //private DataBaseConnection DataBase;

        public string tag;

        public string valor;

        public string sql;

        public TestDataVar(string prmTag, string prmValor)//', DataBaseConnection prmDataBase)
        {
            //DataBase = prmDataBase;

            tag = prmTag;

            valor = prmValor;
        }

    }

    public class TestDataVars : List<TestDataVar>
    {

       // public TestDataPool Pool;

        public TestDataVar Corrente;

       // private TestTrace Trace { get => Pool.Trace; }

        //public TestDataVars(TestDataPool prmPool)
        //{
        //    Pool = prmPool;
        //}

        public string Criar(string prmVar)//, DataBaseConnection prmDataBase)
        {

            //
            // Identidica TAG e VALOR da variável 
            //
            
            string tag = myString.GetFirst(prmVar, prmDelimitador: "=").Trim();
            string valor = myString.GetLast(prmVar, prmDelimitador: "=").Trim();

            //
            // Verifica se a variável já existe ...
            //

            if (!Find(tag, valor))
            {

                Corrente = new TestDataVar(tag, valor);//, prmDataBase);

                Add(Corrente);

            }

            return Corrente.tag;

        }
        public string GetValor(string prmTag)
        {
            if (Find(prmTag))
                return Corrente.valor;
            return ("");
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

        public bool Find(string prmTag)
        {
            foreach (TestDataVar var in this)

                if (myString.IsEqual(var.tag, prmTag))
                {
                    Corrente = var; return (true);
                }
            return (false);
        }

        private bool Find(string prmTag, string prmValor)
        {
            if (Find(prmTag))
            {
                Corrente.valor = prmValor; return (true);
            }
            return (false);
        }

    }

}


