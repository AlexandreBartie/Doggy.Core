using Dooggy.Factory.Robot;
using Dooggy.POC.AutomacaoTestes;
using Dooggy.POC.MassaTestes;
using System;
using System.Data.SqlClient;

namespace Dooggy
{
    static class Program
    {
        static void Main()
        {

            GerarMassaTeste();

            //ExecutarAutomacaoTeste();//

        }

        static void GerarMassaTeste()
        {

            POC_MassaTestes POC = new POC_MassaTestes();

            POC.Start(prmPathDataFiles: @"C:\Users\alexa\OneDrive\Área de Trabalho\MassaTeste\");

        }

        static void ExecutarAutomacaoTeste()
        {

            POC_AutomacaoTestes POC = new POC_AutomacaoTestes();

            POC.Start(prmTipoDriver: eTipoDriver.ChromeDriver);

    }


    }

}
