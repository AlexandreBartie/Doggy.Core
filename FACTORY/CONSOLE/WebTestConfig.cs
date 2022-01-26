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

        public TestConfigImport Import;

        public TestConfigPath Path;

        public TestConfigFormat Format;

        public TestConfigTimeout Timeout;

        private DataBasesConnection Bases => Console.Pool.Bases;

        public string status => GetStatus();

        public bool IsOK => Path.IsOK && Bases.IsOK;

        public TestConsoleConfig(TestConsole prmConsole)
        {

            Console = prmConsole;

            Import = new TestConfigImport(this);

            Path = new TestConfigPath(this);
            Format = new TestConfigFormat(this);
            Timeout = new TestConfigTimeout(this);

        }

        public bool Setup(string prmArquivoCFG, bool prmPlay)
        {

            if (Import.Setup(prmArquivoCFG))
                { Console.Load(prmPlay); return true; }

            return false;

        }

        public bool Run(string prmBloco) => Import.Run(prmBloco);

        public string GetStatus() => Console.Dados.log() + " | " + Timeout.log() + " | " + Format.log() + " | " + Path.log();

    }
    public class TestConfigTimeout
    {

        private TestConsoleConfig Config;

        private TestDataPool Pool => Config.Console.Pool;
        private TestDataConnect Connect => Pool.Connect;
        private TestDataTratamento Tratamento => Pool.Tratamento;

        public TestConfigTimeout(TestConsoleConfig prmConfig)
        {
            Config = prmConfig;
        }

        public void SetConnectTimeOut(int prmSegundos)
        {
            Connect.SetConnectTimeOut(prmSegundos);
        }
        public void SetCommandTimeOut(int prmSegundos)
        {
            Connect.SetCommandTimeOut(prmSegundos);
        }
        private string connect_timeout() => String.Format("-connectDB: {0}", Connect.connect_timeout);
        private string command_timeout() => String.Format("-commandSQL: {0}", Connect.command_timeout);
        public string log() => String.Format(">timeout: {0}, {1}", connect_timeout(), command_timeout());

    }
    public class TestConfigFormat
    {

        private TestConsoleConfig Config;

        private TestDataPool Pool => Config.Console.Pool;
        private TestDataConnect Connect => Pool.Connect;
        private TestDataTratamento Tratamento => Pool.Tratamento;

        public TestConfigFormat(TestConsoleConfig prmConfig)
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
        public void SetFormatDate(string prmDate)
        {
            Tratamento.dateFormatDefault = prmDate;
        }
        private string today() => String.Format("-today: {0}", Tratamento.GetDateAnchor());
        private string date() => String.Format("-date: {0}", Tratamento.dateFormatDefault);
        public string log() => String.Format(">format: {0}, {1}", today(), date());

    }
    public class TestConfigPath
    {

        private TestConsoleConfig Config;

        private TestConsoleInput Input => Config.Console.Input;
        private TestConsoleOutput Output => Config.Console.Output;

        public string path_INI => Input.GetPath();
        public string path_OUT => Output.GetPath();
        public string path_LOG => Output.GetPathLOG();
        public bool IsOK => xString.IsFull(path_INI) && xString.IsFull(path_OUT) && xString.IsFull(path_LOG);

        public TestConfigPath(TestConsoleConfig prmConfig)
        {
            Config = prmConfig;
        }

        public void SetPathINI(string prmPathINI) => Input.SetPath(prmPathINI);
        public void SetPathOUT(string prmPathOUT) => Output.SetPath(prmPathOUT);
        public void SetPathLOG(string prmPathLOG) => Output.SetPathLOG(prmPathLOG);

        public string log() => String.Format(">path: -ini: '{0}', -out: '{1}', -log: '{2}'", path_INI, path_OUT, path_LOG);

    }
    public class TestConfigImport
    {
        public TestConsoleConfig Config;

        private TestConsole Console => Config.Console;

        private TestTrace Trace => Console.Trace;
        private TestDataConnect Connect => Console.Pool.Connect;

        private FileTXT File;

        public string nome => File.nome;
        private string nome_completo => File.nome_completo;

        private string grupo;

        private string linha;


        private string prefixo_grupo = ">";
        private string prefixo_parametro = "-";

        private string delimitador = ":";

        public TestConfigImport(TestConsoleConfig prmConfig)
        {
            Config = prmConfig;
        }

        public bool Setup(string prmArquivoCFG)
        {

            File = new FileTXT();

            if (File.Open(prmArquivoCFG))
            {

                if (Run(prmBloco: File.txt()))
                    { Trace.LogConfig.LoadConfig(prmArquivoCFG: nome_completo); return (true); }
                else
                    { Trace.LogConfig.FailLoadConfig(prmArquivoCFG: nome_completo, Config.status); return (false); }

            }

            Trace.LogFile.FailDataFileOpen(nome_completo, File.path);

            return false;
        }

        public bool Run(string prmBloco)
        {
            foreach (string line in new xMemo(prmBloco, prmSeparador: Environment.NewLine))
            {
                if (SetLine(line))
                {
                    if (IsGroup())
                        SetGroup();
                    else
                        SetParameter();
                }
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

            string tag; string valor; string sigla;
      
            tag = Bloco.GetBloco(linha, prmDelimitadorInicial: prefixo_parametro, prmDelimitadorFinal: delimitador).Trim().ToLower();
            valor = Bloco.GetBlocoDepois(linha, delimitador, prmTRIM: true);

            sigla = Bloco.GetBloco(tag, prmDelimitadorInicial: "[", prmDelimitadorFinal: "]").Trim().ToLower();

            if (sigla != "")
                tag = Bloco.GetBlocoAntes(tag, prmDelimitador: "[",prmTRIM: true).ToLower();

            switch (grupo)
            {

                case "path":
                    SetGroupPath(tag, valor); break;

                case "dbase":
                    SetGroupDBase(tag, sigla, valor); break;

                case "timeout":
                    SetGroupTimeout(tag, valor); break;

                case "format":
                    SetGroupFormat(tag, valor); break;

                default:
                    Trace.LogConfig.FailFindGroup(grupo); break;
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

                default:
                    Trace.LogConfig.FailFindParameter(prmTag, prmValor); break;
            }
        }
        private void SetGroupDBase(string prmTag, string prmSigla, string prmValor)
        {
            switch (prmTag)
            {
                case "db":
                    Connect.Oracle.AddJSON(prmSigla, prmValor); break;

                default:
                    Trace.LogConfig.FailFindParameter(prmSigla, prmValor); break;
            }
        }
        private void SetGroupTimeout(string prmTag, string prmValor)
        {
            switch (prmTag)
            {
                case "connectdb":
                    Config.Timeout.SetConnectTimeOut(xInt.GetNumero(prmValor)); break;

                case "commandsql":
                    Config.Timeout.SetCommandTimeOut(xInt.GetNumero(prmValor)); break;

                default:
                    Trace.LogConfig.FailFindParameter(prmTag, prmValor); break;
            }
        }
        private void SetGroupFormat(string prmTag, string prmValor)
        {
            switch (prmTag)
            {
                case "today":
                    Config.Format.SetToday(prmValor); break;

                case "date":
                    Config.Format.SetFormatDate(prmValor); break;

                default:
                    Trace.LogConfig.FailFindParameter(prmTag, prmValor); break;
            }
        }

        private bool IsGroup() => (Prefixo.IsPrefixo(linha, prefixo_grupo, delimitador));
        private void SetGroup() => grupo = (Prefixo.GetPrefixo(linha, prefixo_grupo, delimitador).Trim().ToLower());

    }

}




