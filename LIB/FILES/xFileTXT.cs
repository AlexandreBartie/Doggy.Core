using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Text;

namespace Dooggy.LIB
{
    public class xFileTXT
    {

        public string[] lines;

        private bool _IsOK;

        public bool IsOK { get => _IsOK; }

        public virtual bool Open(string prmPath, string prmName)
        {

            try
            {
                
                lines = System.IO.File.ReadAllLines(prmPath + prmName);

                _IsOK = true;

            }
            catch (Exception e)
            {

                _IsOK = false;

            }

            return (_IsOK);

        }


        public string memo()
        {
            string text = "";

            if (IsOK)
            {
                foreach (string line in lines)
                {

                    text += line + Environment.NewLine;

                }
            }

            return (text);

        }

    }
}
