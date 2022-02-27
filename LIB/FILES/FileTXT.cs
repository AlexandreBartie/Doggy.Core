using System;
using System.Diagnostics;
using System.Windows.Forms;
using System.Text;
using System.IO;
using Dooggy.Lib.Vars;

namespace Dooggy.Lib.Files
{
    public class FileTXT
    {

        public string[] lines;

        public string arquivo;

        public string path => System.IO.Path.GetDirectoryName(arquivo);
        public string nome_completo => System.IO.Path.GetFileName(arquivo);

        private Diretorio _diretorio;

        private Encoding encoding = null;

        private bool IsErro;

        public bool IsOK => !IsErro;

        public void SetEncoding(Encoding prmEncoding) => encoding = prmEncoding;

        public Diretorio Diretorio { get { if (_diretorio == null) _diretorio = new Diretorio(); return _diretorio; } }

        public virtual bool IsValidName(string prmArquivo) => (myString.GetFirst(prmArquivo, prmDelimitador: ".") != "");

        public virtual bool Open(string prmPath, string prmName) => Open(prmArquivo: prmPath + prmName);
        public virtual bool Open(string prmArquivo)
        {

            try
            {

                arquivo = prmArquivo;

                if ((encoding == null))
                    lines = System.IO.File.ReadAllLines(arquivo);
                else
                    lines = System.IO.File.ReadAllLines(arquivo, encoding);

                IsErro = false;

            }
            catch
            {

                IsErro = true;

            }

            return (IsOK);

        }

        public bool Save(string prmPath, string prmArquivo, string prmConteudo) => Save(prmPath, prmArquivo, prmConteudo, prmEncoding: Encoding.UTF8);
        public bool Save(string prmPath, string prmArquivo, string prmConteudo, Encoding prmEncoding)
        {

            if (Diretorio.Criar(prmPath))
            {

                try
                {

                    arquivo = prmPath + prmArquivo;

                    File.WriteAllText(arquivo, prmConteudo, prmEncoding);

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
