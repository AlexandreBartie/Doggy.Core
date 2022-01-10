using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Text;

namespace Dooggy.Factory
{

    public delegate void Notify();

    public class TestTrace : TestTraceWrite
    {

        public event Notify LogExecutado;


        public TestTraceLog Geral;

        public TestTraceErro Erro;

        public TestTraceLogApp LogApp;

        public TestTraceLogData LogData;

        public TestTraceLogPath LogPath;

        public TestTraceLogFile LogFile;

        public TestTraceLogRobot LogRobot;

        public TestTraceLogConsole LogConsole;

        public TestTraceMsg Corrente => Msg;

        public TestTrace()
        {

            Geral = new TestTraceLog();

            Erro = new TestTraceErro();

            LogApp = new TestTraceLogApp();

            LogData = new TestTraceLogData();

            LogFile = new TestTraceLogFile();

            LogPath = new TestTraceLogPath();

            LogRobot = new TestTraceLogRobot();

            LogConsole = new TestTraceLogConsole();

            Setup(this);

        }

        public void OnLogExecutado()
        {
            LogExecutado?.Invoke();
        }

    }
    public class TestTraceLogApp : TestTraceLog
    {

        public void ExeRunning(string prmNome, string prmVersao) { msgApp(string.Format("-name {0} -version: {1}", prmNome, prmVersao)); }

    }
    public class TestTraceLogData : TestTraceLogData_Fail
    {

        public void DBConnection(string prmTag, string prmStatus) => msgSQL(string.Format("act# -db:[{0}] -status: {1}", prmTag, prmStatus));
        public void SQLExecution(string prmTag, string prmSQL) => msgSQL(string.Format(@"act# -db:[{0}] -sql: {1}", prmTag, prmSQL));

        public void ViewsSelection(string prmTag, int prmQtde)
        {

            if (prmQtde > 0)
                msgData(string.Format(@"act# -view:[{0}] -itens: {1}", prmTag, prmQtde));
            else
                msgErro(string.Format(@"msg# -view[{0}] -desc: View sem dados", prmTag));

        }

    }
    public class TestTraceLogData_Fail : TestTraceLog
    {

        public void FailDBConnection(string prmTag, string prmStringConexao, Exception prmErro) => FailConnection(prmMSG: "Conexão com Banco de Dados falhou", prmVar: "string", prmTag, prmStringConexao, prmErro);
        public void FailSQLConnection(string prmTag, string prmSQL, Exception prmErro) => FailConnection(prmMSG: "Comando SQL falhou", prmVar: "sql", prmTag, prmSQL, prmErro);
        public void FailSQLNoDataBaseConnection(string prmTag, string prmSQL, Exception prmErro) => FailConnection(prmMSG: "Banco de Dados não está aberto. SQL", prmVar: "sql", prmTag, prmSQL, prmErro);

        public void FailSQLDataModelConnection(string prmTag, string prmModel, Exception prmErro) => FailConnection(prmMSG: "Model View não foi criado adequadamente.", prmTag, prmModel, prmErro);

        public void FailNoDataViewDetected(string prmTag) => msgErro(prmTexto: string.Format("Data View não foi identificado ... >>> fluxo: [{0}] não executou o SQL ...", prmTag));

        private void FailConnection(string prmMSG, string prmTag, string prmFluxo, Exception prmErro) => msgErro(String.Format(@"{0} >>> tag:[{1}] {2}:", prmMSG, prmTag, prmFluxo), prmErro);
        private void FailConnection(string prmMSG, string prmVar, string prmTag, string prmSQL, Exception prmErro) => msgErro(String.Format(@"{0} >>> tag:[{2}] {1}: ""{3}""", prmMSG, prmVar, prmTag, prmSQL), prmErro);

    }

    public class TestTraceLogPath : TestTraceLog
    {
        public void SetPath(string prmContexto, string prmPath) => msgPath(String.Format(@"def# {0} -path: {1}", prmContexto, prmPath));

    }
    public class TestTraceLogFile : TestTraceLog
    {

        public void DataFileImport(string prmArquivo, string prmSubPath) => DataFileAction(prmAcao: "READ", prmContexto: "Importado com sucesso", prmArquivo, prmSubPath);
        public void DataFileExport(string prmArquivo, string prmSubPath, string prmEncoding) => DataFileAction(prmAcao: "SAVE", prmContexto: "Salvo com sucesso", prmArquivo, prmSubPath, prmEncoding);
        public void DataFileMute(string prmArquivo, string prmSubPath, string prmEncoding) => DataFileAction(prmAcao: "MUTE", prmContexto: "Silenciado com sucesso", prmArquivo, prmSubPath, prmEncoding);

        private void DataFileAction(string prmAcao, string prmContexto, string prmArquivo, string prmSubPath) => DataFileAction(prmAcao, prmContexto, prmArquivo, prmSubPath, prmEncoding: "");
        private void DataFileAction(string prmAcao, string prmContexto, string prmArquivo, string prmSubPath, string prmEncoding)
        {

            string msg = string.Format(@"-file: '{0}' -msg: {1}", prmArquivo, prmContexto);

            if (xString.IsStringOK(prmEncoding))
                msg += @" -encoding: " + prmEncoding;

            if (xString.IsStringOK(prmSubPath))
                msg += @" -path: ..\" + prmSubPath;

            msgFile(prmAcao, msg);

        }

        public void DataFileFormatTXT(string prmConteudo) => msgFile(prmTipo: "TXT", prmConteudo);
        public void DataFileFormatCSV(string prmConteudo) => msgFile(prmTipo: "CSV", prmConteudo);
        public void DataFileFormatJSON(string prmConteudo) => msgFile(prmTipo: "JSON", prmConteudo);

        public void FailDataFileEncoding(string prmPath, string prmArquivo, string prmEncoding) => msgErro(String.Format("Falha no formato encoding [{2}] ... -file: {0} -path: {1}", prmArquivo, prmPath, prmEncoding));
        public void FailDataFileExport(string prmPath, string prmArquivo) => msgErro(String.Format("Falha na criação do arquivo ... -file: {0} -path: {1}", prmArquivo, prmPath));
        public void FailDataFileOpen(string prmPath, string prmArquivo) => FailDataFileOpenDefault(prmLocal: String.Format("-file: {0} -path: {1}", prmArquivo, prmPath));
        public void FailJSONFormat(string prmContexto, string prmFluxo, Exception prmErro) => msgErro(prmTexto: String.Format(@"Fluxo JSON: [invalid format] ... contexto: {0} fluxo: {1}", prmContexto, prmFluxo));

        private void FailDataFileOpenDefault(string prmLocal) => msgErro(String.Format("Falha na abertura do arquivo ... {0}", prmLocal));

    }
    public class TestTraceLogRobot : TestTraceLog
    {

        public void ActionTag(string prmTag, string prmConteudo) => msgTrace(String.Format("{0,7} <{1}>", prmTag, prmConteudo));

        public void ActionElement(string prmAcao, string prmElemento) => ActionElement(prmAcao, prmElemento, prmValor: null);
        public void ActionElement(string prmAcao, string prmElemento, string prmValor)
        {

            string msg = String.Format("#{0} {1,15}", prmAcao, prmElemento);

            if (prmValor != null)
                msg += " := " + prmValor;

            ActionTag(prmTag: "Comando", msg);

        }

        public void ActionFail(string prmComando, Exception e) => msgErro("ACTION FAIL: ROBOT." + prmComando, e);

        public void ActionFailFormatJSON(string prmComando, Exception e) => msgErro("ACTION FAIL: JSON invalid format." + prmComando, e);

        public void TargetNotFound(string prmTAG) => msgErro("TARGET NOT FOUND: " + prmTAG);

    }
    public class TestTraceLogConsole : TestTraceLog
    {
        
        public void PlayCommand(string prmTipo, string prmKeyWord, string prmTarget) => msgTrace(String.Format("Running {0,10} ... -key: {1} -target: {2}", prmTipo, prmKeyWord, prmTarget));

        public void WriteKeyWord(string prmKeyWord, string prmTarget) => msgCode(String.Format("{0}: {1}", prmKeyWord, prmTarget));

        public void WriteKeyWordArg(string prmArg, string prmParametros) => msgCode(String.Format("  -{0}: {1}", prmArg, prmParametros));

        public void FailFindKeyWord(string prmKeyWord) => msgErro(String.Format("Keyword não encontrada ... -key: {0}", prmKeyWord));

        public void FailActionKeyWord(string prmKeyWord) => msgErro(String.Format("Keyword não executada ... -key: {0}", prmKeyWord));

        public void FailArgNewKeyWord(string prmKeyWord, string prmArg, string prmLinha) => msgErro(String.Format("Argumento-Keyword não suportado ... -key: {0} -arg: {1} -line: [{2}]", prmKeyWord, prmArg, prmLinha));

        public void FailArgMergeKeyWord(string prmKeyWord, string prmLinha) => msgErro(String.Format("Argumento-Keyword foi ignorado ... -key: {0} -line: [{1}]", prmKeyWord, prmLinha));

        public void FailFindValueVariable(string prmVariable, string prmTexto) => msgErro(String.Format("Variável não encontrada ... -var: {0} -txt: {1}", prmVariable, prmTexto));
    }
    public class TestTraceLog : TestTraceErro
    {

        public void msgApp(string prmTrace) => Message(prmTipo: "APP", prmTrace);
        public void msgSession(string prmTrace) => Message(prmTipo: "****", prmTrace);
        public void msgCode(string prmTrace) => Message(prmTipo: "CODE", prmTrace);
        public void msgTrace(string prmTrace) => Message(prmTipo: "TRACE", prmTrace);
        public void msgSQL(string prmMensagem) => Message(prmTipo: "SQL", prmMensagem);
        public void msgData(string prmMensagem) => Message(prmTipo: "DATA", prmMensagem);
        public void msgPath(string prmMensagem) => Message(prmTipo: "PATH", prmMensagem);
        public void msgFile(string prmMensagem) => Message(prmTipo: "FILE", prmMensagem);
        public void msgFile(string prmTipo, string prmMensagem) => Message(prmTipo, prmMensagem);
        public void msgShow(string prmMensagem) => Message(prmTipo: "SHOW", prmMensagem);
        public void msgAviso(string prmAviso) => Message(prmTipo: "AVISO", prmAviso);
        public void msgFalha(string prmAviso) => Message(prmTipo: "FALHA", prmAviso);


    }
    public class TestTraceErro : TestTraceWrite
    {

        public void msgErro(string prmTexto) => Message(prmTipo: "ERRO", prmTexto);
        public void msgErro(Exception e) => Message(prmTipo: "ERRO", e.Message);
        public void msgErro(string prmTexto, Exception e) => Message(prmTipo: "ERRO", String.Format("{0} >>> Error: [{1}]", prmTexto, e.Message));

    }

    public class TestTraceWrite
    {

        protected static TestTrace Trace;

        protected static TestTraceMsg Msg;

        public void Setup(TestTrace prmTrace)
        {

            Trace = prmTrace;

            Msg = new TestTraceMsg();

        }

        protected void Message(string prmTipo, string prmTexto)
        {

            if (Msg.Exibir(prmTipo, prmTexto))
                Trace.OnLogExecutado();

        }

    }
    public class TestTraceMsg
    {

        protected TestItemLog Item;

        public string tipo { get => Item.tipo; }
        public string texto { get => Item.texto; }

        public bool Exibir(string prmTipo, string prmTexto)
        {

            Item = new TestItemLog(prmTipo, prmTexto);

            if (!Item.IsVisivel()) return false;

#if DEBUG

            Debug.WriteLine(Item.msg);

#else

            System.Console.WriteLine(Item.msg);

#endif

            return true;

        }

    }

    public class TestItemLog
    {

        public string tipo;
        public string texto;

        public string msg { get => String.Format("[{0,4}] {1} ", tipo, texto); }

        public bool IsVisivel() => (tipo != "CODE");

        public TestItemLog(string prmTipo, string prmTexto)
        {

            tipo = prmTipo;
            texto = prmTexto;

        }

    }

    public class TestDataLog : List<TestItemLog>
    {

        public bool ativo;

        public void Start() => ativo = true;
        public void Stop() => ativo = false;

        public void AddLog(string prmTipo, string prmTexto)
        {

            if (ativo)
                Add(new TestItemLog(prmTipo, prmTexto));

        }


    }

}
