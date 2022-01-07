using Dooggy.Factory.Console;
using Dooggy.Factory.Data;
using Dooggy.Factory.Robot;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Dooggy.Factory
{

    public class TestDataProject : TestFactory
    {
        public TestDataConnect Connect { get => Pool.Connect; }

        public void Start(string prmPathDestino)
        {

            Pool.SetPathOUT(prmPathDestino);

            Call(this, Parameters.GetDataFactoryBlockCode());

        }

        public void Start(string prmPathINI, string prmPathDestino)
        {

            Pool.SetPathOUT(prmPathDestino);

            Console.Start(prmPathINI);

        }

    }
    public class TestRobotProject : TestFactory
    {

        public string name;

        public TestRobotSuites Suites = new TestRobotSuites();

        public void Start(eTipoDriver prmTipoDriver)
        {

            Trace.LogRobot.ActionTag(prmTag: "Projeto", name);

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
}
