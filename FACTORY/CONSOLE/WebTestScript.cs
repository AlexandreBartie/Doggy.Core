using Dooggy.Factory.Data;
using Dooggy;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using Dooggy.Lib.Vars;
using Dooggy.Lib.Generic;

namespace Dooggy.Factory.Console
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
    public class TestScript : TestScriptBehaviour
    {

        public TestTags Tags;

        private TestCode Code;

        public TestTrace Trace => Dados.Trace;

        private TestDataLocal Dados => Console.Dados;
        private TestConsoleInput Input => Console.Input;

        public string key;

        public string tags => Tags.txt;

        public TestScript(TestConsole prmConsole)
        {

            Console = prmConsole;

            Code = new TestCode(this);

            Result = new TestResult(this);

            Tags = new TestTags(this);

        }

        public void Setup() => SetBreakOFF();

        public void Load(string prmArquivoINI, bool prmPlay)
        {

            key = prmArquivoINI;

            Code.Load(prmCode: Input.GetCode(prmArquivoINI), prmArquivoINI, prmPlay);

        }

        public void SetCode(string prmCode) => Result.SetCode(prmCode);
        public void Play(string prmCode, string prmArquivoOUT) => Code.Play(prmCode, prmArquivoOUT);

        public void AddLogItem(TestTraceMsg prmMsg) => Result.AddLogItem(prmTipo: prmMsg.tipo, prmTexto: prmMsg.texto);
        public void AddLogSQL(TestTraceMsg prmMsg) => Result.AddLogSQL(prmSQL: prmMsg.texto, prmTimeElapsed: prmMsg.time_elapsed);
    }

    public class TestScriptBehaviour : TestScritpBreak
    {

        public bool IsDataIndex;

        public void SetArgumento(string prmArg, string prmInstrucao)
        {
            switch (prmArg)
            {
                case "view":
                    SetDataIndex(prmInstrucao);
                    break;
            }
        }

        public void SetDataIndex(string prmInstrucao) => SetDataIndex(prmActive: myString.IsEqual(prmInstrucao,"index"));
        public void SetDataIndex(bool prmActive) => IsDataIndex = prmActive;

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

        private TestDataLocal Dados => Console.Dados;

        private TestConfigPath Path => Console.Config.Path;
        private TestConsoleOutput Output => Console.Output;

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
    public class TestTag
    {

        public string _key;
        public string _valor;

        public string key => _key.ToLower();
        public string valor => _valor.ToUpper();

        public string txt => String.Format("[{0,12} {1}], ", key, valor);

        public TestTag(string prmKey, string prmValor)
        {
            _key = prmKey; _valor = prmValor;
        }

        public void SetValor(string prmValor) => _valor = prmValor;

    }
    public class TestTags : List<TestTag>
    {

        private TestScript Script;

        //private TestTag Corrente;

        public string txt => GetTXT();

        public TestTags(TestScript prmScript)
        {
            Script = prmScript;
        }

        public void AddTag(string prmKey, string prmValor)
        {

            foreach (TestTag Tag in this)

                if (myString.IsEqual(Tag.key, prmKey))
                {

                    Tag.SetValor(prmValor);

                    return;

                }

            Add(new TestTag(prmKey, prmValor));

        }
        private string GetTXT()
        {

            xMemo memo = new xMemo();

            foreach (TestTag Tag in this)
                memo.Add(Tag.txt);

            return memo.memo;

        }

    }
}
