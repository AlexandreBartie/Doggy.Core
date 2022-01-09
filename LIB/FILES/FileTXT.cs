using System;
using System.Diagnostics;
using System.Windows.Forms;
using System.Text;
using System.IO;

namespace Dooggy.Lib.Files
{
    public class FileTXT
    {

        public string[] lines;

        private Diretorio _diretorio;

        private Encoding encoding = null;

        private bool _IsOK;

        public bool IsOK { get => _IsOK; }

        public void SetEncoding(Encoding prmEncoding) => encoding = prmEncoding;

        public Diretorio Diretorio { get { if (_diretorio == null) _diretorio = new Diretorio(); return _diretorio; } }

        public virtual bool Open(string prmPath, string prmName, string prmExtensao) => Open(prmPath, prmName + "." + prmExtensao);
        public virtual bool Open(string prmPath, string prmName) => Open(prmArquivo: prmPath + prmName);
        public virtual bool Open(string prmArquivo)
        {

            try
            {

                if ((encoding == null))
                    lines = System.IO.File.ReadAllLines(prmArquivo);
                else
                    lines = System.IO.File.ReadAllLines(prmArquivo, encoding);

                _IsOK = true;

            }
            catch
            {

                _IsOK = false;

            }

            return (_IsOK);

        }

        public bool Save(string prmPath, string prmArquivo, string prmConteudo) => Save(prmPath, prmArquivo, prmConteudo, prmEncoding: Encoding.UTF8);
        public bool Save(string prmPath, string prmArquivo, string prmConteudo, Encoding prmEncoding)
        {

            if (Diretorio.Criar(prmPath))
            {

                try
                {
                    File.WriteAllText(prmPath + prmArquivo, prmConteudo, prmEncoding);

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
