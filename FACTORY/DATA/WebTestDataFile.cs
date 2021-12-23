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

        public bool Save(eTipoFileFormat prmTipo, string prmTags, string prmNome) => Save(prmTipo, prmTags, prmNome, prmSubPath: "");
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

        public bool SaveFile(string prmNome, string prmConteudo, eTipoFileFormat prmTipo) => SaveFile(prmNome, prmSubPath: "", prmConteudo, prmTipo);
        public bool SaveFile(string prmNome, string prmSubPath, string prmConteudo, eTipoFileFormat prmTipo) => Output.Save(prmNome, prmSubPath: "", prmConteudo, prmExtensao: GetExtensao(prmTipo));

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

        public void SetPathDestino(string prmPath) => Pool.SetPathDestino(prmPath);

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
            else
                Trace.LogFile.FailDataFileOpen(path, prmNome, prmExtensao);

            return ("");

        }

        public string GetPath(string prmSubPath) => Dados.Pool.GetPathDestino(prmSubPath);

    }
    public class TestDataOutput : TestDataPath
    {

        public TestDataOutput(TestDataLocal prmDados)
        {

            Dados = prmDados;

        }

        public bool Save(string prmNome, string prmSubPath, string prmConteudo, string prmExtensao)
        {

            if (xString.IsStringOK(prmNome))
            {

                string path = GetPath(prmSubPath);


                if (File.Save(path, prmNome, prmConteudo, prmExtensao))
                {

                    Trace.LogFile.DataFileExport(prmNome, prmExtensao, prmSubPath);

                    return (true);

                }

                Trace.LogFile.FailDataFileExport(path, prmNome, prmExtensao);

            }

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

}
