using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dooggy.Lib.Vars
{
    public static class myString
    {

        public static bool IsFull(string prmTexto)
        {
            if (prmTexto != null)
                return (prmTexto != "");
            return (false);
             
        }
        //public static bool IsFullEx(string prmTexto) => (GetCount(prmTexto) > 0);
        public static bool IsNull(string prmTexto) => (prmTexto == null);
        public static bool IsEmpty(string prmTexto) => !(IsFull(prmTexto));

        public static bool IsEqual(string prmTextoA, string prmTextoB) => GetLower(prmTextoA) == GetLower(prmTextoB);
        public static bool IsNoEqual(string prmTextoA, string prmTextoB) => !IsEqual(prmTextoA,prmTextoB);


        public static bool IsStartsWith(string prmTextoA, string prmTextoB) => GetLower(prmTextoA).StartsWith(GetLower(prmTextoB));


        public static bool IsContem(string prmTextoA, string prmTextoB) => IsContain(prmTextoA, prmTextoB, prmInverter: false);

        public static bool IsContido(string prmTextoA, string prmTextoB) => IsContain(prmTextoA, prmTextoB, prmInverter: true);

        public static bool IsContain(string prmTextoA, string prmTextoB, bool prmInverter)
        {

            string textoA; string textoB;

            if (prmInverter)
                { textoA = prmTextoB; textoB = prmTextoA; }
            else
                { textoA = prmTextoA; textoB = prmTextoB; }

            if (myString.IsFull(textoA) && myString.IsFull(textoB))
                return GetLower(textoA).Contains(GetLower(textoB));

            return (false);

        }

        public static string GetLower(string prmTexto)
        {

            if (IsFull(prmTexto))
                return (prmTexto.ToLower().Trim());

            return ("");

        }

        public static string GetChar(string prmTexto, int prmIndice) => (GetSubstring(prmTexto, prmIndice, 1));

        public static string GetFirst(string prmTexto) => GetChar(prmTexto, prmIndice: 1);
        public static string GetFirst(string prmTexto, int prmTamanho)
        {
            if (IsFull(prmTexto) && prmTamanho != 0)
                if (myInt.IsPositivo(prmTamanho))
                    return GetSubstring(prmTexto, prmIndice: 1, prmTamanho);
                else
                    return GetLast(prmTexto, myInt.GetPositivo(prmTamanho));
            return ("");

        }
        public static string GetFirst(string prmTexto, string prmDelimitador)
        {
            if (IsFull(prmTexto))
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
                if (IsFull(prmTexto))
                    return GetChar(prmTexto, prmIndice: prmTexto.Length);

                return ("");

            }

        public static string GetLast(string prmTexto, int prmTamanho)
        {

            int tamanho = prmTamanho;

            if (IsFull(prmTexto))
            {

                if (prmTamanho < 0)
                    tamanho = prmTexto.Length - myInt.GetPositivo(prmTamanho);

                return GetSubstring(prmTexto, prmIndice: prmTexto.Length - tamanho + 1, tamanho);

            }
            return ("");

        }
        public static string GetLast(string prmTexto, string prmDelimitador)
        {
            if (IsFull(prmTexto))
            {
                
                int indice = GetPosicao(prmTexto, prmDelimitador);

                if (indice != 0)
                    return GetSubstring(prmTexto, prmIndice: indice + 1, prmTamanho: prmTexto.Length - indice);
            }

            return ("");

        }

        //public static bool GetFindEx(string prmTexto, string prmParte) { if (myString.IsNull(prmParte)) return false; return GetFind(prmTexto, prmParte); }
        public static bool GetFind(string prmTexto, string prmParte)
        {
            if (IsFull(prmTexto) && IsFull(prmParte))
                return (prmTexto.ToLower().IndexOf(prmParte.ToLower()) != -1);
            return false;
        }
        public static int GetPosicao(string prmTexto, string prmParte)
        {
            if (IsFull(prmTexto) && IsFull(prmParte))
                return (prmTexto.IndexOf(prmParte)) + prmParte.Length;
            return (0);
        }

        public static string GetSubPosicao(string prmTexto, int prmIndice, int prmIndiceFinal) => GetSubstring(prmTexto, prmIndice, prmIndiceFinal - prmIndice + 1);

        public static string GetSubstring(string prmTexto, int prmIndice) => GetSubstring(prmTexto, prmIndice, prmTamanho: prmTexto.Length - prmIndice + 1);
        public static string GetSubstring(string prmTexto, int prmIndice, int prmTamanho)
        {

            int indice = prmIndice - 1;

            if ((prmTamanho >= 1) && IsFull(prmTexto))

                if ((indice >= 0) && (prmTexto.Length >= indice + prmTamanho))
                    return (prmTexto.Substring(indice, prmTamanho));
                else
                    return (prmTexto);

            return ("");
        }

        public static string GetSubstituir(string prmTexto, string prmVelho, string prmNovo)
        {
            string txt = GetFull(prmTexto);
            string novo = GetFull(prmNovo);
            string velho = GetFull(prmVelho);

            if (IsFull(velho))
                return (txt.Replace(velho, novo));

            return txt;
        }
        
        public static string GetRemove(string prmTexto, string prmParte)
        {
            return (GetSubstituir(prmTexto, prmVelho: prmParte, prmNovo: ""));
        }
        public static string GetReverse(string prmTexto)
        {
            if (IsFull(prmTexto))
                return (new string(prmTexto.Reverse().ToArray()));
            return ("");
        }
        public static string GetNoBlank(string prmTexto)
        {
            if (IsFull(prmTexto))
                return (GetRemove(prmTexto, " "));
            return ("");
        }
        public static int GetCount(string prmTexto)
        {
            if (IsFull(prmTexto))
                return (prmTexto.Length);
            return (0);
        }

        public static string GetFull(string prmTexto) => GetFull(prmTexto, prmPadrao: "");
        public static string GetFull(string prmTexto, string prmPadrao)
        {
            if (IsFull(prmTexto))
                return prmTexto;

            return (prmPadrao);
        }
        //public static string GetFullEx(string prmTexto) => GetFullEx(prmTexto, prmPadrao: "");
        //public static string GetFullEx(string prmTexto, string prmPadrao)
        //{
        //    if (IsFullEx(prmTexto))
        //        return prmTexto;

        //    return (prmPadrao);
        //}
        public static string GetJSON(string prmFluxo)
        {
            if (myString.IsFull(prmFluxo))
                return string.Format("{ {0} }", prmFluxo);

            return ("{ }");
        }
        public static string Concat(string prmValorA, string prmValorB, string prmSeparador)
        {

            string valorA = GetFull(prmValorA);
            string valorB = GetFull(prmValorB);

            if (valorA == "")
                return valorB;
            
            if (valorB == "")
                return valorA;

            return (valorA + prmSeparador + valorB);
        }
    
    }


}
