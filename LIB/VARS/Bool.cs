using System;
using System.Collections.Generic;
using System.Text;

namespace BlueRocket.CORE.Lib.Vars
{
    public static class myBool
    {

        public static string IIf(bool prmCondicao, string prmTrue, string prmFalse)
        {

            if (prmCondicao)
                return (prmTrue);

            return (prmFalse);

        }

    }
}
