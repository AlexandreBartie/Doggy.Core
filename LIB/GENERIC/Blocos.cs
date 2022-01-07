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

        public static string GetBlocoAntes(string prmTexto, string prmDelimitador) => GetBlocoAntes(prmTexto, prmDelimitador, prmTRIM: false);
        public static string GetBlocoAntes(string prmTexto, string prmDelimitador, bool prmTRIM)
        {

            if (xString.IsStringOK(prmTexto))
            {

                string retorno = prmTexto;

                if (xString.IsStringOK(prmDelimitador))
                {

                    int indice = prmTexto.IndexOf(prmDelimitador);

                    if (indice != -1)
                        retorno = (xString.GetFirst(prmTexto, prmTamanho: indice));
                }

                if (prmTRIM)
                    retorno = retorno.Trim();

                return (retorno);

            }

            return ("");

        }
        public static string GetBlocoDepois(string prmTexto, string prmDelimitador) => GetBlocoDepois(prmTexto, prmDelimitador, prmTRIM: false);
        public static string GetBlocoDepois(string prmTexto, string prmDelimitador, bool prmTRIM)
        {

            if (xString.IsStringOK(prmTexto) && xString.IsStringOK(prmDelimitador))
            {

                int indice = prmTexto.IndexOf(prmDelimitador);

                if (indice != -1)
                {

                    string retorno = (xString.GetLast(prmTexto, prmTamanho: prmTexto.Length - prmDelimitador.Length - indice));

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
        public static bool IsPrefixo(string prmTexto, string prmPrefixo) => (xString.GetFirst(prmTexto, prmTamanho: prmPrefixo.Length) == prmPrefixo);
        public static string GetPrefixo(string prmTexto, string prmPrefixo, string prmDelimitador) => GetPrefixo(prmTexto, prmPrefixo, prmDelimitador, prmPreserve: false);
        public static string GetPrefixo(string prmTexto, string prmPrefixo, string prmDelimitador, bool prmPreserve)
        {


            string retorno = "";

            if (xString.IsStringOK(prmTexto) && IsPrefixo(prmTexto, prmPrefixo))
            {

                if (xString.GetFind(prmTexto, prmDelimitador))
                    retorno = Bloco.GetBloco(prmTexto, prmPrefixo, prmDelimitador);
                else
                    retorno = xString.GetLast(prmTexto, prmTamanho: -prmPrefixo.Length);

                if (prmPreserve)
                    retorno = prmPrefixo + retorno + prmDelimitador;

            }

            return (retorno.Trim());

        }

        public static string GetPrefixoRemove(string prmTexto, string prmPrefixo) => GetPrefixoRemove(prmTexto, prmPrefixo, prmDelimitador: "");
        public static string GetPrefixoRemove(string prmTexto, string prmPrefixo, bool prmTRIM) => GetPrefixoRemove(prmTexto, prmPrefixo, prmDelimitador: "", prmTRIM);
        public static string GetPrefixoRemove(string prmTexto, string prmPrefixo, string prmDelimitador) => GetPrefixoRemove(prmTexto, prmPrefixo, prmDelimitador, prmTRIM: false);
        public static string GetPrefixoRemove(string prmTexto, string prmPrefixo, string prmDelimitador, bool prmTRIM)
        {

            string prefixo;

            prefixo = GetPrefixo(prmTexto, prmPrefixo, prmDelimitador);

            prefixo = xString.GetLast(prmTexto, prmTamanho: xInt.GetNegativo(prefixo.Length + 2));

            if (prmTRIM)
                return prefixo.Trim();

            return prefixo;

        }

    }

}