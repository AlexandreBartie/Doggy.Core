using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using System.Diagnostics;
using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Edge;

namespace MeuSeleniumCSharp
{
    public enum eTipoDriver : int
    {
        ChromeDriver = 0,
        EdgeDriver = 1
    }
    public class TestProject : IDataLocalConnection
    {

        public string name;

        public DataPoolConnection Pool = new DataPoolConnection();

        public List<TestSuite> Suites = new List<TestSuite>();

        public TestKernel Kernel = new TestKernel();

        public TestProject()
        {

            Dados.Setup(this, Pool);

            Kernel.Call(this, Kernel.GetProjectBlockCode());

        }

        public TestConfig Config
        { get => Kernel.Config; }

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
        public void Pause(int prmSegundos)
        { Kernel.Pause(prmSegundos); }

    }
    public class TestSuite : IDataLocalConnection
    {

        public TestProject Projeto;

        private TestMotor Motor;

        public eTipoDriver tipoDriver;

        public List<TestScript> Scripts = new List<TestScript>();

        public string nome { get => this.GetType().Name; }

        public TestConfig Config { get => Projeto.Config; }

        public DataPoolConnection Pool => Projeto.Pool;

        public void Setup(TestProject prmProjeto)
        {
            
            Projeto = prmProjeto;

            Dados.Setup(this, prmProjeto.Pool);

        }

        public void AddScript(TestScript prmScript)
        {
            Scripts.Add(prmScript);
        }

        public void Executar(eTipoDriver prmTipoDriver)
        {

            tipoDriver = prmTipoDriver;

            Motor = new TestMotor(this);

            foreach (TestScript Script in Scripts)
            {
                Script.Executar(Motor);

            }

            Encerrar();

        }

        private void Encerrar()
        {

            if (!Config.OnlyDATA)
            {

                Projeto.Pause(Config.PauseAfterTestSuite);

                Motor.Encerrar();
            }
            
        }

    }
    public class TestScript : IDataLocalConnection
    {

        public string nome;

        public TestMotor Motor;

        public QA_WebRobot Robot => Motor.Robot;

        public QA_MassaDados Massa => Robot.Massa;

        public TestSuite Suite => Motor.Suite;

        public TestConfig Config => Suite.Config;

        private void Metodo(string prmMetodo) => Robot.Kernel.Call(this, prmMetodo);

        public void Executar(TestMotor prmMotor)
        {

            this.nome = this.GetType().Name;
            this.Motor = prmMotor;

            Dados.Setup(this, Suite.Pool);


            if (MetodoDADOS())
            {
                if (MetodoSETUP())
                {

                    MetodoEXECUCAO();

                }

                Robot.Pause(prmSegundos: Config.PauseAfterTestCase);

            }

        }
        private bool MetodoDADOS()
        {

            Metodo("DATA");

            if (Config.OnlyDATA)
                return (false);

            return (Massa.IsOK);

        }
        private bool MetodoSETUP()
        {

            Metodo("SETUP");

            return (true);

        }
        private void MetodoEXECUCAO()
        {

            if (!Massa.IsONLINE)
                Metodo("PLAY;CHECK;CLEANUP");
            else
            {
                while (Massa.Next())
                {
                    Metodo("PLAY;CHECK;CLEANUP");
                    Motor.Refresh();
                }
            }
        }
    }
    public class TestMotor
    {

        public TestSuite Suite;

        private IWebDriver _driver;

        public TestMotor(TestSuite prmSuite)
        { Suite = prmSuite; }

        private QA_WebRobot _robot;

        public QA_WebRobot Robot
        {
            get
            {
                if (_robot == null)
                    { _robot = new QA_WebRobot(this); }
                return _robot;
            }
        }
        public IWebDriver driver
        {
            get
            {
                if (_driver == null)
                {

                    Debug.Assert(false);

                    switch (Suite.tipoDriver)
                    {

                        case eTipoDriver.EdgeDriver:
                            _driver = new EdgeDriver();
                            break;

                        default:
                            _driver = new ChromeDriver();
                            break;

                    }

                }
                return _driver;
            }
        }

        public void Refresh() => Robot.Page.Refresh();

        public void Encerrar() => Robot.Quit();

/*        public IWebElement GetElementByXPath(string prmXPath)
        {
            try
            {
                return driver.FindElement(By.XPath(prmXPath));
            }
            catch (Exception e)
            {
                Robot.Debug.Erro(e);
            }
            return (null);
        }
        public ReadOnlyCollection<IWebElement> GetElementsByXPath(string prmXPath)
        {
            try
            {
                return driver.FindElements(By.XPath(prmXPath));
            }
            catch (Exception e)
            {
                Robot.Debug.Erro(e);
            }
            return (null);
        }*/

    }
    public class TestConfig
    {

        public bool OnlyDATA;

        public int PauseAfterTestCase;

        public int PauseAfterTestScript;

        public int PauseAfterTestSuite;

    }
    public class TestLog
    {
        public bool Aviso(string prmAviso)
        {
            Debug.Print(prmAviso);
            return false;
        }
        public bool Erro(string prmErro)
        {
            Debug.Print(prmErro);
            return false;
        }

    }
    public class TestKernel
    {

        public TestLog Log = new TestLog();

        public TestConfig Config = new TestConfig();

        public string GetProjectBlockCode() => ("DATA; BUILD; START");

        public string GetAdicaoElementos() => ("+");
    
        public string GetXPathBuscaRaizElementos() => "//*[@{0}='{1}']";

        public bool Call(Object prmObjeto, string prmMetodo)
        {

            bool vlOk = true;

            xLista lista = new xLista(prmMetodo);

            foreach (string metodo in lista)
            {
                try
                {
                    prmObjeto.GetType().GetMethod(metodo).Invoke(prmObjeto, null);
                }

                catch
                {
                    Log.Aviso(string.Format("Método não encontrado: {0}.{1}", prmObjeto.GetType().FullName, metodo));

                    vlOk = false;

                }

            }

            return (vlOk);

        }
        public void Pause(int prmSegundos)
        { Thread.Sleep(TimeSpan.FromSeconds(prmSegundos)); }

    }
}
