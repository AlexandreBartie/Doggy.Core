using Dooggy;
using Dooggy.Factory.Console;
using Dooggy.Factory.Data;
using Dooggy.Factory.Robot;
using Dooggy.Lib.Generic;
using Dooggy.Lib.Parse;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Dooggy.Factory
{
    public class TestFactory : ITestDataLocal
    {

        public xJSON args = new xJSON();

        public TestConsole Console;

        public TestDataPool Pool = new TestDataPool();
        
        public TestConfig Config = new TestConfig();

        public TestTrace Trace = new TestTrace();

        public TestParameters Parameters = new TestParameters();

        public TestFactory()
        {

            Dados.Setup(this, Pool);

            Console = new TestConsole(this);

            Trace.LogExecutado += TraceExecutado;

        }

        public void TraceExecutado()
        {

            Console.AddLog();

        }
        public bool SetApp(string prmParametros, string prmNomeApp, string prmVersaoApp)
        {

            Trace.LogApp.ExeRunning(prmNome: Application.ProductName, prmVersao: Application.ProductVersion);

            return(Setup(prmParametros));

        }

        public bool Setup(string prmParametros)
        {

            if (args.Parse(prmParametros))
                return (TestDataBase());

            Trace.LogFile.FailJSONFormat(prmContexto: "Parâmetros do Projeto", prmFluxo: prmParametros, prmErro: args.Erro);

            return (false);

        }

        private bool TestDataBase()
        {



            return (true);

        }

        public bool Call(Object prmObjeto, string prmMetodo)
        {

            bool vlOk = true;

            xLista lista = new xLista();

            lista.Parse(prmMetodo, prmSeparador: ",");

            foreach (string metodo in lista)
            {
                try
                {
                    prmObjeto.GetType().GetMethod(metodo).Invoke(prmObjeto, null);
                }

                catch
                {

                    Trace.Geral.msgAviso(string.Format("Método [{0}.{1}] não encontrado. [ error: {2} ]", prmObjeto.GetType().Name, metodo, prmObjeto.GetType().FullName));

                    vlOk = false;

                }

            }

            return (vlOk);

        }

    }
    public class TestConfig
    {
        //
        // Parâmetros da Massa de Testes
        //


        public bool onlyDATA;

        public Encoding EncodedDataJUNIT;


        //
        // Pausa na Automação (em segundos)
        //

        public int pauseAfterTestCase;

        public int pauseAfterTestRobotScript;

        public int pauseAfterTestRobotSuite;

        //
        // Parâmetros da Arquitetura de Testes
        //

        public string GetProjectBlockCode() => ("BASE, DATA, BUILD, CONFIG");
        public string GetScriptBlockCode() => ("PLAY, CHECK, CLEANUP");
        public string GetAdicaoElementos() => ("+");

        public string GetXPathBuscaRaizElementos() => "//*[@{0}='{1}']";

    }
    public class TestParameters
    {
        public string GetDataFactoryBlockCode() => ("CONFIG, BASE, DATA");
        public string GetRobotFactoryBlockCode() => ("BASE, DATA, BUILD, CONFIG");
        public string GetScriptBlockCode() => ("PLAY, CHECK, CLEANUP");
        public string GetAdicaoElementos() => ("+");
        public string GetXPathBuscaRaizElementos() => "//*[@{0}='{1}']";

    }

}
