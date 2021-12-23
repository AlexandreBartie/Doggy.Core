using Dooggy.Factory.Data;
using Dooggy.Lib.Files;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dooggy.Factory.Console
{
    public class TestConsoleImport
    {

        private TestConsole Console;

        private Path PathDataINI = new Path();

        private FileTXT File = new FileTXT();

        public string arquivoINI;

        private string extensao = "ini";

        public TestDataLocal Dados => Console.Dados;
        public TestTrace Trace => Console.Trace;

        public TestConsoleImport(TestConsole prmConsole)
        {

            Console = prmConsole;

        }

        public void Setup(string prmPathINI)
        {

            PathDataINI.Setup(prmPathINI);

            Trace.LogPath.SetPath(prmContexto: "ConfiguracaoMassaTestes", prmPathINI);

        }

        public void Start(string prmPathINI)
        {

            Setup(prmPathINI);


            //Play(prmArquivo: );
            //Play(prmArquivo: );

        }

        public void Play(string prmNome) => Play(prmNome, prmSubPath: "");
        public void Play(string prmNome, string prmSubPath)
        {

            arquivoINI = prmNome;

            Console.Play(prmBloco: Open(arquivoINI, prmSubPath));

        }
        private string Open(string prmNome, string prmSubPath)
        {

            string path = PathDataINI.GetPath(prmSubPath);

            if (File.Open(path, prmNome, extensao))
            {

                Trace.LogFile.DataFileImport(prmNome, extensao, prmSubPath);

                return File.txt();

            }

            else
                Trace.LogFile.FailDataFileOpen(arquivoINI);

            return ("");

        }

    }
}
