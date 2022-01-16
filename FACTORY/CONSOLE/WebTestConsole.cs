using Dooggy.Factory.Data;
using Dooggy.Lib.Files;
using Dooggy.Lib.Generic;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Dooggy.Factory.Console
{
    public class TestConsole
    {

        public TestFactory Factory;

        public TestConsoleInput Input;

        public TestConsoleOutput Output;

        public TestConsoleScripts Scripts;

        public TestDataLocal Dados => Factory.Dados;
        public TestDataPool Pool => Factory.Pool;
        public TestTrace Trace => Factory.Trace;

        public TestConsoleResult Result => Script.Result;

        public TestConsoleScript Script { get => Scripts.Corrente; }

        public bool IsDbOK => Pool.IsDbOK;

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
                Input.Start();

        }

        public void SetAncora(DateTime prmAncora) => Pool.SetAncora(prmAncora);
        public void SetDBStatus(bool prmBloqueado) => Pool.SetDBStatus(prmBloqueado);
        public void AddLog() => Scripts.AddLog();


        public void Load() => Load(prmPlay: false);
        public void Load(bool prmPlay) => Input.Load(prmPlay);

        public void Load(string prmArquivoINI) => Scripts.Load(prmArquivoINI, prmPlay: false);
        public void Import(string prmArquivoINI) => Scripts.Load(prmArquivoINI, prmPlay: true);

        public void Play(string prmCode) => Play(prmCode, prmArquivoOUT: "");
        public void Play(string prmCode, string prmArquivoOUT) => Scripts.Play(prmCode, prmArquivoOUT);

        public void Save(string prmData, eTipoFileFormat prmTipo, string prmEncoding) => Scripts.Save(prmData, prmTipo, prmEncoding);

        public bool SetScript(string prmKey) => Scripts.FindScript(prmKey);

        public bool SaveCode(string prmCode) => Scripts.SaveCode(prmCode);

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

        public void Load(string prmArquivoINI, bool prmPlay) { GetScript(prmArquivoINI); Corrente.Load(prmArquivoINI, prmPlay); }

        public void Play(string prmCode, string prmArquivoOUT) { GetScript(prmArquivoOUT); Corrente.Play(prmCode, prmArquivoOUT); }

        public void Save(string prmData, eTipoFileFormat prmTipo, string prmEncoding) => Corrente.Save(prmData, prmTipo, prmEncoding);

        public bool SaveCode(string prmCode) => Corrente.SaveINI(prmCode);

        public void AddLog()
        {
            if (TemCorrente)
                Corrente.AddLog(Trace.Msg);
        }

        public string GetLog()
        {
            if (TemCorrente)
                return Corrente.Result.log;
            
            return ("");
        }

        private void GetScript(string prmKey)
        {
            if (!FindScript(prmKey))
                NewScript(prmKey);

            Pool.Cleanup();
        }

        private void NewScript(string prmKey) { Corrente = new TestConsoleScript(Console); Add(Corrente); }

        public bool FindScript(string prmKey)
        {

            foreach (TestConsoleScript Script in this)

                if (xString.IsEqual(Script.key, prmKey))
                {

                    Corrente = Script;

                    Trace.LogConsole.SetScript(prmKey);

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

        public TestConsoleResult Result;

        private TestTrace Trace => Dados.Trace;

        private TestDataLocal Dados => Console.Dados;

        private TestConsoleInput Input => Console.Input;

        private TestConsoleOutput Output => Console.Output;

        public string key;

        public TestConsoleScript(TestConsole prmConsole)
        {

            Console = prmConsole;

            Commands = new TestCommands(this);

            Builder = new TestBuilder(this);

            Result = new TestConsoleResult();

        }
        public void Load(string prmArquivoINI, bool prmPlay)
        {

            key = prmArquivoINI;

            Result.Setup(prmArquivoINI);

            Build(prmCode: Open(prmArquivoINI), prmArquivoINI, prmPlay);

        }

        private string Open(string prmArquivoINI) => Dados.File.Open(prmArquivoINI, Input.GetPath());

        public void Play(string prmCode, string prmArquivoOUT) => Build(prmCode, prmArquivoOUT, prmPlay: true);

        public bool Save(string prmData, eTipoFileFormat prmTipo, string prmEncoding)
        {

            Result.SetData(prmData);

            return Dados.File.Save(prmNome: Output.GetScriptOrigem(), prmPath: Output.GetPathFull(prmTipo), prmConteudo: prmData, prmExtensao: Output.GetExtensao(prmTipo), prmEncoding);

        }
        public bool SaveINI(string prmCode)
        {

            Result.SetCode(prmCode);

            return Dados.File.Save(prmNome: Output.GetScriptOrigem(), prmPath: Input.GetPath(), prmConteudo: prmCode);

        }
        
        private void Build(string prmCode, string prmArquivoOUT, bool prmPlay)
        {

            Commands.Clear();
            
            Result.LogStart(prmArquivoOUT);

            if (Dados.TestConnect())
            {

                Builder.Compile(prmCode);

                if (prmPlay)
                    Commands.Play();
            }

            Result.LogStop();

        }

        public void AddLog(TestTraceMsg prmMsg) => Result.AddLog(prmTipo: prmMsg.tipo, prmTexto: prmMsg.texto);

    }
    public class TestConsoleResult
    {

        public string name_INI;

        public string name_OUT;

        public string code;

        public string data;

        private TestConsoleLog Log;

        public string log => Log.txt;

        public TestConsoleResult()
        {

            Log = new TestConsoleLog();

        }

        public void Setup(string prmArquivoINI)
        {

            name_INI = prmArquivoINI;

        }
        public void LogStart(string prmArquivoOUT)
        {

            name_OUT = prmArquivoOUT;

            Reset();

            Log.Start();

        }

        private void Reset() => Log.Clear();

        public void SetCode(string prmCode) => code = prmCode;

        public void SetData(string prmData) => data = prmData;

        public void LogStop() => Log.Stop();

        public void AddLog(string prmTipo, string prmTexto) => Log.AddNew(prmTipo, prmTexto);

    }
    public class TestConsoleLog : List<TestItemLog>
    {

        public bool ativo;

        public void Start() => ativo = true;
        public void Stop() => ativo = false;

        private xMemo memo;

        public string txt { get => GetTXT(); }

        public TestConsoleLog()
        {

            memo = new xMemo();

        }

        public void AddNew(string prmTipo, string prmTexto)
        {

            if (ativo)
            {

                Add(new TestItemLog(prmTipo, prmTexto));

                Debug.WriteLine(String.Format("line: {0,3} -txt: {1}", this.Count, prmTexto));

            }

        }

        private string GetTXT()
        {

            int a = this.Count;

            memo.Clear();

            foreach (TestItemLog item in this)
                memo.Add(item.msg);

            return (memo.txt() + Environment.NewLine);
        }

    }

    public class TestConsoleInput : TestConsoleIO
    {

        private TestConsole Console;

        private Diretorio PathINI;
        private TestTrace Trace => Console.Trace;

        public string GetPath() => (PathINI.path);
        public Arquivos GetArquivosINI() => PathINI.files.GetFiltro("*.ini");

        public TestConsoleInput(TestConsole prmConsole)
        {

            Console = prmConsole;

            PathINI = new Diretorio();

    }

        public void SetPath(string prmPath)
        {

            PathINI.SetPath(prmPath);

            Trace.LogPath.SetPath(prmContexto: " OrigemMassaTestes", prmPath);

        }

        public void Start()
        {

            foreach (Arquivo file in GetArquivosINI())
                Console.Import(prmArquivoINI: file.nome_curto);

        }

        public void Load(bool prmPlay)
        {

            foreach (Arquivo file in GetArquivosINI())
                Console.Load(prmArquivoINI: file.nome_curto);

        }

    }
    public class TestConsoleOutput : TestConsoleIO
    {

        private TestConsole Console;

        private Path PathOUT;

        private TestConsoleOutput Output => Console.Output;

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

            string nome = Console.Result.name_OUT;

            if (xString.IsStringOK(nome))
                return (nome);

            return (Console.Result.name_INI);

        }

        public string GetPath() => (PathOUT.path);
        public string GetPath(string prmSubPath) => (PathOUT.GetPath(prmSubPath));

        public string GetPathFull(eTipoFileFormat prmTipo) => (GetPath(prmSubPath: Output.GetExtensao(prmTipo)));

    }
    public class TestConsoleIO
    {

        public string GetExtensao(eTipoFileFormat prmTipo) => GetObtemExtensao(prmTipo);

        private string GetObtemExtensao(eTipoFileFormat prmTipo)
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


    }



}
