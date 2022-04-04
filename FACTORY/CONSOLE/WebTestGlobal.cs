using Dooggy.LIBRARY;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dooggy.CORE
{

    public class DataTags : myTags
    {

        private DataPool Pool;

        private TestTrace Trace => Pool.Trace;

        public DataTags(DataPool prmPool)
        {
            Pool = prmPool;
        }

        public void SetGlobal()
        {
            foreach (myTag Tag in Pool.Global.Tags)
                Add(Tag.Dominio);
        }

        public new bool Add(string prmCommand)
        {
            if (base.Add(prmCommand))
                return true;

            Trace.LogConsole.FailConfigTag(prmCommand);

            return false;
        }

        public bool SetTag(string prmTupla) => SetTag(new myTupla(prmTupla));
        private bool SetTag(myTupla prmTupla) => SetTag(prmTupla.name, prmTupla.value);
        private bool SetTag(string prmTag, string prmValue)
        {
            if (IsFind(prmTag))
                if (FindKey(prmTag).SetValue(prmValue))
                    return true;
                else
                    Trace.LogConsole.FailFindTagOption(prmTag, prmValue);
            else
               Trace.LogConsole.FailFindTagName(prmTag);

            return false;
        }
    }

    public class DataTagOptions : myTagOptions
    {

    }
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

        private DataPool Pool;

        public DataVar Corrente;

        public DataVars(DataPool prmPool)
        {
            Pool = prmPool;
        }

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

                if (myString.IsMatch(var.tag, prmTag))
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


