using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using Dooggy;

namespace Dooggy.GoogleSearch
{
    public class GoogleSearchTextoTeste : RobotScript
    {
        public void DATA()
        {

            Massa.Add(prmFlow: @"{'Nome':'Alexandre Bartie'}");
            Massa.Add(prmFlow: @"{'Nome':'Teste de Software'}");
            Massa.Add(prmFlow: @"{'Nome':'Albert Einstein'}");
            Massa.Add(prmFlow: @"{'Nome':'G4MAERYT'}");
            Massa.Add(prmFlow: @"{'Nome':'FOCAS'}");

            Massa.Save();

        }
        public void SETUP()
        {

            Robot.GoURL(prmUrl: "http://www.google.com.br");

            Robot.Mapping("Nome", "name=q");

        }

        public void PLAY()
        {

            Robot.Input("Nome", "Alexandre Bartie");

        }
        public void CHECK()
        {

            Robot.Submit();

            Robot.Pause(2);

        }
        public void CLEANUP()
        {



        }

    }

    public class GoogleSearchImagemTeste : RobotScript
    {

        public void SETUP()
        {

            Robot.GoURL(prmUrl: "http://www.google.com.br");

            Robot.Mapping("Nome", "name=q");

        }

        public void PLAY()
        {

            Robot.Input("Nome", "Alexandre Bartie");

        }

        public void CHECK()
        {

            Robot.Submit();

        }
        public void CLEANUP()
        {



        }


    }
}
