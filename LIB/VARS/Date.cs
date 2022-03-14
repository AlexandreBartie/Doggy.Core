using BlueRocket.CORE.Tools.Calc;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace BlueRocket.CORE.Lib.Vars
{
    public static class myDate
    {
        public static DateTime Calc(string prmSintaxe) => Calc(prmDate: DateTime.Today, prmSintaxe);
        public static DateTime Calc(DateTime prmDate, string prmSintaxe)
        {
            DynamicDate Data = new DynamicDate(prmDate);

            return Data.Calc(prmSintaxe);
        }

        public static string View(DateTime prmDate, string prmSintaxe)
        {
            DynamicDate Date = new DynamicDate(prmDate);

            return (Date.View(prmSintaxe));
        }

        public static string Static(DateTime prmDate, string prmFormato)
        {
            DynamicDate Date = new DynamicDate(prmDate);

            return (Date.Static(prmFormato));
        }
    }
}
