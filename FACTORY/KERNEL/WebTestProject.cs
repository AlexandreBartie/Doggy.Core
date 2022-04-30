using Katty;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Dooggy
{

    public class TestDataProject : TestFactory
    {
        public DataConnect Connect { get => Pool.Connect; }

        
    }
    public class RobotProject : TestFactory
    {

        public string name;

        public string pathWebDriver;

        public eTipoDriver tipoDriver;

        public RobotSuites Suites = new RobotSuites();

        public void Start(eTipoDriver prmTipoDriver, string prmPathDriver)
        {

            Trace.LogRobot.ActionTag(prmTag: "Projeto", name);

            tipoDriver = prmTipoDriver;

            pathWebDriver = prmPathDriver;

            BuildSuites();

            PlaySuites();

        }

        private void BuildSuites()
        {

            Call(this, prmMetodo: Parameters.GetRobotFactoryBlockCode());

        }

        private void PlaySuites()
        {
            foreach (RobotSuite Suite in Suites)
                Suite.Executar();
        }

        public void AddSuite(RobotSuite prmSuite)
        {
            prmSuite.Setup(this);

            Suites.Add(prmSuite);
        }

        public void Pause(int prmSegundos)
        { Thread.Sleep(TimeSpan.FromSeconds(prmSegundos)); }

    }
}
