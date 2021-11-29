using Dooggy.Factory.Robot;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dooggy.BookRoom
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
