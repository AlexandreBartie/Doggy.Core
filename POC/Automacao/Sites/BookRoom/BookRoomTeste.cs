using BlueRocket.CORE.Factory.Robot;
using System;
using System.Collections.Generic;
using System.Text;

namespace BlueRocket.CORE.BookRoom
{

    public class BookRoomTeste : TestRobotScript
    {
        private void Executar()
        {

            Robot.Action.GoURL(prmUrl: "http://eliasnogueira.com/external/selenium-java-architecture/");


            //Robot.Find.ByName("q");
            //Robot.SetTexto("Alexandre Bartie");
            //Robot.Submit();

        }
    }
}
