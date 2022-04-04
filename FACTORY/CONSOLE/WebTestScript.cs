using Dooggy.LIBRARY;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace Dooggy.CORE
{
    public class TestScripts : List<TestScript>
    {

        private TestConsole Console;

        public TestScript Corrente;

        public TestTrace Trace => Console.Trace;

        public DataPool Pool => Console.Pool;

        private bool TemCorrente => (Corrente != null);

        public TestScripts(TestConsole prmConsole)
        {

            Console = prmConsole;

        }

        public void Load(string prmArquivoINI, bool prmPlay) { GetScript(prmArquivoINI); Corrente.Load(prmArquivoINI, prmPlay); }
        public void Play(string prmCode, string prmArquivoOUT) { GetScript(prmArquivoOUT); Corrente.Play(prmCode, prmArquivoOUT); }

        public void AddLogItem()
        {
            if (TemCorrente)
                Corrente.AddLogItem(Trace.Msg);
        }
        public void AddLogSQL(string prmError)
        {
            if (TemCorrente)
                Corrente.AddLogSQL(Trace.Msg, prmError);
        }

        public string GetLog()
        {
            if (TemCorrente)
                return Corrente.Result.Log.txt;

            return ("");
        }

        private void GetScript(string prmKey)
        {
            if (!FindScript(prmKey))
                NewScript(prmKey);

            Corrente.Setup();

            Pool.Cleanup();
        }

        private void NewScript(string prmKey) { Corrente = new TestScript(Console); Add(Corrente); }

        public bool FindScript(string prmKey)
        {

            foreach (TestScript Script in this)

                if (myString.IsMatch(Script.key, prmKey))
                {

                    Corrente = Script;

                    Trace.LogConsole.SetScript(prmKey);

                    return true;

                }

            Corrente = null;

            return false;

        }

        public string GetLista()
        {

            xMemo memo = new xMemo(prmSeparador: Environment.NewLine);

            foreach (TestScript Script in this)
                memo.Add(Script.key);

            return (memo.memo);

        }

    }
    public class TestScript : TestScriptTags
    {

        private TestCode Code;

        public TestTrace Trace => Pool.Trace;
        private TestConsoleInput Input => Console.Input;

        public string key;

        public TestScript(TestConsole prmConsole)
        {

            Console = prmConsole;

            Code = new TestCode(this);

            Result = new TestResult(this);

            SetupTags();

        }

        public void Setup() => SetBreakOFF();

        public void Load(string prmArquivoINI, bool prmPlay)
        {

            key = prmArquivoINI;

            Code.Load(prmCode: Input.GetCode(prmArquivoINI), prmArquivoINI, prmPlay);

        }

        public void SetCode(string prmCode) => Result.SetCode(prmCode);
        public void Play(string prmCode, string prmArquivoOUT) => Code.Play(prmCode, prmArquivoOUT);

        public void AddLogItem(TraceMSG prmMsg) => Result.AddLogItem(prmTipo: prmMsg.tipo, prmTrace: prmMsg.msg);
        public void AddLogSQL(TraceMSG prmMsg, string prmError) => Result.AddLogSQL(prmTrace: prmMsg.msg, prmSQL: prmMsg.sql, prmTimeElapsed: prmMsg.time_elapsed, prmError);
    }

    public class TestScriptTags : TestScritpBreak
    {

        public DataTags Tags;

        public DataPool Pool => Console.Pool;

        public void SetupTags()
        {
            Tags = new DataTags(Pool); Tags.SetGlobal();
        }

        public void SetTag(string prmSintaxe) => Tags.SetTag(prmSintaxe);

    }
    public class TestScritpBreak : TestScriptSave
    {

        public bool IsBreak;

        public void Break(string prmOptions)
        {
            switch (prmOptions)
            {
                case "":
                    SetBreak();
                    break;
                
                case "on":
                    SetBreakON();
                    break;

                case "off":
                    SetBreakOFF();
                    break;
            }

        }

        private void SetBreak() => IsBreak = !IsBreak;
        private void SetBreakON() => IsBreak = true; 
        public void SetBreakOFF() => IsBreak = false;

    }

    public class TestScriptSave
    {

        public TestConsole Console;

        public TestResult Result;

        public eTipoFileFormat tipo;

        public string encoding;
        public string extensao;

        private DataSource Dados => Console.Dados;

        private TestConfigPath Path => Console.Config.Path;
        private TestConsoleOutput Output => Console.Output;

        public bool CommandDB(string prmCommand) => Console.Bases.ExecuteNoSQL(prmCommand, prmTimeOut: Console.Config.DB.timeoutSQL);

        public void Save(string prmOptions)
        { 
            
            SetOptions(prmOptions);

            Result.SetData(prmData: Dados.output(tipo));

            Output.SaveOUT(Result.data, tipo, encoding, extensao, Result.Log.txt);
        }
        private void SetOptions(string prmOptions)
        {

            xLista options = new xLista(prmOptions, prmSeparador: ".");

            tipo = Path.GetTipoFormato(options.Get(prmIndice: 1, prmPadrao: "csv"));

            encoding = myString.GetLower(options.Get(prmIndice: 2, prmPadrao: "UTF8"));
            extensao = myString.GetLower(options.Get(prmIndice: 3, prmPadrao: Path.GetExtensao(tipo)));

        }

    }


    //public class TestScriptTag
    //{
    //    public string _name;
    //    public string _valor;

    //    public string name => myString.GetLower(_name);
    //    public string valor => myString.GetUpper(_valor);

    //    public bool IsMatch(string prmName) => myString.IsMatch(name, prmName);

    //    public string log => String.Format("[{0,10}] '{1}'", name, valor);

    //    public TestScriptTag(string prmKey)
    //    {
    //        _name = prmKey;
    //    }
        
    //    public TestScriptTag(string prmKey, string prmValor)
    //    {
    //        _name = prmKey; _valor = prmValor;
    //    }

    //    public void SetValor(string prmValor) => _valor = prmValor;

    //}
    //public class TestScriptTags : List<TestScriptTag>
    //{

    //    private TestScript Script;

    //    private TestTrace Trace => Script.Trace;

    //    private DataTags MainTags => Script.Global.Tags;

    //    public string log => GetLOG();

    //    public TestScriptTags(TestScript prmScript)
    //    {
    //        Script = prmScript; Setup();
    //    }

    //    private void Setup()
    //    {
    //        foreach (var item in MainTags)
    //            Add(new TestScriptTag(item.name, item.padrao));
    //    }

    //    private string GetLOG()
    //    {

    //        xMemo log = new xMemo();

    //        foreach (TestScriptTag Tag in this)
    //            log.Add(Tag.log);

    //        return log.memo;

    //    }

    //}
}
