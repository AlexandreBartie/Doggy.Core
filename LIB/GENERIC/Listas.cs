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

        private string delimitadorInicial = "[";
        private string delimitadorFinal = "]";

        private string _tag;
        private string _valor;

        private string _bruto;
        private string _descricao;

        public string tag { get => _tag; }
        public string valor { get => _valor; }

        public string bruto { get => _bruto; }
        public string descricao { get => _descricao; }

        public string memo { get => GetMemo(); }

        private bool TemDados { get => xString.IsStringOK(tag + valor); }
        private bool TemDescricao { get => (descricao != ""); }

        public xTupla()
        { }

        public xTupla(string prmTexto)
        {

            Parse(prmTexto);

        }
        public xTupla(string prmTexto, string prmSeparador)
        {

            Parse(prmTexto, prmSeparador);

        }

        public void Parse(string prmTexto)
        {

            Parse(prmTexto, prmSeparador: "=");

        }

        public void Parse(string prmTexto, string prmSeparador)
        {

            //
            // Get descricao Tupla (estão entre delimitadores pré-definidos)
            //

            _descricao = Blocos.GetBloco(prmTexto, delimitadorInicial, delimitadorFinal).Trim();

            //
            // Remove descricao da Tupla (para permitir identificar "tag" e "valor")
            //

            _bruto = Blocos.GetBlocoRemove(prmTexto, delimitadorInicial, delimitadorFinal);

            //
            // Identifica "tag" e "valor"
            //

            xLista lista = new xLista(bruto, prmSeparador);

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
                texto += tag + ": '" + valor + "'";

            if (TemDescricao)
                texto += " <" + descricao + ">";

            return texto.Trim();

        }

    }

    public class xTuplas : List<xTupla>
    {

        public string memo { get => GetMemo(); }

        public xTuplas(string prmLista)
        {
            Parse(prmLista, prmSeparador: ",");
        }

        public xTuplas(string prmLista, string prmSeparador)
        {
            
            Parse(prmLista, prmSeparador);

        }

        public virtual void Parse(string prmLista, string prmSeparador)
        {

            if (xString.IsStringOK(prmLista))
            {

                foreach (string item in new xLista(prmLista, prmSeparador))
                {
                    this.Add(new xTupla(item));
                }

            }

        }
        public bool IsContem(string prmItem)
        {

            foreach (xTupla tupla in this)
            {
                if (prmItem.ToLower().Contains(tupla.bruto.ToLower()))
                    return true;
            }

            return (false);
        }
        private string GetMemo()
        {

            string memo = ""; string separador = "";

            foreach (xTupla tupla in this)
            {
                memo += separador + tupla.memo;

                separador = ", ";
            }

            return (memo);

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

        public string log { get => GetTXT(); }

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

        public bool IsContem(string prmTexto) => (GetContain(prmTexto) != 0);

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
        private string GetTXT()
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
