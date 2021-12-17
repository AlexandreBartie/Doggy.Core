using Dooggy.Factory.Robot;
using Dooggy.POC.AutomacaoTestes;
using Dooggy.POC.MassaTestes;
using System;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace Dooggy
{
    static class Program
    {

        static POC_MassaTestes MassaTestes;

        static POC_AutomacaoTestes AutomacaoTestes;

        static void Main()
        {

            if (true)
                GerarMassaTeste();
            else
                ExecutarAutomacaoTeste();

        }

        static void GerarMassaTeste()
        {

            MassaTestes = new POC_MassaTestes();

            string path = @"c:\MassaTestes\POC\";
            string parametros = @"{ 'branch': '1085', 'port': '1521' }";

            //    Console.WriteLine("Por favor, informe o path + parâmetros do banco de dados (branch, porta) no formato json ...");
            //    Console.WriteLine("Siga o exemplo abaixo ...");

            //    Console.WriteLine(Application.ProductName + " <path_massa_testes> " + @"{ 'branch': '1085', 'port': '1521' }");

            MassaTestes.Setup(parametros,prmNomeApp: Application.ProductName, prmVersaoApp: Application.ProductVersion);

            MassaTestes.Start(path);

        }

        static void ExecutarAutomacaoTeste()
        {

            POC_AutomacaoTestes Automacao = new POC_AutomacaoTestes();

            Automacao.Start(prmTipoDriver: eTipoDriver.ChromeDriver);

    }


    }

}
