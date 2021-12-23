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

        static POC_AutomacaoTestesScripts Automacao;

        static POC_GeracaoMassaConsole MassaTestesByConsole;
        static POC_GeracaoMassaScripts MassaTestesByScript;

        static bool IsGerarMassaTestes = true;
        static bool IsGerarMassaByConsole = true;

        static void Main()
        {

            if (IsGerarMassaByConsole)
                GeracaoMassaConsole();

            else if (IsGerarMassaTestes)
                GeracaoMassaScripts();

            else
                AutomacaoTestesScripts();

        }

        static void GeracaoMassaConsole()
        {

            MassaTestesByConsole = new POC_GeracaoMassaConsole();

            string path_in = @"c:\MassaTestes\POC\Console\IN";
            string path_out = @"c:\MassaTestes\POC\Console\OUT";

            string parametros = @"{ 'branch': '1085', 'port': '1521' }";

            //    Console.WriteLine("Por favor, informe o path + parâmetros do banco de dados (branch, porta) no formato json ...");
            //    Console.WriteLine("Siga o exemplo abaixo ...");

            //    Console.WriteLine(Application.ProductName + " <path_massa_testes> " + @"{ 'branch': '1085', 'port': '1521' }");

            MassaTestesByConsole.Setup(parametros,prmNomeApp: Application.ProductName, prmVersaoApp: Application.ProductVersion);

            MassaTestesByConsole.Start(path_in, path_out);

        }

        static void GeracaoMassaScripts()
        {

            MassaTestesByScript = new POC_GeracaoMassaScripts();

            string path_in = @"c:\MassaTestes\POC\Scripts\IN";
            string parametros = @"{ 'branch': '1085', 'port': '1521' }";

            //    Console.WriteLine("Por favor, informe o path + parâmetros do banco de dados (branch, porta) no formato json ...");
            //    Console.WriteLine("Siga o exemplo abaixo ...");

            //    Console.WriteLine(Application.ProductName + " <path_massa_testes> " + @"{ 'branch': '1085', 'port': '1521' }");

            MassaTestesByScript.Setup(parametros, prmNomeApp: Application.ProductName, prmVersaoApp: Application.ProductVersion);

            MassaTestesByScript.Start(path_in);

        }
        static void AutomacaoTestesScripts()
        {

            Automacao = new POC_AutomacaoTestesScripts();

            Automacao.Start(prmTipoDriver: eTipoDriver.ChromeDriver);

        }


    }

}
