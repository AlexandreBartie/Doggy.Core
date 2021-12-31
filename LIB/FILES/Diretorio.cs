using Dooggy.Lib.Files;
using Dooggy.Lib.Generic;
using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.IO;
using System.Text;
using static Dooggy.xInt;

namespace Dooggy
{

    public class Path
    {

        public string path = "";

        private bool TemTerminal { get => xString.GetLast(path) == @"\"; }

        public bool Setup(string prmPath)
        {

            if (xString.IsStringOK(prmPath))
            {
                
                path = prmPath;

                return (true);

            }

            return(false);

        }
        public bool Criar() => Criar(path);
        public bool Criar(string prmPath)
        {

            if (Setup(prmPath))
            {

                try
                {
                    // Se o diretório não existir...
                    if (!Directory.Exists(path))
                    {

                        // Criamos o diretório ...
                        Directory.CreateDirectory(path);

                    }

                    return (true);
                }
                catch
                { }

            }

            return (false);

        }


        public string GetPath(string prmSubPath) => GetPath(prmSubPath, prmNome: "");
        public string GetPath(string prmSubPath, string prmNome) => GetPath(prmSubPath, prmNome, prmExtensao: "");
        public string GetPath(string prmSubPath, string prmNome, string prmExtensao)
        {

            string retorno = path;

            if (xString.IsStringOK(prmSubPath))
            {
                if (!TemTerminal)
                    retorno += @"\";

                retorno += prmSubPath + @"\";

            }

            if (xString.IsStringOK(prmNome))
            {

                retorno += prmNome;

                if (xString.IsStringOK(prmExtensao))
                    retorno += "." + prmExtensao;

            }

            return (retorno);

        }

    }

    public class Diretorio : Path
    {

        public Arquivos files;

        public Diretorio()
        {

            files = new Arquivos(this);

        }
        public Diretorio(string prmPath)
        {

            Setup(prmPath);

            files = new Arquivos(this);

        }

    }

    public class Arquivo : ArquivoInfo
    {

        private Diretorio Diretorio;

        public Arquivo(string prmNome, Diretorio prmDiretorio)
        {

            info = new FileInfo(prmNome);

            Diretorio = prmDiretorio;

        }

    }

    public class Arquivos : List<Arquivo>
    {

        private Diretorio Diretorio;

        public string lista { get => GetLista(); }

        public string qtde_linhas { get => GetLinhas(); }

        public Arquivos(Diretorio prmDiretorio)
        {

            Diretorio = prmDiretorio;

        }

        public Arquivos GetFiltro() => GetFiltro(prmFiltro: "*.*");
        public Arquivos GetFiltro(string prmFiltro)
        {

            Arquivos retorno = new Arquivos(Diretorio);

            string[] files = GetFiles(prmFiltro);

            if (files != null)
            {

                foreach (string file in files)
                    retorno.Add(new Arquivo(file, Diretorio));

            }

            return (retorno);

        }
        private string GetLista()
        {

            xMemo memo = new xMemo();

            foreach (Arquivo file in this)
                memo.Add(file.nome);

            return memo.memo();

        }
        private string GetLinhas()
        {

            xMemo memo = new xMemo();

            foreach (Arquivo file in this)
                memo.Add(String.Format("#{0}", file.qtde_linhas));

            return memo.memo();

        }
        private string[] GetFiles(string prmFiltro)
        {

            try
            {
                return (Directory.GetFiles(Diretorio.path, prmFiltro));
            }
            catch (Exception e)
            { Debug.WriteLine (string.Format("{0}: -err: {1}", "Erro na leituras do arquivos do diretório.", e.Message)) ; }

            return (null);

        }

    }

    public class ArquivoInfo
    {

        public FileInfo info;

        public FileTXT file = new FileTXT();

        public string nome { get => info.Name; }
        public string nome_curto { get => xString.GetRemove(nome, prmParte: extensao); }
        public string nome_completo { get => info.FullName; }

        public string path { get => info.DirectoryName; }
        public string path_completo { get => info.FullName; }

        public string extensao { get => info.Extension; }

        public int qtde_linhas { get => GetQtdeLinhas(); }

        private int GetQtdeLinhas()
        {

            if (file.Open(nome_completo))
                return (file.lines.Length);

            return 0;

        }

    }

}
