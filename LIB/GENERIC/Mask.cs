using System;
using System.Collections.Generic;
using System.Text;

namespace Dooggy.Lib.Generic
{
    public class Mask
    {

        private char[] reservado = { '#', '9' };

        private bool IsLivre(char prmItem) => !IsReservado(prmItem);

        public string Get(string prmTexto, string prmFormato)
        {

            int cont = 0; int indice = 0; bool IsEnd = false; string resto;

            // Verifica se existe uma formatação a ser aplicada 

            if (prmFormato == "")
                return prmTexto;

            // Inverter Valores

            string valor = xString.GetReverse(prmTexto);
            string mask = xString.GetReverse(prmFormato);

            string texto = "";

            try
            {

                foreach (char item in mask)
                {

                    indice++;

                    if (!IsEnd)
                    {

                        if (IsReservado(item))
                            { texto = valor[cont] + texto; cont++; }
                        else
                            texto = item + texto;

                        if (cont == valor.Length) IsEnd = true;

                    }
                    else
                    {
                        if (IsLivre(item))
                        {

                            resto = xString.GetSubstring(mask, indice);

                            if (!ContemReservado(resto))
                                texto = item + texto;

                        }

                    }

                }

                return (texto);

            }
            catch (IndexOutOfRangeException e)
            {

                throw new Exception("Value too short to substitute all characters in the mask", e);
            }
        }

        private bool IsReservado(char prmItem)
        {

            foreach (char item in reservado)
                if (item == prmItem) return true;

            return false;

        }

        private bool ContemReservado(string prmTexto)
        {

            foreach (char item in prmTexto)
                if (IsReservado(item)) return true;

            return false;

        }

    }
}
