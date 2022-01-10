using Dooggy.Factory.Data;
using Dooggy.Lib.Generic;
using System;
using System.Collections.Generic;
using System.Text;
using static Dooggy.xInt;

namespace Dooggy.Factory.Console
{
    public class TestCommands : List<TestCommand>
    {

        private TestConsoleSession Sessao;

        public TestCommand Corrente;

        private TestCommandParameters Parametros { get => Corrente.Parametros; }

        public TestCommands(TestConsoleSession prmSessao)
        {

            Sessao = prmSessao;

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
        
    }

    public class TestCommand
    {

        public TestConsole Console;

        public TestSintaxe Sintaxe;

        public TestCommandParameters Parametros;

        private TestCommandAction Action;

        public string keyword { get => Sintaxe.keyword; }
        public TestTrace Trace { get => Console.Trace; }
        public TestDataLocal Dados { get => Console.Dados; }

        public TestCommand(TestSintaxe prmSintaxe, TestConsole prmConsole)
        {

            Console = prmConsole;

            Sintaxe = prmSintaxe.IClone();

            Action = new TestCommandAction(this);

            Parametros = new TestCommandParameters(this);

        }

        public void Play() => Action.Play();

    }

    public class TestCommandAction
    {

        private TestCommand Command;

        private TestSintaxe Sintaxe { get => Command.Sintaxe; }

        private string target { get => Sintaxe.target; }

        private TestConsole Console { get => Command.Console; }

        private eTipoTestCommand tipo { get => Sintaxe.tipo; }

        private TestTrace Trace { get => Sintaxe.Trace; }
        private TestDataLocal Dados { get => Command.Dados; }

        private TestDataPool Pool { get => Dados.Pool; }

        private bool IsAction() => (Sintaxe.tipo > eTipoTestCommand.note);

        public TestCommandAction(TestCommand prmCommand)
        {

            Command = prmCommand;

        }

        public void Play()
        {

            Trace.LogConsole.PlayCommand(Sintaxe.comando, Sintaxe.keyword, Sintaxe.target);

            if (IsAction())
            {

                switch (tipo)
                {

                    case eTipoTestCommand.var:
                        ActionAddDataVar();
                        break;

                    case eTipoTestCommand.raw:
                        ActionAddDataRaw();
                        break;

                    case eTipoTestCommand.view:
                        ActionAddDataView();
                        break;

                    case eTipoTestCommand.item:
                        ActionAddDataFluxo();
                        break;

                    case eTipoTestCommand.savetxt:
                        ActionSaveFile(prmTipo: eTipoFileFormat.txt);
                        break;

                    case eTipoTestCommand.savecsv:
                        ActionSaveFile(prmTipo: eTipoFileFormat.csv); ;
                        break;

                    case eTipoTestCommand.savejson:
                        ActionSaveFile(prmTipo: eTipoFileFormat.json);
                        break;

                    default:
                        Trace.LogConsole.FailActionKeyWord(Sintaxe.keyword);
                        return;
                }

                PlayParametros();

            }

        }

        private void PlayParametros()
        {

            foreach (TestCommandParameter parametro in Command.Parametros)
                PlayArg(parametro.arg, parametro.instrucao);

        }

        private void PlayArg(string prmArg, string prmInstrucao)
        {

            switch (tipo)
            {

                case eTipoTestCommand.var:
                    ActionSetDataVar(prmArg, prmInstrucao);
                    break;

                case eTipoTestCommand.raw:
                    ActionSetDataRaw(prmArg, prmInstrucao);
                    break;

                case eTipoTestCommand.view:
                    ActionSetDataView(prmArg, prmInstrucao);
                    break;

                case eTipoTestCommand.item:
                    ActionSetDataFluxo(prmArg, prmInstrucao);
                    break;

                default:
                    return;
            }

        }

        private void ActionAddDataRaw() => Dados.AddDataRaw(prmTag: target);
        private void ActionSetDataRaw(string prmArg, string prmInstrucao) => Pool.SetDataRaw(prmArg, prmInstrucao);

        private void ActionAddDataVar() => Dados.AddDataVar(prmTag: target);
        private void ActionSetDataVar(string prmArg, string prmInstrucao) => Pool.SetDataVar(prmArg, prmInstrucao); 
        
        private void ActionAddDataView() => Dados.AddDataView(prmTag: target);
        private void ActionSetDataView(string prmArg, string prmInstrucao) => Pool.SetDataView(prmArg, prmInstrucao);

        private void ActionAddDataFluxo() => Dados.AddDataFluxo(prmTag: target);
        private void ActionSetDataFluxo(string prmArg, string prmInstrucao) => Pool.SetDataFluxo(prmArg, prmInstrucao);

        private void ActionSaveFile(eTipoFileFormat prmTipo)
        {

            Console.Save(prmData: Dados.output(target, prmTipo));

            Dados.File.SaveFile(prmNome: Console.GetArquivoOUT(), prmConteudo: Console.resultado, prmTipo, prmEncoding: Sintaxe.opcoes);

        }

    }

    public class TestCommandParameter
    {

        private TestCommand Command;

        public string arg;

        private xMemo Memo;

        public string instrucao { get => Memo.memo(); }

        private TestTrace Trace { get => Command.Trace; }

        public TestCommandParameter(string prmArg, TestCommand prmCommand)
        {

            arg = prmArg;

            Memo = new xMemo(prmSeparador: " ");

            Command = prmCommand;

        }

        public void Add(string prmLinha)
        {

            Memo.Add(prmLinha);

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
