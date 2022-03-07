using Dooggy.Lib.Files;
using Dooggy;
using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Dooggy.Lib.Vars;
using Dooggy.Lib.Generic;

namespace Dooggy.Lib.Files
{

    public class Path
    {

        public string path = "";

        private bool TemTerminal { get => myString.GetLast(path) == @"\"; }

        public bool IsFull => myString.IsFull(path);

        public bool IsFind => Directory.Exists(path);

        public bool SetPath(string prmPath)
        {

            path = prmPath;

            return(IsFull);

        }
        public bool SetSubPath(string prmSubPath) => SetPath(prmPath: path + prmSubPath);

        public bool Criar() => Criar(path);
        public bool Criar(string prmPath)
        {

            if (SetPath(prmPath))
            {
                try
                {
                    if (!IsFind)
                        // Se o diretório não existir... Criamos o diretório ...
                        Directory.CreateDirectory(path);
                }
                catch
                { }

            }
            return (IsFind);
        }

        public string GetPath(string prmSubPath) => GetPath(prmSubPath, prmNome: "");
        public string GetPath(string prmSubPath, string prmNome) => GetPath(prmSubPath, prmNome, prmExtensao: "");
        public string GetPath(string prmSubPath, string prmNome, string prmExtensao)
        {

            string retorno = path;

            if (myString.IsFull(prmSubPath))
            {
                if (!TemTerminal)
                    retorno += @"\";

                retorno += prmSubPath + @"\";
            }

            if (myString.IsFull(prmNome))
            {
                retorno += prmNome;

                if (myString.IsFull(prmExtensao))
                    retorno += "." + prmExtensao;
            }
            return (retorno);
        }
    }

    public class Diretorio : Path
    {

        public Arquivos files;

        public Arquivos Filtro(string prmFiltro) => files.GetFiltro(prmFiltro);

        public Diretorio()
        {

            files = new Arquivos(this);

        }
        public Diretorio(string prmPath)
        {

            SetPath(prmPath);

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

            return memo.txt;

        }
        private string GetLinhas()
        {

            xMemo memo = new xMemo();

            foreach (Arquivo file in this)
                memo.Add(String.Format("#{0}", file.qtde_linhas));

            return memo.txt;

        }
        private string[] GetFiles(string prmFiltro)
        {

            try
            {
                return (Directory.GetFiles(Diretorio.path, prmFiltro));
            }
            catch (Exception e)
            { Debug.WriteLine (string.Format("{0}: -err: {1}", "Erro de carregamento dos arquivos do diretório.", e.Message)) ; }

            return (null);

        }

    }

    public class ArquivoInfo
    {

        public FileInfo info;

        public FileTXT file = new FileTXT();

        public string nome { get => info.Name; }
        public string nome_curto { get => myString.GetRemove(nome, prmParte: extensao); }
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
