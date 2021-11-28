using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading;
using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System.Text;
using Dooggy.Factory.Data;
using Dooggy.Factory.Trace;

namespace Dooggy.Factory.Robot
{
    public enum eTipoDriver : int
    {
        ChromeDriver = 0,
        EdgeDriver = 1
    }
    public class TestProject : ITestDataLocal
    {

        public TestFactory Factory;
        
        public List<TestSuite> Suites = new List<TestSuite>();

        private string _name;

        public TestProject()
        {

            Factory = new TestFactory();

            Dados.Setup(this, Pool);

            Factory.Call(this, Factory.Parameters.GetProjectBlockCode());

        }

        public TestDataPool Pool { get => Factory.Pool; }

        public TestConfig Config { get => Factory.Config; }

        public TestTraceAction Trace { get => Factory.Trace.Action; }

        public string name { get => _name; }

        public void Setup(string prmName)
        {

            _name = prmName;

            Trace.ActionArea("Projeto de Teste", name);

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
        public void Pause(int prmSegundos)
        { Factory.Pause(prmSegundos); }

    }
    public class TestSuite : ITestDataLocal
    {

        public TestProject Projeto;

        private TestMotor Motor;

        public eTipoDriver tipoDriver;

        public List<TestScript> Scripts = new List<TestScript>();

        public string nome { get => this.GetType().Name; }

        public TestConfig Config { get => Projeto.Config; }

        public TestTraceAction Trace { get => Projeto.Trace; }

        public TestDataPool Pool => Projeto.Pool;

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
    public class TestScript : ITestDataLocal
    {

        public string nome;

        public TestMotor Motor;

        public QA_WebRobot Robot => Motor.Robot;

        public QA_MassaDados Massa => Robot.Massa;

        public TestSuite Suite => Motor.Suite;

        public TestFactory Factory => Robot.Factory;

        public TestTraceAction Trace => Suite.Trace;

        private void Metodo(string prmMetodo) => Robot.Factory.Call(this, prmMetodo);

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

                Robot.Pause(prmSegundos: Factory.Config.PauseAfterTestCase);

            }

        }
        private bool MetodoDADOS()
        {

            Metodo("DATA");

            return (Massa.IsOK);

        }
        private bool MetodoSETUP()
        {

            Metodo("SETUP");

            return (true);

        }
        private void MetodoEXECUCAO()
        {

            string blockCode = Factory.Parameters.GetScriptBlockCode();


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
                            //_driver = new EdgeDriver();
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

    }

}
