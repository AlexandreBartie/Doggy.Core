using Dooggy.Factory.Data;
using Dooggy.Lib.Generic;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dooggy.Factory.Console
{
    public class TestConsoleFactory
    {

        public TestDataLocal Dados;

        public TestConsoleMenu Menu;

        private TestConsoleCommands Commands;

        public TestTrace Trace => Dados.Trace;

        public TestConsoleFactory(TestDataLocal prmDados)
        {

            Dados = prmDados;

            Menu = new TestConsoleMenu(this);

            Commands = new TestConsoleCommands(this);

        }

        public bool Play(string prmCommand)
        {
            
            Commands.Play(prmCommand);
            
            return (true);

        }

        public bool Play(string prmWord, string prmTarget) => Play(prmWord, prmTarget, prmRules: "");
        public bool Play(string prmWord, string prmTarget, string prmRules) => Play(Commands.GetFormat(prmWord, prmTarget, prmRules));

        public void SetData(string prmData) => Commands.dados = prmData;
        public string GetData() => Commands.dados;
    }

    public class TestConsoleMenu
    {

        public TestConsoleFactory Console;

        public TestDataLocal Dados { get => Console.Dados; }

        public TestConsoleMenu(TestConsoleFactory prmConsole)
        {

            Console = prmConsole;

        }
        public void ActionDataView(string prmTag)
        {

            Dados.AddDataView(prmTag);

        }

        public void ActionDataFluxo(string prmTag, string prmSQL)
        {

            Dados.AddDataFluxo(prmTag, prmSQL);

        }

        public void ActionDataModel(string prmTag, string prmModelo)
        {

            Dados.AddDataModel(prmTag, prmModelo);

        }

        public void ActionDataVariant(string prmTag, string prmRegra)
        {

            Dados.AddDataVariant(prmTag, prmRegra);

        }
        public void ActionSaveFile(string prmTags, eTipoFileFormat prmTipo)
        {

            Console.SetData(prmData: Dados.SaveFile(prmTags, prmTipo));

        }

    }

    public class TestConsoleCommands : List<TestConsoleCommand>
    {

        public TestConsoleFactory Console;

        public string dados;

        public xLinhas linhas;

        public TestConsoleCommands(TestConsoleFactory prmConsole)
        {

            Console = prmConsole;

        }

        public void Play(string prmCommand)
        {

            linhas = new xLinhas(prmCommand);

            foreach (string linha in linhas)

                Add(new TestConsoleCommand(linha, Console));

        }

        public string GetFormat(string prmWord, string prmTarget, string prmRules)
        {

            string command = string.Format("[{0}]{1}", prmWord, prmTarget);

            if (xString.IsStringOK(prmRules))
                command += " <<# " + prmRules + " #>>";

            return (command);

        }

    }

    public class TestConsoleCommand
    {

        private TestConsoleFactory Console;

        public string command;

        public string keyword;

        public string target;

        public string dados;

        public TestConsoleMenu Menu { get => Console.Menu; }

        public TestTrace Trace { get => Console.Trace; }

        public TestConsoleCommand(string prmCommand, TestConsoleFactory prmConsole)
        {

            Console = prmConsole;

            Setup(prmCommand);

            Execute();

        }

        private void Setup(string prmCommand)
        {

            command = prmCommand;

            string bloco; string parametros;

            //
            // Obter o KeyWord do comando a ser executado ...
            //

            bloco = Blocos.GetBloco(prmCommand, prmDelimitadorInicial: "[", prmDelimitadorFinal: "]", prmPreserve: true);

            keyword = xString.GetNoBlank(Blocos.GetBloco(prmCommand, prmDelimitadorInicial: "[", prmDelimitadorFinal: "]")).ToLower();

            //
            // Obter TARGET e RULES para executar o comando ...
            //

            parametros = xString.GetRemove(prmCommand, bloco);

            bloco = Blocos.GetBloco(parametros, prmDelimitadorInicial: "<<#", prmDelimitadorFinal: "#>>", prmPreserve: true);

            target = Blocos.GetBlocoAntes(parametros, bloco, prmTRIM: true);

            dados = (Blocos.GetBloco(parametros, prmDelimitadorInicial: "<<#", prmDelimitadorFinal: "#>>")).Trim();

        }

        public void Execute()
        {

            switch (keyword)
            {

                case "dataview":
                    ActionDataView();
                    break;

                case "datafluxo":
                    ActionDataFluxo();
                    break;

                case "datamodel":
                    ActionDataModel();
                    break;

                case "datavariant":
                    ActionDataVariant();
                    break;

                case "savetxt":
                    ActionSaveTXT();
                    break;

                case "savecsv":
                    ActionSaveCSV();
                    break;

                case "savejson":
                    ActionSaveJSON();
                    break;

                default:
                    Trace.LogConsole.FailKeyWord(keyword);
                    return ;
            }

            Trace.LogConsole.ActionKeyWord(command);

        }
        private void ActionDataView()
        {

            Menu.ActionDataView(prmTag: target);

        }

        private void ActionDataFluxo()
        {

            Menu.ActionDataFluxo(prmTag: target, prmSQL: dados);

        }
        private void ActionDataModel()
        {

            Menu.ActionDataModel(prmTag: target, prmModelo: dados);

        }

        private void ActionDataVariant()
        {

            Menu.ActionDataVariant(prmTag: target, prmRegra: dados);

        }
        private void ActionSaveTXT()
        {

            Menu.ActionSaveFile(prmTags: target, prmTipo: eTipoFileFormat.txt);

        }

        private void ActionSaveCSV()
        {

            Menu.ActionSaveFile(prmTags: target, prmTipo: eTipoFileFormat.csv);

        }

        private void ActionSaveJSON()
        {

            Menu.ActionSaveFile(prmTags: target, prmTipo: eTipoFileFormat.json);

        }
    }




}
