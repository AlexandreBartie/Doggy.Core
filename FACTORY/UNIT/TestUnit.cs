using Dooggy.FACTORY.UNIT;
using Dooggy.Lib.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace Dooggy.FACTORY.UNIT
{

    public class UTC
    {
        
        public LinesUTC inputList;
        public LinesUTC outputList;
        private LinesUTC resultList;

        private string _log;

        public void input() => input(prmText: "");
        public void input(string prmText) => inputList.Add(prmText);

        public void output() => output(prmText: "");
        public void output(string prmText) => outputList.Add(prmText);

        public string inputTXT => inputList.txt;
        public string outputTXT => outputList.txt;

        public string resultTXT => resultList.txt;
        public string log => _log;

        public UTC()
        {
            Setup();
        }

        private void Setup()
        {
            inputList = new LinesUTC();
            outputList = new LinesUTC();
            resultList = new LinesUTC();
        }

        public void AssertTest(string prmResult)
        {

            resultList.Add(prmResult);

            _log = TestUnityLog.GetAnalise(prmGerado: prmResult, prmEsperado: outputTXT);

            // assert
            if (!outputList.IsEqual(resultList.txt))
                Assert.Fail(log);
        }

    }

    public class LinesUTC : List<string>
    {

        public bool IsFull => (this.Count > 0);
        public bool IsEqual(string prmText) => (txt == prmText);

        public string txt => GetTXT();

        public void Add() => Add(prmText: "");
        public new void Add(string prmText)
        {
            if (prmText != null)
                base.Add(prmText);
        }
        private string GetTXT()
        {
            string txt = "";

            foreach (string texto in this)
                txt += texto + Environment.NewLine;

            return txt;
        }


    }

    public class xTestCase
    {
        //private String selector;

        //public TestCase(String selector)
        //{
        //    this.selector = selector;
        //}

        /// Run whatever code you need to get ready for the test to run.
        protected void setUp() { }

        /// Release whatever resources you used for the test.
        protected void tearDown() { }
        /// Run the selected method and register the result.
/*        public void Run(TestResult result)
        {
            //try
            //{
               // Run();
            }
            //catch (Throwable e)
            //{
            //    result.error(this, e);
            //    return;
            //}
            //result.pass(this);
        }*/
    }
    
/*    public class TestRobotSuite
    {
        public string name;

        private List<TestCase> testCases = new List<TestCase>();

    public TestRobotSuite(String name)
        {
            this.name = name;
        }

        public TestRobotSuite addTestCase(TestCase testCase)
        {
            testCases.Add(testCase);
            return this;
        }

        public TestResult run()
        {
            TestResult result = new TestResult(name);
            foreach (TestCase Teste in  testCases)
            {
                Teste.Run(result);
            }
            return result;
        }
    }

    public class TestResult
    {
        public  String name;

        public List<String> errors = new List<String>();
        public List<String> passed = new List<String>();

        public TestResult(String name)
        {
            this.name = name;
        }

        public void error(TestCase testCase, Exception error)
        {
            errors.Add(String.Format("%s: %s", testCase, error));
        }

        public void pass(TestCase testCase)
        {
            passed.Add(testCase.ToString());
        }

        public int count()
        {
            return passed.Count + errors.Count;
        }
    }*/
}
