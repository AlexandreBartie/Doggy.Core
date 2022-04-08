using Dooggy.LIBRARY;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dooggy.CORE
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

        public bool IsError => Log.IsError;

        public bool IsSlow => SQL.IsSlow;

        public bool IsData => (myString.IsFull(data));

        public bool IsDataSaved;
        public bool IsChanged => (myString.IsNoEqual(_code, _codeZero));

        public TestResultLog Log;

        public TestResultBase Err => Log.Err;
        public TestResultSQL SQL => Log.SQL;

        public TestResult(TestScript prmScript)
        {

            Script = prmScript;

            Log = new TestResultLog();

        }

        public void Setup(string prmCode, string prmArquivoINI)
        {

            SetSave(prmCode);

            name_INI = prmArquivoINI;

        }

        public void LogStart(string prmArquivoOUT) { Log.Start(); name_OUT = prmArquivoOUT; IsDataSaved = false; }
        public void LogStop() => Log.Stop();

        public void AddLogItem(string prmTipo, string prmTrace) => Log.AddItem(prmTipo, prmTrace);
        public void AddLogSQL(string prmTrace, string prmSQL, long prmTimeElapsed, string prmError) => Log.AddSQL(prmTrace, prmSQL, prmTimeElapsed, prmError);
        public void SetSave(string prmCode) { _codeZero = prmCode; SetCode(_codeZero); }
        public void SetCode(string prmCode) => _code = prmCode;
        public void SetData(string prmData) { _data = prmData; IsDataSaved = true; }
        public void UndoCode() => SetSave(prmCode: Script.Console.Input.GetCode(name_INI));

    }
    public class TestResultLog
    {

        public bool ativo;

        public TestResultMain Main;

        public TestResultSQL SQL;

        public TestResultBase Err => Main.Err;

        public string txt => Main.txt;

        public bool IsError => Err.IsFull;

        public TestResultLog()
        {
            Clear();
        }

        private void Clear()
        {
            Main = new TestResultMain(); SQL = new TestResultSQL();
        }

        public void Start() { Clear(); ativo = true; }
        public void AddItem(string prmTipo, string prmTrace)
        {
            if (ativo)
            {

                TraceMSG item = new TraceMSG(prmTipo, prmTrace);

                Main.Add(item);

            }

        }
        public void AddSQL(string prmTrace, string prmSQL, long prmTimeElapsed, string prmError)
        {
            if (ativo)
            {

                TraceMSG item = new TraceMSG(prmTrace, prmSQL, prmTimeElapsed, prmError);

                SQL.Add(item);

            }

        }
        public void Stop() => ativo = false;

    }

    public class TestResultMain : TestResultBase
    {

        public TestResultBase Err => GetFiltro(prmTipo: "ERRO");

        public TestResultBase Filtro(string prmTipo) => GetFiltro(prmTipo);

        private TestResultBase GetFiltro(string prmTipo)
        {

            TestResultBase result = new TestResultBase();

            foreach (TraceMSG item in this)
                if (item.IsMatch(prmTipo))
                    result.Add(item);

            return (result);
        }

    }

    public class TestResultSQL : TestResultBase
    {
        public string log => GetLOG();

        public double timeSeconds => GetTime();
        public double timeBigger => GetTime(prmBigger: true);
        public double timeAverage => myDouble.GetAverage(timeSeconds, qtde);

        public bool IsRunned => (timeSeconds != 0 && IsFull);

        public bool IsSlow => (timeBigger > 3);

        public string qtdTestsTXT => myString.Texting(qtde.ToString(), IsRunned);
        public string timeSecondsTXT => myString.Texting(myFormat.MilisecondsToString(timeSeconds), IsRunned);
        public string timeAverageTXT => myString.Texting(myFormat.MilisecondsToString(timeAverage), IsRunned);
        public string timeBiggerTXT => myString.Texting(myFormat.MilisecondsToString(timeBigger), IsRunned);

        private string GetLOG()
        {

            xMemo log = new xMemo();

            foreach (TraceMSG item in this)
                log.Add(String.Format("[{0,4}] {1}", item.title, item.sql));

            return (log.memo);
        }

        private double GetTime() => GetTime(prmBigger: false);
        private double GetTime(bool prmBigger)
        {
            double time = 0;

            foreach (TraceMSG item in this)
                if (prmBigger)
                    time = myDouble.GetBigger(time, item.time_seconds);
                else
                    time += item.time_seconds;

            return time;
        }

    }

    public class TestResultBase : List<TraceMSG>
    {
        public string txt => GetTXT();

        public int qtde => this.Count;
        public bool IsFull => (qtde > 0);

        private string GetTXT()
        {

            xMemo log = new xMemo();

            foreach (TraceMSG item in this)
                log.Add(item.txt);

            return (log.memo);
        }



    }
}
