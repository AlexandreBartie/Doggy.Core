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

        public TestConfigMode Mode;

        public TestConfigPath Path;
        public TestConfigFormat Format;
        public TestConfigTimeout Timeout;

        public TestDataPool Pool => Console.Pool;
        private DataBasesConnection Bases => Pool.Bases;

        public string status => GetStatus();

        public bool IsOK => Path.IsOK && Bases.IsOK;

        public TestConsoleConfig(TestConsole prmConsole)
        {
            Console = prmConsole;

            Import = new TestConfigImport(this);

            Mode = new TestConfigMode(this);

            Path = new TestConfigPath(this);
            Format = new TestConfigFormat(this);
            Timeout = new TestConfigTimeout(this);
        }

        public bool Setup(string prmArquivoCFG, bool prmPlay)
        {
            Mode.SetMode(prmPlay);

            if (Import.Setup(prmArquivoCFG))
                { Console.Load(prmPlay); return true; }

            return false;
        }

        public bool Run(string prmBloco) => Import.Run(prmBloco);

        public string GetStatus()
        {
            if (Console.IsDbOK)
                return Console.Dados.log + " | " + Timeout.log + " | " + Format.log + " | " + Path.log;

            return Console.Dados.log;
        
        }

    }
    public class TestConfigTimeout
    {

        private TestConsoleConfig Config;

        public int connect_timeout = 30;
        public int command_timeout = 20;

        public TestConfigTimeout(TestConsoleConfig prmConfig)
        {
            Config = prmConfig;
        }

        public void SetConnectTimeOut(int prmSegundos) => connect_timeout = prmSegundos;
        public void SetCommandTimeOut(int prmSegundos) => command_timeout = prmSegundos;

        private string txtconnectTimeout() => String.Format("-connectDB: {0}", connect_timeout);
        private string txtcommandTimeout() => String.Format("-commandSQL: {0}", command_timeout);
        public string log => String.Format(">timeout: {0}, {1}", txtconnectTimeout(), txtcommandTimeout());

    }
    public class TestConfigFormat
    {

        private TestConsoleConfig Config;
        private TestDataTratamento Tratamento => Config.Pool.Tratamento;


        public DateTime anchor = DateTime.Now;

        public string dateFormatDefault = "DD/MM/AAAA";
        public string saveFormatDefault => GetSaveDefault();

        private string baseSaveFormatDefault;

        public TestConfigFormat(TestConsoleConfig prmConfig)
        {
            Config = prmConfig;
        }

        public void SetToday(string prmDate)
        {
            DynamicDate Data = new DynamicDate(anchor);

            anchor = Data.Calc(prmSintaxe: prmDate);
        }
        public void SetToday(DateTime prmDate)
        {
            anchor = prmDate;
        }
        public void SetFormatDate(string prmFormatDefault)
        {
            dateFormatDefault = prmFormatDefault;
        }
        public void SetFormatSave(string prmFormatDefault)
        {
            baseSaveFormatDefault = prmFormatDefault;
        }

        private string GetSaveDefault()
        {
            string format = "";

            if (xString.IsFull(baseSaveFormatDefault))
                format = string.Format("[{0}]", baseSaveFormatDefault);

            return string.Format(">save{0}:", format);
        }

        private string txt_today => String.Format("-today: {0}", Tratamento.GetDateAnchor());
        private string txt_date => String.Format("-date: {0}", dateFormatDefault);
        private string txt_save => String.Format("-save: {0}", baseSaveFormatDefault);

        public string log => String.Format(">format: {0}, {1}, {2}", txt_today, txt_date, txt_save);

    }
    public class TestConfigMode
    {

        private TestConsoleConfig Config;

        public bool IsAutoPlay;

        public TestConfigMode(TestConsoleConfig prmConfig)
        {
            Config = prmConfig;
        }

        public void SetMode(bool prmAutoPlay) => IsAutoPlay = prmAutoPlay;

    }
    public class TestConfigPath
    {

        private TestConsoleConfig Config;

        private TestTrace Trace => Config.Console.Trace;

        public Diretorio INI;
        public Diretorio OUT;
        public Diretorio LOG;

        public bool IsOK => INI.IsFull && OUT.IsFull && LOG.IsFull;

        public TestConfigPath(TestConsoleConfig prmConfig)
        {
            Config = prmConfig;

            INI = new Diretorio();
            OUT = new Diretorio();
            LOG = new Diretorio();

        }

        public void SetINI(string prmPath)
        {

            INI.SetPath(prmPath);

            Trace.LogPath.SetPath(prmContexto: "OrigemMassaTestes", prmPath);

        }

        public void SetOUT(string prmPath)
        {

            OUT.SetPath(prmPath);

            Trace.LogPath.SetPath(prmContexto: "DestinoMassaTestes", prmPath);

        }
        public void SetLOG(string prmPath)
        {

            LOG.SetPath(prmPath);

            Trace.LogPath.SetPath(prmContexto: "LogMassaTestes", prmPath);

        }

        public string GetExtensao(eTipoFileFormat prmTipo)
        {
            switch (prmTipo)
            {
                case eTipoFileFormat.txt:
                    return "txt";

                case eTipoFileFormat.json:
                    return "json";
            }
            return "csv";
        }
        public eTipoFileFormat GetTipoFormato(string prmTipo)
        {
            switch (prmTipo)
            {
                case "txt":
                    return eTipoFileFormat.txt;

                case "json":
                    return eTipoFileFormat.json;
            }
            return eTipoFileFormat.csv;
        }
        public string GetPathINI() => (INI.path);
        public string GetPathOUT() => (OUT.path);
        public string GetPathOUT(string prmSubPath) => (OUT.GetPath(prmSubPath));
        public string GetPathLOG() => (LOG.path);

        public string GetPathFullOUT(eTipoFileFormat prmTipo) => (GetPathOUT(prmSubPath: GetExtensao(prmTipo)));

        public string log => String.Format(">path: -ini: '{0}', -out: '{1}', -log: '{2}'", INI.path, OUT.path, LOG.path);

    }
    public class TestConfigImport
    {
        public TestConsoleConfig Config;

        private TestConsole Console => Config.Console;

        private TestTrace Trace => Console.Trace;
        private TestDataConnect Connect => Console.Pool.Connect;

        private FileTXT File;

        public bool IsOK;

        public string nome { get { if (IsOK) return File.nome; return ""; } }
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

            File = new FileTXT(); IsOK = false;

            if (File.Open(prmArquivoCFG))
            {

                if (Run(prmBloco: File.txt()))
                { Trace.LogConfig.LoadConfig(prmArquivoCFG: nome_completo); IsOK = true; return true; }
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
                    Config.Path.SetINI(prmValor); break;

                case "out":
                    Config.Path.SetOUT(prmValor); break;

                case "log":
                    Config.Path.SetLOG(prmValor); break;

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

                case "save":
                    Config.Format.SetFormatSave(prmValor); break;

                default:
                    Trace.LogConfig.FailFindParameter(prmTag, prmValor); break;
            }
        }

        private bool IsGroup() => (Prefixo.IsPrefixo(linha, prefixo_grupo, delimitador));
        private void SetGroup() => grupo = (Prefixo.GetPrefixo(linha, prefixo_grupo, delimitador).Trim().ToLower());

    }

}




