using Dooggy.Lib.Files;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dooggy.Factory.Data
{
    public class TestDataFile
    {

        private TestDataLocal Dados;

        private TestDataExport Export;

        public TestDataPool Pool { get => Dados.Pool; }

        public TestDataFile(TestDataLocal prmDados)
        {

            Dados = prmDados;

            Export = new TestDataExport(Dados);

        }

        public bool SaveJSON(string prmTags, string prmNome) => SaveJSON(prmTags, prmNome, prmSubPath: "");
        public bool SaveJSON(string prmTags, string prmNome, string prmSubPath) => Export.Save(prmNome, prmSubPath, prmConteudo: Dados.json(prmTags), prmExtensao: "json");

        public bool SaveCSV(string prmTags, string prmNome) => SaveCSV(prmTags, prmNome, prmSubPath: "");
        public bool SaveCSV(string prmTags, string prmNome, string prmSubPath) => Export.Save(prmNome, prmSubPath, prmConteudo: Dados.csv(prmTags), prmExtensao: "csv");

        public bool SaveTXT(string prmTags, string prmNome, string prmCabecalho) => SaveTXT(prmTags, prmNome, prmCabecalho, prmSubPath: "");
        public bool SaveTXT(string prmTags, string prmNome, string prmCabecalho, string prmSubPath) => Export.Save(prmNome, prmSubPath, prmConteudo: Dados.txt(prmTags, prmCabecalho, prmColunaExtra: true), prmExtensao: "csv");


        public string OpenTXT(string prmNome) => OpenTXT(prmNome, prmSubPath: "");
        public string OpenTXT(string prmNome, string prmSubPath) => Export.Open(prmNome, prmSubPath, prmExtensao: "csv");

        public void SetPathDestino(string prmPath) => Pool.SetPathDestino(prmPath);

        public void SaveAll(string prmTags, string prmNome, string prmCabecalho)
        {

            // Formato JSON

            Dados.File.SaveJSON(prmTags, prmNome, prmSubPath: "json");

            // Formato CSV

            Dados.File.SaveCSV(prmTags, prmNome, prmSubPath: "csv");

            // Formato TXT com cabeçalho e coluna adicional ...

            Dados.File.SaveTXT(prmTags, prmNome, prmCabecalho, prmSubPath: "txt");



        }

    }
    public class TestDataExport
    {

        private TestDataLocal Dados;

        private xFileTXT File = new xFileTXT();

        public TestTraceLogFile Log { get => Dados.Pool.LogFile; }

        public TestDataExport(TestDataLocal prmDados)
        {

            Dados = prmDados;

        }

        public string Open(string prmNome, string prmSubPath, string prmExtensao)
        {

            string path = GetPath(prmSubPath);

            if (File.Open(path, prmNome, prmExtensao))
                return File.txt();

            return ("");

        }

        public bool Save(string prmNome, string prmSubPath, string prmConteudo, string prmExtensao)
        {

            string path = GetPath(prmSubPath);


            if (File.Save(path, prmNome, prmConteudo, prmExtensao))
            {

                Log.DataFileExport(prmNome, prmSubPath, prmExtensao);

                return (true);

            }

            Log.FailDataFileExport(path, prmNome, prmExtensao);

            return (false);
        }

        public string GetPath(string prmSubPath) => Dados.Pool.GetPath(prmSubPath);

    }
}
