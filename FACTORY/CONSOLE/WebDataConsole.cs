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

        public string arquivoINI => Import.ArquivoINI.nome;

        public string output { get => Sessoes.output; }

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

        public void Start(string prmPathINI) => Import.Start(prmPathINI);

        public void ImportINI(string prmArquivoINI) => Import.Play(prmArquivoINI);

        public void Play(string prmBloco) => Sessoes.Play(prmBloco);

        public void Save(string prmData) => Sessoes.Save(prmData);



        //public void Write(string prmLinha) => Commands.Write(prmLinha);
        //public void Write(string prmWord, string prmTarget) => Write(prmWord, prmTarget, prmParameters: "");
        //public void Write(string prmWord, string prmTarget, string prmParameters) => Write(prmLinha: Commands.GetFormatLine(prmWord, prmTarget, prmParameters));


    }

    public class TestSessions : List<TestSession>
    {

        private TestConsole Console;

        public TestSession Corrente;

        public TestTrace Trace => Console.Trace;

        public string output { get => Corrente.output; }

        public TestSessions(TestConsole prmConsole)
        {

            Console = prmConsole;

        }

        public void Play(string prmBloco) { Criar(); Corrente.Play(prmBloco); }

        public void Save(string prmData) { Corrente.Save(prmData); }

        private void Criar()
        {

            Corrente = new TestSession(Console);

            Add(Corrente);

        }

    }

    public class TestSession
    {

        private TestConsole Console;

        private TestCommands Commands;

        public string arquivoINI { get => Console.arquivoINI; }

        public string output { get => Commands.output; }

        public TestSession(TestConsole prmConsole)
        {

            Console = prmConsole;

            Commands = new TestCommands(prmConsole);

        }

        public void Play(string prmBloco) => Commands.Play(prmBloco); 
        
        public void Save(string prmData) => Commands.output = prmData;

    }

}
