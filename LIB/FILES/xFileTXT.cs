using System;
using System.Diagnostics;
using System.Windows.Forms;
using System.Text;
using System.IO;

namespace Dooggy.Lib.Files
{
    public class xFileTXT
    {

        public string[] lines;

        private xDiretorio _diretorio;

        private Encoding encoding = null;

        private bool _IsOK;

        public bool IsOK { get => _IsOK; }

        public void SetEncoding(Encoding prmEncoding) => encoding = prmEncoding;

        public xDiretorio Diretorio { get { if (_diretorio == null) _diretorio = new xDiretorio(); return _diretorio; } }

        public virtual bool Open(string prmPath, string prmName, string prmExtensao) => Open(prmPath, prmName + "." + prmExtensao);
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
            catch
            {

                _IsOK = false;

            }

            return (_IsOK);

        }

        public bool Save(string prmPath, string prmName, string prmConteudo) => Save(prmPath, prmName, prmConteudo, prmExtensao: "txt");
        public bool Save (string prmPath, string prmName, string prmConteudo, string prmExtensao)
        {

            string nome_completo = GetNomeCompleto(prmPath, prmName, prmExtensao);

            if (Diretorio.Criar(prmPath))
            {

                try
                {
                    File.WriteAllText(nome_completo, prmConteudo);

                    return (true);
                }
                catch
                { }

            }

            return (false);

        }

        public string txt()
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

        private string GetNomeCompleto(string prmPath, string prmNome, string prmExtensao) => String.Format("{0}{1}.{2}", prmPath, prmNome, prmExtensao);

    }
}
