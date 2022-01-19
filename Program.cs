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

            string path_ini = @"c:\MassaTestes\POC\Console\INI\";
            string path_out = @"c:\MassaTestes\POC\Console\OUT\GeradorFake\";

            string parametros = @"{ 'branch': '1085', 'port': '1521' }";

            MassaTestesByConsole.SetApp(parametros,prmNomeApp: Application.ProductName, prmVersaoApp: Application.ProductVersion);

            MassaTestesByConsole.Run(path_ini, path_out);

        }

        static void GeracaoMassaScripts()
        {

            MassaTestesByScript = new POC_GeracaoMassaScripts();

            string path_ini = @"c:\MassaTestes\POC\Console\INI\";
            string path_out = @"c:\MassaTestes\POC\Console\OUT\GeradorFake\";

            string parametros = @"{ 'branch': '1085', 'port': '1521' }";

            MassaTestesByScript.SetApp(parametros, prmNomeApp: Application.ProductName, prmVersaoApp: Application.ProductVersion);

            MassaTestesByScript.Setup(path_ini, path_out, prmStart: true);

        }
        static void AutomacaoTestesScripts()
        {

            Automacao = new POC_AutomacaoTestesScripts();

            Automacao.Start(prmTipoDriver: eTipoDriver.ChromeDriver);

        }


    }

}
