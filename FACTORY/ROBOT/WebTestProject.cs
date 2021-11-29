using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading;
using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System.Text;
using Dooggy.Factory.Data;
using Dooggy.Factory;

namespace Dooggy.Factory.Robot
{
    public enum eTipoDriver : int
    {
        ChromeDriver = 0,
        EdgeDriver = 1
    }
    public class TestRobotProject : TestFactory
    {
        
        public string name;

        public List<TestRobotSuite> Suites = new List<TestRobotSuite>();

        public void Start(eTipoDriver prmTipoDriver)
        {

            Trace.Action.ActionArea("Projeto de Teste", name);

            BuildSuites();

            PlaySuites(prmTipoDriver);

        }

        private void BuildSuites()
        {

            Call(this, Parameters.GetRobotFactoryBlockCode());

        }
        
        private void PlaySuites(eTipoDriver prmTipoDriver)
        {

            foreach (TestRobotSuite Suite in Suites)
                Suite.Executar(prmTipoDriver);

        }

        public void AddSuite(TestRobotSuite prmSuite)
        {

            prmSuite.Setup(this);

            Suites.Add(prmSuite);
        }

        public void Pause(int prmSegundos)
        { Thread.Sleep(TimeSpan.FromSeconds(prmSegundos)); }

    }
    public class TestRobotSuite : ITestDataLocal
    {

        public TestRobotProject Projeto;

        private QA_WebMotor Motor;

        public eTipoDriver tipoDriver;

        public List<TestRobotScript> Scripts = new List<TestRobotScript>();

        public string nome { get => this.GetType().Name; }

        public TestConfig Config { get => Projeto.Config; }

        public TestRobotTrace Trace { get => Projeto.Trace.Action; }

        public TestDataPool Pool => Projeto.Pool;

        public void Setup(TestRobotProject prmProjeto)
        {
            
            Projeto = prmProjeto;

            Dados.Setup(this, prmProjeto.Pool);

        }

        public void AddScript(TestRobotScript prmScript)
        {
            Scripts.Add(prmScript);
        }

        public void Executar(eTipoDriver prmTipoDriver)
        {

            tipoDriver = prmTipoDriver;

            Motor = new QA_WebMotor(this);

            Trace.ActionArea("Suite de Teste", this.nome);

            foreach (TestRobotScript Script in Scripts)
            {
                Script.Executar(Motor);
            }

            Encerrar();

        }

        private void Encerrar()
        {

            if (!Config.onlyDATA)
            {

                Projeto.Pause(Config.pauseAfterTestRobotSuite);

                Motor.Encerrar();
            }
            
        }

    }
    public class TestRobotScript : ITestDataLocal
    {

        public string nome;

        public QA_WebMotor Motor;

        public QA_WebRobot Robot => Motor.Robot;

        public QA_MassaDados Massa => Robot.Massa;

        public TestRobotSuite Suite => Motor.Suite;

        public TestRobotProject Projeto => Robot.Projeto;

        public TestRobotTrace Trace => Suite.Trace;

        private void Metodo(string prmMetodo) => Projeto.Call(this, prmMetodo);

        public void Executar(QA_WebMotor prmMotor)
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

                Robot.Pause(prmSegundos: Projeto.Config.pauseAfterTestCase);

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

            string blockCode = Projeto.Parameters.GetScriptBlockCode();


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


}
