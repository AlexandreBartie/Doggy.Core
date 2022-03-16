using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading;
using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System.Text;

namespace BlueRocket.KERNEL
{
    public enum eTipoDriver : int
    {
        ChromeDriver = 0,
        EdgeDriver = 1
    }
    public class TestRobotSuite
    {

        public TestRobotProject Projeto;

        private QA_WebMotor Motor;

        public eTipoDriver tipoDriver;

        public TestRobotScripts Scripts = new TestRobotScripts();

        public string nome { get => this.GetType().Name; }

        public TestConfig Config { get => Projeto.Config; }

        public TestTraceLogRobot Log { get => Projeto.Trace.LogRobot; }

        public TestDataPool Pool => Projeto.Pool;

        public void Setup(TestRobotProject prmProjeto)
        {
            
            Projeto = prmProjeto;

            //Dados.Setup(this, prmProjeto.Pool);

        }

        public void AddScript(TestRobotScript prmScript)
        {
            Scripts.Add(prmScript);
        }

        public void Executar(eTipoDriver prmTipoDriver)
        {

            tipoDriver = prmTipoDriver;

            Motor = new QA_WebMotor(this);

            Log.ActionTag(prmTag: "Suite", this.nome);

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
    public class TestRobotScript
    {

        public string nome;

        public QA_WebMotor Motor;

        public QA_WebRobot Robot => Motor.Robot;

        public QA_MassaDados Massa => Robot.Massa;

        public TestRobotSuite Suite => Motor.Suite;

        public TestRobotProject Projeto => Robot.Projeto;

        public TestTraceLogRobot Trace => Suite.Log;

        private void Metodo(string prmMetodo) => Projeto.Call(this, prmMetodo);

        public void Executar(QA_WebMotor prmMotor)
        {

            this.nome = this.GetType().Name;
            this.Motor = prmMotor;

            Trace.ActionTag(prmTag: "Script", this.nome);

            //Dados.Setup(this, Suite.Pool);

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


            if (Massa.IsON)
            {

                int cont = 0;
                
                // Massa ON-LINE
                 while (Massa.IsCurrent)
                {

                    Trace.ActionTag(prmTag: "Teste", string.Format("#{0}", cont++));

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

    public class TestRobotSuites : List<TestRobotSuite> { }
    public class TestRobotScripts : List<TestRobotScript> { }

}
