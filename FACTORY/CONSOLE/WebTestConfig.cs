using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;
using Dooggy.LIBRARY;

namespace Dooggy.CORE
{
    public class TestConsoleConfig
    {

        public TestConsole Console;

        public TestConfigImport Import;

        public TestConfigMode Mode;

        public TestConfigCSV CSV;
        public TestConfigPath Path;
        public TestConfigDB DB;

        public TestConfigMain Main;

        public TestConfigValidation Validation;

        public DataPool Pool => Console.Pool;
        public DataGlobal Global => Console.Pool.Global;

        private DataBases Bases => Pool.Bases;
        public DataConnect Connect => Pool.Connect;

        public string status() => GetStatus();

        public bool IsOK => Path.IsOK && Bases.IsOK;

        public TestConsoleConfig(TestConsole prmConsole)
        {
            Console = prmConsole;

            Import = new TestConfigImport(this);

            Mode = new TestConfigMode(this);

            CSV = new TestConfigCSV(this);

            Path = new TestConfigPath(this);

            DB = new TestConfigDB(this);

            Main = new TestConfigMain(this);

            Validation = new TestConfigValidation(this);

        }

        public bool Setup(string prmArquivoCFG, bool prmPlay)
        {
            if (Load(prmArquivoCFG, prmPlay))
                { Console.Load(prmPlay); return true; }

            return false;
        }

        public bool Load(string prmArquivoCFG) => Load(prmArquivoCFG, prmPlay: false);
        private bool Load(string prmArquivoCFG, bool prmPlay)
        {
            Mode.SetMode(prmPlay);

            return (Import.Setup(prmArquivoCFG));
        }

        public bool Parse(string prmBloco) => Import.Parse(prmBloco);

        private string GetStatus()
        {
            
            xLista status = new xLista(Console.Dados.log, prmSeparador: " | ");

            if (Console.IsDbOK)
            {
                status.Add(DB.log); status.Add(CSV.log); status.Add(Path.log);

            }

            status.Add(Validation.log);

            return status.txt;
      
        }

    }
    public class TestConfigDB
    {

        private TestConsoleConfig Config;

        private DataConnect Connect => Config.Connect;

        private xLinhas SetupDB;

        public int timeoutDB => Connect.timeoutDB;
        public int timeoutSQL => Connect.timeoutSQL;

        public TestConfigDB(TestConsoleConfig prmConfig)
        {
            Config = prmConfig; SetupDB = new xLinhas();
        }

        public void SetSetup(string prmCommand) => SetupDB.Add(prmCommand);

        public void SetTimeOutDB(int prmSegundos) => Connect.timeoutDB = prmSegundos;
        public void SetTimeOutSQL(int prmSegundos) => Connect.timeoutSQL = prmSegundos;

        public string GetSetupDB()
        {
            xLinhas commands = new xLinhas();

            foreach (string linha in SetupDB)
                commands.Add(string.Format(">db: {0}",linha));

            return commands.memo;
        }

        private string txtSetup => String.Format("-setup: {0}", SetupDB.Export(" | "));
        private string txtTimeOutDB => String.Format("-timeoutDB: {0}", timeoutDB);
        private string txtTimeOutSQL => String.Format("-timeoutSQL: {0}", timeoutSQL);
        public string log => String.Format(">connect: {0}, {1}, {2}", txtSetup, txtTimeOutDB, txtTimeOutSQL);
    }

    public class TestConfigCSV
    {
        private TestConsoleConfig Config;

        private DataFormat Format => Config.Connect.Format;

        public string formatRegion;

        public string formatToday;

        private string formatSaveDefault;

        public CultureInfo Culture => Format.Culture;
        public DateTime dateAnchor => Format.dateAnchor;
        public string maskDateDefault => Format.maskDateDefault;


        public TestConfigCSV(TestConsoleConfig prmConfig)
        {
            Config = prmConfig;
        }
        public void SetToday(string prmFormat)
        {
            formatToday = prmFormat;

            Format.dateAnchor = myDate.Calc(prmDate: dateAnchor, prmSintaxe: prmFormat);
        }
        public void SetToday(DateTime prmDate)
        {
            Format.dateAnchor = prmDate;
        }
        public void SetFormatDate(string prmFormat)
        {
            Format.maskDateDefault = prmFormat;
        }

        public void SetRegion() => SetRegion(prmRegion: "");
        public void SetRegion(string prmRegion)
        {
            formatRegion = prmRegion; Format.SetCulture(prmRegion);
        }
        public void SetFormatSave(string prmFormat)
        {
            formatSaveDefault = prmFormat;
        }
        
        public string GetSaveDefault()
        {
            string format = "";

            if (myString.IsFull(formatSaveDefault))
                format = string.Format("[{0}]", formatSaveDefault);

            return string.Format(">save{0}:", format);
        }

        public string TextToCSV(string prmText, string prmFormat) => myCSV.TextToCSV(prmText, prmFormat);
        public string DateToCSV(DateTime prmDate, string prmFormat) => myCSV.DateToCSV(prmDate, prmFormat);
        public string DoubleToCSV(Double prmNumber, string prmFormat) => myCSV.DoubleToCSV(prmNumber, prmFormat, prmCulture: Culture);

        private string txt_region => String.Format("-region: {0}", formatRegion);
        private string txt_today => String.Format("-today: {0}", formatToday);
        private string txt_date => String.Format("-date: {0}", maskDateDefault);
        private string txt_save => String.Format("-save: {0}", formatSaveDefault);

        public string log => String.Format(">csv: {0}, {1}, {2}, {3}", txt_region, txt_today, txt_date, txt_save);

    }

    public class TestConfigValidation
    {

        private TestConsoleConfig Config;

        private TestConfigCSV CSV => Config.CSV;

        public TestConfigValidation(TestConsoleConfig prmConfig)
        {
            Config = prmConfig;
        }

        public string log => GetTest(prmDate: CSV.dateAnchor, prmNumber: 1234.5);

        private string txt_date(DateTime prmDate) => String.Format("-date: {0}", myCSV.DateToCSV(prmDate, CSV.maskDateDefault));
        private string txt_double(Double prmDouble) => String.Format("-double: {0}", myCSV.DoubleToCSV(prmDouble, CSV.Culture));

        private string GetTest(DateTime prmDate, Double prmNumber) => String.Format(">test: {0}, {1}", txt_date(prmDate), txt_double(prmNumber));

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

        public Diretorio CFG;
        public Diretorio INI;
        public Diretorio OUT;
        public Diretorio LOG;

        public bool IsOK => CFG.IsFull && INI.IsFull && OUT.IsFull && LOG.IsFull;

        public TestConfigPath(TestConsoleConfig prmConfig)
        {
            Config = prmConfig;

            CFG = new Diretorio();
            INI = new Diretorio();
            OUT = new Diretorio();
            LOG = new Diretorio();

        }
        public void SetCFG(string prmPath)
        {

            CFG.SetPath(prmPath);

            Trace.LogPath.SetPath(prmContexto: "ConfiguracaoGeral", prmPath);

        }

        public void SetINI(string prmPath)
        {

            INI.SetPath(prmPath);

            Trace.LogPath.SetPath(prmContexto: "OrigemScriptTestes", prmPath);

        }

        public void SetOUT(string prmPath)
        {

            OUT.SetPath(prmPath);

            Trace.LogPath.SetPath(prmContexto: "DestinoMassaTestes", prmPath);

        }
        public void SetSubOUT(string prmSubPath)
        {

            OUT.SetSubPath(prmSubPath);

            Trace.LogPath.SetSubPath(prmContexto: "... DestinoMassaTestes", prmSubPath);

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

        public string log => String.Format(">path: -cfg: '{0}', -ini: '{1}', -out: '{2}', -log: '{3}'", CFG.path, INI.path, OUT.path, LOG.path);

    }
    public class TestConfigMain
    {

        private TestConsoleConfig Config;

        private DataTags Tags => Config.Pool.Global.Tags;

        
        public TestConfigMain(TestConsoleConfig prmConfig)
        {
            Config = prmConfig;
        }

        public void AddTag(string prmSintaxe) => Tags.Add(prmSintaxe);

        //public string log => String.Format(">path: -ini: '{0}', -out: '{1}', -log: '{2}'", INI.path, OUT.path, LOG.path);

    }
    public class TestConfigImport
    {
        public TestConsoleConfig Config;

        private TestConsole Console => Config.Console;

        private TestTrace Trace => Console.Trace;
        private DataConnect Connect => Console.Pool.Connect;

        public bool IsOK;

        private FileTXT File;

        public FileTXT FileCFG;

        private string var_pathCFG = "#(PathArquivoCFG)";

        private Arquivos GetArquivosCFG() => Config.Path.CFG.Filtro("*.cfg");

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

            IsOK = false;

            if (Load(prmArquivoCFG, prmMain: true))
            {
                LoadPath();  IsOK = true; return true;
            }

            return false;
        }

        private void LoadPath()
        {
            foreach (Arquivo file in GetArquivosCFG())
                Load(prmArquivoCFG: file.nome_completo, prmMain: false);
        }

        private bool Load(string prmArquivoCFG, bool prmMain)
        {

            File = new FileTXT();

            if (File.Open(prmArquivoCFG))
            {

                if (prmMain)
                { FileCFG = File;  }

                if (Parse(prmBloco: File.txt()))
                { Trace.LogConfig.LoadCFG(prmFile: File); return true; }
                else
                { Trace.LogConfig.FailLoadCFG(prmFile: File, Config.status()); return (false); }

            }

            Trace.LogFile.FailDataFileOpen(prmFile: File);

            return false;
        }
        public bool Parse(string prmBloco)
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

            if (myString.IsFull(prmLinha))
                { linha = prmLinha; return true; }

            return false;

        }

        public void SetParameter()
        {

            string tag; string valor; string sigla; 
      
            tag = Bloco.GetBloco(linha, prmDelimitadorInicial: prefixo_parametro, prmDelimitadorFinal: delimitador).Trim().ToLower();
            valor = Bloco.GetBlocoDepois(linha, delimitador, prmTRIM: true);

            sigla = new BlocoColchetes().GetParametro(tag).ToLower();

            if (sigla != "")
                tag = new BlocoColchetes().GetPrefixo(tag).ToLower();

            switch (grupo)
            {

                case "path":
                    SetGroupPath(tag, valor); break;

                case "dbase":
                    SetGroupDBase(tag, sigla, valor); break;

                case "connect":
                    SetGroupConnect(tag, valor); break;

                case "csv":
                    SetGroupCSV(tag, valor); break;

                case "global":
                    SetGroupGlobal(tag, valor); break;

                default:
                    Trace.LogConfig.FailFindGroupCFG(grupo); break;
            }

        }

        private void SetGroupPath(string prmTag, string prmValor)
        {
            string path = prmValor;

            if (FileCFG != null)
                path = myString.GetSubstituir(path, var_pathCFG, FileCFG.path);
            
            switch (prmTag)
            {
                case "cfg":
                    Config.Path.SetCFG(path); break;

                case "ini":
                    Config.Path.SetINI(path); break;

                case "out":
                    Config.Path.SetOUT(path); break;

                case "log":
                    Config.Path.SetLOG(path); break;

                default:
                    Trace.LogConfig.FailFindParameterCFG(prmTag, path); break;
            }
        }
        private void SetGroupDBase(string prmTag, string prmSigla, string prmValor)
        {
            switch (prmTag)
            {
                case "db":
                    Connect.Assist.Oracle.AddJSON(prmSigla, prmValor); break;

                default:
                    Trace.LogConfig.FailFindParameterCFG(prmSigla, prmValor); break;
            }
        }
        private void SetGroupConnect(string prmTag, string prmValor)
        {
            switch (prmTag)
            {
                case "setup":
                    Config.DB.SetSetup(prmValor); break;

                case "timeoutdb":
                    Config.DB.SetTimeOutDB(myInt.GetNumero(prmValor)); break;

                case "timeoutsql":
                    Config.DB.SetTimeOutSQL(myInt.GetNumero(prmValor)); break;

                default:
                    Trace.LogConfig.FailFindParameterCFG(prmTag, prmValor); break;
            }
        }
        private void SetGroupCSV(string prmTag, string prmValor)
        {
            switch (prmTag)
            {
                case "region":
                    Config.CSV.SetRegion(prmValor); break;

                case "today":
                    Config.CSV.SetToday(prmValor); break;

                case "date":
                    Config.CSV.SetFormatDate(prmValor); break;

                case "save":
                    Config.CSV.SetFormatSave(prmValor); break;

                default:
                    Trace.LogConfig.FailFindParameterCFG(prmTag, prmValor); break;
            }
        }
        private void SetGroupGlobal(string prmTag, string prmValor)
        {
            switch (prmTag)
            {
                case "tag":
                    Config.Main.AddTag(prmSintaxe: prmValor); break;

                default:
                    Trace.LogConfig.FailFindParameterCFG(prmTag, prmValor); break;
            }
        }
        private bool IsGroup() => (BlocoPrefixo.IsPrefixo(linha, prefixo_grupo, delimitador));
        private void SetGroup() => grupo = (BlocoPrefixo.GetPrefixo(linha, prefixo_grupo, delimitador).Trim().ToLower());

    }

}




