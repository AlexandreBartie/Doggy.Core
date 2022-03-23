using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Text;
using Dooggy.LIBRARY;

namespace Dooggy.CORE
{

    public class TestTrace : TraceLog
    {

        public TestTraceLogRobot LogRobot;

        public TestTraceLogConfig LogConfig;

        public TestTraceLogConsole LogConsole;

        public TestTrace()
        {
            LogRobot = new TestTraceLogRobot();

            LogConfig = new TestTraceLogConfig();

            LogConsole = new TestTraceLogConsole();

            Setup(this);
        }
 
    }

    public class TestTraceLogRobot : TraceTipo
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
    public class TestTraceLogConfig : TraceTipo
    {
        public void LoadCFG(FileTXT prmFile) => msgCFG(String.Format("Arquivo CFG carregado ... -file: {0} -path: {1}", prmFile.nome, prmFile.path));

        public void FailLoadCFG(FileTXT prmFile, string prmStatus) => msgErro(String.Format("Parâmetros incompletos no arquivo CFG ... -status: {2} -file: {0} -path: {1}", prmFile.nome, prmFile.path, prmStatus));

        public void FailFindGroupCFG(string prmGroup) => msgErro(String.Format("Grupo do arquivo CFG não encontrado ... -grp: {0}", prmGroup));

        public void FailFindParameterCFG(string prmParameter, string prmValor) => msgErro(String.Format("Parâmetro do arquivo CFG não encontrado ... -item: {0} -valor: {1}", prmParameter, prmValor));


    }
    public class TestTraceLogConsole : TestTraceLogConsole_Fail
    {

        public void SetScript(string prmScript) => msgDef(String.Format("Script Selecionado ... -ini: {0}", prmScript));

        public void PlayCommand(string prmTipo, string prmKeyWord, string prmTarget) => msgPlay(String.Format("Running {0,10} ... -key: {1} -target: {2}", prmTipo, prmKeyWord, prmTarget));

        public void WriteKeyWord(string prmKeyWord, string prmTarget) => msgCode(String.Format("{0}: {1}", prmKeyWord, prmTarget));

        public void WriteKeyWordArg(string prmArg, string prmParametros) => msgCode(String.Format("  -{0}: {1}", prmArg, prmParametros));

        public void SetValueVariable(string prmVariable, string prmValue) => msgDef(String.Format("Variável modificada ... -var: {0} = {1}", prmVariable, prmValue));

    }
    public class TestTraceLogConsole_Fail : TraceTipo
    {

        public void FailFindKeyWord(string prmKeyWord) => msgErro(String.Format("Keyword não encontrada ... -key: {0}", prmKeyWord));
        public void FailActionKeyWord(string prmKeyWord) => msgErro(String.Format("Keyword não executada ... -key: {0}", prmKeyWord));
        public void FailArgNewKeyWord(string prmKeyWord, string prmArg, string prmLinha) => msgErro(String.Format("Argumento-Keyword não suportado ... -arg: {0}.{1} -line: [{2}]", prmKeyWord, prmArg, prmLinha));
        public void FailArgMergeKeyWord(string prmKeyWord, string prmLinha) => msgErro(String.Format("Argumento-Keyword foi ignorado ... -key: {0} -line: [{1}]", prmKeyWord, prmLinha));

        public void FailEnterVariable(myTupla prmTupla) => msgErro(String.Format("DataEnter não encontrado em campos INPUT ... -enter: {0} = {1}", prmTupla.var_sql, prmTupla.value_sql));
        public void FailCheckVariable(myTupla prmTupla) => msgErro(String.Format("DataCheck não encontrado em campos OUTPUT... -check: {0} = {1}", prmTupla.var_sql, prmTupla.value_sql));

        public void FailFindTag(string prmTag, string prmCommand) => msgErro(String.Format("Tag não encontrada ... -tag: {0} -cmd: {1}", prmTag, prmCommand));
        public void FailFindTagElement(string prmTag, string prmValue) => msgErro(String.Format("Elemento Tag não encontrada ... -tag: {0} -value: {1}", prmTag, prmValue));
        public void FailScriptTag(string prmTag, string prmValue) => msgErro(String.Format("Script TAG não atualizada ... -tag: {0} -value: {1}", prmTag, prmValue));

        public void FailFindVariable(string prmVariable, string prmCommand) => msgErro(String.Format("Variável não encontrada ... -var: {0} -cmd: {1}", prmVariable, prmCommand));
        public void FailFindFunction(string prmFunction, string prmCommand) => msgErro(String.Format("Função não encontrada ... -fnc: {0} -cmd: {1}", prmFunction, prmCommand));

        public void IgnoredArgLine(string prmArg) => msgErro(String.Format("Nenhum comando encontrado antes do argumento ... {0}", prmArg));

    }

}
