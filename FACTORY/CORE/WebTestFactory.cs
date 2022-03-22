using BlueRocket.KERNEL;
using BlueRocket.LIBRARY;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace BlueRocket.KERNEL
{
    public class TestFactory
    {

        public TestDataPool Pool;

        public TestDataSource Dados => Pool.Dados;

        public TestConsole Console;

        public TestConfig Config;

        public TestTrace Trace;

        public TestParameters Parameters;

        public TestFactory()
        {

            Trace = new TestTrace();

            Trace.LogExecutado += TraceLogExecutado;
            Trace.SqlExecutado += TraceSqlExecutado;

            Pool = new TestDataPool(this);

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
