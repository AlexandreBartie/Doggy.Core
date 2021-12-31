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

        public void Setup(string prmPathINI)
        {

            ArquivoINI.Setup(prmPathINI);

        }

        public void Start(string prmPathINI)
        {

            Setup(prmPathINI);

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

        public void Setup(string prmPathINI)
        {

            DiretorioINI.Setup(prmPathINI);

            Trace.LogPath.SetPath(prmContexto: "ConfiguracaoMassaTestes", prmPathINI);

        }

        public string Open(string prmArquivoINI, string prmSubPath)
        {

            nome = prmArquivoINI; sub_path = prmSubPath;

            File = new FileTXT();

            if (File.Open(path_completo, prmArquivoINI, extensao))
            {

                Trace.LogFile.DataFileImport(prmArquivoINI, extensao, prmSubPath);

                return File.txt();

            }

            else
                Trace.LogFile.FailDataFileOpen(path_completo + nome + "." + extensao);

            return ("");

        }


    }

    //public class TestConsoleConexao
    //{

    //    private TestConsole Console;

    //    private TestDataConnect Connect => Console.Pool.Connect;

    //    private xJSON args => Console.Project.args;

    //    public TestConsoleConexao(TestConsole prmConsole)
    //    {

    //        Console = prmConsole;

    //    }
    //    public void Setup()
    //    {

    //        //Connect.Oracle.user = "desenvolvedor_sia";
    //        //Connect.Oracle.password = "asdfg";

    //        //Connect.Oracle.host = "10.250.1.35";
    //        //Connect.Oracle.port = args.GetValor("port", "1521");

    //        //Connect.Oracle.service = "branch_1085.prod01.redelocal.oraclevcn.com";

    //        //Connect.Oracle.Add(prmTag: "SIA");

    //    }


    //}
}
