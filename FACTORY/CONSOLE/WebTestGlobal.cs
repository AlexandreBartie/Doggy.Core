using Dooggy.LIBRARY;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dooggy.CORE
{

    public class DataVar
    {
        public string tag;

        public string valor;

        public string sql;

        public DataVar(string prmTag, string prmValor)
        {
            tag = prmTag;

            valor = prmValor;
        }
    }

    public class DataVars : List<DataVar>
    {

        public DataVar Corrente;

        public string Criar(string prmVar)
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
                Corrente = new DataVar(tag, valor);

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
            foreach (DataVar var in this)

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


