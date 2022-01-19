using Dooggy.Factory.Data;
using Dooggy.Lib.Data;
using Dooggy.Lib.Files;
using Dooggy.Lib.Generic;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dooggy.Factory.Console
{
    public class TestConsoleConfig
    {

        public TestConsole Console;

        public TestConfigInput Input;

        public TestConfigPath Path;
        public TestConfigAnchor Anchor;
        
        private DataBasesConnection Bases => Console.Pool.Bases;

        public string log => GetLog();

        public bool IsOK => Path.IsOK && Bases.IsOK;

        public TestConsoleConfig(TestConsole prmConsole)
        {

            Console = prmConsole;

            Input = new TestConfigInput(this);

            Path = new TestConfigPath(this);
            Anchor = new TestConfigAnchor(this);

        }

        public bool Setup(string prmArquivoCFG) => Input.Setup(prmArquivoCFG);

        public bool Parse(string prmBloco) => Input.Parse(prmBloco);

        public string GetLog()
        {
            string txt = "";

            txt += Path.log();
            txt += Anchor.log();

            txt += Console.Dados.log;

            return txt;
        }

    }
    public class TestConfigAnchor
    {

        private TestConsoleConfig Config;

        public TestDataTratamento Tratamento => Config.Console.Pool.Tratamento;

        public TestConfigAnchor(TestConsoleConfig prmConfig)
        {
            Config = prmConfig;
        }

        public void SetDate(string prmDate)
        {
            DynamicDate Data = new DynamicDate(Tratamento.anchor);

            Tratamento.anchor = Data.Calc(prmSintaxe: prmDate);
        }
        public void SetDate(DateTime prmDate)
        {
            Tratamento.anchor = prmDate;
        }

        public string log() => Tratamento.anchor.ToString("dd/MMM/yyyy") + Environment.NewLine;

    }
    public class TestConfigPath
    {

        private TestConsoleConfig Config;

        private TestConsoleInput Input => Config.Console.Input;
        private TestConsoleOutput Output => Config.Console.Output;

        public string path_INI => Input.GetPath();
        public string path_OUT => Output.GetPath();

        public bool IsOK => xString.IsFull(path_INI) && xString.IsFull(path_OUT);

        public TestConfigPath(TestConsoleConfig prmConfig)
        {
            Config = prmConfig;
        }

        public void SetPathINI(string prmPathINI) => Input.SetPath(prmPathINI);
        public void SetPathOUT(string prmPathOUT) => Output.SetPath(prmPathOUT);

        public string log()
        {
            string txt = "";

            txt += path_INI + Environment.NewLine;
            txt += path_OUT + Environment.NewLine;

            return txt;

        }

    }

    public class TestConfigBases
    {

        public string path_INI;
        public string path_OUT;

        public bool IsOK => xString.IsFull(path_INI) && xString.IsFull(path_OUT);

    }
    public class TestConfigInput
    {
        private TestConsoleConfig Config;

        private TestConsole Console => Config.Console;
        private TestDataConnect Connect => Console.Pool.Connect;

        private TestTrace Trace => Console.Trace;

        private FileTXT File;

        public string arquivoCFG;


        public TestConfigInput(TestConsoleConfig prmConfig)
        {
            Config = prmConfig;
        }

        public bool Setup(string prmArquivoCFG)
        {

            arquivoCFG = prmArquivoCFG;

            File = new FileTXT();

            if (File.Open(arquivoCFG))
                return Parse(File.txt());

            return (false);

        }

        public bool Parse(string prmBloco)
        {

            foreach (string line in new xMemo(prmBloco, prmSeparador: Environment.NewLine))
                SetParametros(line);

            return Config.Path.IsOK;

        }

        private void SetParametros(string prmLinha)
        {

            string tag; string valor;

            if (xString.IsFull(prmLinha))
            {

                tag = Bloco.GetBloco(prmLinha, prmDelimitadorInicial: "[", prmDelimitadorFinal: "]").Trim();
                valor = Bloco.GetBlocoRemove(prmLinha, prmDelimitadorInicial: "[", prmDelimitadorFinal: "]").Trim();

                switch (tag.ToLower())
                {

                    case "path_ini":
                        Config.Path.SetPathINI(valor); break;

                    case "path_out":
                        Config.Path.SetPathOUT(valor); break;

                    case "dbase":
                        Connect.Oracle.AddJSON(tag, valor); break;

                    case "date":
                        Console.SetAnchor(valor); break;

                }

            }

        }

        private string GetValue(xLinhas Linhas, string prmTag)
        {

            string texto = Linhas.GetFind(prmTag); string valor = "";

            if (xString.IsFull(texto))
                valor = Bloco.GetBlocoRemove(texto, prmDelimitadorInicial: "[", prmDelimitadorFinal: "]", prmTRIM: true);

            return (valor);

        }

    }

}




