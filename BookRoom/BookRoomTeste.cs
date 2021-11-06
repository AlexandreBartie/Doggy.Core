using System;
using System.Collections.Generic;
using System.Text;

namespace MeuSeleniumCSharp.BookRoom
{

    public class BookRoomTeste : QA_WebScript
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
