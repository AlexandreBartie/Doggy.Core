using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using BlueRocket.CORE;

namespace BlueRocket.CORE.GoogleSearch
{
    public class GoogleSearchTextoTeste : TestRobotScript
    {
        public void DATA()
        {

            //string arquivo = @"ArqDadosBaixaManualTesteNegativo.csv";

            //if (false)
            //{

            //    if (Massa.Fonte.FileJUnit.Open(prmPath: @"C:\Users\alexa\OneDrive\Área de Trabalho\", prmName: arquivo))
            //    {

            //        if (true)
            //        {
            //            foreach (xTestCaseJUnit teste in Massa.Fonte.FileJUnit.TestCases)
            //            {

            //                Debug.WriteLine("===============================================");
            //                Debug.WriteLine(teste.nome);
            //                Debug.WriteLine("===============================================");
            //                Debug.WriteLine(teste.Parametros.memo());
            //                Debug.WriteLine(teste.Flows.memo());
            //                Debug.WriteLine("===============================================");

            //            }
            //        }
            //        else
            //        {

            //            foreach (xMemo Flow in Massa.Fonte.FileJUnit.JSON.Dados)
            //            {

            //                Debug.WriteLine(Flow.memo());

            //                Debug.Assert(false);

            //            }
            //        }
            //    }
            //}

            Massa.Add(prmFlow: @"{'Nome':'Alexandre Bartie'}");
            Massa.Add(prmFlow: @"{'Nome':'Teste de Software'}");
            Massa.Add(prmFlow: @"{'Nome':'Albert Einstein'}");

            Massa.Save();

        }
        public void SETUP()
        {

            //Robot.GoURL(prmUrl: "http://www.google.com.br");

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

    public class GoogleSearchImagemTeste : TestRobotScript
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
