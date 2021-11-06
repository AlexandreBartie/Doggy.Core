using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Diagnostics;

namespace MeuSeleniumCSharp
{
    class QA_WebHub
    {

        private QA_WebSuite Suite = new QA_WebSuite();

        public void Executar(eTipoDriver prmTipoDriver)
        {
            // Script = new GoogleSearch.GoogleSearchTextoTeste(Grid);
            //Script = new GoogleSearch.GoogleSearchImagemTeste(Grid);

            //Script = new BookRoom.BookRoomTeste(Grid);

            Suite.AddScript(new Katalon.KatalonTeste());

            Suite.Executar(prmTipoDriver);

        }

    }

    public class QA_WebSuite
    {

        private QA_WebMotor Motor;

        public List<QA_WebScript> Scripts = new List<QA_WebScript>();

        public void AddScript(QA_WebScript prmScript)
        {
            Scripts.Add(prmScript);
        }

        public void Executar(eTipoDriver prmTipoDriver)
        {
            Motor = new QA_WebMotor(eTipoDriver.ChromeDriver);

            foreach (QA_WebScript Script in Scripts)
            { 
                Script.Executar(Motor);
            }

            Motor.Encerrar();

        }

    }
}
