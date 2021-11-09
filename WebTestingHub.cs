using System.Collections.Generic;
using System.Diagnostics;

namespace MeuSeleniumCSharp
{
    public class TestProject
    {

        public TestHub _hub;

        public TestConfig Config = new TestConfig();

        public TestHub Hub
        {
            get
            {
                if (_hub == null)
                    _hub = new TestHub(this);
                return _hub;
            }

        }                

    }

    public class TestHub
    {

        public TestProject Projeto;

        private List<TestSuite> Suites = new List<TestSuite>();
        public TestHub(TestProject prmProjeto)
        {
            Projeto = prmProjeto;
        }

        public void AddSuite(TestSuite prmSuite)
        {

            prmSuite.Setup(this);

            Suites.Add(prmSuite);
        }
        public void Executar(eTipoDriver prmTipoDriver)
        {

            foreach (TestSuite Suite in Suites)
                Suite.Executar(prmTipoDriver);

        }

    }
    public class TestConfig
    {

        public int PauseAfterTestCase;

        public int PauseAfterTestScript;

        public int PauseAfterTestSuite;

    }
}
