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
    public class TestFactory
    {

        //public myJSON args = new myJSON();

        public TestDataPool Pool;

        public TestDataLocal Dados => Pool.Dados;

        public TestConsole Console;

        public TestConfig Config;

        public TestTrace Trace;

        public TestParameters Parameters;

        public TestFactory()
        {

            Pool = new TestDataPool(this);

            Console = new TestConsole(this);

            Config = new TestConfig(this);

            Parameters = new TestParameters();

            Trace = new TestTrace();

            Trace.LogExecutado += TraceExecutado;

        }

        public void TraceExecutado()
        {

            Console.AddLog();

        }
        public void EXE(string prmArquivoCFG, bool prmPlay, string prmAppName, string prmAppVersion)
        {
            Trace.LogApp.SetApp(prmAppName, prmAppVersion);

            Console.Setup(prmArquivoCFG, prmPlay);
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

        private TestFactory Factory;

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

        public TestConfig(TestFactory prmFactory)
        {
            Factory = prmFactory;
        }

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
