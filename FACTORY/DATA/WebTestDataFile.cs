using Dooggy.Lib.Files;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dooggy.Factory.Data
{
    public class TestDataFile
    {

        private TestDataLocal Dados;

        private TestDataInput Input;

        private TestDataOutput Output;

        public TestDataPool Pool { get => Dados.Pool; }

        public TestDataFile(TestDataLocal prmDados)
        {

            Dados = prmDados;

            Input = new TestDataInput(Dados);

            Output = new TestDataOutput(Dados);

         }

        public bool Save(eTipoFileFormat prmTipo, string prmTags, string prmNome, string prmSubPath)
        {

            switch (prmTipo)
            {

                case eTipoFileFormat.csv:
                    return SaveCSV(prmTags, prmNome, prmSubPath);

                case eTipoFileFormat.txt:
                    return SaveTXT(prmTags, prmNome, prmSubPath);

            }

            return SaveJSON(prmTags, prmNome, prmSubPath);

        }

        public string Open(eTipoFileFormat prmTipo, string prmNome, string prmSubPath)
        {

            switch (prmTipo)
            {

                case eTipoFileFormat.csv:
                    return OpenCSV(prmNome, prmSubPath);

                case eTipoFileFormat.txt:
                    return OpenTXT(prmNome, prmSubPath);

            }

            return OpenJSON(prmNome, prmSubPath);

        }

        public string GetExtensao(eTipoFileFormat prmTipo)
        {

            switch (prmTipo)
            {

                case eTipoFileFormat.csv:
                    return "csv";

                case eTipoFileFormat.txt:
                    return "txt";

            }

            return "json";

        }

        public bool SaveFile(string prmNome, string prmConteudo, eTipoFileFormat prmTipo, string prmEncoding) => SaveFile(prmNome, prmSubPath: "", prmConteudo, prmTipo, prmEncoding);
        public bool SaveFile(string prmNome, string prmSubPath, string prmConteudo, eTipoFileFormat prmTipo, string prmEncoding) => Output.Save(prmNome, prmSubPath, prmConteudo, prmExtensao: GetExtensao(prmTipo), prmEncoding);

        public bool SaveJSON(string prmTags, string prmNome) => SaveJSON(prmTags, prmNome, prmSubPath: "");
        public bool SaveJSON(string prmTags, string prmNome, string prmSubPath) => Output.Save(prmNome, prmSubPath, prmConteudo: Dados.json(prmTags), prmExtensao: "json");

        public bool SaveCSV(string prmTags, string prmNome) => SaveCSV(prmTags, prmNome, prmSubPath: "");
        public bool SaveCSV(string prmTags, string prmNome, string prmSubPath) => Output.Save(prmNome, prmSubPath, prmConteudo: Dados.csv(prmTags), prmExtensao: "csv");

        public bool SaveTXT(string prmTags, string prmNome) => SaveTXT(prmTags, prmNome, prmSubPath: "");
        public bool SaveTXT(string prmTags, string prmNome, string prmSubPath) => Output.Save(prmNome, prmSubPath, prmConteudo: Dados.txt(prmTags), prmExtensao: "txt");

        public string OpenJSON(string prmNome) => OpenJSON(prmNome, prmSubPath: "");
        public string OpenJSON(string prmNome, string prmSubPath) => Input.Open(prmNome, prmSubPath, prmExtensao: "json");

        public string OpenCSV(string prmNome) => OpenCSV(prmNome, prmSubPath: "");
        public string OpenCSV(string prmNome, string prmSubPath) => Input.Open(prmNome, prmSubPath, prmExtensao: "csv");

        public string OpenTXT(string prmNome) => OpenTXT(prmNome, prmSubPath: "");
        public string OpenTXT(string prmNome, string prmSubPath) => Input.Open(prmNome, prmSubPath, prmExtensao: "txt");

        public void SetPathOUT(string prmPath) => Pool.SetPathOUT(prmPath);

        public void SaveAll(string prmTags, string prmNome)
        {

            // Formato JSON

            Dados.File.SaveJSON(prmTags, prmNome, prmSubPath: "json");

            // Formato CSV

            Dados.File.SaveCSV(prmTags, prmNome, prmSubPath: "csv");

            // Formato TXT com cabeçalho e coluna adicional ...

            Dados.File.SaveTXT(prmTags, prmNome, prmSubPath: "txt");

        }

    }

    public class TestDataInput : TestDataPath
    {

        public TestDataInput(TestDataLocal prmDados)
        {

            Dados = prmDados;

        }

        public string Open(string prmNome, string prmSubPath, string prmExtensao)
        {

            string path = GetPath(prmSubPath);

            if (File.Open(path, prmNome, prmExtensao))
                return File.txt();
            
            Trace.LogFile.FailDataFileOpen(path, prmArquivo: prmNome + "." + prmExtensao);

            return ("");

        }

        public string GetPath(string prmSubPath) => Dados.Pool.GetPathDestino(prmSubPath);

    }
    public class TestDataOutput : TestDataPath
    {
        public string path;
        public string arquivo;

        private TestDataEncoding Encode;

        public TestDataOutput(TestDataLocal prmDados)
        {

            Dados = prmDados;

            Encode = new TestDataEncoding(this);

        }

        public bool Save(string prmNome, string prmSubPath, string prmConteudo, string prmExtensao) => Save(prmNome, prmSubPath, prmConteudo, prmExtensao, prmEncoding: "");
        public bool Save(string prmNome, string prmSubPath, string prmConteudo, string prmExtensao, string prmEncoding)
        {

            if (xString.IsStringOK(prmNome))
            {

                path = GetPath(prmSubPath);

                arquivo = prmNome + "." + prmExtensao;

                if (File.Save(path, arquivo, prmConteudo, prmEncoding: Encode.Find(prmEncoding)))
                {

                    Trace.LogFile.DataFileExport(arquivo, prmSubPath, prmEncoding);

                    return (true);

                }

                Trace.LogFile.FailDataFileExport(path, arquivo);

            }
            else
                Trace.LogFile.DataFileMute(path, arquivo, prmEncoding);

            return (false);
        }

        public string GetPath(string prmSubPath) => Dados.Pool.GetPathDestino(prmSubPath);


    }
    public class TestDataPath
    {

        public TestDataLocal Dados;

        public FileTXT File = new FileTXT();

        public TestTrace Trace { get => Dados.Trace; }

    }

    public class TestDataEncoding
    {

        private TestDataOutput Ouput;

        public TestDataEncoding(TestDataOutput prmOuput)
        {

            Ouput = prmOuput;

        }
        public Encoding Find(string prmTipoEncoding)
        {

            string tipo = prmTipoEncoding.Trim().ToLower();

            switch (tipo)
            {
                case "":
                case "utf8":
                    return (Encoding.UTF8);

                case "utf7":
                    return (Encoding.UTF7);

                case "utf32":
                    return (Encoding.UTF32);

                case "unicode":
                    return (Encoding.Unicode);

                case "ascii":
                    return (Encoding.ASCII);

            }

            return (FindByCodePage(tipo));

        }

        private Encoding FindByCodePage(string prmEncoding)
        {

            int code_page = xInt.GetNumero(prmEncoding);

            foreach (EncodingInfo info in System.Text.Encoding.GetEncodings())
            {
                if (info.CodePage == code_page)
                    return info.GetEncoding();
            }

            Ouput.Trace.LogFile.FailDataFileEncoding(Ouput.path, Ouput.arquivo, prmEncoding);

            return (Encoding.Default);

        }


    }
}
