using Dooggy.Lib.Generic;
using Dooggy.Lib.Vars;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dooggy.FACTORY.UNIT
{
    public static class TestUnityAnalise
    {
        public static string GetAnaliseTexto(string prmTexto)
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
