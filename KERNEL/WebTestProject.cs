using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading;
using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Edge;
using System.Text;

namespace Dooggy
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

        public TestConfig Config { get => Kernel.Config; }

        public TestTrace Trace { get => Kernel.Trace; }

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

        public TestTrace Trace { get => Projeto.Trace; }

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

            Trace.ActionArea("Suite de Teste", this.nome);

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

        public TestKernel Kernel => Robot.Kernel;

        public TestTrace Trace => Suite.Trace;

        private void Metodo(string prmMetodo) => Robot.Kernel.Call(this, prmMetodo);

        public void Executar(TestMotor prmMotor)
        {

            this.nome = this.GetType().Name;
            this.Motor = prmMotor;

            Trace.ActionArea("Script de Teste", this.nome);

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

            string blockCode = Kernel.GetScriptBlockCode();


            if (Trace.ActionMassaOnLine(Massa.IsONLINE))
            {
                // Massa ON-LINE
                while (Massa.IsCurrent)
                {
                    Metodo(prmMetodo: blockCode);
                    Motor.Refresh();

                    Massa.Next();
                }
            }
            else
                // Massa OFF-LINE
                Metodo(prmMetodo: blockCode);
        }
    }
    public class TestMotor
    {

        public TestSuite Suite;

        private QA_WebRobot _robot;

        private IWebDriver _driver;

        public TestMotor(TestSuite prmSuite)
        { Suite = prmSuite; }


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

        public string PathFileSources;

        public Encoding EncodedDataJUNIT;
        
        public bool OnlyDATA;

        public int PauseAfterTestCase;

        public int PauseAfterTestScript;

        public int PauseAfterTestSuite;

    }

    public class TestTrace
    {

        public TestLog Log;

        public TestTraceInternalError InternalError;

        public TestTrace()
        {

            Log = new TestLog();

            InternalError = new TestTraceInternalError(this);

        }


        public void ActionArea(string prmArea, string prmName) => Log.Trace(String.Format("{0}: [{1}]", prmArea, prmName));

        public bool ActionMassaOnLine(bool prmMassaOnLine)
        {

            string texto;

            if (prmMassaOnLine)
                texto = "ON-LINE";
            else
                texto = "OFF-LINE";

            Log.Trace(String.Format("Massa de Testes '{0}'", texto));

            return (prmMassaOnLine);

        }

        public void ActionElement(string prmAcao, string prmElemento) => ActionElement(prmAcao, prmElemento, prmValor: null);
        public void ActionElement(string prmAcao, string prmElemento, string prmValor)
        {

            string msg = String.Format("ACTION: {0} {1,15} := {1}", prmAcao, prmElemento);

            if (prmValor != null)
                msg += " := " + prmValor;

            Log.Trace(msg);

        }

        public void ActionFail(string prmComando, Exception e) => InternalError.ActionFail(prmComando, e);

        // public bool ActionFail(string prmErro, string prmDestaque) => (Log.Erro(String.Format("{0} ... [{1}]", prmErro, prmDestaque)));


        //public void NoFindMetodh(string prmAcao, string prmElemento, string prmValor)

        //Log.Aviso(string.Format("Método {0}.{1} não encontrado. [{2}]





        //

    }

    public class TestTraceInternalError
    {

        private TestTrace Trace;

        public TestLog Log { get => Trace.Log; }

        public TestTraceInternalError(TestTrace prmTrace)
        {

            Trace = prmTrace;

        }
        public void ActionFail(string prmComando, Exception e) => Log.Erro("ACTION FAIL: ROBOT." + prmComando, e);

        public void TargetNotFound(string prmTAG) => Log.Erro("TARGET NOT FOUND: " + prmTAG);

    }
    public class TestLog
    {

        public bool Trace(string prmTrace) => Message("TRACE", prmTrace);
        public bool Aviso(string prmAviso) => Message("AVISO", prmAviso);
        public bool Falha(string prmAviso) => Message("FALHA", prmAviso);
        public bool Erro(Exception e) => Message("ERRO", e.Message);
        public bool Erro(string prmErro) => Message("ERRO", prmErro);
        public bool Erro(string prmErro, Exception e) => Message("ERRO", String.Format("{0} Error: {1}", prmErro,e.Message));
        public bool Interno(string prmErro) => Message("KERNEL", prmErro);

        private bool Message(string prmTipo, string prmMensagem)
        {
            Debug.WriteLine(String.Format("[{0,5}]: {1} ", prmTipo, prmMensagem));
            return false;
        }

    }
    public class TestKernel
    {

        public TestTrace Trace = new TestTrace();

        public TestConfig Config = new TestConfig();

        public string GetProjectBlockCode() => ("DATA, BUILD, CONFIG");
        public string GetScriptBlockCode() => ("PLAY, CHECK, CLEANUP");
        public string GetAdicaoElementos() => ("+");
    
        public string GetXPathBuscaRaizElementos() => "//*[@{0}='{1}']";

        public bool Call(Object prmObjeto, string prmMetodo)
        {

            bool vlOk = true;

            xLista lista = new xLista();

            lista.Parse(prmMetodo, prmSeparador: ",");

            foreach (string metodo in lista)
            {
                try
                {
                    prmObjeto.GetType().GetMethod(metodo).Invoke(prmObjeto, null);
                }

                catch
                {
                    
                    Trace.Log.Aviso(string.Format("Método {0}.{1} não encontrado. [{2}]", prmObjeto.GetType().Name, metodo, prmObjeto.GetType().Assembly.GetName()));

                    vlOk = false;

                }

            }

            return (vlOk);

        }
        public void Pause(int prmSegundos)
        { Thread.Sleep(TimeSpan.FromSeconds(prmSegundos)); }

    }
}
