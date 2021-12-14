using System;
using System.Collections.Generic;
using System.Text;

namespace Dooggy
{
    public static class xSubString
    {
        public static string GetBloco(string prmTexto, string prmDelimitador) => GetBloco(prmTexto, prmDelimitador, prmDelimitador);
        public static string GetBloco(string prmTexto, string prmDelimitador, bool prmPreserve) => GetBloco(prmTexto, prmDelimitador, prmDelimitador, prmPreserve);
        public static string GetBloco(string prmTexto, string prmDelimitadorInicial, string prmDelimitadorFinal) => GetBloco(prmTexto, prmDelimitadorInicial, prmDelimitadorFinal, prmPreserve: false);
        public static string GetBloco(string prmTexto, string prmDelimitadorInicial, string prmDelimitadorFinal, bool prmPreserve) => GetBloco(prmTexto, prmDelimitadorInicial, prmDelimitadorFinal, prmPreserve, prmExtract: false);
        public static string GetBloco(string prmTexto, string prmDelimitadorInicial, string prmDelimitadorFinal, bool prmPreserve, bool prmExtract)
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

            if (prmPreserve)
                retorno = prmDelimitadorInicial + retorno + prmDelimitadorFinal;

            if (prmExtract)
                retorno = xString.GetRemove(prmTexto, retorno);

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

    }
}
