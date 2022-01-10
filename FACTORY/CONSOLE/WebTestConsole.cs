using Dooggy.Factory.Data;
using Dooggy.Lib.Files;
using Dooggy.Lib.Generic;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dooggy.Factory.Console
{
    public class TestConsole
    {

        public TestFactory Factory;

        private TestConsoleInput Input;

        private TestConsoleSessions Sessoes;

        public TestDataLocal Dados => Factory.Dados;
        public TestDataPool Pool => Factory.Pool;
        public TestTrace Trace => Factory.Trace;

        private TestConsoleOutput Output => Sessoes.Output;

        public string resultado { get => Output.resultado; }

        public TestConsole(TestFactory prmFactory)
        {

            Factory = prmFactory;

            Input = new TestConsoleInput(this);

            Sessoes = new TestConsoleSessions(this);


        }

        public void Setup(string prmPathINI, string prmPathOUT)
        {

            Input.SetPathINI(prmPathINI);

            Pool.SetPathOUT(prmPathOUT);

        }
        public void SetAncora(DateTime prmAncora) => Pool.SetAncora(prmAncora);
        public void Start(string prmPathINI) => Input.Start(prmPathINI);

        public void Import(string prmArquivoINI) => Input.Play(prmArquivoINI);

        public void Play(string prmBloco) => Play(prmBloco, prmArquivoOUT: "");
        public void Play(string prmBloco, string prmArquivoOUT) => Sessoes.Play(prmBloco, prmArquivoOUT);
        public void Save(string prmData) => Sessoes.Save(prmData);

        public void AddLog() => Sessoes.AddLog();
        
        public string GetArquivoOUT()
        {
            
            string nome = Input.ArquivoINI.nome;

            if (xString.IsStringOK(nome))
                return(nome);

            return (Output.nome);

        }

    }

    public class TestConsoleInput
    {

        private TestConsole Console;

        public TestConsoleArquivoINI ArquivoINI;

        public TestConsoleInput(TestConsole prmConsole)
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

    public class TestConsoleOutput
    {

        public string nome;

        public string resultado;

        public void Setup(string prmArquivoOUT) => nome = prmArquivoOUT;

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

    public class TestConsoleSessions : List<TestConsoleSession>
    {

        private TestConsole Console;

        public TestConsoleSession Corrente;

        public TestTrace Trace => Console.Trace;

        public TestDataPool Pool => Console.Pool;

        public TestConsoleOutput Output => Corrente.Output;

        public TestConsoleSessions(TestConsole prmConsole)
        {

            Console = prmConsole;

        }

        public void Play(string prmBloco, string prmArquivoOUT) { Criar(); Corrente.Play(prmBloco, prmArquivoOUT); }

        public void Save(string prmData) { Corrente.Save(prmData); }

        private void Criar()
        {

            Corrente = new TestConsoleSession(Console);

            Add(Corrente);

            Pool.Cleanup();

        }

        public void AddLog()
        {

            if (Corrente != null)
                Corrente.AddLog(prmMsg: Trace.Corrente);

        }

    }

    public class TestConsoleSession
    {

        public TestConsole Console;

        private TestBuilder Builder;

        public TestCommands Commands;

        public TestConsoleOutput Output;

        public TestDataLog Log;

        public string output { get => Output.resultado; }

        public TestConsoleSession(TestConsole prmConsole)
        {

            Console = prmConsole;

            Commands = new TestCommands(this);

            Builder = new TestBuilder(this);

            Output = new TestConsoleOutput();

            Log = new TestDataLog();

        }

        public void Play(string prmBloco, string prmArquivoOUT)
        {

            Log.Start();

            Output.Setup(prmArquivoOUT);

            Builder.Compile(prmBloco);

            Commands.Play();

            Log.Stop();

        }

        public void Save(string prmData) => Output.resultado = prmData;

        public void AddLog(TestTraceMsg prmMsg) => Log.AddLog(prmTipo: prmMsg.tipo, prmTexto: prmMsg.texto);

    }

}
