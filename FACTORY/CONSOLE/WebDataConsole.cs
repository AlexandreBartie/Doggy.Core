using Dooggy.Factory.Data;
using Dooggy.Lib.Generic;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dooggy.Factory.Console
{
    public class TestConsole
    {

        private TestCommands Commands;

        private TestConsoleImport Importacao;


        public TestDataLocal Dados;

        public TestDataPool Pool => Dados.Pool;
        public TestTrace Trace => Dados.Trace;

        public string GetArquivoINI => Importacao.arquivoINI;

        public string output { get => Commands.output; }

        public TestConsole(TestDataLocal prmDados)
        {

            Dados = prmDados;

            Commands = new TestCommands(this);

            Importacao = new TestConsoleImport(this);

        }

        public void Setup(string prmPathDestino, string prmPathINI)
        {

            Pool.SetPathDestino(prmPathDestino);

            Importacao.Setup(prmPathINI);

        }

        public void Start(string prmPathINI) => Importacao.Start(prmPathINI);

        public void Import(string prmNome) => Importacao.Play(prmNome);

        public void Write(string prmLinha) => Commands.Write(prmLinha);
        public void Write(string prmWord, string prmTarget) => Write(prmWord, prmTarget, prmParameters: "");
        public void Write(string prmWord, string prmTarget, string prmParameters) => Write(prmLinha: Commands.GetFormatLine(prmWord, prmTarget, prmParameters));

        public void Play() => Commands.Play();
        public void Play(string prmBloco) => Commands.Play(prmBloco);

        public void Save(string prmData) => Commands.output = prmData;

    }

    public class TestCommands : List<TestCommand>
    {

        public TestConsole Console;

        public TestCommand Corrente;

        public string output = "";

        public TestCommands(TestConsole prmConsole)
        {

            Console = prmConsole;

        }

        public void Write(string prmLinha)
        {

            string linha = prmLinha.Trim();

            if (linha == "") return;

            //
            // Identifica KeyWord:Target do comando a ser executado ...
            //

            string key_target = Blocos.GetBloco(linha, prmDelimitadorInicial: "<", prmDelimitadorFinal: ">");

            if (GetNewCommand(key_target))
                Add(Corrente);
            else
                Corrente.AddParametros(linha);

        }

        public void Play() => Execute();
        public void Play(string prmBloco)
        {

            foreach (string linha in new xLinhas(prmBloco))
                Write(linha);

            Play();

        }

        private void Execute()
        {

            foreach (TestCommand command in this)
                command.Play();

        }
        private bool GetNewCommand(string prmKeyTarget)
        {

            if (prmKeyTarget == "") return (false);

            //
            // Separar o KeyWord e Target  ...
            //

            string keyword = xString.GetNoBlank(xString.GetInicial(prmKeyTarget, prmDelimitador: ":")).ToLower();
            string target = xString.GetNoBlank(xString.GetFinal(prmKeyTarget, prmDelimitador: ":")).ToLower();

            //
            // Criar Novo Comando  ...
            //

            Corrente = new TestCommand(keyword, target, Console);

            return (true);

        }

        public string GetFormatLine(string prmWord, string prmTarget, string prmParameters)
        {

            string command = xString.GetNoBlank( string.Format("{0}:{1}", prmWord, prmTarget));

            if (xString.IsStringOK(prmParameters))
                command += " " + prmParameters;

            return (command);

        }

    }

    public enum eTipoTestCommand : int
    {

        fail = -1,

        note = 0,

        view  = 1,
        item  = 2,
        model = 3,
        var   = 4,

        savetxt  = 21,
        savecsv  = 22,
        savejson = 23,
    }
    public class TestCommand
    {

        public TestConsole Console;

        public string keyword;

        public string target;

        public string _args;

        public eTipoTestCommand tipo; 

        private xLista Args;

        public TestCommandParameters Parametros;


        private TestCommandAction Action;

        public TestTrace Trace { get => Console.Trace; }

        public TestDataLocal Dados { get => Console.Dados; }

        public TestCommand(string prmKeyWord, string prmTarget, TestConsole prmConsole)
        {

            Console = prmConsole;

            keyword = prmKeyWord;

            target = prmTarget;

            Action = new TestCommandAction(this);

            Parametros = new TestCommandParameters(this);

           Setup();

        }

        public void Play()
        {

            Action.Play();

        }

        public void AddParametros(string prmLinha)
        {

            string linha = prmLinha;

            //
            // Obter PARAMETRO do comando ...
            //

            string key_arg = Blocos.GetBloco(prmLinha, prmDelimitadorInicial: "-", prmDelimitadorFinal: ":");

            if (GetArg(key_arg))
            {

                Parametros.Criar(key_arg);

                linha = xString.GetFinal(prmLinha, prmDelimitador: ":").Trim();

            }

            if (!Parametros.Write(linha))
                Trace.LogConsole.FailMergeKeyWord(keyword, linha);

        }

        private void Setup()
        {

            switch (keyword)
            {

                case "note":
                    tipo = eTipoTestCommand.note;
                    break;

                case "view":
                case "dataview":
                    tipo = eTipoTestCommand.view;
                    _args = "descricao;saida;mask";
                    break;

                case "item":
                case "datafluxo":
                    tipo = eTipoTestCommand.item;
                    _args = "sql";
                    break;

                case "model":
                case "datamodel":
                    tipo = eTipoTestCommand.model;
                    _args = "tabelas;campos";
                    break;

                case "var":
                case "datavariant":
                    tipo = eTipoTestCommand.var;
                    _args = "condicao;ordem";
                    break;
                
                case "txt":
                case "savetxt":
                    tipo = eTipoTestCommand.savetxt;
                    break;

                case "csv":
                case "savecsv":
                    tipo = eTipoTestCommand.savecsv;
                    break;
                
                case "json":
                case "savejson":
                    tipo = eTipoTestCommand.savejson;
                    break;

                default:
                    tipo = eTipoTestCommand.fail;
                    Trace.LogConsole.FailFindKeyWord(keyword);
                    return ;
            }

            Args = new xLista(_args);

            Trace.LogConsole.WriteKeyWord(keyword, target);

        }

        private bool GetArg(string prmArg)
        {

            if (xString.IsStringOK(prmArg))
            {

                if (Args.IsContem(prmArg))
                    return (true);

                Trace.LogConsole.FailArgKeyWord(keyword, prmArg);

            }

            return (false);

        }

    }

    public class TestCommandAction
    {

        private TestCommand Command;

        private string target { get => Command.target; }

        private string arquivoINI { get => Console.GetArquivoINI; }

        private TestConsole Console { get => Command.Console; }

        private eTipoTestCommand tipo { get => Command.tipo; }

        private TestTrace Trace { get => Command.Trace; }
        private TestDataLocal Dados { get => Command.Dados; }

        private TestDataPool Pool { get => Dados.Pool; }

        private bool IsAction() => (Command.tipo > eTipoTestCommand.note);

        public TestCommandAction(TestCommand prmCommand)
        {

            Command = prmCommand;

        }

        public void Play()
        {

            if (!IsAction()) return;

            switch (tipo)
            {

                case eTipoTestCommand.view:
                    ActionAddDataView();
                    break;

                case eTipoTestCommand.item:
                    ActionAddDataFluxo();
                    break;

                case eTipoTestCommand.model:
                    ActionAddDataModel();
                    break;

                case eTipoTestCommand.var:
                    ActionAddDataVariant();
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
                    Trace.LogConsole.FailActionKeyWord(Command.keyword);
                    return;
            }

            PlayParametros();

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

                case eTipoTestCommand.view:
                    ActionSetDataView(prmArg, prmInstrucao);
                    break;

                case eTipoTestCommand.item:
                    ActionSetDataFluxo(prmArg, prmInstrucao);
                    break;

                case eTipoTestCommand.model:
                    //ActionDataModel(prmArg, prmInstrucao);
                    break;

                case eTipoTestCommand.var:
                    //ActionDataVariant(prmArg, prmInstrucao);
                    break;

                default:
                    return;
            }

        }
        private void ActionAddDataView() => Dados.AddDataView(prmTag: target);
        private void ActionSetDataView(string prmArg, string prmInstrucao) => Pool.SetDataView(prmArg, prmInstrucao);

        private void ActionAddDataFluxo() => Dados.AddDataFluxo(prmTag: target);
        private void ActionSetDataFluxo(string prmArg, string prmInstrucao) => Pool.SetDataFluxo(prmArg, prmInstrucao);

        private void ActionAddDataModel() => Dados.AddDataModel(prmTag: target);
        //private void ActionSetDataModel(string prmArg, string prmInstrucao) => Pool.SetDataModel(prmArg, prmInstrucao);

        private void ActionAddDataVariant() => Dados.AddDataVariant(prmTag: target);
        //private void ActionSetDataVariant(string prmArg, string prmInstrucao) => Pool.SetDataVariant(prmArg, prmInstrucao);

        private void ActionSaveFile(eTipoFileFormat prmTipo)
        {

            if (arquivoINI != "")
            {

                Console.Save(prmData: Dados.GetOutput(target, prmTipo));

                Dados.File.SaveFile(prmNome: arquivoINI, prmConteudo: Console.output, prmTipo);

            }

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

        public TestCommandParameters(TestCommand prmCommand)
        {

            Command = prmCommand;

        }

        public void Criar(string prmArg)
        {

            Corrente = new TestCommandParameter(prmArg, Command);

            Add(Corrente);

        }
        public bool Write(string prmLinha)
        {

            if (Corrente != null)
            {
                Corrente.Add(prmLinha);
                return true;
            }

            return (false);

        }

    }

}
