using BlueRocket.LIBRARY;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace BlueRocket.CORE
{
    public class TestScripts : List<TestScript>
    {

        private TestConsole Console;

        public TestScript Corrente;

        public TestTrace Trace => Console.Trace;

        public TestDataPool Pool => Console.Pool;

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
        public void AddLogSQL()
        {
            if (TemCorrente)
                Corrente.AddLogSQL(Trace.Msg);
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

                if (myString.IsEqual(Script.key, prmKey))
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

            return (memo.memo_ext);

        }

    }
    public class TestScript : TestScritpBreak
    {

        private TestCode Code;

        public TestScriptTags Tags;

        public TestTrace Trace => Dados.Trace;

        public TestDataGlobal Global => Console.Pool.Global;

        private TestDataSource Dados => Console.Dados;
        private TestConsoleInput Input => Console.Input;

        public string key;

        public TestScript(TestConsole prmConsole)
        {

            Console = prmConsole;

            Code = new TestCode(this);

            Result = new TestResult(this);

            Tags = new TestScriptTags(this);

        }

        public void Setup() => SetBreakOFF();

        public void Load(string prmArquivoINI, bool prmPlay)
        {

            key = prmArquivoINI;

            Code.Load(prmCode: Input.GetCode(prmArquivoINI), prmArquivoINI, prmPlay);

        }

        public void SetTag(string prmLinha) => Tags.SetTag(prmLinha);
        public void SetCode(string prmCode) => Result.SetCode(prmCode);
        public void Play(string prmCode, string prmArquivoOUT) => Code.Play(prmCode, prmArquivoOUT);

        public void AddLogItem(TraceMSG prmMsg) => Result.AddLogItem(prmTipo: prmMsg.tipo, prmTrace: prmMsg.texto);
        public void AddLogSQL(TraceMSG prmMsg) => Result.AddLogSQL(prmTrace: prmMsg.texto, prmSQL: prmMsg.sql, prmTimeElapsed: prmMsg.time_elapsed);
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

        private TestDataSource Dados => Console.Dados;

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
    public class TestScriptTag
    {

        public string _key;
        public string _valor;

        public string key => myString.GetLower(_key);
        public string valor => myString.GetUpper(_valor);

        public bool IsEqual(string prmKey) => myString.IsEqual(key, prmKey);

        public string log => String.Format("[{0,10}] '{1}'", key, valor);

        public TestScriptTag(string prmKey)
        {
            _key = prmKey;
        }
        
        public TestScriptTag(string prmKey, string prmValor)
        {
            _key = prmKey; _valor = prmValor;
        }

        public bool SetValor(string prmValor) { _valor = prmValor; return true; }
        public bool SetValor(string prmValor, string prmKey)
        {
            if (IsEqual(prmKey))
                return SetValor(prmValor);
            return false;
        }

    }
    public class TestScriptTags : List<TestScriptTag>
    {

        private TestScript Script;

        private TestTrace Trace => Script.Trace;

        private TestDataTags MainTags => Script.Global.Tags;

        private string delimitador = "=";

        public string log => GetLOG();

        public TestScriptTags(TestScript prmScript)
        {
            Script = prmScript; Setup();
        }

        private void Setup()
        {
            foreach (var item in MainTags)
                Add(new TestScriptTag(item.key));
        }

        public void SetTag(string prmLinha)
        {

            string tag = myString.GetFirst(prmLinha, prmDelimitador: delimitador).Trim();
            string valor = myString.GetLast(prmLinha, prmDelimitador: delimitador).Trim();

            SetTag(prmTag: tag, prmValue: valor, prmCommand: prmLinha);

        }
        private void SetTag(string prmTag, string prmValue, string prmCommand)
        {
            if (MainTags.FindIt(prmTag))
                if (MainTags.Find(prmTag).IsContem(prmValue))
                    SetValue(prmTag, prmValue);
                else
                    Trace.LogConsole.FailFindTagElement(prmTag, prmValue);
            else
                Trace.LogConsole.FailFindTag(prmTag, prmCommand);
        }
        
        private void SetValue(string prmTag, string prmValue)
        {
            foreach (TestScriptTag Tag in this)
                if (Tag.SetValor(prmValue, prmTag))
                    return;

            Trace.LogConsole.FailScriptTag(prmTag, prmValue);
        }
        private string GetLOG()
        {

            xMemo log = new xMemo();

            foreach (TestScriptTag Tag in this)
                log.Add(Tag.log);

            return log.memo;

        }

    }
}
