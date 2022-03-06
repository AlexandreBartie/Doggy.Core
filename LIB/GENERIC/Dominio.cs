using Dooggy.Lib.Vars;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dooggy.Lib.Generic
{
    public class myDominio
    {

        public string key;

        private xLista lista;

        public string log => GetLog();

        public string txt => lista.txt;

        public bool TemKey => myString.IsFull(key);
        public bool TemLista => lista.IsFull;

        private string key_start = "[";
        private string key_finish = "]";

        public myDominio(string prmLista)
        {
            Parse(prmLista);
        }
        
        public myDominio(string prmKey, string prmLista)
        {
            Parse(prmKey, prmLista);
        }

        private void Parse(string prmTexto)
        {
            string texto;

            key = Bloco.GetBloco(prmTexto, key_start, key_finish).Trim();

            if (TemKey)
                texto = Bloco.GetBlocoDepois(prmTexto, key_finish, prmTRIM: true);
            else
                texto = prmTexto;

            Parse(key, texto);
        }

        private void Parse(string prmKey, string prmLista)
        {
            key = prmKey; SetLista(prmLista);
        }

        public void SetLista(string prmLista) => lista = new xLista(prmLista);
        public bool IsContem(string prmItem) => lista.IsContem(prmItem);

        public bool IsEqual(string prmKey) => myString.IsEqual(key, prmKey);

        private string GetLog()
        {
            if (TemKey)
                if (TemLista)
                    return (string.Format("{0}: {1}", key, txt).Trim());
                else
                    return key;

            return txt;
        }

    }

    public class myDominios : List<myDominio>
    {
        public string log => GetLOG();

        public void AddItens(List<string> prmLista)
        {
            foreach (string linha in prmLista)
                AddItem(linha);
        }
        public void AddItem(string prmLista) => AddNew(new myDominio(prmLista));
        public void AddItem(string prmKey, string prmLista) => AddNew(new myDominio(prmKey, prmLista));

        private void AddNew(myDominio prmItem)
        {
            string key = prmItem.key;

            if (FindIt(key))
                Find(key).SetLista(prmItem.txt);
            else
                Add(prmItem);
        }

        public myDominio Find(string prmKey)
        {
            foreach (myDominio item in this)
                if (item.IsEqual(prmKey))
                    return item;
            return null;
        }

        public bool FindIt(string prmKey)
        {
            foreach (myDominio item in this)
                if (item.IsEqual(prmKey))
                    return true;
            return false;
        }

        private string GetLOG()
        {
            string txt = "";

            foreach (myDominio item in this)
                txt += item.log + Environment.NewLine;

            return txt;
        }

    }

}
