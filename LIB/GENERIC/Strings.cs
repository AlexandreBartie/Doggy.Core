using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dooggy
{
    public static class xString
    {
        public static string GetBloco(string prmTexto, string prmDelimitador) => GetBloco(prmTexto, prmDelimitador, prmDelimitador);
        public static string GetBloco(string prmTexto, string prmDelimitador, bool prmPreservar) => GetBloco(prmTexto, prmDelimitador, prmDelimitador, prmPreservar);
        public static string GetBloco(string prmTexto, string prmDelimitadorInicial, string prmDelimitadorFinal) => GetBloco(prmTexto, prmDelimitadorInicial, prmDelimitadorFinal, prmPreservar: false);
        public static string GetBloco(string prmTexto, string prmDelimitadorInicial, string prmDelimitadorFinal, bool prmPreservar)
        {

            string retorno = "";

            if (prmTexto != null)
            {

                int inicio = prmTexto.IndexOf(prmDelimitadorInicial);

                int limite = inicio + prmDelimitadorInicial.Length;

                int final = prmTexto.IndexOf(prmDelimitadorFinal, limite);

                if ((inicio >= 0) & (final >= inicio))
                    retorno = (prmTexto.Substring(limite, final - limite));

            }

            if (prmPreservar)
                retorno = prmDelimitadorInicial + retorno + prmDelimitadorFinal;

            return (retorno);

        }

        public static string GetTroca(string prmTexto, string prmDelimitador, string prmDelimitadorNovo) => GetTroca(prmTexto, prmDelimitador, prmDelimitador, prmDelimitadorNovo);
        public static string GetTroca(string prmTexto, string prmDelimitadorInicial, string prmDelimitadorFinal, string prmDelimitadorNovo) => GetTroca(prmTexto, prmDelimitadorInicial, prmDelimitadorFinal, prmDelimitadorNovo, prmDelimitadorNovo);
        public static string GetTroca(string prmTexto, string prmDelimitadorInicial, string prmDelimitadorFinal, string prmDelimitadorInicialNovo, string prmDelimitadorFinalNovo)
        {

            string texto = prmTexto;

            while (true) 
            {

                string bloco = GetBloco(texto, prmDelimitadorInicial, prmDelimitadorFinal);

                if (bloco == "")
                    break;

                string trecho_velho = prmDelimitadorInicial + bloco + prmDelimitadorFinal;

                string trecho_novo = prmDelimitadorInicialNovo + bloco + prmDelimitadorFinalNovo;

                texto = texto.Replace(trecho_velho, trecho_novo);

            }

            return (texto);

        }
        public static string GetSubstring(string prmValor, int prmIndice)
        {

            if ((prmValor != null) & (prmValor.Length >= prmIndice))
            {
                return (prmValor.Substring(prmIndice));
            }

            return ("");
        }
        public static string GetReverse(string prmValor)
        {
            
            if (prmValor != null)
                return (new string(prmValor.Reverse().ToArray()));

            return ("");
        }
        public static string GetMask(string prmValor, string prmMask)
        {
            
            int cont = 0; int indice = 0; char simbolo = '#'; bool IsEnd = false; string resto;

            // Verifica se existe uma formatação a ser aplicada 

            if (prmMask == "")
                return prmValor;

            // Inverter Valores

            string valor = GetReverse(prmValor);
            string mask = GetReverse(prmMask);

            string texto = "";

            try
            {

                foreach (char item in mask)
                {

                    indice++;

                    if (!IsEnd)
                    {

                        if (item == simbolo)
                        { texto = valor[cont] + texto; cont++; }
                        else
                            texto = item + texto;

                        if (cont == valor.Length) IsEnd = true;
                    
                    }
                    else
                    {
                        if (item != simbolo)
                        {

                            resto = GetSubstring(mask, indice);

                            if (!resto.Contains(simbolo))
                                texto = item + texto;

                        }


                    }

                }

                return (texto);

            }
            catch (IndexOutOfRangeException e)
            {

                throw new Exception("Value too short to substitute all substitute characters in the mask", e);
            }
        }


    }

}
