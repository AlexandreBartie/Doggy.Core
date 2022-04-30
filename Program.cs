using Dooggy;
using Dooggy.POC.AutomacaoTestes;
using System;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace Dooggy
{
    static class Program
    {

        //static TestDataProject ProjetoMassaDados;

        static void Main()
        {
            
            //
            // Projeto: Massa de Testes
            //

            //ProjetoMassaDados = new TestDataProject();

            //ConsoleScriptsINI(prmGerarMassaTestes: true);

            //
            // Projeto: Automação de Testes
            //

            AutomacaoTeste();
        }

        static void ConsoleScriptsINI(bool prmGerarMassaTestes)
        {
            //string arquivoCFG = @"C:\MassaTestes\QA\CLI\projeto-teste.cfg";

            //ProjetoMassaDados.EXE(arquivoCFG, prmGerarMassaTestes, prmAppName: Application.ProductName, prmAppVersion: Application.ProductVersion);

        }

        static void AutomacaoTeste()
        {

            POC_AutomacaoTestesScripts ProjetoAutomacao = new POC_AutomacaoTestesScripts();

            ProjetoAutomacao.Start(prmTipoDriver: eTipoDriver.ChromeDriver, prmPathDriver: @"C:\DEVOPS\YDUQS\FRAMEWORK\TOOLS\DRIVERS");

        }


    }

}
