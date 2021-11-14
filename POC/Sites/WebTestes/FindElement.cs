using System;
using System.Collections.Generic;
using OpenQA.Selenium;

namespace MeuSeleniumCSharp.WebTestes
{
    public class FindElementTeste : TestScript
    {

        public void SETUP()
        {

            Robot.GoURL(prmUrl: "https://katalon-test.s3.amazonaws.com/aut/html/form.html");

            IWebElement teste = Robot.GetElementByName("gender");

            string filtro;

            while (true)
            {

                //filtro = "//*[@name='gender'].contains(text(),'Female')]";

                //filtro = "//*[contains(text(),'Female')]";

                filtro = "//*[@name='gender']";

                try
                {
                    IReadOnlyCollection<IWebElement> lista = Robot.GetElementsByXPath(filtro);

                    Robot.Trace.Stop();
                    break;
                }
                catch (Exception e)
                {
                    Robot.Trace.Erro(e);
                   }

                Robot.Trace.Stop();
            }

            int cont = 0;

            foreach (IWebElement item in teste.FindElements(By.XPath(filtro)))
            {
                cont++;

                Robot.Trace.Stop(String.Format("Contador: {0} - {1}", cont, item.GetAttribute("label")));

                item.Click();

            }

        }
        public void PLAY()
        {

            Robot.Submit();
        }
        public void CHECK()
        {

        }
        public void CLEANUP()
        {

        }

    }
}
