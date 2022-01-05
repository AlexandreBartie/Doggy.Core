using Dooggy.Factory.Data;
using Dooggy.Lib.Generic;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dooggy.Factory.Console
{
    public class TestConsole
    {

        public TestDataProject Project;

        private TestConsoleImport Import;

        private TestSessions Sessoes;

        public TestDataLocal Dados => Project.Dados;
        public TestDataPool Pool => Dados.Pool;
        public TestTrace Trace => Dados.Trace;

        private TestConsoleExport Export => Sessoes.Export;

        public string output { get => Export.resultado; }

        public TestConsole(TestDataProject prmDataProject)
        {

            Project = prmDataProject;   

            Import = new TestConsoleImport(this);

            Sessoes = new TestSessions(this);

        }

        public void Setup(string prmPathINI, string prmPathOUT)
        {

            Import.Setup(prmPathINI);

            Pool.SetPathDestino(prmPathOUT);

        }
        public void SetAncora(DateTime prmAncora) => Pool.SetAncora(prmAncora);
        public void Start(string prmPathINI) => Import.Start(prmPathINI);

        public void ImportINI(string prmArquivoINI) => Import.Play(prmArquivoINI);

        public void Play(string prmBloco) => Play(prmBloco, prmArquivoOUT: "");
        public void Play(string prmBloco, string prmArquivoOUT) => Sessoes.Play(prmBloco, prmArquivoOUT);
        public void Save(string prmData) => Sessoes.Save(prmData);
        
        public string GetArquivoOUT()
        {
            
            string nome = Import.ArquivoINI.nome;

            if (xString.IsStringOK(nome))
                return(nome);

            return (Export.nome);

        }

    }

    public class TestSessions : List<TestSession>
    {

        private TestConsole Console;

        public TestSession Corrente;

        public TestTrace Trace => Console.Trace;

        public TestConsoleExport Export => Corrente.Export; 

        public TestSessions(TestConsole prmConsole)
        {

            Console = prmConsole;

        }

        public void Play(string prmBloco, string prmArquivoOUT) { Criar(); Corrente.Play(prmBloco, prmArquivoOUT); }

        public void Save(string prmData) { Corrente.Save(prmData); }

        private void Criar()
        {

            Corrente = new TestSession(Console);

            Add(Corrente);

        }

    }

    public class TestSession
    {

        public TestConsole Console;

        private TestBuilder Builder;

        public TestConsoleExport Export;

        public string output { get => Export.resultado; }

        public TestSession(TestConsole prmConsole)
        {

            Console = prmConsole;

            Builder = new TestBuilder(this);

            Export = new TestConsoleExport();

        }

        public void Play(string prmBloco, string prmArquivoOUT)
        {

            Export.Setup(prmArquivoOUT);

            Builder.Play(prmBloco);

        }
        
        public void Save(string prmData) => Export.resultado = prmData;

    }

    public class TestConsoleExport
    {

        public string nome;

        public string resultado;

        public void Setup(string prmArquivoOUT) => nome = prmArquivoOUT;

    }

}
