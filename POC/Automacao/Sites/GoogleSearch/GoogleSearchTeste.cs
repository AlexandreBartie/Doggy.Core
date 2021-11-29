using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using Dooggy.Factory.Robot;
using Dooggy.Lib.Files;
using Dooggy.Lib.Generic;

namespace Dooggy.GoogleSearch
{
    public class GoogleSearchTextoTeste : TestRobotScript
    {
        public void DATA()
        {

            if (false)
            {

                string arquivo = "ArqDadosBaixaManualTesteNegativo.csv";

                if (Massa.Fonte.FileJUnit.Open(prmPath: @"C:\Users\alexa\OneDrive\Área de Trabalho\", prmName: arquivo))
                {

                    if (true)
                    {
                        foreach (xTestCaseJUnit teste in Massa.Fonte.FileJUnit.TestCases)
                        {

                            Debug.WriteLine("===============================================");
                            Debug.WriteLine(teste.nome);
                            Debug.WriteLine("===============================================");
                            Debug.WriteLine(teste.Parametros.memo());
                            Debug.WriteLine(teste.Fluxos.memo());
                            Debug.WriteLine("===============================================");

                        }
                    }
                    else
                    {

                        foreach (xMemo fluxo in Massa.Fonte.FileJUnit.JSON.Dados)
                        {

                            Debug.WriteLine(fluxo.memo());

                            Debug.Assert(false);

                        }
                    }
                }
            }

            Massa.Add(prmFluxo: @"{'Nome':'Alexandre Bartie'}");
            Massa.Add(prmFluxo: @"{'Nome':'Teste de Software'}");
            Massa.Add(prmFluxo: @"{'Nome':'Albert Einstein'}");

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
