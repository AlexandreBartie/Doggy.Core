using Dooggy.Lib.Generic;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dooggy.Factory.Console
{
    public class TestResult
    {

        private TestScript Script;

        private TestConsoleInput Input => Script.Console.Input;

        public string name_INI;

        public string name_OUT;

        private string _codeZero;

        private string _code;
        private string _data;

        public string code { get => _code; }
        public string data { get => _data; }

        public string log => Log.txt;

        public bool IsChanged => (xString.IsNoEqual(_code, _codeZero));
        public bool IsData => (xString.IsFull(data));
        public bool IsError => Log.erro;

        public bool IsDataSaved;

        private TestResultLog Log;


        public TestResult(TestScript prmScript)
        {

            Script = prmScript;

            Log = new TestResultLog();

        }

        public void Setup(string prmArquivoINI)
        {

            name_INI = prmArquivoINI;

        }

        public void LogStart(string prmArquivoOUT) { Log.Start(); name_OUT = prmArquivoOUT; IsDataSaved = false; }
        public void LogStop() => Log.Stop();

        public void AddLog(string prmTipo, string prmTexto) => Log.AddNew(prmTipo, prmTexto);

        public void SetCodeZero() { _codeZero = ""; SetCodeZero(prmCode: ""); }
        public void SetCodeZero(string prmCode) { _codeZero += prmCode; SetCode(_codeZero); _data = ""; }

        public void SetCode(string prmCode) => _code = prmCode;
        public void SetData(string prmData) { _data = prmData; IsDataSaved = true; }
        public void UndoCode() => SetCode(prmCode: Script.Console.Input.GetCode(name_INI));

    }
    public class TestResultLog : List<TestItemLog>
    {

        public bool ativo;

        public bool erro = false;

        public void Start() { Clear(); ativo = true; erro = false; }
        public void Stop() => ativo = false;

        private xMemo memo;

        public string txt { get => GetTXT(); }

        public TestResultLog()
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

            return (memo.memo_ext);
        }

    }
}
