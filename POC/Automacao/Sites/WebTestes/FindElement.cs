using System;
using System.Collections.Generic;
using Dooggy.CORE;
using OpenQA.Selenium;

namespace Dooggy.CORE.WebTestes
{
    public class FindElementTeste : TestRobotScript
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

                }
                catch (Exception e)
                {
                    Robot.Erro.msgErro(e);
                   }
            }

            //int cont = 0;

            //foreach (IWebElement item in teste.FindElements(By.XPath(filtro)))
            //{
            //    cont++;

            //    item.Click();

            //}

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
