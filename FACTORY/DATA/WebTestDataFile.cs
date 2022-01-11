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

        public bool Save(eTipoFileFormat prmTipo, string prmTags, string prmNome, string prmPath)
        {

            switch (prmTipo)
            {

                case eTipoFileFormat.csv:
                    return SaveCSV(prmTags, prmNome, prmPath);

                case eTipoFileFormat.txt:
                    return SaveTXT(prmTags, prmNome, prmPath);

            }

            return SaveJSON(prmTags, prmNome, prmPath);

        }

        public string Open(eTipoFileFormat prmTipo, string prmNome, string prmPath)
        {

            switch (prmTipo)
            {

                case eTipoFileFormat.csv:
                    return OpenCSV(prmNome, prmPath);

                case eTipoFileFormat.txt:
                    return OpenTXT(prmNome, prmPath);

            }

            return OpenJSON(prmNome, prmPath);

        }

        private bool SaveJSON(string prmTags, string prmNome, string prmPath) => SaveFile(prmNome, prmPath, prmConteudo: Dados.json(prmTags), prmExtensao: "json");
        private bool SaveCSV(string prmTags, string prmNome, string prmPath) => SaveFile(prmNome, prmPath, prmConteudo: Dados.csv(prmTags), prmExtensao: "csv");
        private bool SaveTXT(string prmTags, string prmNome, string prmPath) => SaveFile(prmNome, prmPath, prmConteudo: Dados.txt(prmTags), prmExtensao: "txt");

        private bool SaveFile(string prmNome, string prmPath, string prmConteudo, string prmExtensao) => Output.Save(prmNome, prmPath, prmConteudo, prmExtensao);
        public bool SaveFile(string prmNome, string prmPath, string prmConteudo, string prmExtensao, string prmEncoding) => Output.Save(prmNome, prmPath, prmConteudo, prmExtensao, prmEncoding);



        public string OpenJSON(string prmNome, string prmPath) => Input.Open(prmNome, prmPath, prmExtensao: "json");
        public string OpenCSV(string prmNome, string prmPath) => Input.Open(prmNome, prmPath, prmExtensao: "csv");
        public string OpenTXT(string prmNome, string prmPath) => Input.Open(prmNome, prmPath, prmExtensao: "txt");

        //public void SaveAll(string prmTags, string prmNome)
        //{

        //    // Formato JSON

        //    Dados.File.SaveJSON(prmTags, prmNome, prmPath: "json");

        //    // Formato CSV

        //    Dados.File.SaveCSV(prmTags, prmNome, prmPath: "csv");

        //    // Formato TXT com cabeçalho e coluna adicional ...

        //    Dados.File.SaveTXT(prmTags, prmNome, prmPath: "txt");

        //}

    }

    public class TestDataInput : TestDataPath
    {

        public TestDataInput(TestDataLocal prmDados)
        {

            Dados = prmDados;

        }

        public string Open(string prmNome, string prmPath, string prmExtensao)
        {

            if (File.Open(prmPath, prmNome, prmExtensao))
                return File.txt();

            Trace.LogFile.FailDataFileOpen(prmPath, prmArquivo: prmNome + "." + prmExtensao);

            return ("");

        }

        //public string GetPath(string prmSubPath) => Dados.Pool.GetPathDestino(prmSubPath);

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

        public bool Save(string prmNome, string prmPath, string prmConteudo, string prmExtensao) => Save(prmNome, prmPath, prmConteudo, prmExtensao, prmEncoding: "");
        public bool Save(string prmNome, string prmPath, string prmConteudo, string prmExtensao, string prmEncoding)
        {

            if (xString.IsStringOK(prmNome))
            {

                path = prmPath;

                arquivo = prmNome + "." + prmExtensao;

                if (File.Save(path, arquivo, prmConteudo, prmEncoding: Encode.Find(prmEncoding)))
                {

                    Trace.LogFile.DataFileExport(arquivo, prmPath, prmEncoding);

                    return (true);

                }

                Trace.LogFile.FailDataFileExport(path, arquivo);

            }
            else
                Trace.LogFile.DataFileMute(path, arquivo, prmEncoding);

            return (false);
        }

        //public string GetPath(string prmSubPath) => Dados.Pool.GetPathDestino(prmSubPath);


    }
    public class TestDataPath
    {

        public TestDataLocal Dados;

        public FileTXT File = new FileTXT();

        public TestTrace Trace { get => Dados.Trace; }

    }

    public class TestDataEncoding
    {

        private TestDataOutput Output;

        public TestTrace Trace { get => Output.Trace; }

        public TestDataEncoding(TestDataOutput prmOutput)
        {

            Output = prmOutput;

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

            if (code_page != -1)

            {

                foreach (EncodingInfo info in System.Text.Encoding.GetEncodings())
                {
                    if (info.CodePage == code_page)
                        return info.GetEncoding();
                }

                Encoding encode = TryInstallCodePage(prmCodePage: code_page);

                if (encode != null) return (encode);
  
            }

            Trace.LogFile.FailDataFileEncoding(Output.path, Output.arquivo, prmEncoding);

            return (Encoding.Default);

        }

        private Encoding TryInstallCodePage(int prmCodePage)
        {

            Encoding encode = CodePagesEncodingProvider.Instance.GetEncoding(prmCodePage);

            return (encode);

        }

    }
}
