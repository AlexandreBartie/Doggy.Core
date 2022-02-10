using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Dooggy.Lib.Vars
{

    public static class myFormat
    {

        private static string mark_csv = @"""";

        public static string TextToString(string prmText, string prmFormat) => myMask.Get(prmText, prmFormat);

        public static string DateToString(string prmFormat) => DateToString(prmDate: DateTime.Now, prmFormat);
        public static string DateToString(DateTime prmDate, string prmFormat) => prmDate.ToString(GetDateFormat(prmFormat));

        public static string DoubleToString(Double prmNumber) => DoubleToString(prmNumber, prmCulture: CultureInfo.InvariantCulture);
        public static string DoubleToString(Double prmNumber, CultureInfo prmCulture) => DoubleToString(prmNumber, prmFormat: "", prmCulture);
        public static string DoubleToString(Double prmNumber, string prmFormat) => DoubleToString(prmNumber, prmFormat, prmCulture: CultureInfo.InvariantCulture);
        public static string DoubleToString(Double prmNumber, string prmFormat, CultureInfo prmCulture) => DoubleToString(prmNumber, prmFormat, prmCulture, prmCSV: false);
        public static string DoubleToString(Double prmNumber, string prmFormat, CultureInfo prmCulture, bool prmCSV) 
        {
            string retorno = prmNumber.ToString(prmFormat, prmCulture);

            if (prmCSV && prmCulture.NumberFormat.NumberDecimalSeparator == "," )
                retorno = mark_csv + retorno + mark_csv;

            return retorno;
        }

        private static string GetDateFormat(string prmFormato)
        {

            string formato = prmFormato;

            formato = myString.GetSubstituir(formato, "A", "y");
            formato = myString.GetSubstituir(formato, "a", "y");

            formato = myString.GetSubstituir(formato, "D", "d");
            formato = myString.GetSubstituir(formato, "m", "M");

            return (formato);

        }

    }

    public static class myCSV
    {

        public static string TextToCSV(string prmText, string prmFormat) => myFormat.TextToString(prmText, prmFormat);

        public static string DateToCSV(DateTime prmDate, string prmFormat) => myFormat.DateToString(prmDate, prmFormat);

        public static string DoubleToCSV(Double prmNumber, CultureInfo prmCulture) => DoubleToCSV(prmNumber, prmFormat: "", prmCulture);
        public static string DoubleToCSV(Double prmNumber, string prmFormat, CultureInfo prmCulture) => myFormat.DoubleToString(prmNumber, prmFormat, prmCulture, prmCSV: true);

    }
    internal static class myMask
    {

        private static char[] reservado = { '#', '9' };

        private static bool IsLivre(char prmItem) => !IsReservado(prmItem);

        internal static string Get(string prmText, string prmFormat)
        {

            int cont = 0; int indice = 0; bool IsEnd = false; string resto;

            // Verifica se existe uma formatação a ser aplicada 

            if (prmFormat == "")
                return prmText;

            // Inverter Valores

            string valor = myString.GetReverse(prmText);
            string mask = myString.GetReverse(prmFormat);

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

                            resto = myString.GetSubstring(mask, indice);

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

        private static bool IsReservado(char prmItem)
        {

            foreach (char item in reservado)
                if (item == prmItem) return true;

            return false;

        }

        private static bool ContemReservado(string prmText)
        {

            foreach (char item in prmText)
                if (IsReservado(item)) return true;

            return false;

        }

    }

}
