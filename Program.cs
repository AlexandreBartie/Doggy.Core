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


            GerarMassaTeste(prmPath: @"c:\MassaTestes\");

            //ExecutarAutomacaoTeste();//

        }

        static void GerarMassaTeste(string prmPath)
        {

            POC_MassaTestes POC = new POC_MassaTestes();

            POC.Start(prmPath,  prmParametros: @"{ 'path': 'c:\MassaTestes\', 'branch': '1084', 'porta': '1521' }");

        }

        static void ExecutarAutomacaoTeste()
        {

            POC_AutomacaoTestes POC = new POC_AutomacaoTestes();

            POC.Start(prmTipoDriver: eTipoDriver.ChromeDriver);

    }


    }

}
