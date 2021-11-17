using System;
using System.Collections.Generic;

namespace Miranda
{
    public class TestCase
    {
        private String selector;

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
    
/*    public class TestSuite
    {
        public string name;

        private List<TestCase> testCases = new List<TestCase>();

    public TestSuite(String name)
        {
            this.name = name;
        }

        public TestSuite addTestCase(TestCase testCase)
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
