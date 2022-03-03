using System;
using System.Collections.Generic;
using System.Text;

namespace Dooggy.Lib.Generic
{
    public class myDominio
    {

        private string key;

        private xLista lista;

        public myDominio(string prmKey, string prmLista)
        {
            key = prmKey;  lista = new xLista(prmLista);
        }

        public bool IsContem(string prmItem) => lista.IsContem(prmItem);

    }

    public class myDominios : List<myDominio>
    {
        public void AddItem(string prmKey, string prmLista) => Add(new myDominio(prmKey, prmLista));
    }

}
