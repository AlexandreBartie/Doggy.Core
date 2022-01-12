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

        private TestConsoleScripts Scripts;

        public TestDataLocal Dados => Factory.Dados;
        public TestDataPool Pool => Factory.Pool;
        public TestTrace Trace => Factory.Trace;

        public TestConsoleLog Log => Script.Log;

        public TestConsoleScript Script { get => Scripts.Corrente; }

        public Arquivos GetArquivosINI() => Input.GetArquivosINI();

        public void SetAncora(DateTime prmAncora) => Pool.SetAncora(prmAncora);

        public TestConsole(TestFactory prmFactory)
        {

            Factory = prmFactory;

            Input = new TestConsoleInput(this);

            Output = new TestConsoleOutput(this);

            Scripts = new TestConsoleScripts(this);

        }

        public void Setup(string prmPathINI, string prmPathOUT) => Setup(prmPathINI, prmPathOUT, prmStart: false);
        public void Setup(string prmPathINI, string prmPathOUT, bool prmStart)
        {

            Input.SetPath(prmPathINI);

            Output.SetPath(prmPathOUT);

            if (prmStart)
                Input.Start(prmPathINI);

        }
        
        public void Import(string prmArquivoINI) => Scripts.Import(prmArquivoINI);

        public void Play(string prmCode) => Play(prmCode, prmArquivoOUT: "");
        public void Play(string prmCode, string prmArquivoOUT) => Scripts.Play(prmCode, prmArquivoOUT);
        public void Save(string prmData) => Scripts.Save(prmData);

        public void AddLog() => Scripts.AddLog();

    }

    public class TestConsoleInput : TestConsoleArquivoINI
    {

        public Arquivos GetArquivosINI() => PathINI.files.GetFiltro("*.ini");

        public TestConsoleInput(TestConsole prmConsole)
        {

            Console = prmConsole;

        }

        public void SetPath(string prmPath)
        {

            PathINI.SetPath(prmPath);

            Trace.LogPath.SetPath(prmContexto: "OrigemMassaTestes", prmPath);

        }

        public void Start(string prmPath)
        {

            foreach (Arquivo file in GetArquivosINI())
                Console.Import(prmArquivoINI: file.nome_curto);

        }

    }

    public class TestConsoleOutput
    {

        private TestConsole Console;

        private Path PathOUT;

        private TestConsoleInput Input => Console.Input;

        public TestTrace Trace => Console.Trace;

        public TestConsoleOutput(TestConsole prmConsole)
        {

            Console = prmConsole;

            PathOUT = new Path();

        }

        public void SetPath(string prmPath)
        {

            PathOUT.SetPath(prmPath);

            Trace.LogPath.SetPath(prmContexto: "DestinoMassaTestes", prmPath);

        }
        public string GetScriptOrigem()
        {

            string nome = Console.Log.nome_OUT;

            if (xString.IsStringOK(nome))
                return (nome);

            return (Console.Log.nome_INI);

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
        public string GetPath(string prmSubPath) => (PathOUT.GetPath(prmSubPath));
    }

    public class TestConsoleLog : TestDataLog
    {

        public string nome_INI;

        public string nome_OUT;

        public string code;

        public string resultado;

        private TestDataLog Log;

        public void Setup(string prmArquivoINI)
        {

            nome_INI = prmArquivoINI;

        }
        public void Start(string prmArquivoOUT)
        {

            nome_OUT = prmArquivoOUT;

            base.Start();

        }

        public void SetCode(string prmCode)
        {

            code = prmCode;

        }

        public void SetResultado(string prmResultado)
        {

            resultado = prmResultado;

        }

    }

    public class TestConsoleScripts : List<TestConsoleScript>
    {

        private TestConsole Console;

        public TestConsoleScript Corrente;

        public TestTrace Trace => Console.Trace;

        public TestDataPool Pool => Console.Pool;

        private bool TemCorrente => (Corrente != null);

        public TestConsoleScripts(TestConsole prmConsole)
        {

            Console = prmConsole;

        }

        public void Import(string prmArquivoINI) { GetScript(prmArquivoINI); Corrente.Import(prmArquivoINI); } 

        public void Play(string prmCode, string prmArquivoOUT) { GetScript(prmArquivoOUT); Corrente.Play(prmCode, prmArquivoOUT); }

        public void Save(string prmData) { Corrente.Save(prmData); }


        public void AddLog()
        {

            if (TemCorrente)
                Corrente.AddLog(prmMsg: Trace.Corrente);

        }

        public string GetLog()
        {

            if (TemCorrente)
                return Corrente.Log.txt;


            return ("");
        }

        private void GetScript(string prmKey)
        {

            if (!FindScript(prmKey))
                NewScript(prmKey);

            Pool.Cleanup();

        }

        private void NewScript(string prmKey) { Corrente = new TestConsoleScript(Console); Add(Corrente); }

        private bool FindScript(string prmKey)
        {

            foreach (TestConsoleScript Script in this)

                if (xString.IsEqual(Script.key, prmKey))
                {

                    Corrente = Script;

                    return true;

                }

            return false;

        }

    }

    public class TestConsoleScript
    {

        public TestConsole Console;

        private TestBuilder Builder;

        public TestCommands Commands;

        public TestConsoleLog Log;

        public TestConsoleInput Input => Console.Input;


        public string key;

        public TestConsoleScript(TestConsole prmConsole)
        {

            Console = prmConsole;

            Commands = new TestCommands(this);

            Builder = new TestBuilder(this);

            Log = new TestConsoleLog();

        }

        public void Import(string prmArquivoINI)
        {

            Log.Setup(prmArquivoINI);

            Play(prmCode: Input.GetCode(prmArquivoINI), prmArquivoINI);

        }

        public void Play(string prmCode, string prmArquivoOUT)
        {

            Log.Start(prmArquivoOUT);

            Builder.Compile(prmCode);

            Commands.Play();

            Log.Stop();

        }

        public void Save(string prmData) => Log.SetResultado(prmData);

        public void AddLog(TestTraceMsg prmMsg) => Log.AddLog(prmTipo: prmMsg.tipo, prmTexto: prmMsg.texto);

    }

    public class TestConsoleArquivoINI
    {

        public TestConsole Console;
        private TestConsoleScript Script => Console.Script;
        public TestTrace Trace => Console.Trace;


        public Diretorio PathINI;

        private string nome;
        private string nome_extendido { get => nome + "." + extensao; }
        private string path { get => PathINI.path; }

        private string extensao = "ini";

        private FileTXT File;

        public TestConsoleArquivoINI()
        {

            PathINI = new Diretorio();

        }

        public string GetCode(string prmArquivoINI)
        {

            nome = prmArquivoINI;

            File = new FileTXT();

            if (File.Open(PathINI.path, nome_extendido))
            {

                Trace.LogFile.DataFileImport(nome_extendido);

                return File.txt();

            }

            else
                Trace.LogFile.FailDataFileOpen(path, nome_extendido);

            return ("");

        }
    }

}
