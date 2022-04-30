using Katty;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Dooggy
{
    public class TestFactory
    {

        public DataPool Pool;

        public DataSource Dados => Pool.Dados;

        public TestConsole Console;

        public TestConfig Config;

        public TestTrace Trace;

        public TestParameters Parameters;

        public TestFactory()
        {

            Trace = new TestTrace();

            Trace.LogExecutado += TraceLogExecutado;
            Trace.SqlExecutado += TraceSqlExecutado;

            Pool = new DataPool(this);

            Console = new TestConsole(this);

            Config = new TestConfig(this);

            Parameters = new TestParameters();

        }

        public void TraceLogExecutado() => Console.AddLogItem();

        public void TraceSqlExecutado(string prmError) => Console.AddLogSQL(prmError);

        public void EXE(string prmArquivoCFG, bool prmPlay, string prmAppName, string prmAppVersion)
        {
            Trace.LogApp.SetApp(prmAppName, prmAppVersion);

            Console.Setup(prmArquivoCFG, prmPlay);
        }
        public bool Call(Object prmObjeto, string prmMetodo)
        {

            bool vlOk = true; xLista lista; MethodInfo bloco;

            lista = new xLista(prmMetodo, prmSeparador: ",");

            foreach (string metodo in lista)
            {
                try
                {
                    bloco = prmObjeto.GetType().GetMethod(metodo);

                    if (bloco != null)
                    {
                        Trace.LogRobot.msgPlay(prmTrace: String.Format("Bloco Automação acionado. -bloco: {0} -metodo: {1}", bloco.Name, metodo));

                        bloco.Invoke(prmObjeto, null);
                    }
                }

                catch (Exception e)
                {

                    Trace.Geral.msgAviso(string.Format("Método [{0}.{1}] fail. [ -class:{2} -error: {3} ]", prmObjeto.GetType().Name, metodo, prmObjeto.GetType().FullName, e.Message));

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
        // Parâmetros da Automacao
        //

        //public string pathWebDriver;

        //
        // Parâmetros da Massa de Testes
        //
        public bool onlyDATA;

        public Encoding EncodedDataJUNIT;


        //
        // Pausa na Automação (em segundos)
        //
        public int pauseAfterTestCase;

        public int pauseAfterRobotScript;

        public int pauseAfterRobotSuite;

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
