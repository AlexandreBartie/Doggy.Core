using Dooggy.Lib.Generic;
using Dooggy.Lib.Vars;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dooggy.FACTORY.UNIT
{
    public static class TestUnityLog
    {
        public static string GetAnalise(string prmGerado, string prmEsperado)
        {
            return string.Format("{4}Gerado: <{1}>{4}{0}{4}Esperado:<{3}>{4}{2}{4}", prmGerado, GetAnaliseTexto(prmGerado), prmEsperado, GetAnaliseTexto(prmEsperado), Environment.NewLine);
        }

        private static string GetAnaliseTexto(string prmTexto)
        {

            try
            {
                if (myString.IsFull(prmTexto))
                {
                    string txt = string.Format("[{0}]", prmTexto.Length);

                    foreach (string linha in new xLista(prmTexto))
                        txt += string.Format(":{0}", linha.Length);

                    return txt;
                }
            }
            catch (Exception e)
            { return (string.Format("{0} -err: {1}", prmTexto, e.Message)); }            

            return ("");
        }

    }
}
