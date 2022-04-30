using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading;
using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System.Text;
using Katty;

namespace Dooggy
{
    public enum eTipoDriver : int
    {
        ChromeDriver = 0,
        EdgeDriver = 1
    }
    public class RobotSuite
    {

        public RobotProject Projeto;

        private WebMotor Motor;

        public string nome => this.GetType().Name;
        
        public DataPool Pool => Projeto.Pool;
        public TestConfig Config => Projeto.Config;

        public RobotScripts Scripts = new RobotScripts();

        public TestTrace Trace => Projeto.Trace;
        public TestTraceLogRobot Log => Trace.LogRobot;

        public void Setup(RobotProject prmProjeto)
        {
            
            Projeto = prmProjeto;

            //Dados.Setup(this, prmProjeto.Pool);

        }

        public void AddScript(RobotScript prmScript)
        {
            Scripts.Add(prmScript);
        }

        public void Executar()
        {

            Motor = new WebMotor(this);

            if (Motor.IsWorking)
            {
                Log.ActionTag(prmTag: "Suite", this.nome);

                foreach (RobotScript Script in Scripts)
                     Script.Executar(Motor);

                Encerrar();
            }
            else
                Trace.Erro.msgErro("Automação abortada: WebDriver não localizado ...");

            Motor = null;

        }

        private void Encerrar()
        {

            if (!Config.onlyDATA)
            {

                Projeto.Pause(Config.pauseAfterRobotSuite);

                Motor.Encerrar();
            }
            
        }

    }
    public class RobotScript
    {

        public string nome;

        public WebMotor Motor;

        public WebRobot Robot => Motor.Robot;

        public RobotData Massa => Robot.Massa;

        public RobotSuite Suite => Motor.Suite;
        public RobotProject Projeto => Robot.Projeto;

        public TestTrace Trace => Suite.Trace;
        public TestTraceLogRobot Log => Suite.Log;

        private void Metodo(string prmMetodo) => Projeto.Call(this, prmMetodo);

        public void Executar(WebMotor prmMotor)
        {

            this.nome = this.GetType().Name;
            this.Motor = prmMotor;

            Log.ActionTag(prmTag: "Script", this.nome);

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

                    Log.ActionTag(prmTag: "Teste", string.Format("#{0}", cont++));

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

    public class RobotSuites : List<RobotSuite> { }
    public class RobotScripts : List<RobotScript> { }

    public class RobotData
    {
        private WebRobot Robot;

        public RobotSource Fonte;

        public myJSON JSON = new myJSON();

        public bool IsON;

        public RobotData(WebRobot prmRobot)
        {

            Robot = prmRobot;

            Fonte = new RobotSource(this);

        }

        public RobotProject Project { get => Robot.Projeto; }
        private TestTrace Trace { get => Robot.Trace; }
        private DataPool Pool { get => Project.Pool; }
        public bool IsOK { get => JSON.IsOK; }
        public bool IsCurrent { get => JSON.IsCurrent; }
        public void Add(string prmFlow)
        {
            //if (IsSTATIC)
            JSON.Add(prmFlow);
            //else
            //AddCombine(prmFlow, prmMestre: DefaultView.json());
        }
        public void Add(string prmFlow, string prmView)
        {
            AddCombine(prmFlow, prmMestre: Pool.json(prmView));
        }
        private void AddCombine(string prmFlow, string prmMestre)
        {
            JSON.Add(prmFlow, prmMestre);
        }
        public bool Save()
        {
            IsON = true;

            if (!JSON.Save())
            { Trace.Erro.msgErro("ERRO{JSON:Save} " + JSON.Flow); }

            return (JSON.IsOK);

        }
        public bool Next()
        {
            return (JSON.Next());
        }

        public string GetValor(string prmKey, string prmPadrao)
        {
            return (JSON.GetValor(prmKey, prmPadrao));
        }

    }
    public class RobotSource
    {

        private RobotData Massa;

        private FileTXT _FileTXT;

        private FileJUNIT _FileJUnit;

        private TestConfig Config { get => Massa.Project.Config; }

        public FileTXT FileTXT
        {
            get
            {
                if (_FileTXT is null)
                    _FileTXT = new FileTXT();
                return (_FileTXT);
            }
            set
            {
                _FileTXT = value;
            }
        }
        public FileJUNIT FileJUnit
        {
            get
            {
                if (_FileJUnit is null)
                {
                    _FileJUnit = new FileJUNIT();
                    _FileJUnit.SetEncoding(Config.EncodedDataJUNIT);
                }
                return (_FileJUnit);
            }
            set
            {
                _FileJUnit = value;
            }
        }
        public RobotSource(RobotData prmMassa)
        {

            Massa = prmMassa;

        }

    }

}
