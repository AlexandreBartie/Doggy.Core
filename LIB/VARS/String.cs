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

        public static bool IsEmpty(string prmTexto) => !(IsStringOK(prmTexto));

        public static bool IsEqual(string prmTextoA, string prmTextoB) => GetLower(prmTextoA) == GetLower(prmTextoB);

        public static bool IsContem(string prmTextoA, string prmTextoB) => IsContain(prmTextoA, prmTextoB, prmInverter: false);

        public static bool IsContido(string prmTextoA, string prmTextoB) => IsContain(prmTextoA, prmTextoB, prmInverter: true);

        public static bool IsContain(string prmTextoA, string prmTextoB, bool prmInverter)
        {

            string textoA; string textoB;

            if (prmInverter)
                { textoA = prmTextoB; textoB = prmTextoA; }
            else
                { textoA = prmTextoA; textoB = prmTextoB; }

            if (xString.IsStringOK(textoA) && xString.IsStringOK(textoB))
                return GetLower(textoA).Contains(GetLower(textoB));

            return (false);

        }

        public static string GetLower(string prmTexto)
        {

            if (IsStringOK(prmTexto))
                return (prmTexto.ToLower().Trim());

            return ("");

        }

        public static string GetChar(string prmTexto, int prmIndice) => (GetSubstring(prmTexto, prmIndice, 1));

        public static string GetFirst(string prmTexto) => GetChar(prmTexto, prmIndice: 1);
        public static string GetFirst(string prmTexto, int prmTamanho)
        {
            if (IsStringOK(prmTexto))
                return GetSubstring(prmTexto, prmIndice: 1, prmTamanho);

            return ("");

        }
        public static string GetFirst(string prmTexto, string prmDelimitador)
        {
            if (IsStringOK(prmTexto))
            {

                int indice = GetPosicao(prmTexto, prmDelimitador);

                if (indice != 0)
                    return GetSubstring(prmTexto, prmIndice: 1, prmTamanho: indice-1);
    
                return (prmTexto);

            }

            return ("");

        }
        public static string GetLast(string prmTexto)
            {
                if (IsStringOK(prmTexto))
                    return GetChar(prmTexto, prmIndice: prmTexto.Length);

                return ("");

            }

        public static string GetLast(string prmTexto, int prmTamanho)
        {

            int tamanho = prmTamanho;

            if (IsStringOK(prmTexto))
            {

                if (prmTamanho < 0)
                    tamanho = prmTexto.Length - xInt.GetPositivo(prmTamanho);

                return GetSubstring(prmTexto, prmIndice: prmTexto.Length - tamanho + 1, tamanho);

            }
            return ("");

        }
        public static string GetLast(string prmTexto, string prmDelimitador)
        {
            if (IsStringOK(prmTexto))
            {
                
                int indice = GetPosicao(prmTexto, prmDelimitador);

                if (indice != 0)
                {
                    return GetSubstring(prmTexto, prmIndice: indice + 1, prmTamanho: prmTexto.Length - indice);
                }
            }

            return ("");

        }
        public static bool GetFind(string prmTexto, string prmParte) => (GetPosicao(prmTexto, prmParte) != 0);
        public static int GetPosicao(string prmTexto, string prmParte)
        {
            if (IsStringOK(prmTexto))
            {

                return (prmTexto.IndexOf(prmParte)) + 1;
            }

            return (0);

        }
        public static string GetSubstring(string prmTexto, int prmIndice) => GetSubstring(prmTexto, prmIndice, prmTamanho: prmTexto.Length - prmIndice + 1);
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

        public static string GetSubstituir(string prmTexto, string prmVelho, string prmNovo)
        {

            if (IsStringOK(prmTexto))
                return (prmTexto.Replace(prmVelho, prmNovo));

            return ("");

        }
        
        public static string GetRemove(string prmTexto, string prmParte)
        {

            return (GetSubstituir(prmTexto, prmVelho: prmParte, prmNovo: ""));

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
        
    }


}
