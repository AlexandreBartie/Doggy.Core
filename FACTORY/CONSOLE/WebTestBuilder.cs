using BlueRocket.LIBRARY;
using System;
using System.Collections.Generic;
using System.Text;

namespace BlueRocket.CORE
{

    public enum eTipoTestCommand : int
    {
        eCommandFail = -1,

        eCommandNote = 10,
        eCommandInclude = 11,
        eCommandDB = 12,

        eCommandTag = 21,
        eCommandVar = 22,

        eCommandLoc = 30,
        eCommandRaw = 31,
        eCommandView = 32,
        eCommandFlow = 33,

        eCommandBreak = 50,

        eCommandSave = 60,
    }

    public class TestBuilder
    {

        private TestCode Code;

        private TestSintaxe Sintaxe;

        public TestConsole Console { get => Code.Console; }
        public TestScript Script { get => Code.Script; }
        private TestBlocks Blocks { get => Code.Blocks; }
        public TestResult Result { get => Code.Result; }
        public TestTrace Trace { get => Console.Trace; }

        public TestBuilder(TestCode prmCode)
        {

            Code = prmCode;

            Sintaxe = new TestSintaxe(this);

        }

        public void Compile(string prmCode)
        {
            Result.SetCode(prmCode);

            Merge(prmCode: Console.Config.DB.GetSetupDB());

            Merge(prmCode);

            if (Blocks.IsNeedSaveData)
            {
                Blocks.CriarSAVE();

                Merge(prmCode: Console.Config.CSV.GetSaveDefault());
            }

        }

        private void Merge(string prmCode)
        {

            foreach (string linha in new xLinhas(prmCode))
            {
                if (Sintaxe.IsNewLine(linha))
                {
                    if (Sintaxe.IsNewTag())
                    {


                    }
                    else if (Sintaxe.IsNewCommand())
                        Blocks.AddCommand(Sintaxe);
                    else
                    {
                        if (Blocks.TemCommand)
                            Blocks.AddParameter(Sintaxe);
                        else
                            Trace.LogConsole.IgnoredArgLine(linha);
                    }

                }

                Blocks.AddLine(linha);
            }
        }
    }
    public class TestSintaxe : TestSintaxeCommand
    {
        public TestConsole Console => Builder.Console;
        
        private string linha;

        private string keyword_note = ">>";

        private string keyword_start = ">";
        private string keyword_finish = ":";

        private string option_start = "[";
        private string option_finish = "]";

        private string tag_start = "<";
        private string tag_finish = ">";

        private bool TemKeyword() => (keyword != "");
        private bool TemOption() => (options != "");
        public TestSintaxe(TestBuilder prmBuilder)
        {
            Builder = prmBuilder;
        }

        public bool IsNewLine(string prmLinha)
        {

            linha = prmLinha.Trim();

            Cleanup();

            return (linha != "");

        }

        public bool IsNewTag()
        {
            return (false);
        }
        public bool IsNewCommand()
        {
            if (IsPrefixoNote())
                GetTargetNote();

            else if (IsNewKeyword())
                GetTargetCommand();

            if (TemKeyword())
                { Setup(); return (true); }

            return (false);
        }

        public bool IsNewParametro() => Argumento.IsNewParametro(linha);

        private bool IsPrefixoNote() => (Prefixo.IsPrefixo(linha, prmPrefixo: keyword_note));

        private bool IsPrefixoTag() => (Prefixo.IsPrefixo(linha, prmPrefixo: tag_start, prmDelimitador: tag_finish));

        private bool IsNewKeyword()
        {

            keyword = GetKeywordCommand();

            options = GetOptionsCommand();

            key = GetBaseKeyCommand();

            return (TemKeyword());

        }

        private bool  GetTagBlock()
        {

            tag = Prefixo.GetPrefixo(linha, prmPrefixo: tag_start, prmDelimitador: tag_finish).ToUpper();

            return (tag != "");

        }
        
        private void GetTargetNote()
        {

            key = keyword_note;

            keyword = keyword_note;

            target = Prefixo.GetPrefixoRemove(linha, prmPrefixo: keyword_note, prmTRIM: true);

        }

        private void GetTargetCommand()
        {

            target = Prefixo.GetPrefixoRemove(linha, prmPrefixo: keyword_start, prmDelimitador: keyword_finish, prmTRIM: true);

        }

        private string GetKeywordCommand()
        {

            return Prefixo.GetPrefixo(linha, prmPrefixo: keyword_start, prmDelimitador: keyword_finish).ToLower();

        }
        private string GetOptionsCommand()
        {

            return Bloco.GetBloco(keyword, prmDelimitadorInicial: option_start, prmDelimitadorFinal: option_finish);

        }

        private string GetBaseKeyCommand()
        {

            return Bloco.GetBlocoAntes(keyword, prmDelimitador: option_start, prmTRIM: true);

        }

        public TestSintaxe IClone() => (TestSintaxe)this.MemberwiseClone();

    }

    public class TestSintaxeCommand
    {

        public TestBuilder Builder;

        public TestTrace Trace { get => Builder.Trace; }
        public TestScript Script { get => Builder.Script; }
        

        public string key;

        public string tag;

        public string keyword;

        public string target;

        public string options;

        private string args;

        public eTipoTestCommand tipo;

        public bool IsPlay => GetPlay();

        public bool IsAction => (tipo >= eTipoTestCommand.eCommandBreak);


        public string comando { get => GetComando(); }

        public TestSintaxeArgumento Argumento;

        public TestSintaxeCommand()
        {

            Argumento = new TestSintaxeArgumento();

        }

        protected void Setup()
        {
            switch (key)
            {

                case ">>":
                case "note":
                    tipo = eTipoTestCommand.eCommandNote;
                    break;

                case "db":
                case "setup":
                    tipo = eTipoTestCommand.eCommandDB;
                    args = "null";
                    break;

                case "tag":
                case "tags":
                    tipo = eTipoTestCommand.eCommandTag;
                    args = "null";
                    break;

                //case "var":
                //    tipo = eTipoTestCommand.eCommandVar;
                //    args = "sql";
                //    break;

                case "var":
                case "loc":
                case "local":
                    tipo = eTipoTestCommand.eCommandLoc;
                    args = "sql";
                    break;

                case "raw":
                case "data":
                    tipo = eTipoTestCommand.eCommandRaw;
                    args = "header;null;*";
                    break;

                case "view":
                case "dataview":
                    tipo = eTipoTestCommand.eCommandView;
                    args = "name;tables;links;input;output;alias,mask";
                    break;

                case "item":
                case "dataflow":
                    tipo = eTipoTestCommand.eCommandFlow;
                    args = "enter;check;sql;filter;order;mask;index";
                    break;

                case "save":
                case "datasave":
                    tipo = eTipoTestCommand.eCommandSave;
                    args = "";//args = "encode;extensao";
                    break;

                case "break":
                    tipo = eTipoTestCommand.eCommandBreak;
                    args = "";//args = "encode;extensao";
                    break;

                default:
                    tipo = eTipoTestCommand.eCommandFail;
                    Trace.LogConsole.FailFindKeyWord(keyword);

                    return;
            }

            Argumento.Setup(args);

            Trace.LogConsole.WriteKeyWord(keyword, target);

        }

        private string GetComando()
        {

            switch (tipo)
            {

                case eTipoTestCommand.eCommandFail:
                    return "fail";

                case eTipoTestCommand.eCommandBreak:
                    return "break";

                case eTipoTestCommand.eCommandNote:
                    return "note";

                case eTipoTestCommand.eCommandDB:
                    return "dbase";

                case eTipoTestCommand.eCommandTag:
                    return "tag";

                case eTipoTestCommand.eCommandVar:
                    return "var";

                case eTipoTestCommand.eCommandLoc:
                    return "loc";

                case eTipoTestCommand.eCommandRaw:
                    return "data";

                case eTipoTestCommand.eCommandView:
                    return "view";

                case eTipoTestCommand.eCommandFlow:
                    return "flow";

                case eTipoTestCommand.eCommandSave:
                    return "save";

            }

            return("null");

        }
        protected void Cleanup()
        {

            key = "";
            keyword = "";
            target = "";
            options = "";

        }

        private bool GetPlay()
        {
            if (Script.IsBreak)
                return (IsAction);

            return (true);
        }

    }

    public class TestSintaxeArgumento
    {

        public string key;

        public string parametro;

        public string linha;

        private xLista lista;

        private string arg_start = "-";
        private string arg_finish = ":";

        private string arg_default => arg_start + arg_finish;

        private bool IsArg() => (key != "");
        public bool IsOk() => GetOK();

        public void Setup(string prmArgs)
        {

            lista = new xLista(prmArgs);

        }
        public bool IsNewParametro(string prmLinha)
        {

            linha = prmLinha.Trim();

            if (GetArgCommand())
            {

                GetArgDescription();

                return true;

            }

            parametro = linha;

            return false;

        }

        private bool GetArgCommand()
        {

            parametro = "";

            if (linha.StartsWith(arg_default))
                key = "null";
            else
                key = Bloco.GetBloco(linha, prmDelimitadorInicial: arg_start, prmDelimitadorFinal: arg_finish);

            return (IsArg());

        }

        private void GetArgDescription()
        {

            parametro = myString.GetLast(linha, prmDelimitador: ":").Trim();

        }

        private bool GetOK()
        {
            if (lista != null)
                return lista.IsContem(key);
            
            return false;
        }


    }

}
