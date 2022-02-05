using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Text;
using Dooggy.Lib.Generic;
using Dooggy.Factory.Console;

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

        public TestTraceLogConfig LogConfig;

        public TestTraceLogConsole LogConsole;

        public TestTraceMsg Msg;

        public TestTrace()
        {

            Geral = new TestTraceLog();

            Erro = new TestTraceErro();

            LogApp = new TestTraceLogApp();

            LogData = new TestTraceLogData();

            LogFile = new TestTraceLogFile();

            LogPath = new TestTraceLogPath();

            LogRobot = new TestTraceLogRobot();

            LogConfig = new TestTraceLogConfig();

            LogConsole = new TestTraceLogConsole();

            Msg = new TestTraceMsg();

            Setup(this);

        }

        public void OnLogExecutado()
        {
            LogExecutado?.Invoke();
        }

        public bool Exibir(string prmTipo, string prmTexto) => Msg.Exibir(prmTipo, prmTexto);

    }
    public class TestTraceLogApp : TestTraceLog
    {

        public void SetApp(string prmAppName, string prmAppVersion) { msgApp(string.Format("-name {0} -version: {1}", prmAppName, prmAppVersion)); }

    }
    public class TestTraceLogData : TestTraceLogData_Fail
    {

        public void DBConnection(string prmTag, string prmStatus) => msgData(string.Format("act# -db:[{0}] -status: {1}", prmTag, prmStatus));
        public void SQLExecution(string prmTag, string prmSQL, bool prmTemDados) => GetSQLExecution(prmMsg: string.Format(@"act# -db:[{0}] -sql: {1}", prmTag, prmSQL), prmTemDados);
        public void SQLNoCommand() => msgSQL("act# -null -msg: Nenhum comando SQL foi encontrado para esse item.");

        public void SQLViewsSelection(string prmTag, int prmQtde)
        {
            if (prmQtde > 0)
                msgSet(string.Format(@"-view:[{0}] -itens: {1}", prmTag, prmQtde));
            else
                msgErro(string.Format(@"msg# -view[{0}] -desc: View sem dados", prmTag));
        }

        private void GetSQLExecution(string prmMsg, bool prmTemDados)
        {
            if (prmTemDados)
                msgSQL(prmMsg);
            else
                msgErro(prmMsg, prmErro: "ZERO Results");
        }

    }
    public class TestTraceLogData_Fail : TestTraceLog
    {

        public void FailDBBlocked(string prmTag, string prmConexao) => FailConnection(prmMSG: "Conexão bloqueada", prmTag, prmVar: "-string", prmConexao, prmErro: @"APENAS para testes unitários");
        public void FailDBConnection(string prmTag, string prmConexao, Exception prmErro) => FailConnection(prmMSG: "Conexão falhou", prmTag, prmVar: "-string", prmConexao, prmErro);
        public void FailSQLConnection(string prmTag, string prmSQL, Exception prmErro) => FailConnection(prmMSG: "SQL falhou", prmTag, prmVar: "-sql", prmSQL, prmErro);
        public void FailSQLNoDataBaseConnection(string prmTag, string prmSQL, Exception prmErro) => FailConnection(prmMSG: "DB Desconectado", prmTag, prmVar: "-sql", prmSQL, prmErro);
        public void FailNoDataViewDetected(string prmTag) => msgErro(prmTexto: string.Format("Data View não identificada ... >>> fluxo: [{0}] não executou o SQL ...", prmTag));

        private void FailConnection(string prmMSG, string prmTag, string prmVar, Exception prmErro) => FailConnection(prmMSG, prmTag, prmVar, GetMsgErro(prmErro));
        private void FailConnection(string prmMSG, string prmTag, string prmVar, string prmErro) => msgErro(String.Format(@"{0} >>> tag:[{1}] {2}", prmMSG, prmTag, prmVar), prmErro);

        private void FailConnection(string prmMSG, string prmTag, string prmVar, string prmSQL, Exception prmErro) => FailConnection(prmMSG, prmTag, prmVar, prmSQL, GetMsgErro(prmErro));
        private void FailConnection(string prmMSG, string prmTag, string prmVar, string prmSQL, string prmErro) => msgErro(String.Format(@"{0} ... -error: [{1}] -db:[{2}] {3}: {4} ", prmMSG, prmErro, prmTag, prmVar, prmSQL));

        private string GetMsgErro(Exception prmErro) { if (prmErro != null) return (prmErro.Message); return (""); }

    }

    public class TestTraceLogPath : TestTraceLog
    {
        public void SetPath(string prmContexto, string prmPath) => msgSet(String.Format(@"{0,15} -path: {1}", prmContexto, prmPath));

    }
    public class TestTraceLogFile : TestTraceLog
    {

        public void DataFileOpen(string prmArquivo, string prmPath) => DataFileAction(prmAcao: "READ", prmContexto: "Importado com sucesso", prmArquivo, prmPath);

        public void DataFileSave(string prmArquivo, string prmPath) => DataFileAction(prmAcao: "CODE", prmContexto: "Script salvo com sucesso", prmArquivo, prmPath, prmEncoding: "default");
        public void DataFileSave(string prmArquivo, string prmPath, string prmEncoding) => DataFileAction(prmAcao: "SAVE", prmContexto: "Salvo com sucesso", prmArquivo, prmPath, prmEncoding);
        public void DataFileMute(string prmArquivo, string prmPath, string prmEncoding) => DataFileAction(prmAcao: "MUTE", prmContexto: "Silenciado com sucesso", prmArquivo, prmPath, prmEncoding);

        private void DataFileAction(string prmAcao, string prmContexto, string prmArquivo, string prmPath) => DataFileAction(prmAcao, prmContexto, prmArquivo, prmPath, prmEncoding: "");
        private void DataFileAction(string prmAcao, string prmContexto, string prmArquivo, string prmPath, string prmEncoding)
        {

            string txt;

            if (prmAcao == "MUTE")
                txt = "save";
            else
                txt = prmAcao.ToLower();

            //

            string msg = string.Format("act# -{0}: {1}.", txt, prmContexto);

            if (xString.IsFull(prmEncoding))
                msg += @" -encoding: " + prmEncoding;
            
            if (xString.GetFirst(prmArquivo, prmDelimitador: ".") != "")
            {
                msg += @" -file: " + prmArquivo;

            }


            msgFile(prmAcao, msg);

        }

        public void DataFileFormatTXT(string prmConteudo) => msgFile(prmTipo: "TXT", prmConteudo);
        public void DataFileFormatCSV(string prmConteudo) => msgFile(prmTipo: "CSV", prmConteudo);
        public void DataFileFormatJSON(string prmConteudo) => msgFile(prmTipo: "JSON", prmConteudo);

        public void FailDataFileEncoding(string prmEncoding) => msgErro(String.Format("Formato encoding [{0}] não encontrado ...", prmEncoding));
        public void FailDataFileSave(string prmArquivo, string prmPath) => msgErro(String.Format("Falha na criação do arquivo ... -file: {0} -path: {1}", prmArquivo, prmPath));
        public void FailDataFileOpen(string prmArquivo, string prmPath) => FailDataFileOpenDefault(prmLocal: String.Format("-file: {0} -path: {1}", prmArquivo, prmPath));
        public void FailJSONFormat(string prmContexto, string prmFluxo, Exception prmErro) => msgErro(prmTexto: String.Format(@"Fluxo JSON: [invalid format] ... contexto: {0} fluxo: {1}", prmContexto, prmFluxo));

        private void FailDataFileOpenDefault(string prmLocal) => msgErro(String.Format("Falha na abertura do arquivo ... {0}", prmLocal));

    }
    public class TestTraceLogRobot : TestTraceLog
    {

        public void ActionTag(string prmTag, string prmConteudo) => msgPlay(String.Format("{0,7} <{1}>", prmTag, prmConteudo));

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
    public class TestTraceLogConfig : TestTraceLog
    {
        public void LoadConfig(string prmArquivoCFG) => msgSet(String.Format("Arquivo CFG carregado ... -file: {0}", prmArquivoCFG));

        public void FailLoadConfig(string prmArquivoCFG, string prmStatus) => msgSet(String.Format("Parâmetros incompletos no arquivo CFG ... -file: {0} -status: {1}", prmArquivoCFG, prmStatus));

        public void FailFindGroup(string prmGroup) => msgErro(String.Format("Grupo do arquivo CFG não encontrado ... -grp: {0}", prmGroup));

        public void FailFindParameter(string prmParameter, string prmValor) => msgErro(String.Format("Parâmetro do arquivo CFG não encontrado ... -item: {0} -valor: {1}", prmParameter, prmValor));


    }
    public class TestTraceLogConsole : TestTraceLogConsole_Fail
    {

        public void SetScript(string prmScript) => msgSet(String.Format("Script Selecionado ... -ini: {0}", prmScript));

        public void PlayCommand(string prmTipo, string prmKeyWord, string prmTarget) => msgPlay(String.Format("Running {0,10} ... -key: {1} -target: {2}", prmTipo, prmKeyWord, prmTarget));

        public void WriteKeyWord(string prmKeyWord, string prmTarget) => msgCode(String.Format("{0}: {1}", prmKeyWord, prmTarget));

        public void WriteKeyWordArg(string prmArg, string prmParametros) => msgCode(String.Format("  -{0}: {1}", prmArg, prmParametros));

    }
    public class TestTraceLogConsole_Fail : TestTraceLog
    {

        public void FailFindKeyWord(string prmKeyWord) => msgErro(String.Format("Keyword não encontrada ... -key: {0}", prmKeyWord));
        public void FailActionKeyWord(string prmKeyWord) => msgErro(String.Format("Keyword não executada ... -key: {0}", prmKeyWord));
        public void FailArgNewKeyWord(string prmKeyWord, string prmArg, string prmLinha) => msgErro(String.Format("Argumento-Keyword não suportado ... -arg: {0}.{1} -line: [{2}]", prmKeyWord, prmArg, prmLinha));
        public void FailArgMergeKeyWord(string prmKeyWord, string prmLinha) => msgErro(String.Format("Argumento-Keyword foi ignorado ... -key: {0} -line: [{1}]", prmKeyWord, prmLinha));
        public void FailFindValueVariable(string prmVariable, string prmTexto) => msgErro(String.Format("Variável não encontrada ... -var: {0} -txt: {1}", prmVariable, prmTexto));

        public void IgnoredArgLine(string prmArg) => msgErro(String.Format("Nenhum comando encontrado antes do argumento ... {0}", prmArg));

    }

    public class TestTraceLog : TestTraceErro
    {

        public void msgApp(string prmTrace) => Message(prmTipo: "APP", prmTrace);
        public void msgCode(string prmTrace) => Message(prmTipo: "CODE", prmTrace);
        public void msgSet(string prmTrace) => Message(prmTipo: "SET", prmTrace, prmPrefixo: "def" );
        public void msgPlay(string prmTrace) => Message(prmTipo: "PLAY", prmTrace);

        public void msgSQL(string prmMensagem) => Message(prmTipo: "SQL", prmMensagem);
        public void msgData(string prmMensagem) => Message(prmTipo: "DAT", prmMensagem);
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

        public void msgErro(string prmTexto, Exception e) => msgErro(prmTexto, prmErro: e.Message);
        public void msgErro(string prmTexto, string prmErro) => Message(prmTipo: "ERRO", String.Format(">>>> [{0}] {1}", prmErro, prmTexto));
    }

    public class TestTraceWrite
    {

        protected static TestTrace Trace;

        public void Setup(TestTrace prmTrace)
        {

            Trace = prmTrace;

        }

        protected void Message(string prmTipo, string prmTexto) => Message(prmTipo, prmTexto, prmPrefixo: "");
        protected void Message(string prmTipo, string prmTexto, string prmPrefixo)
        {
            string texto = prmTexto;

            if (prmPrefixo != "")
                texto = prmPrefixo + "# " + texto;

            if (Trace.Exibir(prmTipo, texto))
                Trace.OnLogExecutado();
        }

    }
    public class TestTraceMsg : TestItemLog
    {

        public bool Exibir(string prmTipo, string prmTexto)
        {

            tipo = prmTipo;
            texto = prmTexto;

            if (!IsVisivel) return false;

#if DEBUG

            Debug.WriteLine(msg);

#else

            System.Console.WriteLine(msg);

#endif

            return true;

        }

    }

    public class TestItemLog
    {

        public string tipo;
        public string texto;

        public string msg => String.Format("[{0,4}] {1} ", tipo, texto);

        public bool IsVisivel => ((tipo != "CODE") && (tipo != "PLAY"));

        public TestItemLog() { }

        public TestItemLog(string prmTipo, string prmTexto)
        {

            tipo = prmTipo;

            texto = prmTexto;

        }

    }

}
