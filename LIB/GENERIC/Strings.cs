using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dooggy
{

    public static class xString
    {

        public static bool IsStringOK(string prmTexto)
        {
            if (prmTexto != null)
                return (prmTexto.Trim() != "");
            return (false);

        }

        public static string GetChar(string prmTexto, int prmIndice) => (GetSubstring(prmTexto, prmIndice, 1));

        public static string GetFirst(string prmTexto) => GetChar(prmTexto, prmIndice: 1);
        public static string GetLast(string prmTexto)
        {
            if (IsStringOK(prmTexto))
                return GetChar(prmTexto, prmIndice: prmTexto.Length);

            return ("");

        }

        public static string GetInicial(string prmTexto, int prmTamanho)
        {
            if (IsStringOK(prmTexto))
                return GetSubstring(prmTexto, prmIndice: 1, prmTamanho);

            return ("");

        }
        public static string GetInicial(string prmTexto, string prmDelimitador)
        {
            if (IsStringOK(prmTexto))
            {

                int indice = GetFind(prmTexto, prmDelimitador);

                if (indice != 0)
                    return GetSubstring(prmTexto, prmIndice: 1, prmTamanho: indice-1);
    
                return (prmTexto);

            }

            return ("");

        }
        public static string GetFinal(string prmTexto, int prmTamanho)
        {
            if (IsStringOK(prmTexto))
                return GetSubstring(prmTexto, prmIndice: prmTexto.Length - prmTamanho + 1, prmTamanho);
            
            return ("");

        }
        public static string GetFinal(string prmTexto, string prmDelimitador)
        {
            if (IsStringOK(prmTexto))
            {
                
                int indice = GetFind(prmTexto, prmDelimitador);

                if (indice != 0)
                {
                    return GetSubstring(prmTexto, prmIndice: indice + 1, prmTamanho: prmTexto.Length - indice);
                }
            }

            return ("");

        }
        public static int GetFind(string prmTexto, string prmDelimitador)
        {
            if (IsStringOK(prmTexto))
            {

                return (prmTexto.IndexOf(prmDelimitador)) + 1;
            }

            return (0);

        }
        public static string GetSubstring(string prmTexto, int prmIndice) => GetSubstring(prmTexto, prmIndice, prmTamanho: prmTexto.Length - prmIndice);
        public static string GetSubstring(string prmTexto, int prmIndice, int prmTamanho)
        {

            int indice = prmIndice - 1;

            if ((prmTamanho >= 1) && IsStringOK(prmTexto))

                if ((indice >= 0) && (prmTexto.Length >= indice + prmTamanho))
                    return (prmTexto.Substring(indice, prmTamanho));
                else
                    return (prmTexto);

            return ("");
        }
        
        public static string GetRemove(string prmTexto, string prmParte)
        {

            if (IsStringOK(prmTexto))
                return (prmTexto.Replace(prmParte, newValue: ""));

            return ("");

        }
        public static string GetReverse(string prmTexto)
        {

            if (IsStringOK(prmTexto))
                return (new string(prmTexto.Reverse().ToArray()));

            return ("");
        }
        public static string GetNoBlank(string prmTexto)
        {

            if (IsStringOK(prmTexto))
                return (GetRemove(prmTexto, " "));

            return ("");
        }
        
        public static string GetMask(string prmTexto, string prmMask)
        {

            int cont = 0; int indice = 0; char simbolo = '#'; bool IsEnd = false; string resto;

            // Verifica se existe uma formatação a ser aplicada 

            if (prmMask == "")
                return prmTexto;

            // Inverter Valores

            string valor = GetReverse(prmTexto);
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
