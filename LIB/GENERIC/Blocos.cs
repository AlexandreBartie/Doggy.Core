using System;
using System.Collections.Generic;
using System.Text;
using static Dooggy.xInt;

namespace Dooggy
{
    public static class Bloco
    {
        public static string GetBloco(string prmTexto, string prmDelimitador) => GetBloco(prmTexto, prmDelimitador, prmDelimitador);
        public static string GetBloco(string prmTexto, string prmDelimitador, bool prmPreserve) => GetBloco(prmTexto, prmDelimitador, prmDelimitador, prmPreserve);
        public static string GetBloco(string prmTexto, string prmDelimitadorInicial, string prmDelimitadorFinal) => GetBloco(prmTexto, prmDelimitadorInicial, prmDelimitadorFinal, prmPreserve: false);
        public static string GetBloco(string prmTexto, string prmDelimitadorInicial, string prmDelimitadorFinal, bool prmPreserve)
        {

            string retorno = "";

            if (xString.IsStringOK(prmTexto))
            {

                int inicio = prmTexto.IndexOf(prmDelimitadorInicial);

                int limite = inicio + prmDelimitadorInicial.Length;

                int final = prmTexto.IndexOf(prmDelimitadorFinal, limite);

                if ((inicio >= 0) & (final >= inicio))
                    retorno = (prmTexto.Substring(limite, final - limite));

            }

            if (prmPreserve)
                retorno = prmDelimitadorInicial + retorno + prmDelimitadorFinal;

            return (retorno);

        }

        public static string GetBlocoRemove(string prmTexto, string prmDelimitadorInicial, string prmDelimitadorFinal) => GetBlocoRemove(prmTexto, prmDelimitadorInicial, prmDelimitadorFinal, prmTRIM: false);
        public static string GetBlocoRemove(string prmTexto, string prmDelimitadorInicial, string prmDelimitadorFinal, bool prmTRIM)
        {

            string retorno = GetBloco(prmTexto, prmDelimitadorInicial, prmDelimitadorFinal, prmPreserve: true);

            string parte_inicial = Bloco.GetBlocoAntes(prmTexto, retorno);
            string parte_final = Bloco.GetBlocoDepois(prmTexto, retorno);

            if (prmTRIM)
                return (parte_inicial.Trim() + " " + parte_final.Trim()).Trim();

            return (parte_inicial + parte_final);

        }

        public static string GetBlocoAntes(string prmTexto, string prmItem) => GetBlocoAntes(prmTexto, prmItem, prmTRIM: false);
        public static string GetBlocoAntes(string prmTexto, string prmItem, bool prmTRIM)
        {

            if (xString.IsStringOK(prmTexto))
            {

                string retorno = prmTexto;

                if (xString.IsStringOK(prmItem))
                {

                    int indice = prmTexto.IndexOf(prmItem);

                    if (indice != -1)
                        retorno = (xString.GetFirst(prmTexto, prmTamanho: prmTexto.IndexOf(prmItem)));
                }

                if (prmTRIM)
                    retorno = retorno.Trim();

                return (retorno);

            }

            return ("");

        }
        public static string GetBlocoDepois(string prmTexto, string prmItem) => GetBlocoDepois(prmTexto, prmItem, prmTRIM: false);
        public static string GetBlocoDepois(string prmTexto, string prmItem, bool prmTRIM)
        {

            if (xString.IsStringOK(prmTexto) && xString.IsStringOK(prmItem))
            {

                int indice = prmTexto.IndexOf(prmItem);

                if (indice != -1)
                {

                    string retorno = (xString.GetLast(prmTexto, prmTamanho: prmTexto.Length - prmItem.Length - indice));

                    if (prmTRIM)
                        retorno = retorno.Trim();

                    return (retorno);
                }


            }

            return ("");

        }
        public static string GetBlocoTroca(string prmTexto, string prmDelimitador, string prmDelimitadorNovo) => GetBlocoTroca(prmTexto, prmDelimitador, prmDelimitador, prmDelimitadorNovo);
        public static string GetBlocoTroca(string prmTexto, string prmDelimitadorInicial, string prmDelimitadorFinal, string prmDelimitadorNovo) => GetBlocoTroca(prmTexto, prmDelimitadorInicial, prmDelimitadorFinal, prmDelimitadorNovo, prmDelimitadorNovo);
        public static string GetBlocoTroca(string prmTexto, string prmDelimitadorInicial, string prmDelimitadorFinal, string prmDelimitadorInicialNovo, string prmDelimitadorFinalNovo)
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

    public static class Prefixo
    {

        public static string GetPrefixo(string prmTexto, string prmSinal, string prmDelimitador) => GetPrefixo(prmTexto, prmSinal, prmDelimitador, prmPreserve: false);
        public static string GetPrefixo(string prmTexto, string prmSinal, string prmDelimitador, bool prmPreserve)
        {


            string retorno = "";

            if (xString.IsStringOK(prmTexto) && (xString.GetFirst(prmTexto) == prmSinal))
            {

                if (xString.GetFind(prmTexto, prmDelimitador))
                    retorno = Bloco.GetBloco(prmTexto, prmSinal, prmDelimitador);
                else
                    retorno = xString.GetLast(prmTexto, prmTamanho: -1);

                if (prmPreserve)
                    retorno = prmSinal + retorno + prmDelimitador;

            }

            return (retorno.Trim());

        }

        public static string GetPrefixoRemove(string prmTexto, string prmSinal, string prmDelimitador)
        {

            string prefixo = GetPrefixo(prmTexto, prmSinal, prmDelimitador);

            return (xString.GetLast(prmTexto, prmTamanho: xInt.GetNegativo(prefixo.Length + 2)));

        }

    }

}