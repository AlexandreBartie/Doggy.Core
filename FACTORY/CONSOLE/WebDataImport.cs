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

        private Diretorio DiretorioINI = new Diretorio();

        public string arquivoINI {  get => DiretorioINI.path; }

        private TestConsoleConexao Conexao;

        private string extensao = "ini";

        public TestDataLocal Dados => Console.Dados;
        public TestTrace Trace => Console.Trace;

        public TestConsoleImport(TestConsole prmConsole)
        {

            Console = prmConsole;

            Conexao = new TestConsoleConexao(prmConsole);

        }

        public void Setup(string prmPathINI)
        {

            DiretorioINI.Setup(prmPathINI);

            Trace.LogPath.SetPath(prmContexto: "ConfiguracaoMassaTestes", prmPathINI);

        }

        public void Start(string prmPathINI)
        {

            Conexao.Setup();

            Setup(arquivoINI);

            foreach (Arquivo file in DiretorioINI.files.GetFiltro())
                Play(prmArquivoINI: file.nome_curto);

        }

        public void Play(string prmArquivoINI) => Play(prmArquivoINI, prmSubPath: "");

        public void Play(string prmArquivoINI, string prmSubPath)
        {

            Console.Play(prmBloco: Open(prmArquivoINI, prmSubPath));

        }
        private string Open(string prmArquivoINI, string prmSubPath)
        {

            string path = DiretorioINI.GetPath(prmSubPath);

            FileTXT File = new FileTXT();

            if (File.Open(path, prmArquivoINI, extensao))
            {

                Trace.LogFile.DataFileImport(prmArquivoINI, extensao, prmSubPath);

                return File.txt();

            }

            else
                Trace.LogFile.FailDataFileOpen(path + arquivoINI + "." + extensao);

            return ("");

        }

    }

    public class TestConsoleConexao
    {

        private TestConsole Console;

        private TestDataConnect Connect => Console.Pool.Connect;

        private xJSON args => Console.Project.args;

        public TestConsoleConexao(TestConsole prmConsole)
        {

            Console = prmConsole;

        }
        public void Setup()
        {

            //Connect.Oracle.user = "desenvolvedor_sia";
            //Connect.Oracle.password = "asdfg";

            //Connect.Oracle.host = "10.250.1.35";
            //Connect.Oracle.port = args.GetValor("port", "1521");

            //Connect.Oracle.service = "branch_1085.prod01.redelocal.oraclevcn.com";

            //Connect.Oracle.Add(prmTag: "SIA");

        }


    }
}
