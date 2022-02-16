using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Dooggy.Lib.Vars
{

    public static class myFormat
    {

        private static string mark_csv = @"""";

        public static string TextToString(string prmText, string prmFormat) => myText.Get(prmText, prmFormat);

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
    internal static class myCSV
    {

        public static string TextToCSV(string prmText, string prmFormat) => myFormat.TextToString(prmText, prmFormat);

        public static string DateToCSV(DateTime prmDate, string prmFormat) => myFormat.DateToString(prmDate, prmFormat);

        public static string DoubleToCSV(Double prmNumber, CultureInfo prmCulture) => DoubleToCSV(prmNumber, prmFormat: "", prmCulture);
        public static string DoubleToCSV(Double prmNumber, string prmFormat, CultureInfo prmCulture) => myFormat.DoubleToString(prmNumber, prmFormat, prmCulture, prmCSV: true);

    }
    internal static class myText
    {

        private static char[] chave = { '#', '9' };

        private static char[] reserva = { '#', '9', '0' };

        private static bool IsLivre(char prmItem) => !IsReserva(prmItem);

        internal static string Get(string prmText, string prmFormat)
        {

            int cont = 0; int indice = 0; bool IsTextEnd = false; string restoMask;

            // Verifica se existe uma formatação a ser aplicada 

            if (prmFormat == "")
                return prmText;

            // Inverter Valores

            string valor = myString.GetReverse(prmText);
            string mask = myString.GetReverse(prmFormat);

            string texto = "";

            try
            {
                foreach (char letra in mask)
                {
                    indice++;

                    if (!IsTextEnd)
                    {
                        if (IsReserva(letra))
                            { 
                            texto = valor[cont] + texto; 
                            cont++; 
                            }
                        else
                            texto = letra + texto;

                        if (cont == valor.Length) IsTextEnd = true;
                    }
                    else
                    {
                        if (IsLivre(letra))
                        {
                            restoMask = myString.GetSubstring(mask, indice);

                            if (!ContemChave(restoMask))
                                texto = letra + texto;
                        }
                        else if (letra == '0')
                           texto = letra + texto;
                    }
                }

                return (texto);

            }
            catch (IndexOutOfRangeException e)
            {

                throw new Exception("Value too short to substitute all characters in the mask", e);
            }
        }

        private static bool IsReserva(char prmItem)
        {
            foreach (char item in reserva)
                if (item == prmItem) return true;

            return false;
        }
        private static bool IsChave(char prmItem)
        {
            foreach (char item in chave)
                if (item == prmItem) return true;

            return false;
        }
        private static bool ContemChave(string prmText)
        {
            foreach (char item in prmText)
                if (IsChave(item)) return true;
            return false;

        }

    }

}
