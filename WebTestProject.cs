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
    public class TestProject
    {

        public TestHub Hub;
        
        public TestKernel Kernel = new TestKernel();

        public TestProject()
        {
            Hub = new TestHub(this);

            Kernel.InvokeMethod(this, prmMetodo: "DATA;BUILD;START");

        }
        public void Pause(int prmSegundos)
        { Thread.Sleep(TimeSpan.FromSeconds(prmSegundos)); }

    }

    public class TestHub
    {

        public TestProject Projeto;

        public DataPoolConnection Pool = new DataPoolConnection();

        private List<TestSuite> Suites = new List<TestSuite>();

        public string name; 

        public TestHub(TestProject prmProjeto)
        {
            Projeto = prmProjeto;
        }

        public TestConfig Config

        { get => Projeto.Kernel.Config; }

        public bool AddDataBase(string prmTag, string prmConexao)
        {
            return (Pool.AddDataBase(prmTag, prmConexao));
        }
        public bool AddDataView(string prmTag, string prmSQL)
        {
            return (Pool.AddDataView(prmTag, prmSQL));
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
    public class TestSuite
    {

        private TestHub Hub;

        private TestMotor Motor;

        public eTipoDriver tipoDriver;

        public List<TestScript> Scripts = new List<TestScript>();
        public string nome
        {
            get => this.GetType().Name;
        }

        public TestProject Projeto
        {
            get => Hub.Projeto;
        }
        public TestConfig Config
        {
            get => Projeto.Kernel.Config;
        }

        public void Setup(TestHub prmHub)
        {
            Hub = prmHub;
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

            Projeto.Pause(Config.PauseAfterTestSuite);

            Motor.Encerrar();

        }

    }
    public class TestScript
    {

        public string nome;

        public TestMotor Motor;

        public void Executar(TestMotor prmMotor)
        {

            this.nome = this.GetType().Name;
            this.Motor = prmMotor;


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

            return (Dados.IsOK);

        }
        private bool MetodoSETUP()
        {

            Metodo("SETUP");

            return (true);

        }
        private void MetodoEXECUCAO()
        {

            if (Dados.IsOFF)
                Metodo("PLAY;CHECK;CLEANUP");
            else
            {
                while (Dados.Next())
                {
                    Metodo("PLAY;CHECK;CLEANUP");
                    Motor.Refresh();
                }
            }
        }

        public QA_WebRobot Robot
        {
            get => Motor.Robot;
        }
        public QA_MassaDados Dados
        {
            get => Robot.Dados;
        }
        public DataPoolConnection Pool
        {
            get => Robot.Pool;
        }
        public TestSuite Suite
        {
            get => Motor.Suite;
        }
        public TestConfig Config
        {
            get => Suite.Config;
        }
        private void Metodo(string prmMetodo)
        { Robot.Kernel.InvokeMethod(this, prmMetodo); }
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
                if (_robot == null) { _robot = new QA_WebRobot(this); }
                return _robot;
            }
        }
        public IWebDriver driver
        {
            get
            {
                if (_driver == null)
                {
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

        public void Refresh()
        {

            Robot.Page.Refresh();
        }

        public IWebElement GetElementByXPath(string prmXPath)
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
        }
        public void Encerrar()
        { Robot.Quit(); }

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
        public bool InvokeMethod(Object prmObjeto, string prmMetodo)
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
                    Log.Aviso(string.Format("Método não encontrado: {0}.{1}" + prmObjeto.GetType().FullName, metodo));

                    vlOk = false;
                }

            }

            return (vlOk);

        }

        public void Pause(int prmSegundos)
        { Thread.Sleep(TimeSpan.FromSeconds(prmSegundos)); }

    }

}
