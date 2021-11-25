using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using MyDooggy.LIB.PARSE;

namespace Dooggy.LIB.FILES
{
    public class xFileJUnit : xFileTXT
    {

        private xTestCasesJUnit Cases;

        private xTestJSONJUnit _JSON;

        public List<xTestCaseJUnit> TestCases { get => Cases.TestCases; }

        public string separador { get => ","; }

        public xTestJSONJUnit JSON
        {
            get
            {
                if (_JSON == null)
                    _JSON = new xTestJSONJUnit(this);
                return (_JSON);
            }

            set { _JSON = value; }

        }

        public override bool Open(string prmPath, string prmName)
        {

            Cases = null;

            if (base.Open(prmPath, prmName))
            {

                Cases = new xTestCasesJUnit(this);

                foreach (string line in base.lines)
                {

                    Cases.AddLine(line);

                }

            }

            return (base.IsOK);

        }

    }
    public class xTestCasesJUnit
    {

        private xFileJUnit File;

        internal List<xTestCaseJUnit> TestCases;

        private xTestCaseJUnit TestCaseCurrent ;

        public xTestCasesJUnit(xFileJUnit prmFile)
        {

            File = prmFile;

            TestCases = new List<xTestCaseJUnit>();

        }

        public void AddLine(string prmLine)
        {
            if (IsTestCase(prmLine))

                AddCase(prmLine);

            else

                AddFluxo(prmLine);

        }


        private void AddCase(string prmLine)
        {

            TestCaseCurrent = new xTestCaseJUnit(prmLine, File);

            TestCases.Add(TestCaseCurrent);

        }

        private void AddFluxo(string prmLine) 
        {

            TestCaseCurrent.Fluxos.Add(prmLine);

        }

        public string memo()
        {

            string lista = "";
            string aux = "";

            foreach (xTestCaseJUnit test_case in TestCases)
            {

                lista += aux + (test_case.nome);

                aux = Environment.NewLine;

            }

            return lista;
        }

        private bool IsTestCase(string prmLine) => (!prmLine.StartsWith(","));

    }
    public class xTestCaseJUnit
    {

        public xFileJUnit File;

        public xTestDataJUnit Fluxos;

        public xTestParametersJUnit Parametros;

        public string nome { get => Parametros.nome; }

        public string separador { get => File.separador; }

        public xTestCaseJUnit(string prmLine, xFileJUnit prmFile)
        {

            File = prmFile;

            Fluxos = new xTestDataJUnit(this);

            Parametros = new xTestParametersJUnit(this, prmLine);

        }

        public string memo()
        {
            return string.Format("CT[{0,30}]: {1}{2}", nome, Parametros.memo(), Fluxos.memo()) ; 
        }

    }
    public class xTestParametersJUnit
    {

        private xTestCaseJUnit TestCase;

        public string nome;

        private xMemo Lista;

        public xTestParametersJUnit(xTestCaseJUnit prmTestCase, string prmLine)
        {

            TestCase = prmTestCase;

            Lista = new xMemo(prmLine, TestCase.separador);

            nome = Lista.GetRemove();

        }
        public string GetJSON(xMemo prmFluxo)
        {

            string lista = "";

            string aux = "";

            int cont = 0;

            foreach (string atributo in Lista)
            {

                cont++;

                lista += aux + string.Format("'{0}': '{1}'", atributo, prmFluxo.Item(cont));

                aux = ",";

            }

            return ("{ "+ lista + " }");

        }
        public string memo() => Lista.memo(TestCase.separador);
 
    }
    public class xTestDataJUnit
    {

        private xTestCaseJUnit TestCase;

        public List<xMemo> Dados;

        public xTestDataJUnit(xTestCaseJUnit prmTestCase)
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
        public string memo()
        {

            string lista = "";
            string aux = "";

            foreach (xMemo fluxo in Dados)
            {

                lista += aux + (fluxo.memo(TestCase.separador));

                aux = Environment.NewLine;

            }

            return lista;
        }

    }
    public class xTestJSONJUnit
    {

        private xFileJUnit File;

        public List<xMemo> Dados;

        public xTestJSONJUnit(xFileJUnit prmFile)
        {

            File = prmFile;

            Dados =  new List<xMemo>();

            GetJSON();

        }

        private void GetJSON()
        {

            foreach (xTestCaseJUnit teste in File.TestCases)
            {

                foreach (xMemo fluxo in teste.Fluxos.Dados)
                {

                    Debug.WriteLine(  teste.Parametros.GetJSON(fluxo));

                }

            }

        }
        public string memo()
        {

            string lista = "";
            string aux = "";

            foreach (xMemo fluxo in Dados)
            {

                lista += aux + (fluxo.memo(File.separador));

                aux = Environment.NewLine;

            }

            return lista;
        }


    }
}
