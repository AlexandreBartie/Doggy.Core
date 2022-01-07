using Dooggy.Factory.Data;
using Dooggy.Lib.Generic;
using System;
using System.Collections.Generic;
using System.Text;
using static Dooggy.Factory.TestTraceMsg;

namespace Dooggy.Factory.Console
{
    public class TestConsole
    {

        public TestFactory Factory;

        private TestConsoleImport Import;

        private TestSessions Sessoes;

        public TestDataLocal Dados => Factory.Dados;
        public TestDataPool Pool => Factory.Pool;
        public TestTrace Trace => Factory.Trace;

        private TestConsoleExport Export => Sessoes.Export;

        public string output { get => Export.resultado; }

        public TestConsole(TestFactory prmFactory)
        {

            Factory = prmFactory;

            Import = new TestConsoleImport(this);

            Sessoes = new TestSessions(this);


        }

        public void Setup(string prmPathINI, string prmPathOUT)
        {

            Import.SetPathINI(prmPathINI);

            Pool.SetPathOUT(prmPathOUT);

        }
        public void SetAncora(DateTime prmAncora) => Pool.SetAncora(prmAncora);
        public void Start(string prmPathINI) => Import.Start(prmPathINI);

        public void ImportINI(string prmArquivoINI) => Import.Play(prmArquivoINI);

        public void Play(string prmBloco) => Play(prmBloco, prmArquivoOUT: "");
        public void Play(string prmBloco, string prmArquivoOUT) => Sessoes.Play(prmBloco, prmArquivoOUT);
        public void Save(string prmData) => Sessoes.Save(prmData);

        public void AddLog() => Sessoes.AddLog();
        
        public string GetArquivoOUT()
        {
            
            string nome = Import.ArquivoINI.nome;

            if (xString.IsStringOK(nome))
                return(nome);

            return (Export.nome);

        }

    }

    public class TestSessions : List<TestSession>
    {

        private TestConsole Console;

        public TestSession Corrente;

        public TestTrace Trace => Console.Trace;

        public TestDataPool Pool => Console.Pool;

        public TestConsoleExport Export => Corrente.Export; 

        public TestSessions(TestConsole prmConsole)
        {

            Console = prmConsole;

        }

        public void Play(string prmBloco, string prmArquivoOUT) { Criar(); Corrente.Play(prmBloco, prmArquivoOUT); }

        public void Save(string prmData) { Corrente.Save(prmData); }

        private void Criar()
        {

            Corrente = new TestSession(Console);

            Add(Corrente);

            Pool.Cleanup();

        }

        public void AddLog()
        {

            if (Corrente != null)
                Corrente.AddLog(prmMsg: Trace.Corrente);

        }

    }

    public class TestSession
    {

        public TestConsole Console;

        private TestBuilder Builder;

        public TestConsoleExport Export;

        public TestDataLog Log;

        public string output { get => Export.resultado; }

        public TestSession(TestConsole prmConsole)
        {

            Console = prmConsole;

            Builder = new TestBuilder(this);

            Export = new TestConsoleExport();

            Log = new TestDataLog();

        }

        public void Play(string prmBloco, string prmArquivoOUT)
        {

            Log.Start();

            Export.Setup(prmArquivoOUT);

            Builder.Play(prmBloco);

            Log.Stop();

        }
        
        public void Save(string prmData) => Export.resultado = prmData;

        public void AddLog(TestTraceMsg prmMsg)
        {

            Log.AddLog(prmTipo: prmMsg.tipo, prmTexto: prmMsg.texto);

        }

    }

    public class TestConsoleExport
    {

        public string nome;

        public string resultado;

        public void Setup(string prmArquivoOUT) => nome = prmArquivoOUT;

    }

}
