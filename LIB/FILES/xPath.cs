using System;
using System.Collections.Generic;
using System.Text;

namespace Dooggy.Lib.Files
{
    public class xPath
    {

        private string path;

        private bool TemTerminal { get => xString.GetLast(path) == @"\"; }

        public void SetPath(string prmPath)
        {

            path = prmPath;

        }

        public string GetPath() => GetPath(prmSubPath: "");
        public string GetPath(string prmSubPath)
        {

            string retorno = path;

            if (prmSubPath != "")
            {
                if (!TemTerminal)
                    retorno += @"\";

                retorno += prmSubPath + @"\";
            }


            return (retorno);

        }

    }
}
