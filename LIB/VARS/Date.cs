using System;
using System.Collections.Generic;
using System.Text;

namespace Dooggy
{
    public static class xDate
    {

        public static string GetFormatacao(DateTime prmData, string prmFormato)
        {

            string retorno = prmData.ToString(xDate.GetFormatacao(prmFormato));

            return (retorno);

        }

        private static string GetFormatacao(string prmFormato)
        {

            string formato = prmFormato;

            formato = xString.GetSubstituir(formato, "A", "y");
            formato = xString.GetSubstituir(formato, "a", "y");

            formato = xString.GetSubstituir(formato, "D", "d");
            formato = xString.GetSubstituir(formato, "m", "M");

            return (formato);

        }

    }
}
