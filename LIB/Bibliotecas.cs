using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Dooggy
{
    public class xTupla
    {
        private string separador;
        public xTupla()
        {
            separador = "=";
        }
        public xTupla(string prmSeparador)
        {
            separador = prmSeparador;
        }

        private string _tag;
        private string _valor;

        public string tag
        { get => _tag; }
        public string valor
        { get => _valor; }
        public string memo
        { get => tag + separador + valor; }
        public void Set(string prmTupla)
        {
            xLista Lista = new xLista(prmTupla, prmSeparador: "=");

            _tag = Lista.primeiro();
            _valor = Lista.ultimo();
        }
        private void Set(string prmTag, string prmValor)
        {
            _tag = prmTag;
            _valor = prmValor;
        }
        public string GetOpcao(string prmKey, string prmOpcoes)
        {

            xLista Lista = new xLista(prmOpcoes, prmSeparador: ";");

            return (Lista.GetFindx("[" + prmKey + "]"));

        }
    }
    public class xLista : List<string>
    {
        private string separador = ";";

        public xLista()
        {

        }
        public xLista(string prmLista)
        {
            Importar(prmLista);
        }
        public xLista(string prmLista, string prmSeparador)
        {

            separador = prmSeparador;

            Importar(prmLista);
        }

        public void Importar(string prmLista)
        {

            foreach (string item in prmLista.Split(separador))
            {
                this.Add(item.Trim());
            }

        }

        public string memo() => memo(prmSeparador: "");
        public string memo(string prmSeparador)
        {

            string lista = "";
            string separador = "";

            foreach (string item in this)
            {

                string texto = separador + item.Trim();

                lista += texto;

                separador = prmSeparador;

            }

            return lista;
        }
        public int qtde { get => this.Count; }

        public bool vazio { get => (qtde == 0); }

        public bool IsOK { get => !vazio; }

        public string item(int prmIndice)
        {
            if (!vazio)
                return (this[prmIndice - 1]);
            else
                return ("");
        }
        public int GetID(string prmTexto)
        {
            return (this.IndexOf(prmTexto));
        }
        public string GetFind(string prmTexto)
        {
            foreach (string vlItem in this)
            {
                if (vlItem.StartsWith(prmTexto))
                { return (vlItem); }
            }
            return ("");
        }
        public int GetContain(string prmTexto)
        {
            int cont = 0;

            foreach (string vlItem in this)
            {
                cont++;
                if (vlItem.ToLower().Contains(prmTexto.ToLower()))
                { return (cont); }
            }
            return (0);
        }

        public string GetFindx(string prmTexto)
        {
            return GetFind(prmTexto).Replace(prmTexto, "");
        }
        public string primeiro()
        { return item(1); }
        public string ultimo()
        { return item(qtde); }
    }


}


