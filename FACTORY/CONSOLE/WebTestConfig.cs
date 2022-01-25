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
        public TestConfigPerfil Perfil;
        
        private DataBasesConnection Bases => Console.Pool.Bases;

        public string log => GetLog();

        public bool IsOK => Path.IsOK && Bases.IsOK;

        public TestConsoleConfig(TestConsole prmConsole)
        {

            Console = prmConsole;

            Input = new TestConfigInput(this);

            Path = new TestConfigPath(this);
            Perfil = new TestConfigPerfil(this);

        }

        public bool Setup(string prmArquivoCFG) => Input.Setup(prmArquivoCFG);

        public bool Run(string prmBloco) => Input.Run(prmBloco);

        public string GetLog()
        {
            string txt = "";

            txt += Path.log();
            txt += Perfil.log();

            txt += Console.Dados.log;

            return txt;
        }

    }
    public class TestConfigPerfil
    {

        private TestConsoleConfig Config;

        private TestDataPool Pool => Config.Console.Pool;
        private TestDataConnect Connect => Pool.Connect;
        private TestDataTratamento Tratamento => Pool.Tratamento;

        public TestConfigPerfil(TestConsoleConfig prmConfig)
        {
            Config = prmConfig;
        }

        public void SetToday(string prmDate)
        {
            DynamicDate Data = new DynamicDate(Tratamento.anchor);

            Tratamento.anchor = Data.Calc(prmSintaxe: prmDate);
        }
        public void SetToday(DateTime prmDate)
        {
            Tratamento.anchor = prmDate;
        }
        public void SetTimeOut(int prmSegundos)
        {
            Connect.SetCommandTimeOut(prmSegundos);
        }

        private string anchor() => String.Format("TODAY: {0}", Tratamento.anchor.ToString("dd/MMM/yyyy")) + Environment.NewLine;
        private string timeout() => String.Format("TIMEOUT: {0}", Connect.command_timeout) + Environment.NewLine;
        public string log() => anchor() + timeout();

    }
    public class TestConfigPath
    {

        private TestConsoleConfig Config;

        private TestConsoleInput Input => Config.Console.Input;
        private TestConsoleOutput Output => Config.Console.Output;

        public string path_INI => Input.GetPath();
        public string path_OUT => Output.GetPath();
        public string path_LOG => Output.GetPathLOG();
        public bool IsOK => xString.IsFull(path_INI) && xString.IsFull(path_OUT);

        public TestConfigPath(TestConsoleConfig prmConfig)
        {
            Config = prmConfig;
        }

        public void SetPathINI(string prmPathINI) => Input.SetPath(prmPathINI);
        public void SetPathOUT(string prmPathOUT) => Output.SetPath(prmPathOUT);
        public void SetPathLOG(string prmPathLOG) => Output.SetPathLOG(prmPathLOG);

        public string log()
        {
            string txt = "";

            txt += "INI: " + path_INI + Environment.NewLine;
            txt += "OUT: " + path_OUT + Environment.NewLine;
            txt += "LOG: " + path_LOG + Environment.NewLine;

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
        public TestConsoleConfig Config;

        private TestConsole Console => Config.Console;
        private TestDataConnect Connect => Console.Pool.Connect;

        private FileTXT File;

        private string grupo;

        private string linha;

        public TestConfigInput(TestConsoleConfig prmConfig)
        {
            Config = prmConfig;
        }

        public bool Setup(string prmArquivoCFG)
        {

            File = new FileTXT();

            if (File.Open(prmArquivoCFG))
                return Run(prmBloco: File.txt());

            return false;
        }

        public bool Run(string prmBloco)
        {

            foreach (string line in new xMemo(prmBloco, prmSeparador: Environment.NewLine))
            {

                if (SetLine(line))

                    if (IsGroup())
                        SetGroup();
                    else
                        SetParameter();

            }

            return Config.Path.IsOK;

        }

        public bool SetLine(string prmLinha)
        {

            if (xString.IsFull(prmLinha))
                { linha = prmLinha; return true; }

            return false;

        }

        public void SetParameter()
        {

            string tag; string valor;
      
            tag = Bloco.GetBloco(linha, prmDelimitadorInicial: "[", prmDelimitadorFinal: "]").Trim().ToLower();
            valor = Bloco.GetBlocoRemove(linha, prmDelimitadorInicial: "[", prmDelimitadorFinal: "]").Trim();

            switch (grupo)
            {

                case "path":
                    SetGroupPath(tag, valor); break;

                case "data":
                    SetGroupData(tag, valor); break;

                case "perfil":
                    SetGroupPerfil(tag, valor); break;

            }

        }

        private void SetGroupPath(string prmTag, string prmValor)
        {
            switch (prmTag)
            {
                case "ini":
                    Config.Path.SetPathINI(prmValor); break;

                case "out":
                    Config.Path.SetPathOUT(prmValor); break;

                case "log":
                    Config.Path.SetPathLOG(prmValor); break;
            }
        }
        private void SetGroupData(string prmTag, string prmValor)
        {
            Connect.Oracle.AddJSON(prmTag, prmValor);
        }
        private void SetGroupPerfil(string prmTag, string prmValor)
        {
            switch (prmTag)
            {
                case "today":
                    Config.Perfil.SetToday(prmValor); break;

                case "timeout":
                    Config.Perfil.SetTimeOut(xInt.GetNumero(prmValor)); break;
            }
        }

        private string GetValue(xLinhas Linhas, string prmTag)
        {

            string texto = Linhas.GetFind(prmTag); string valor = "";

            if (xString.IsFull(texto))
                valor = Bloco.GetBlocoRemove(texto, prmDelimitadorInicial: "[", prmDelimitadorFinal: "]", prmTRIM: true);

            return (valor);

        }

        private bool IsGroup() => (Prefixo.IsPrefixo(linha, prmPrefixo: ">>"));
        private void SetGroup() => grupo = (Prefixo.GetPrefixo(linha, prmPrefixo: ">>").Trim().ToLower());

    }

}




