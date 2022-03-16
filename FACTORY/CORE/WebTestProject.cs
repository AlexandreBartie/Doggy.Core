using BlueRocket.LIBRARY;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace BlueRocket.KERNEL
{

    public class TestDataProject : TestFactory
    {
        public DataConnect Connect { get => Pool.Connect; }

        
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
