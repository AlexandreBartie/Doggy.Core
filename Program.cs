using Dooggy.Factory;
using Dooggy.Factory.Robot;
using Dooggy.POC.AutomacaoTestes;
using System;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace Dooggy
{
    static class Program
    {

        static TestDataProject ProjetoMassaDados;

        static void Main()
        {

            ProjetoMassaDados = new TestDataProject();

            ConsoleScriptsINI(prmGerarMassaTestes: true);

        }

        static void ConsoleScriptsINI(bool prmGerarMassaTestes)
        {
            string arquivoCFG = @"C:\MassaTestes\POC\CLI\projeto-teste.cfg";

            ProjetoMassaDados.EXE(arquivoCFG, prmGerarMassaTestes, prmAppName: Application.ProductName, prmAppVersion: Application.ProductVersion);

        }

    }

}
