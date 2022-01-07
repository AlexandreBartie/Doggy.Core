using Dooggy.Factory;
using Dooggy.Factory.Console;
using Dooggy.Factory.Data;
using Dooggy.Lib.Files;
using Dooggy.Lib.Parse;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dooggy.Factory.Console
{
    public class TestConsoleImport
    {

        private TestConsole Console;

        public TestConsoleArquivoINI ArquivoINI;

        public TestConsoleImport(TestConsole prmConsole)
        {

            Console = prmConsole;

            ArquivoINI = new TestConsoleArquivoINI(Console);

        }

        public void SetPathINI(string prmPath)
        {

            ArquivoINI.SetPath(prmPath);

        }

        public void Start(string prmPath)
        {

            SetPathINI(prmPath);

            foreach (Arquivo file in ArquivoINI.GetDisponiveis())
                Play(prmArquivoINI: file.nome_curto);

        }

        public void Play(string prmArquivoINI) => Play(prmArquivoINI, prmSubPath: "");

        public void Play(string prmArquivoINI, string prmSubPath) => Console.Play(prmBloco: Open(prmArquivoINI, prmSubPath));

        private string Open(string prmArquivoINI, string prmSubPath) => ArquivoINI.Open(prmArquivoINI, prmSubPath);

    }

    public class TestConsoleArquivoINI
    {

        private TestConsole Console;

        private Diretorio DiretorioINI = new Diretorio();

        private FileTXT File;

        public string nome;
        public string nome_extendido { get => nome + "." + extensao; }

        public string path { get => DiretorioINI.path; }

        private string sub_path;
        private string path_completo => DiretorioINI.GetPath(sub_path);

        private string extensao = "ini";

        public Arquivos GetDisponiveis() => DiretorioINI.files.GetFiltro("*.ini");

        private TestTrace Trace => Console.Trace;

        public TestConsoleArquivoINI(TestConsole prmConsole)
        {

            Console = prmConsole;

        }

        public void SetPath(string prmPath)
        {

            DiretorioINI.Setup(prmPath);

            Trace.LogPath.SetPath(prmContexto: "OrigemMassaTestes", prmPath);

        }

        public string Open(string prmArquivoINI, string prmSubPath)
        {

            nome = prmArquivoINI; sub_path = prmSubPath;

            File = new FileTXT();

            if (File.Open(path_completo, nome_extendido))
            {

                Trace.LogFile.DataFileImport(nome_extendido, prmSubPath);

                return File.txt();

            }

            else
                Trace.LogFile.FailDataFileOpen(path_completo, nome_extendido);

            return ("");

        }


    }


}
