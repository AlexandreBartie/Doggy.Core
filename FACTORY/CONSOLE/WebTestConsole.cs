using Dooggy.Factory.Data;
using Dooggy.Lib.Data;
using Dooggy.Lib.Files;
using Dooggy.Lib.Generic;
using Dooggy.Lib.Parse;
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

        public TestConsoleConfig Config;

        public TestConsoleScripts Scripts;

        public TestDataLocal Dados => Factory.Dados;
        public TestDataPool Pool => Factory.Pool;
        public TestTrace Trace => Factory.Trace;

        public TestConsoleResult Result => Script.Result;

        public TestConsoleScript Script { get => Scripts.Corrente; }

        public bool IsOK => Config.IsOK;
        public bool IsDbOK => Pool.IsDbOK;
        public TestConsole(TestFactory prmFactory)
        {

            Factory = prmFactory;

            Input = new TestConsoleInput(this);

            Output = new TestConsoleOutput(this);

            Config = new TestConsoleConfig(this);

            Scripts = new TestConsoleScripts(this);

        }

        public bool Setup(string prmArquivoCFG) => Config.Setup(prmArquivoCFG);

        public void Setup(string prmPathINI, string prmPathOUT) => Setup(prmPathINI, prmPathOUT, prmStart: false);
        public void Setup(string prmPathINI, string prmPathOUT, bool prmStart)
        {

            Input.SetPath(prmPathINI);

            Output.SetPath(prmPathOUT);

            if (prmStart)
                Input.Start();

        }

        public void SetAnchor(DateTime prmAncora) => Config.Anchor.SetDate(prmAncora);
        public void SetAnchor(string prmAncora) => Config.Anchor.SetDate(prmAncora);


        public void SetDBStatus(bool prmBloqueado) => Pool.SetDBStatus(prmBloqueado);
        public void AddLog() => Scripts.AddLog();


        public void Load() => Load(prmPlay: false);
        public void Load(bool prmPlay) => Input.Load(prmPlay);

        public void Import(string prmArquivoINI) => Load(prmArquivoINI, prmPlay: true);
        public void Load(string prmArquivoINI) => Load(prmArquivoINI, prmPlay: false);
        public void Load(string prmArquivoINI, bool prmPlay) => Scripts.Load(prmArquivoINI, prmPlay);

        public void Play(string prmCode) => Play(prmCode, prmArquivoOUT: "");
        public void Play(string prmCode, string prmArquivoOUT) => Scripts.Play(prmCode, prmArquivoOUT);

        public void Save(string prmData, eTipoFileFormat prmTipo, string prmEncoding) => Output.SaveOUT(prmData, prmTipo, prmEncoding);
        public bool SaveCode(string prmCode) => Input.SaveINI(prmCode);
        public void SetCode(string prmCode) => Script.SetCode(prmCode);
        public void UndoCode() => Script.Result.UndoCode();

        public bool SetScript(string prmKey) => Scripts.FindScript(prmKey);

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

            Result = new TestConsoleResult(this);

        }
        public void Load(string prmArquivoINI, bool prmPlay)
        {

            key = prmArquivoINI;

            Result.Setup(prmArquivoINI);

            Build(prmCode: Input.GetCode(prmArquivoINI), prmArquivoINI, prmPlay);

        }

        public void SetCode(string prmCode) => Result.SetCode(prmCode);

        public void Play(string prmCode, string prmArquivoOUT) => Build(prmCode, prmArquivoOUT, prmPlay: true);

        private void Build(string prmCode, string prmArquivoOUT, bool prmPlay)
        {

            Commands.Clear();
            
            Result.LogStart(prmArquivoOUT);

            Builder.Compile(prmCode);

            if (Dados.DoConnect() && prmPlay)
                Commands.Play();

            Result.LogStop();

        }

        public void AddLog(TestTraceMsg prmMsg) => Result.AddLog(prmTipo: prmMsg.tipo, prmTexto: prmMsg.texto);

    }
    public class TestConsoleResult
    {

        private TestConsoleScript Script;
        private TestConsoleInput Input => Script.Console.Input;


        public string name_INI;

        public string name_OUT;

        private string _codeZero;

        private string _code;
        private string _data;

        public string code { get => _code; }
        public string data { get => _data; }

        public string log => Log.txt;

        public bool IsChanged => (xString.IsNoEqual(_code,_codeZero));
        public bool IsData => (xString.IsFull(data));
        public bool IsError => Log.erro;

        private TestConsoleLog Log;


        public TestConsoleResult(TestConsoleScript prmScript)
        {

            Script = prmScript;

            Log = new TestConsoleLog();

        }

        public void Setup(string prmArquivoINI)
        {

            name_INI = prmArquivoINI;

        }

        public void LogStart(string prmArquivoOUT) { Log.Start(); name_OUT = prmArquivoOUT;  }
        public void LogStop() => Log.Stop();

        public void AddLog(string prmTipo, string prmTexto) => Log.AddNew(prmTipo, prmTexto);

        public void SetCodeZero(string prmCode) { _codeZero = prmCode; SetCode(prmCode); _data = ""; }
        public void SetCode(string prmCode) => _code = prmCode;
        public void SetData(string prmData) => _data = prmData;
        public void UndoCode() => SetCode(prmCode: Script.Console.Input.GetCode(name_INI));

    }
    public class TestConsoleLog : List<TestItemLog>
    {
       
        public bool ativo;

        public bool erro = false;

        public void Start() { Clear(); ativo = true; erro = false;  }
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

                erro = erro || xString.IsEqual(prmTipo, "erro");
            }

        }

        private string GetTXT()
        {

            memo.Clear();

            foreach (TestItemLog item in this)
                memo.Add(item.msg);

            return (memo.txt() + Environment.NewLine);
        }

    }
    public class TestConsoleInput : TestConsoleIO
    {

        private TestConsoleOutput Output => Console.Output;

        private Diretorio PathINI;

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
        public bool SaveINI(string prmCode)
        {

            Result.SetCode(prmCode);

            return Dados.File.Save(prmNome: Output.GetScriptOrigem(), prmPath: GetPath(), prmConteudo: prmCode);

        }

        public string GetCode(string prmArquivoINI) => Dados.File.Open(prmArquivoINI, GetPath());

    }
    public class TestConsoleOutput : TestConsoleIO
    {

        private Path PathOUT;

        public TestConsoleOutput(TestConsole prmConsole)
        {

            Console = prmConsole;

            PathOUT = new Path();

        }

        public bool SaveOUT(string prmData, eTipoFileFormat prmTipo, string prmEncoding)
        {

            Result.SetData(prmData);

            return Dados.File.Save(prmNome: GetScriptOrigem(), prmPath: GetPathFull(prmTipo), prmConteudo: prmData, prmExtensao: GetExtensao(prmTipo), prmEncoding);

        }

        public void SetPath(string prmPath)
        {

            PathOUT.SetPath(prmPath);

            Trace.LogPath.SetPath(prmContexto: "DestinoMassaTestes", prmPath);

        }
        public string GetScriptOrigem()
        {

            string nome = Console.Result.name_OUT;

            if (xString.IsFull(nome))
                return (nome);

            return (Console.Result.name_INI);

        }

        public string GetPath() => (PathOUT.path);
        public string GetPath(string prmSubPath) => (PathOUT.GetPath(prmSubPath));
        public string GetPathFull(eTipoFileFormat prmTipo) => (GetPath(prmSubPath: GetExtensao(prmTipo)));

    }
    public class TestConsoleIO
    {

        internal TestConsole Console;
        internal TestConsoleScript Script => Console.Script;
        internal TestConsoleResult Result => Script.Result;
        internal TestDataLocal Dados => Console.Dados;
        internal TestTrace Trace => Console.Trace;

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

