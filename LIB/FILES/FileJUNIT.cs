using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using BlueRocket.CORE.Lib.Parse;
using BlueRocket.CORE;
using BlueRocket.CORE.Lib.Generic;

namespace BlueRocket.CORE.Lib.Files
{
    public class FileJUNIT : FileTXT
    {

        private TestCasesJUnit Cases;

        private TestJSONJUnit _JSON;

        public List<TestCaseJUnit> TestCases { get => Cases.TestCases; }

        public string separador { get => ","; }

        public TestJSONJUnit JSON
        {
            get
            {
                if (_JSON == null)
                    _JSON = new TestJSONJUnit(this);
                return (_JSON);
            }

            set { _JSON = value; }

        }

        public override bool Open(string prmPath, string prmName)
        {

            Cases = null;

            if (base.Open(prmPath, prmName))
            {

                Cases = new TestCasesJUnit(this);

                foreach (string line in base.lines)
                {

                    Cases.AddLine(line);

                }

            }

            return (base.IsOK);

        }

    }
    public class TestCasesJUnit
    {

        private FileJUNIT File;

        internal List<TestCaseJUnit> TestCases;

        private TestCaseJUnit TestCaseCurrent ;

        public TestCasesJUnit(FileJUNIT prmFile)
        {

            File = prmFile;

            TestCases = new List<TestCaseJUnit>();

        }

        public void AddLine(string prmLine)
        {
            if (IsTestCase(prmLine))

                AddCase(prmLine);

            else

                AddFlow(prmLine);

        }


        private void AddCase(string prmLine)
        {

            TestCaseCurrent = new TestCaseJUnit(prmLine, File);

            TestCases.Add(TestCaseCurrent);

        }

        private void AddFlow(string prmLine) 
        {

            TestCaseCurrent.Flows.Add(prmLine);

        }

        public string memo()
        {

            string lista = "";
            string aux = "";

            foreach (TestCaseJUnit test_case in TestCases)
            {

                lista += aux + (test_case.nome);

                aux = Environment.NewLine;

            }

            return lista;
        }

        private bool IsTestCase(string prmLine) => (!prmLine.StartsWith(","));

    }
    public class TestCaseJUnit
    {

        public FileJUNIT File;

        public TestDataJUnit Flows;

        public TestParametersJUnit Parametros;

        public string nome { get => Parametros.nome; }

        public string separador { get => File.separador; }

        public TestCaseJUnit(string prmLine, FileJUNIT prmFile)
        {

            File = prmFile;

            Flows = new TestDataJUnit(this);

            Parametros = new TestParametersJUnit(this, prmLine);

        }

    }
    public class TestParametersJUnit
    {

        private TestCaseJUnit TestCase;

        public string nome;

        private xMemo Lista;

        public TestParametersJUnit(TestCaseJUnit prmTestCase, string prmLine)
        {

            TestCase = prmTestCase;

            Lista = new xMemo(prmLine, TestCase.separador);

            nome = Lista.GetRemove();

        }
        public string GetJSON(xMemo prmFlow)
        {

            string lista = "";

            string aux = "";

            int cont = 0;

            foreach (string atributo in Lista)
            {

                cont++;

                lista += aux + string.Format("'{0}': '{1}'", atributo, prmFlow.Item(cont));

                aux = ",";

            }

            return ("{ "+ lista + " }");

        }
        public string txt => Lista.Export(TestCase.separador);
 
    }
    public class TestDataJUnit
    {

        private TestCaseJUnit TestCase;

        public List<xMemo> Dados;

        public TestDataJUnit(TestCaseJUnit prmTestCase)
        {

            TestCase = prmTestCase;

            Dados = new List<xMemo>();

        }

        public void Add(string prmLine)
        {

            xParseCSV item = new xParseCSV(prmLine);

            item.GetRemove();

            Dados.Add(item);

        }

    }
    public class TestJSONJUnit
    {

        private FileJUNIT File;

        public List<xMemo> Dados;

        public TestJSONJUnit(FileJUNIT prmFile)
        {

            File = prmFile;

            Dados =  new List<xMemo>();

            GetJSON();

        }

        private void GetJSON()
        {

            foreach (TestCaseJUnit teste in File.TestCases)
            {

                foreach (xMemo Flow in teste.Flows.Dados)
                {

                    Debug.WriteLine(  teste.Parametros.GetJSON(Flow));

                }

            }

        }
        public string memo()
        {

            string lista = "";
            string aux = "";

            foreach (xMemo Flow in Dados)
            {

                lista += aux + (Flow.Export(File.separador));

                aux = Environment.NewLine;

            }

            return lista;
        }


    }
}
