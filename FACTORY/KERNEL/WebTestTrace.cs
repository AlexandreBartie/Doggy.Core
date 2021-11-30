using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Text;

namespace Dooggy.Factory
{
    public class TestTrace : TestTraceLog
    {

        public TestDataTrace DataBase = new TestDataTrace();

        public TestRobotTrace Action = new TestRobotTrace();

    }
    public class TestDataTrace : TestTraceLog
    {

        public void StatusConnection(string prmTag, string prmStatus) { LogSQL(string.Format("Banco de Dados {1}: tag[{0}]", prmTag, prmStatus)); }
        public void SQLExecution(string prmTag, string prmSQL) => LogSQL(string.Format(@"SQL executado: tag:[{0}] sql: ""{1}""", prmTag, prmSQL));

        public void SetPath(string prmTitulo, string prmPath) => LogFile(String.Format(@"Path Definido: tag[{0}] path: ""{1}""", prmTitulo, prmPath));
        public void DataFileExport(string prmNome, string prmSubPath, string prmExtensao) => LogFile(String.Format(@"Arquivo {0}.{1} gerado com sucesso. path: ""..\{2}""", prmNome, prmExtensao, prmSubPath));


        public void FailDataConnection(string prmTag, string prmStringConexao, Exception prmErro) => FailConnection(prmMSG: "Conexão com Banco de Dados falhou", prmVar: "string", prmTag, prmStringConexao, prmErro);
        public void FailSQLConnection(string prmTag, string prmSQL, Exception prmErro) => FailConnection(prmMSG: "Comando SQL falhou", prmVar: "sql", prmTag, prmSQL, prmErro);
        public void FailSQLNoDataBaseConnection(string prmTag, string prmSQL, Exception prmErro) => FailConnection(prmMSG: "Banco de Dados não está aberto. SQL", prmVar: "sql", prmTag, prmSQL, prmErro);
        private void FailConnection(string prmMSG, string prmVar, string prmTag, string prmSQL, Exception prmErro) => LogErro(String.Format(@"{0} >>> tag:[{2}] {1}: ""{3}""", prmMSG, prmVar, prmTag, prmSQL), prmErro);

        public void FailDataFileExport(string prmPath, string prmNome, string prmExtensao) => LogErro(String.Format("Criação do arquivo falhou ... file:[{1}.{2}] path:[{0}]",prmPath, prmNome, prmExtensao));

    }
    public class TestRobotTrace : TestTraceLog
    {

        public void ActionArea(string prmArea, string prmName) => LogTrace(String.Format("{0,20}: [{1}]", prmArea, prmName));

        public bool ActionMassaOnLine(bool prmMassaOnLine)
        {

            string texto;

            if (prmMassaOnLine)
                texto = "ON-LINE";
            else
                texto = "OFF-LINE";

            LogTrace(String.Format("Massa de Testes '{0}'", texto));

            return (prmMassaOnLine);

        }

        public void ActionElement(string prmAcao, string prmElemento) => ActionElement(prmAcao, prmElemento, prmValor: null);
        public void ActionElement(string prmAcao, string prmElemento, string prmValor)
        {

            string msg = String.Format("ACTION: {0} {1,15} := {1}", prmAcao, prmElemento);

            if (prmValor != null)
                msg += " := " + prmValor;

            LogTrace(msg);

        }

        public void ActionJSONFail(string prmComando, Exception e) => LogErro("ACTION FAIL: JSON." + prmComando, e);

        public void ActionFail(string prmComando, Exception e) => LogErro("ACTION FAIL: ROBOT." + prmComando, e);

        public void TargetNotFound(string prmTAG) => LogErro("TARGET NOT FOUND: " + prmTAG);

    }
    public class TestTraceLog
    {

        public void LogTrace(string prmTrace) => Message("TRACE", prmTrace);
        public void LogSQL(string prmMensagem) => Message("SQL", prmMensagem);
        public void LogCursor(string prmMensagem) => Message("CURSOR", prmMensagem);
        public void LogFile(string prmMensagem) => Message("FILE", prmMensagem);
        public void LogShow(string prmMensagem) => Message("SHOW", prmMensagem);
        public void LogAviso(string prmAviso) => Message("AVISO", prmAviso);
        public void LogFalha(string prmAviso) => Message("FALHA", prmAviso);
        public void LogErro(string prmErro) => Message("ERRO", prmErro);
        public void LogErro(Exception e) => Message("ERRO", e.Message);
        public void LogErro(string prmErro, Exception e) => Message("ERRO", String.Format("{0} >>> Error: [{1}]", prmErro, e.Message));


        private void Message(string prmTipo, string prmMensagem)
        {

            String texto = String.Format("[{0,5}]: {1} ", prmTipo, prmMensagem);
        
#if DEBUG

            Debug.WriteLine(texto);

#else

            Console.WriteLine(texto);

#endif

        }

    }
}
