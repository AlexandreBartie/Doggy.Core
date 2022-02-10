using System;
using System.Collections.Generic;
using System.Text;

namespace Dooggy.Lib.Generic
{
    public class myDominio
    {

        private xLista lista;

        public myDominio(string prmLista)
        {
            lista = new xLista(prmLista);
        }

        public bool IsContem(string prmItem) => lista.IsContem(prmItem);

    }
}
