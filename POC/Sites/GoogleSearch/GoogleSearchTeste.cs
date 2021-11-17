using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;


namespace Miranda.GoogleSearch
{
    public class GoogleSearchTextoTeste : TestScript
    {
        public void DATA()
        {

            Massa.Add(prmFluxo: @"{'Nome':'Alexandre Bartie'}");
            Massa.Add(prmFluxo: @"{'Nome':'Teste de Software'}");
            Massa.Add(prmFluxo: @"{'Nome':'Albert Einstein'}");

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
