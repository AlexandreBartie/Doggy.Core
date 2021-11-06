using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;


namespace MeuSeleniumCSharp.GoogleSearch
{
    public class GoogleSearchTextoTeste : QA_WebScript
    {
        private void Executar()
        {

            Robot.GoURL(prmUrl: "http://www.google.com.br");

            Robot.SetTexto("Alexandre Bartie","name=q");

            Robot.Submit();

        }
    }

    public class GoogleSearchImagemTeste : QA_WebScript
    {

        private void Executar()
        {

            Robot.GoURL(prmUrl: "https://www.google.com/maps/search/");

            Robot.SetTexto("Pizzaria", "searchboxinput");
            Robot.Submit();

        }
    }
}
