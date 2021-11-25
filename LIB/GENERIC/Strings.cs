using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dooggy
{
    public static class xString
    {

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
