using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Microsoft.VisualBasic.FileIO;

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
            xLista lista = new xLista(prmTupla, prmSeparador: "=");

            _tag = lista.First;
            _valor = lista.Last;
        }
        private void Set(string prmTag, string prmValor)
        {
            _tag = prmTag;
            _valor = prmValor;
        }
        public string GetOpcao(string prmKey, string prmOpcoes)
        {

            xLista Lista = new xLista(";");

            Lista.Parse(prmOpcoes);

            return (Lista.GetFindx("[" + prmKey + "]"));

        }
    }
    public class xLista : List<string>
    {

        public string separador;

        public xLista()
        {}

        public xLista (string prmSeparador)
        {

            separador = prmSeparador;

        }
        public xLista(string prmLista, string prmSeparador)
        {
            separador = prmSeparador;

            Parse(prmLista);

        }

        public virtual void Parse(string prmLista)
        {

            if (prmLista.Trim() != "")
                foreach (string item in prmLista.Split(separador))
                {
                    this.Add(item.Trim());
                }

        }

        public int qtde { get => this.Count; }

        public bool vazio { get => (qtde == 0); }

        public bool IsOK { get => !vazio; }

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
            if (!vazio)
                return (this[prmIndice - 1]);
            else
                return ("");
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
    public class xParseCSV : xMemo
    {

        string delimitador = "|";//"\"";

        public xParseCSV(string prmTexto)
        { separador = ","; Parse(prmTexto); }

        public xParseCSV(string prmTexto, string prmSeparador)
        { separador = prmSeparador; Parse(prmTexto); }
        
        public override void Parse(string prmLista)
        {

            if (prmLista.Trim() != "")
            {

                string texto;
                string lista = "";
                string resto = prmLista;

                bool IsDelimitedStartON = false;
                bool IsDelimitedEndON = false;

                foreach (string item in prmLista.Split(separador))
                {

                    bool IsCycleBegin = false;
                    bool IsCycleEnding = false;

                    bool IsDetectStart = false;
                    bool IsDetectEnd = false;

                    texto = item.Trim();

                    resto = GetSubstring(resto, prmIndice: item.Length + 1);


                    // Delimitador ISOLADO, tratamento especial
                    if (texto == delimitador)

                        // Delimitador START ON
                        if (IsDelimitedStartON)

                            // sinalizar fechar o ciclo
                            IsCycleEnding = true;

                        // Delimitador START OFF
                        else

                        // sinalizar iniciar o ciclo
                        {
                       
                            IsCycleBegin = true;
                        }

                    else
                    {

                        // Detectar Delimitadores INICIAL e FINAL 
                        IsDetectStart = texto.StartsWith(delimitador);
                        IsDetectEnd = texto.EndsWith(delimitador);

                        // Ciclo: EM ABERTO
                        if ((IsDelimitedStartON))

                            // Delimitador FINAL detectado
                            if (IsDetectEnd)

                                //  sinalizar FECHAR CICLO
                                IsCycleEnding = true;

                            // Delimitador FINAL não detectado
                            else

                                //  manter espacos, pois o CICLO está em aberto
                                texto = item;

                        // Ciclo: NÃO ABERTO
                        else

                            // Delimitador INICIAL localizado: 
                            if ((IsDetectStart & !IsDetectEnd))

                            //sinalizar INICIAR CICLO (apenas se localizador delimitador FINAL no futuro)
                            IsCycleBegin = ExisteDelimitadorEND(resto);

                    }

                    // Ciclo recém INICIADO: retirar espacos brancos iniciais 
                    if (IsCycleBegin)
                        { texto = item.TrimStart(); IsDelimitedStartON = true; }

                    // Ciclo recém FECHADO: : retirar espacos brancos finais 
                    if (IsCycleEnding)
                        { texto = item.TrimEnd(); IsDelimitedEndON = true; }

                    // Delimitadores não ENCONTRADOS
                    if (!(IsDelimitedStartON | IsDelimitedEndON))
                        this.Add(texto);
                    else 
                    { 
                        // Delimitador INICIAL JÁ encontrado
                        if (IsDelimitedStartON)
                        {
                            if (lista == "")
                                lista += texto;
                            else
                               lista += separador + texto;
                        }

                        // Delimitador FINAL JÁ encontrado
                        if (IsDelimitedEndON)
                        { this.Add(lista); lista = ""; IsDelimitedStartON = false; IsDelimitedEndON = false; }

                    }

                }

                if (lista != "")
                    this.Add(lista);

            }    
    
        }

        private string GetSubstring(string prmTexto, int prmIndice)
        {

            if (prmIndice <= prmTexto.Length)
                return prmTexto.Substring(prmIndice);

            return "";
        }

        private bool ExisteDelimitadorEND(string prmTexto)
        {
            string texto = prmTexto.Replace(" ", "");
            string alvo = delimitador + separador;

            if (texto.EndsWith(delimitador))
            { return (true);  }


            return (texto.IndexOf(alvo) != -1) ;
        }


    }

}


