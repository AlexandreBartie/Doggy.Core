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

        public TestConsoleInput Input;

        public TestConsoleOutput Output;

        private TestConsoleSessions Sessoes;

        public TestDataLocal Dados => Factory.Dados;
        public TestDataPool Pool => Factory.Pool;
        public TestTrace Trace => Factory.Trace;

        public TestConsoleResult Result => Sessoes.Result;

        public string log { get => Sessoes.GetLog(); }
        public string resultado { get => Result.resultado; }

        public Arquivos ScriptsINI() => Input.ScriptINI.ScriptsINI();

        public void SetAncora(DateTime prmAncora) => Pool.SetAncora(prmAncora);

        public TestConsole(TestFactory prmFactory)
        {

            Factory = prmFactory;

            Input = new TestConsoleInput(this);

            Output = new TestConsoleOutput(this);

            Sessoes = new TestConsoleSessions(this);

        }

        public void Setup(string prmPathINI, string prmPathOUT)
        {

            Input.SetPath(prmPathINI);

            Output.SetPath(prmPathOUT);

        }
        public void Start(string prmPathINI, string prmPathOUT)
        {

            Setup(prmPathINI, prmPathOUT);

            Input.Start(prmPathINI); ;

        }
        
        public void Import(string prmScriptINI) => Input.Play(prmScriptINI);

        public void Play(string prmBloco) => Play(prmBloco, prmArquivoOUT: "");
        public void Play(string prmBloco, string prmArquivoOUT) => Sessoes.Play(prmBloco, prmArquivoOUT);
        public void Save(string prmData) => Sessoes.Save(prmData);

        public void AddLog() => Sessoes.AddLog();
       
    }

    public class TestConsoleInput
    {

        private TestConsole Console;

        public TestConsoleScriptINI ScriptINI;

        public TestConsoleInput(TestConsole prmConsole)
        {

            Console = prmConsole;

            ScriptINI = new TestConsoleScriptINI(Console);

        }

        public void SetPath(string prmPath)
        {

            ScriptINI.SetPath(prmPath);

        }

        public void Start(string prmPath)
        {

            foreach (Arquivo file in Console.ScriptsINI())
                Play(prmScriptINI: file.nome_curto);

        }

        public void Play(string prmScriptINI) => Play(prmScriptINI, prmSubPath: "");

        public void Play(string prmScriptINI, string prmSubPath) => Console.Play(prmBloco: Open(prmScriptINI, prmSubPath));

        private string Open(string prmScriptINI, string prmSubPath) => ScriptINI.Open(prmScriptINI, prmSubPath);

    }

    public class TestConsoleOutput
    {

        private TestConsole Console;

        private Path PathDataFiles;

        private TestConsoleInput Input => Console.Input;
        private TestConsoleResult Result => Console.Result;

        public TestTrace Trace => Console.Trace;

        public TestConsoleOutput(TestConsole prmConsole)
        {

            Console = prmConsole;

            PathDataFiles = new Path();

        }

        public void SetPath(string prmPath)
        {

            PathDataFiles.Setup(prmPath);

            Trace.LogPath.SetPath(prmContexto: "DestinoMassaTestes", prmPath);

        }
        public string GetScriptOrigem()
        {

            string nome = Input.ScriptINI.nome;

            if (xString.IsStringOK(nome))
                return (nome);

            return (Console.Result.nome);

        }

        public string GetFullPath(eTipoFileFormat prmTipo)
        {

            return (GetPath(prmSubPath: GetExtensao(prmTipo)));

        }
        public string GetExtensao(eTipoFileFormat prmTipo)
        {

            switch (prmTipo)
            {

                case eTipoFileFormat.csv:
                    return "csv";

                case eTipoFileFormat.txt:
                    return "txt";

            }

            return "json";

        }
        public string GetPath(string prmSubPath) => (PathDataFiles.GetPath(prmSubPath));
    }

    public class TestConsoleResult
    {

        public string nome;

        public string resultado;

        public void Setup(string prmArquivoOUT) => nome = prmArquivoOUT;

    }

    public class TestConsoleScriptINI
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

        public Arquivos ScriptsINI() => DiretorioINI.files.GetFiltro("*.ini");

        private TestTrace Trace => Console.Trace;

        public TestConsoleScriptINI(TestConsole prmConsole)
        {

            Console = prmConsole;

        }

        public void SetPath(string prmPath)
        {

            DiretorioINI.Setup(prmPath);


            Trace.LogPath.SetPath(prmContexto: "OrigemMassaTestes", prmPath);

        }

        public string Open(string prmScriptINI, string prmSubPath)
        {

            nome = prmScriptINI; sub_path = prmSubPath;

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

        public TestConsoleResult Result => Corrente.Result;

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

        public string GetLog()
        {

            if (Corrente != null)
                return Corrente.Log.txt;


            return ("");
        }

    }

    public class TestConsoleSession
    {

        public TestConsole Console;

        private TestBuilder Builder;

        public TestCommands Commands;

        public TestConsoleResult Result;

        public TestDataLog Log;

        public string output { get => Result.resultado; }

        public TestConsoleSession(TestConsole prmConsole)
        {

            Console = prmConsole;

            Commands = new TestCommands(this);

            Builder = new TestBuilder(this);

            Result = new TestConsoleResult();

            Log = new TestDataLog();

        }

        public void Play(string prmBloco, string prmArquivoOUT)
        {

            Log.Start();

            Result.Setup(prmArquivoOUT);

            Builder.Compile(prmBloco);

            Commands.Play();

            Log.Stop();

        }

        public void Save(string prmData) => Result.resultado = prmData;

        public void AddLog(TestTraceMsg prmMsg) => Log.AddLog(prmTipo: prmMsg.tipo, prmTexto: prmMsg.texto);

    }

}
