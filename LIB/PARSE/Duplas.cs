using Dooggy.Lib.Generic;
using Dooggy.Lib.Vars;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dooggy.Lib.Parse
{
    public class myTupla
    {

        private string delimitadorInicial = "[";
        private string delimitadorFinal = "]";

        private string separador = "=";

        private string _tag;
        private string _valor;

        private string _bruto;
        private string _descricao;

        public string tag { get => _tag; }
        public string valor { get => _valor; }

        public string bruto { get => _bruto; }
        public string descricao { get => _descricao; }
        public string memo { get => GetMemo(); }

        private bool TemDados { get => myString.IsFull(tag + valor); }
        private bool TemDescricao { get => (descricao != ""); }
        public bool TemTag { get => myString.IsFull(tag); }

        public bool IsMatch(string prmTag) => (TemTag && myString.IsEqual(tag, prmTag));

        public myTupla()
        { }

        public myTupla(string prmTexto)
        {

            Parse(prmTexto);

        }
        public myTupla(string prmTexto, string prmSeparador)
        {

            separador = prmSeparador;

            Parse(prmTexto);

        }

        public void Parse(string prmTexto)
        {

            //
            // Get descricao Tupla (estão entre delimitadores pré-definidos)
            //

            _descricao = Bloco.GetBloco(prmTexto, delimitadorInicial, delimitadorFinal).Trim();

            //
            // Remove descricao da Tupla (para permitir identificar "tag" e "valor")
            //

            _bruto = Bloco.GetBlocoRemove(prmTexto, delimitadorInicial, delimitadorFinal);

            //
            // Identifica "tag" e "valor"
            //

            xLista lista = new xLista(bruto, separador);

            _tag = lista.first;

            SetValue( lista.last_exact);

        }

        public bool SetValue(string prmValor) { _valor = prmValor; return true; }

        public bool SetValue(myTupla prmTupla)
        {
            if (prmTupla.IsMatch(tag))
                return SetValue(prmTupla.valor);

            return (false);
        }

        private string GetMemo()
        {
            string texto = "";

            if (TemDados)
                texto += tag + ": '" + valor + "'";

            if (TemDescricao)
                texto += " <" + descricao + ">";

            return texto.Trim();

        }

    }

    public class myTuplas : List<myTupla>
    {

        private string separador = ",";

        public string key;

        public string memo { get => GetMemo(); }

        public bool IsMatch(string prmKey) => (myString.IsEqual(key, prmKey));

        //public myTuplas()
        //{
        //}
        
        //public myTuplas(string prmLista)
        //{
        //    Parse(prmLista);
        //}

        //public myTuplas(string prmLista, string prmSeparador)
        //{

        //    separador = prmSeparador;

        //    Parse(prmLista);

        //}

        public myTuplas Setup(string prmKey) { key = prmKey; return this; }

        public myTuplas Parse(string prmLista, string prmSeparador) { separador = prmSeparador; return Parse(prmLista); }

        public myTuplas Parse(string prmLista)
        {
            if (myString.IsFull(prmLista))
            {
                foreach (string item in new xLista(prmLista, separador))
                    AddTupla(new myTupla(item));
            }
            return (this);
        }
        public myTuplas Parse(myTuplas prmTuplas)
        {
            foreach (myTupla tupla in prmTuplas)
                AddTupla(tupla);

            return (this);
        }


        public void AddTupla(myTupla prmTupla)
        {
            if (prmTupla.TemTag)

                if (!SetValue(prmTupla))
                    this.Add(prmTupla);
        }
        public void SetValues(string prmValores)
        {

            if (myString.IsFull(prmValores))
            {
                int cont = 0;
                foreach (string item in new xLista(prmValores, separador))
                {
                    if (this.Count == cont) break;

                    this[cont].SetValue(item);

                    cont++;
                }

            }

        }
        private bool SetValue(myTupla prmTupla)
        {
            if (prmTupla.TemTag)
            {

                foreach (myTupla Tupla in this)
                {
                    if (Tupla.SetValue(prmTupla))
                        return true;
                }

            }
            return (false);
        }
        public bool IsContem(string prmItem)
        {

            foreach (myTupla tupla in this)
            {
                if (prmItem.ToLower().Contains(tupla.bruto.ToLower()))
                    return true;
            }

            return (false);
        }
        private string GetMemo()
        {

            string memo = ""; string separador = "";

            foreach (myTupla tupla in this)
            {
                memo += separador + tupla.memo;

                separador = ", ";
            }

            return (memo);

        }

    }

    public class myTuplasBox : List<myTuplas>
    {
        public string descricao;

        private myTuplas Tuplas;

        public string header => descricao + "," + columns;
        public string columns => Tuplas.memo;

        public myTuplasBox()
        {
            Tuplas = new myTuplas();
        }
        public myTuplas AddItem(string prmKey)
        {
            myTuplas Tuplas = new myTuplas().Setup(prmKey);

            return Tuplas;

        }
        public myTuplas GetTuplas(string prmKey)
        {
            foreach (myTuplas Tuplas in this)
            {
                if (Tuplas.IsMatch(prmKey))
                    return Tuplas;
            }
            return null;
        }
    }

}
