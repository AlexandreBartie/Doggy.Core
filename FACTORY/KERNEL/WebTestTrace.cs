using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Text;

namespace Dooggy.Factory
{
    public class TestTrace : TestTraceErro
    {

        public TestTraceLog LogGeneric;

        public TestTraceLogApp LogApp;

        public TestTraceLogData LogData;

        public TestTraceLogPath LogPath;

        public TestTraceLogFile LogFile;

        public TestTraceLogRobot LogRobot;

        public TestTraceLogConsole LogConsole;

        public TestTrace()
        {

            LogGeneric = new TestTraceLog();

            LogApp = new TestTraceLogApp();

            LogData = new TestTraceLogData();

            LogFile = new TestTraceLogFile();

            LogPath = new TestTraceLogPath();

            LogRobot = new TestTraceLogRobot();

            LogConsole = new TestTraceLogConsole();

        }

    }
    public class TestTraceLogApp: TestTraceLog
    {
        
        public void ExeRunning(string prmNome, string prmVersao) { msgStart(string.Format("App: {0} - Versão: {1}", prmNome, prmVersao)); }

    }
    public class TestTraceLogData : TestTraceLog
    {

        //
        // Sucess
        //
        public void DBConnection(string prmTag, string prmStatus) => msgSQL(string.Format("act# -db:[{0}] -status: {1}", prmTag, prmStatus)); 
        public void SQLExecution(string prmTag, string prmSQL) => msgSQL(string.Format(@"act# -db:[{0}] -sql: {1}", prmTag, prmSQL));

        //
        // Sucess or Warning 
        //
        public void ViewsSelection(string prmTag, int prmQtde)
        {

            if (prmQtde > 0)
                msgData(string.Format(@"act# -view:[{0}] -itens: {1}", prmTag, prmQtde));
            else
                msgErro(string.Format(@"msg# -view[{0}] -desc: View sem dados", prmTag));

        }
        //
        // Fails
        //
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
        public void DataFileImport(string prmArquivo, string prmSubPath) => DataFileAction(prmAcao: "READ", prmContexto: "Importado com sucesso" , prmArquivo, prmSubPath);
        public void DataFileExport(string prmArquivo, string prmSubPath, string prmEncoding) => DataFileAction(prmAcao: "SAVE", prmContexto: "Salvo com sucesso", prmArquivo, prmSubPath, prmEncoding);
        public void DataFileMute(string prmArquivo, string prmSubPath, string prmEncoding) => DataFileAction(prmAcao: "MUTE", prmContexto: "Silenciado com sucesso", prmArquivo, prmSubPath, prmEncoding);

        private void DataFileAction(string prmAcao, string prmContexto, string prmArquivo, string prmSubPath) => DataFileAction(prmAcao, prmContexto, prmArquivo, prmSubPath, prmEncoding: "");
        private void DataFileAction(string prmAcao, string prmContexto, string prmArquivo,  string prmSubPath, string prmEncoding)
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

        public void WriteKeyWord(string prmKeyWord, string prmTarget) => msgCode(String.Format("{0}: {1}", prmKeyWord, prmTarget));

        public void WriteKeyWordArg(string prmArg, string prmParametros) => msgCode(String.Format("  -{0}: {1}", prmArg, prmParametros));

        public void FailFindKeyWord(string prmKeyWord) => msgErro(String.Format("KeyWord não encontrada ... -key: {0}", prmKeyWord));

        public void FailActionKeyWord(string prmKeyWord) => msgErro(String.Format("KeyWord não executada ... -key: {0}", prmKeyWord));

        public void FailArgKeyWord(string prmKeyWord, string prmArg) => msgErro(String.Format("Argumento KeyWord não encontrada ... -arg:[{0}.{1}]", prmKeyWord, prmArg));

        public void FailMergeKeyWord(string prmKeyWord, string prmLinha) => msgErro(String.Format("Argumento KeyWord não definido ... -arg:[{0}.{1}]", prmKeyWord, prmLinha));

        public void FailFindValueVariableSQL(string prmVariable, string prmSql) => msgErro(String.Format("Variável não possui valor ... -var:{0} -sql:{1}", prmVariable, prmSql));
    }
    public class TestTraceLog : TestTraceErro
    {

        public void msgStart(string prmTrace) => Message(prmTipo: "START", prmTrace);
        public void msgCode(string prmTrace) => Message(prmTipo: "CODE", prmTrace);
        public void msgPlay(string prmTrace) => Message(prmTipo: "PLAY", prmTrace);
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

    public class TestTraceErro : TestTraceMsg
    {

        public void msgErro(string prmTexto) => Message(prmTipo: "ERRO", prmTexto);
        public void msgErro(Exception e) => Message(prmTipo: "ERRO", e.Message);
        public void msgErro(string prmTexto, Exception e) => Message(prmTipo: "ERRO", String.Format("{0} >>> Error: [{1}]", prmTexto, e.Message));

    }

    public class TestTraceMsg
    {

        public void Message(string prmTipo, string prmMensagem)
        {

            if (prmTipo == "CODE") return;
            
            String texto = String.Format("[{0,4}] {1} ", prmTipo, prmMensagem);

#if DEBUG

            Debug.WriteLine(texto);

#else

            System.Console.WriteLine(texto);

#endif

        }

    }
}
