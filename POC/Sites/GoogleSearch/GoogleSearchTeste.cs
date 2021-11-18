using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;


namespace Dooggy.GoogleSearch
{
    public class GoogleSearchTextoTeste : TestScript
    {
        public void DATA()
        {


            //if (Massa.Fonte.FileTXT.Open(prmPath: @"C:\Users\alexa\OneDrive\Área de Trabalho\", prmName: "ArqDadosAutorizarDebito.csv"))
            //    Debug.Print(Massa.Fonte.FileTXT.memo());

            if (Massa.Fonte.FileJUnit.Open(prmPath: @"C:\Users\alexa\OneDrive\Área de Trabalho\", prmName: "ArqDadosBaixaManualTesteNegativo.csv"))
            {

                if (false)
                {
                    foreach (LIB.xTestCaseJUnit teste in Massa.Fonte.FileJUnit.TestCases)
                    {

                        Debug.WriteLine("===============================================");
                        Debug.WriteLine(teste.nome);
                        Debug.WriteLine("===============================================");
                        Debug.WriteLine(teste.Parametros.memo());
                        Debug.WriteLine(teste.Fluxos.memo());
                        Debug.WriteLine("===============================================");

                        Debug.Assert(false);

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
            else Debug.Assert(false);

            Debug.Print(Massa.Fonte.FileTXT.memo());

            Debug.Assert(false);

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
