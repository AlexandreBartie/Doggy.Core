using Dooggy.LIBRARY;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dooggy.CORE
{

    public class TestCode
    {

        public TestScript Script;

        public TestBuilder Builder;

        public TestBlocks Blocks;

        public string key;

        public TestConsole Console { get => Script.Console; }
        public TestResult Result { get => Script.Result; }
        public TestTrace Trace { get => Script.Trace; }
        public bool DoConnect() => Console.Dados.DoConnect();

        public TestCode(TestScript prmScript)
        {
            Script = prmScript;

            Blocks = new TestBlocks(this);

            Builder = new TestBuilder(this);
        }

        public void Load(string prmCode, string prmArquivoINI, bool prmPlay)
        {
            Result.Setup(prmCode, prmArquivoINI);

            Play(prmCode, prmArquivoINI, prmPlay); 
        }

        public void Play(string prmCode, string prmArquivoOUT) => Play(prmCode, prmArquivoOUT, prmPlay: true);
        public void Play(string prmCode, string prmArquivoOUT, bool prmPlay)
        {
            Blocks.Start(prmArquivoOUT);

            Blocks.Builder(prmCode);

            if (prmPlay)
                Blocks.Play();

             Blocks.Stop();
        }

    }
    public class TestBlocks : List<TestBlock>
    {

        private TestCode Code;

        public TestBlock Corrente;

        private TestConsole Console => Code.Console;
        private TestScript Script => Code.Script;
        private TestResult Result => Code.Result;

        public bool TemCommand => Corrente.TemCommand;
        public bool IsNeedSaveData => IsDataDetected && !IsSaveDetected;
        private bool IsDataDetected => GetDataDetected();
        private bool IsSaveDetected => GetSaveDetected();

        public TestBlocks(TestCode prmCode)
        {
            Code = prmCode;
        }

        public void Start(string prmArquivoOUT)
        {
            Result.LogStart(prmArquivoOUT);

            foreach (TestBlock Block in this)
                Block.Clear();

            Clear(); CriarROOT();
        }

        public void Builder(string prmCode) => Code.Builder.Compile(prmCode);

        public void Play()
        {
            if (Code.DoConnect())
            {
                foreach (TestBlock Block in this)
                    Block.Play();
            }
        }

        public void Stop()
        {
            Result.LogStop();
        }

        public void CriarROOT() => AddBlock(prmKey: "ROOT");
        public void CriarRAW() => AddBlock(prmKey: "RAW");
        public void CriarSAVE() => AddBlock(prmKey: "SAVE");

        public void AddLine(string prmLine) => Corrente.AddLine(prmLine);
        public void AddCommand(TestSintaxe prmSintaxe) => Corrente.AddCommand(new TestCommand(prmSintaxe));
        public void AddParameter(TestSintaxe prmSintaxe) => Corrente.AddParameter(prmSintaxe);

        private void AddBlock(string prmKey) => AddBlock(prmKey, prmInterno: false);
        private void AddBlock(string prmKey, bool prmInterno)
        {
            if (!Find(prmKey))
            {
                Corrente = new TestBlock(prmKey, prmInterno, Code);

                Add(Corrente);
            }
        }

        private bool Find(string prmTag)
        {
            foreach (TestBlock Block in this)

                if (myString.IsEqual(Block.key, prmTag))
                {
                    Corrente = Block;

                    return (true);
                }
            return (false);
        }

        private bool GetDataDetected()
        {
            foreach (TestBlock Block in this)
                if (Block.IsDataDetected)
                    return (true);

            return (false);
        }
        private bool GetSaveDetected()
        {
            foreach (TestBlock Block in this)
                if (Block.IsSaveDetected)
                    return (true);

            return (false);
        }
    }
    public class TestBlock
    {

        public TestCode Code;

        private TestCommands Commands;

        public string key;

        public bool interno;

        public bool TemCommand => Commands.TemCorrente;
        public bool IsDataDetected => Commands.IsDataDetected;
        public bool IsSaveDetected => Commands.IsSaveDetected;
        public string txt => Linhas.memo;   

        private xMemo Linhas;

        public TestBlock(string prmKey, bool prmInterno, TestCode prmCode)
        {
            key = prmKey; interno = prmInterno;  Code = prmCode;

            Commands = new TestCommands(this);

            Linhas = new xMemo();
        }

        public void Clear() { Commands.Clear(); Linhas.Clear(); }
        public void Play() => Commands.Play();

        public void AddLine(string prmLine) => Linhas.Add(prmLine);
        public void AddCommand(TestCommand prmCommand) => Commands.AddCommand(prmCommand);
        public void AddParameter(TestSintaxe prmSintaxe) => Commands.AddParameter(prmSintaxe);

    }

    public class TestCommands : List<TestCommand>
    {

        private TestBlock Block;

        public TestCommand Corrente;

        private TestCode Code => Block.Code;

        private TestTrace Trace => Code.Trace;
        public bool TemCorrente => (Corrente != null);

        public bool IsDataDetected => GetDataDetected();
        public bool IsSaveDetected => GetSaveDetected();

        private TestCommandParameters Parametros { get => Corrente.Parametros; }

        public TestCommands(TestBlock prmBlock)
        {
            Block = prmBlock;
        }

        public void Play()
        {
            foreach (TestCommand command in this)
                command.Play();
        }

        public void AddCommand(TestCommand prmCommand)
        {
            Corrente = prmCommand;

            Add(Corrente);
        }

        public void AddParameter(TestSintaxe prmSintaxe)
        {
            if (prmSintaxe.IsNewParametro())
                Parametros.Criar(prmArg: prmSintaxe.Argumento);
            else
                Parametros.Merge(prmArg: prmSintaxe.Argumento);
        }

        private bool GetDataDetected()
        {
            foreach (TestCommand command in this)
                if (command.IsTipoData)
                    return true;

            return false;
        }
        private bool GetSaveDetected()
        {
            foreach (TestCommand command in this)
                if (command.IsTipoSave)
                    return true;

            return false;
        }
    }
    public class TestCommand
    {

        public TestConsole Console;

        public TestSintaxe Sintaxe;

        public TestCommandParameters Parametros;

        private TestCommandAction Action;

        public string keyword { get => Sintaxe.keyword; }
        public TestTrace Trace { get => Console.Trace; }
        private TestDataSource Dados { get => Console.Dados; }

        public bool IsTipoData => GetTipoData();
        public bool IsTipoSave => GetTipoSave();
        public TestCommand(TestSintaxe prmSintaxe)//, TestConsole prmConsole)
        {

            Console = prmSintaxe.Console;

            Sintaxe = prmSintaxe.IClone();

            Action = new TestCommandAction(this);

            Parametros = new TestCommandParameters(this);

        }

        public void Play() => Action.Play();

        private bool GetTipoData()
        {
            eTipoTestCommand tipo = Sintaxe.tipo;

            return (tipo == eTipoTestCommand.eCommandRaw || tipo == eTipoTestCommand.eCommandView || tipo == eTipoTestCommand.eCommandFlow);
        }
        private bool GetTipoSave() => Sintaxe.tipo == eTipoTestCommand.eCommandSave;

    }
public class TestCommandAction
    {

        private TestCommand Command;

        private TestScript Script { get => Sintaxe.Script; }

        private TestSintaxe Sintaxe { get => Command.Sintaxe; }

        private TestConsole Console { get => Command.Console; }

        private TestConsoleOutput Output { get => Console.Output; }

        private eTipoTestCommand tipo { get => Sintaxe.tipo; }

        private TestTrace Trace { get => Sintaxe.Trace; }

        private TestDataSource Dados { get => Console.Dados; }

        private TestDataPool Pool { get => Dados.Pool; }

        private string target { get => Sintaxe.target; }

        public TestCommandAction(TestCommand prmCommand)
        {
            Command = prmCommand;
        }

        public void Play()
        {

            Trace.LogConsole.PlayCommand(Sintaxe.comando, Sintaxe.keyword, Sintaxe.target);

            if (Sintaxe.IsPlay)
            {

                switch (tipo)
                {
                    case eTipoTestCommand.eCommandNote:
                        break;

                    case eTipoTestCommand.eCommandDB:
                        ActionCommandDB(prmCommand: Sintaxe.target);
                        break;

                    case eTipoTestCommand.eCommandTag:
                        break;

                    case eTipoTestCommand.eCommandVar:
                        ActionAddGlobalVAR();
                        break;

                    case eTipoTestCommand.eCommandLoc:
                        ActionAddLocalVAR();
                        break;

                    case eTipoTestCommand.eCommandRaw:
                        ActionSetDataRaw(Sintaxe.options);
                        break;

                    case eTipoTestCommand.eCommandView:
                        ActionAddDataView();
                        break;

                    case eTipoTestCommand.eCommandFlow:
                        ActionAddDataFlow();
                        break;

                    case eTipoTestCommand.eCommandSave:
                        ActionScriptSave();
                        break;

                    case eTipoTestCommand.eCommandBreak:
                        ActionScriptBreak();
                        break;

                    default:
                        if (!Sintaxe.IsAction)
                            Trace.LogConsole.FailActionKeyWord(Sintaxe.keyword);
                        return;
                }

                PlayParametros();

            }

        }

        private void PlayParametros()
        {
            foreach (TestCommandParameter Parametro in Command.Parametros)
                PlayArg(Parametro.arg, Parametro.Instrucao.txt);
        }

        private void PlayArg(string prmArg, string prmInstrucao)
        {
            switch (tipo)
            {

                case eTipoTestCommand.eCommandTag:
                    ActionSetDataTag(prmInstrucao);
                    break;

                case eTipoTestCommand.eCommandVar:
                    ActionSetGlobalVAR(prmArg, prmInstrucao);
                    break;

                case eTipoTestCommand.eCommandLoc:
                    ActionSetLocalVAR(prmArg, prmInstrucao);
                    break;

                case eTipoTestCommand.eCommandRaw:
                    ActionAddDataRaw(prmArg, prmInstrucao);
                    break;

                case eTipoTestCommand.eCommandView:
                    ActionSetDataView(prmArg, prmInstrucao);
                    break;

                case eTipoTestCommand.eCommandFlow:
                    ActionSetDataFlow(prmArg, prmInstrucao);
                    break;
                
                default:
                    return;
            }
        }

        private void ActionSetDataTag(string prmInstrucao) => Script.SetTag(prmInstrucao);

        private void ActionAddGlobalVAR() => Pool.AddGlobalVAR(prmVar: target);
        private void ActionSetGlobalVAR(string prmArg, string prmInstrucao) => Pool.SetGlobalVAR(prmArg, prmInstrucao);

        private void ActionAddLocalVAR() => Pool.AddLocalVAR(prmVar: target);
        private void ActionSetLocalVAR(string prmArg, string prmInstrucao) => Pool.SetLocalVAR(prmArg, prmInstrucao);

        private void ActionSetDataRaw(string prmOptions) => Pool.SetDataRaw(prmOptions);
        private void ActionAddDataRaw(string prmArg, string prmInstrucao) => Pool.AddDataRaw(prmArg, prmInstrucao);
        
        private void ActionAddDataView() => Dados.AddDataView(prmTag: target);
        private void ActionSetDataView(string prmArg, string prmInstrucao) => Pool.SetDataView(prmArg, prmInstrucao);

        private void ActionAddDataFlow() => Dados.AddDataFlow(prmTag: target);
        private void ActionSetDataFlow(string prmArg, string prmInstrucao) => Pool.SetDataFlow(prmArg, prmInstrucao);

        private void ActionCommandDB(string prmCommand) => Script.CommandDB(prmCommand);
        private void ActionScriptSave() => Script.Save(prmOptions: Sintaxe.options);
        private void ActionScriptBreak() => Script.Break(Sintaxe.options);


    }
    public class TestCommandParameter
    {

        private TestCommand Command;

        public string arg;

        public xLista Instrucao;

        private TestTrace Trace { get => Command.Trace; }

        public TestCommandParameter(string prmArg, TestCommand prmCommand)
        {

            arg = prmArg;

            Instrucao = new xMemo(prmSeparador: " ");

            Command = prmCommand;

        }

        public void Add(string prmLinha)
        {

            Instrucao.Add(prmLinha);

            Trace.LogConsole.WriteKeyWordArg(arg, prmLinha);

        }

    }
    public class TestCommandParameters : List<TestCommandParameter>
    {

        private TestCommand Command;

        public TestCommandParameter Corrente;

        private TestTrace Trace { get => Command.Trace; }

        public TestCommandParameters(TestCommand prmCommand)
        {

            Command = prmCommand;

        }

        public void Criar(TestSintaxeArgumento prmArg)
        {

            if (prmArg.IsOk())
            {

                Corrente = new TestCommandParameter(prmArg.key, Command);

                Add(Corrente);

                Merge(prmArg);

            }
            else
                Trace.LogConsole.FailArgNewKeyWord(Command.keyword, prmArg.key, prmArg.linha);

        }

        public bool Merge(TestSintaxeArgumento prmArg)
        {

            string parametro = prmArg.parametro;

            if (Corrente != null)
            {
                Corrente.Add(parametro);
                return true;
            }
            else
                Trace.LogConsole.FailArgMergeKeyWord(Command.keyword, parametro);

            return (false);

        }

    }
}
