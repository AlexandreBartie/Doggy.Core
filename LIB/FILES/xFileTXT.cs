using System;
using System.Diagnostics;
using System.Windows.Forms;
using System.Text;

namespace Dooggy.LIB.FILES
{
    public class xFileTXT
    {

        public string[] lines;

        private Encoding encoding = null;

        private bool _IsOK;

        public bool IsOK { get => _IsOK; }

        public void SetEncoding(Encoding prmEncoding)
        {

            encoding = prmEncoding;

        }

        public virtual bool Open(string prmPath, string prmName)
        {

            try
            {

                string arquivo = prmPath + prmName;

                if ((encoding == null))
                    lines = System.IO.File.ReadAllLines(arquivo);
                else
                    lines = System.IO.File.ReadAllLines(arquivo, encoding);

                _IsOK = true;

            }
            catch (Exception e)
            {

                _IsOK = false;

                Debug.Assert(false);

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
