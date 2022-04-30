using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Forms;
using System.Text;
using Dooggy;

namespace Dooggy.POC.AutomacaoTestes
{
    public class POC_AutomacaoTestesScripts : RobotProject
    {

        public void BASE() { }
        public void DATA() { }
        public void BUILD()
        {

            this.name = "POC: Testes de Regressão";

            //this.AddSuite(new SuiteKatalon());
            //this.AddSuite(new SuiteGoogle());
            this.AddSuite(new SuiteYouTube());

        }
        public void CONFIG()
        {
            Config.EncodedDataJUNIT = Encoding.UTF7;

            Config.onlyDATA = false;

            Config.pauseAfterTestCase = 10;
        }
    }
   public class SuiteKatalon : RobotSuite
   {
        public SuiteKatalon()
        {
            AddScript(new Katalon.KatalonTeste());
        }
    }
    public class SuiteGoogle : RobotSuite
    {
        public SuiteGoogle()
        {
            AddScript(new GoogleSearch.GoogleSearchTextoTeste());
            AddScript(new GoogleSearch.GoogleSearchImagemTeste());
        }

    }

    public class SuiteYouTube : RobotSuite
    {
        public SuiteYouTube()
        {
            AddScript(new YouTube.YouTubeChannel());
        }

    }
}
