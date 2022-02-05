using System;
using System.Collections.Generic;
using System.Globalization;
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

    public static class xDouble
    {

        public static string GetFormatacao(Double prmNumber, string prmFormato) => GetFormatacao(prmNumber, prmFormato, prmRegionalizacao: false);
        public static string GetFormatacao(Double prmNumber, string prmFormato, bool prmRegionalizacao)
        {
            IFormatProvider cultura;

            if (prmRegionalizacao)
                cultura = CultureInfo.CurrentUICulture;
            else
                cultura = CultureInfo.InvariantCulture;

            return (prmNumber.ToString(prmFormato, provider: cultura));
        }

    }
}
