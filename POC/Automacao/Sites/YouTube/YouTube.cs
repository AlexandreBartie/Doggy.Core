using System;
using System.Collections.Generic;
using System.Text;

namespace Dooggy.YouTube
{
    public class YouTubeChannel : RobotScript
    {
        public void DATA()
        {

            Massa.Add(prmFlow: @"{ 'Pesquisar': 'DGP mundo' }");
            Massa.Add(prmFlow: @"{ 'Pesquisar': 'G4MAERYT' }");

            Massa.Save();

        }
        public void SETUP()
        {

            Robot.GoURL(prmUrl: "https://www.google.com/");

            Robot.Mapping("FazerLogin", "className=gb_1 gb_2 gb_9d gb_ha gb_9c");
            Robot.Mapping("OutraConta", "xpath=//div[@id='buttons']/ytd-button-renderer/a/tp-yt-paper-button");

            Robot.GoURL("https://accounts.google.com/ServiceLogin/signinchooser?hl=pt-BR&passive=true&continue=https%3A%2F%2Fwww.google.com%2Fsearch%3Fq%3Dgoogle%26oq%3Dgoogle%26aqs%3Dedge..69i57j0i433i512l4j69i60j69i65l3.1478j0j4%26sourceid%3Dchrome%26ie%3DUTF-8&ec=GAZAAQ&flowName=GlifWebSignIn&flowEntry=ServiceLogin");



        }
        public void PLAY()
        {

            //Robot.Click("FazerLogin");

            //Robot.Click("OutraConta");

            // Robot.Input("Pesquisar", "algo");







            Robot.Pause(4);

        }
        public void CHECK()
        {

            Robot.Submit();

        }
        public void CLEANUP()
        {

            Robot.Refresh();

        }

    }
}
