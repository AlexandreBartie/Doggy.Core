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

        public TestTraceLogFile LogFile;

        public TestTraceLogRobot LogRobot;

        public TestTraceLogConsole LogConsole;

        public TestTrace()
        {

            LogGeneric = new TestTraceLog();

            LogApp = new TestTraceLogApp();

            LogData = new TestTraceLogData();

            LogFile = new TestTraceLogFile();

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
        public void DBConnection(string prmTag, string prmStatus) => msgSQL(string.Format("Banco de Dados {1}: tag[{0}]", prmTag, prmStatus)); 
        public void SQLExecution(string prmTag, string prmSQL) => msgSQL(string.Format(@"SQL executado: tag:[{0}] sql: ""{1}""", prmTag, prmSQL));

        //
        // Sucess or Warning 
        //
        public void ViewsSelection(string prmTag, int prmQtde)
        {

            if (prmQtde > 0)
                msgData(string.Format(@"View selecionada: tag: [{0}] fluxos: {1}", prmTag, prmQtde));
            else
                msgAviso(string.Format(@"View não encontrada: tag: [{0}]", prmTag));

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

    public class TestTraceLogFile : TestTraceLog
    {

        public void SetPath(string prmContexto, string prmPath) => msgFile(String.Format(@"Path Definido: ""{0}"" tag[{1}]", prmPath, prmContexto));
        public void DataFileExport(string prmNome, string prmSubPath, string prmExtensao) => msgFile(String.Format(@"Arquivo [{0}.{1}] gerado com sucesso. path: ""..\{2}""", prmNome, prmExtensao, prmSubPath));
        public void DataFileFormatTXT(string prmConteudo) => msgFile(prmConteudo, prmTipo: "TXT");
        public void DataFileFormatCSV(string prmConteudo) => msgFile(prmConteudo, prmTipo: "CSV");
        public void DataFileFormatJSON(string prmConteudo) => msgFile(prmConteudo, prmTipo: "JSON");

        public void FailDataFileExport(string prmPath, string prmNome, string prmExtensao) => msgErro(String.Format("Falha na criação do arquivo ... file:[{0}.{1}] path:[{2}]", prmNome, prmExtensao, prmPath));
        public void FailDataFileOpen(string prmPath, string prmNome, string prmExtensao) => msgErro(String.Format("Falha na abertura do arquivo ... file:[{0}.{1}] path:[{2}]", prmNome, prmExtensao, prmPath));
        public void FailJSONFormat(string prmContexto, string prmFluxo, Exception prmErro) => msgErro(prmTexto: String.Format(@"Fluxo JSON: [invalid format] ... contexto: {0} fluxo: {1}", prmContexto, prmFluxo));
  
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

        public void WriteKeyWord(string prmKeyWord, string prmTarget) => msgView(String.Format("{0}:{1}", prmKeyWord, prmTarget));

        public void WriteKeyWordArg(string prmArg, string prmParametros) => msgView(String.Format("  -{0}: {1}", prmArg, prmParametros));

        public void FailFindKeyWord(string prmKeyWord) => msgErro(String.Format("KeyWord não encontrada ... key:[{0}]", prmKeyWord));

        public void FailActionKeyWord(string prmKeyWord) => msgErro(String.Format("KeyWord não executada ... key:[{0}]", prmKeyWord));

        public void FailArgKeyWord(string prmKeyWord, string prmArg) => msgErro(String.Format("Argumento KeyWord não encontrada ... arg:[{0}.{1}]", prmKeyWord, prmArg));

        public void FailMergeKeyWord(string prmKeyWord, string prmLinha) => msgErro(String.Format("Argumento KeyWord não definido ... arg:[{0}.{1}]", prmKeyWord, prmLinha));

    }
    public class TestTraceLog : TestTraceErro
    {

        public void msgStart(string prmTrace) => Message(prmTipo: "START", prmTrace);
        public void msgView(string prmTrace) => Message(prmTipo: "VIEW", prmTrace);
        public void msgPlay(string prmTrace) => Message(prmTipo: "PLAY", prmTrace);
        public void msgTrace(string prmTrace) => Message(prmTipo: "TRACE", prmTrace);
        public void msgSQL(string prmMensagem) => Message(prmTipo: "SQL", prmMensagem);
        public void msgData(string prmMensagem) => Message(prmTipo: "DATA", prmMensagem);
        public void msgFile(string prmMensagem) => msgFile(prmMensagem, prmTipo: "FILE");
        public void msgFile(string prmMensagem, string prmTipo) => Message(prmTipo, prmMensagem);
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

            String texto = String.Format("[{0,5}] {1} ", prmTipo, prmMensagem);

#if DEBUG

            Debug.WriteLine(texto);

#else

            Console.WriteLine(texto);

#endif

        }

    }
}
