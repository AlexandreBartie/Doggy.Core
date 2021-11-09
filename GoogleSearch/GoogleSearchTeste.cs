using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;


namespace MeuSeleniumCSharp.GoogleSearch
{
    public class GoogleSearchTextoTeste : TestScript
    {
        public void DATA()
        {

            Dados.Add(prmFluxo: @"{'Nome':'Alexandre Bartie'}");
            Dados.Add(prmFluxo: @"{'Nome':'Teste de Software'}");
            Dados.Add(prmFluxo: @"{'Nome':'Albert Einstein'}");

            Dados.Save();

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

        }

    }

    public class GoogleSearchImagemTeste : TestScript
    {

        public void SETUP()
        {

            Robot.GoURL(prmUrl: "http://www.google.com.br");

            Robot.Mapping("Nome", "name=q");

        }

        public void PLAY()
        {

            Robot.Input("Nome", "Alexandre Bartie");

            Robot.Submit();

        }
    }
}
