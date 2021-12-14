using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Microsoft.VisualBasic.FileIO;
using System.Linq;
using Dooggy.Lib.Parse;

namespace Dooggy.Lib.Generic
{
    public class xTupla
    {

        private string separador;

        private bool delimitador;

        private string _tag;
        private string _valor;

        public string tag { get => _tag; }
        public string valor { get => _valor; }

        public string memo { get => GetMemo(); }

        private bool TemDados { get => xString.IsStringOK(tag + valor); }

        private bool TemDelimitador { get { if (delimitador && xString.IsStringOK(separador)) return (separador.Trim().Length >= 2); return (false); } }

        private string delimitador_inicial { get { if (TemDelimitador) return xString.GetFirst(separador); return (""); } }
        private string delimitador_final { get { if (TemDelimitador) return xString.GetLast(separador); return (""); } }

        public xTupla(string prmTexto)
        {

            Parse(prmTexto, prmSeparador: "=");

        }
        public xTupla(string prmTexto, string prmSeparador)
        {

            Parse(prmTexto, prmSeparador);

        }

        public xTupla(string prmTexto, string prmSeparador, bool prmDelimitador)
        {

            delimitador = prmDelimitador;

            Parse(prmTexto, prmSeparador);

        }

        private void Parse(string prmTexto, string prmSeparador)
        {

            separador = prmSeparador;

            if (TemDelimitador)
                ParseDelimitador(prmTexto);
            else
                ParseSeparador(prmTexto);

        }

        private void ParseDelimitador(string prmTexto)
        {

            _tag = xSubString.GetBloco(prmTexto, delimitador_inicial, delimitador_final, prmPreserve: true, prmExtract: true).Trim();
            _valor = xSubString.GetBloco(prmTexto, delimitador_inicial, delimitador_final).Trim();

        }
        private void ParseSeparador(string prmTexto)
        {

            xLista lista = new xLista(prmTexto, separador);

            _tag = lista.First;

            if (lista.IsUnico)
                _valor = "";
            else
                _valor = lista.Last;

        }

        private void Set(string prmTag, string prmValor)
        {
            _tag = prmTag;
            _valor = prmValor;

        }
        public string GetOpcao(string prmKey, string prmOpcoes)
        {

            xLista Lista = new xLista();

            Lista.Parse(prmOpcoes, prmSeparador: ";");

            return (Lista.GetFindx("[" + prmKey + "]"));

        }

        private string GetMemo()
        {
            string texto = "";

            if (TemDados)
                texto = tag + ": (" + valor + ")";

            return texto;

        }

    }
    public class xLista : List<string>
    {

        public string separador = ",";

        public xLista()
        {

        }
        
        public xLista(string prmLista)
        { 
            Parse(prmLista); 
        }

        public xLista(string prmLista, string prmSeparador)
        {
            separador = prmSeparador;

            Parse(prmLista);

        }

        public virtual void Parse(string prmLista) => Parse(prmLista, separador);

        public virtual void Parse(string prmLista, string prmSeparador)
        {

            separador = prmSeparador;

            if (xString.IsStringOK(prmLista))
                foreach (string item in prmLista.Split(separador))
                {
                    this.Add(item.Trim());
                }

        }
        public int qtde { get => this.Count; }

        public bool IsOK { get => !IsVazio; }

        public bool IsUnico { get => (qtde == 1); }

        public bool IsVazio { get => (qtde == 0); }

        public string First { get => Item(1); }

        public string Last { get => Item(qtde); }

        public bool IsRange(int prmIndice) => ((prmIndice >= 1) && (prmIndice <= qtde));

        public string log { get => GetLog(); }

        public bool Excluir(int prmIndice)
        {
            if (IsRange(prmIndice))
            {

                this.RemoveAt(prmIndice - 1);

                return (true);
            }

            return (false);

        }
        public string Item(int prmIndice)
        {
            if (!IsVazio)
                return (this[prmIndice - 1]);
            else
                return ("");
        }

        public bool IsContem(string prmItem)
        {

            foreach (string item in this)
            {
                if (prmItem.ToLower().Contains(item.ToLower()))
                    return true;
            }

            return (false);
        }

        public string GetRemove() => GetRemove(prmIndice: 1);

        public string GetRemove(int prmIndice)
        {
            if (IsRange(prmIndice))
            {

                string valor = Item(prmIndice);

                Excluir(prmIndice);

                return (valor);
            }

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
        private string GetLog()
        {

            string aux = "";

            string resultado = "";

            foreach (string item in this)
            {

                resultado += aux + item.Length.ToString();

                aux = separador;

            }

            return (resultado);
        }
    }
    public class xMemo : xLista
    {
        public xMemo()
        { }
        public xMemo(string prmSeparador)
        { separador = prmSeparador; }

        public xMemo(string prmTexto, string prmSeparador)
        { separador = prmSeparador; Parse(prmTexto); }

        public string csv { get => memo(", "); }

        public string memo() => memo(separador);

        public string memo(string prmSeparador)
        {

            string lista = "";
            string aux = "";

            foreach (string item in this)
            {

                lista += (aux + item.Trim());

                aux = prmSeparador;

            }

            return lista;
        }

    }
    public class xMask
    {

        private xJSON lista;

        public bool IsOK { get => (lista.IsOK); }

        public xMask(string prmMask)
        {

            lista = new xJSON(prmMask);

        }

        public string GetValor(string prmValor, string prmKey)
        {

            string mask = lista.GetValor(prmKey);

            return xString.GetMask(prmValor, mask);

        }

    }
}
