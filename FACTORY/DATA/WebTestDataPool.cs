using Dooggy;
using Dooggy.Factory;
using Dooggy.Factory.Console;
using Dooggy.Lib.Data;
using Dooggy.Lib.Files;
using Dooggy.Lib.Generic;
using Dooggy.Lib.Parse;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace Dooggy.Factory.Data
{

    public enum eTipoFileFormat : int
    {
        json = 0,
        csv = 1,
        txt = 2
    }

    public enum eTipoFileEncoding : int
    {
        json = 0,
        csv = 1,
        txt = 2
    }

    public class TestDataPool
    {

        private DataBasesConnection Bases;

        public TestTrace Trace;

        public TestDataConnect Connect;

        private TestDataVars Vars;
        private TestDataViews Views;
        private TestDataModels Modelos;

        private DateTime ancora;


        private Path PathDataFiles;

        public TestDataFluxos Fluxos { get => (DataViewCorrente.Fluxos); }

        public DataBaseConnection DataBaseCorrente { get => (Bases.Corrente); }

        public TestDataView DataViewCorrente { get => (Views.Corrente); }
        public TestDataFluxo DataFluxoCorrente { get => (Fluxos.Corrente); }
        public TestDataModel DataModelCorrente { get => (Modelos.Corrente); }

        private bool IsBaseCorrente { get => (DataBaseCorrente != null); }
        private bool IsModelCorrente { get => (DataModelCorrente != null); }

        public TestDataPool()
        {

            Trace = new TestTrace();

            PathDataFiles = new Path();

            Bases = new DataBasesConnection();

            Vars = new TestDataVars(this);

            Views = new TestDataViews(this);

            Connect = new TestDataConnect(this);

            ancora = DateTime.Now;

        }

        public bool AddDataBase(string prmTag, string prmConexao) => Bases.Criar(prmTag, prmConexao, this);

        public string AddDataVar(string prmTarget) => Vars.Criar(prmTarget, DataBaseCorrente);

        public string AddDataView(string prmTag) => AddDataView(prmTag, prmMask: "");
        public string AddDataView(string prmTag, string prmMask) => Views.Criar(prmTag, prmMask, DataBaseCorrente);

        public bool AddDataFluxo(string prmTag, string prmSQL) => AddDataFluxo(prmTag, prmSQL, prmMask: "");
        public bool AddDataFluxo(string prmTag, string prmSQL, string prmMask) => DataViewCorrente.Fluxos.Criar(prmTag, prmSQL, prmMask, DataViewCorrente);

        public bool AddDataModel(string prmTag, string prmModelo, string prmMask) => Modelos.Criar(prmTag, prmModelo, prmMask, DataBaseCorrente);
        public bool AddDataVariant(string prmTag, string prmRegra) => AddDataVariant(prmTag, prmRegra, prmQtde: 1);

        public bool AddDataVariant(string prmTag, string prmRegra, int prmQtde) => DataModelCorrente.CriarVariacao(prmTag, prmRegra, prmQtde);


        public void SetDataVar(string prmArg, string prmInstrucao) => Vars.SetArgumento(prmArg, prmInstrucao);
        public void SetDataView(string prmArg, string prmInstrucao) => Views.SetArgumento(prmArg, prmInstrucao);
        public void SetDataFluxo(string prmArg, string prmInstrucao) => Fluxos.SetArgumento(prmArg, prmInstrucao);
        public void SetMaskDataFluxo(string prmMask) => Fluxos.SetMask(prmMask);


        public bool IsON()
        {

            bool retorno = false;

            foreach (DataBaseConnection Base in Bases)
            {
                if (!Base.IsON)
                { return (false); }

                retorno = true;
            }

            return (retorno);

        }

        public void SetPathDestino(string prmPath)
        {

            PathDataFiles.Setup(prmPath);

            Trace.LogPath.SetPath(prmContexto: "DestinoMassaTestes", prmPath);

        }
        public void SetAncora(DateTime prmAncora)
        {

            ancora = prmAncora;

        }
        public string GetVariavel(string prmTag)
        {

            if (Vars.Find(prmTag))
                return Vars.Corrente.valor;

            return ("");

        }

        public string GetFuncao(string prmFuncao, string prmParametro)
        {

            switch (prmFuncao)
            {

                case "date":
                    return GetDataDinamica(prmParametro);
 
            }

            return ("");
        }

        public string GetDataDinamica(string prmParametro)
        {

            DynamicDate Date = new DynamicDate(ancora);

            return (Date.View(prmSintaxe: prmParametro));

        }

        public string GetNextKeyDataView() => string.Format("x{0},", Views.Count); 

        public string GetPathDestino(string prmSubPath) => PathDataFiles.GetPath(prmSubPath);

        public string txt(string prmTags) => Views.Save(prmTags, prmTipo: eTipoFileFormat.txt);
        public string csv(string prmTags) => Views.Save(prmTags, prmTipo: eTipoFileFormat.csv);
        public string json(string prmTags) => Views.Save(prmTags, prmTipo: eTipoFileFormat.json);

    }
    public class TestDataLocal
    {

        private Object Origem;

        public TestDataPool Pool;

        public TestDataFile File;

        public TestDataKeyDriven KeyDriven;

        public TestDataView View { get => Pool.DataViewCorrente; }

        public TestTrace Trace { get => Pool.Trace; }

        public TestDataLocal()
        {

            File = new TestDataFile(this);

            KeyDriven = new TestDataKeyDriven(Pool);

        }

        public void Setup(Object prmOrigem, TestDataPool prmPool)
        {

            Origem = prmOrigem;

            Pool = prmPool;

        }
        public bool AddDataBase(string prmTag, string prmConexao) => (Pool.AddDataBase(prmTag, prmConexao));

        public string AddDataVar(string prmTarget) => (Pool.AddDataVar(prmTarget));

        public string AddDataView(string prmTag) => (AddDataView(prmTag, prmMask: ""));
        public string AddDataView(string prmTag, string prmMask) => (Pool.AddDataView(prmTag, prmMask));
        public bool AddDataFluxo(string prmTag) => (AddDataFluxo(prmTag, prmSQL: ""));
        public bool AddDataFluxo(string prmTag, string prmSQL) => (AddDataFluxo(prmTag, prmSQL, prmMask: ""));
        public bool AddDataFluxo(string prmTag, string prmSQL, string prmMask) => (Pool.AddDataFluxo(prmTag, prmSQL, prmMask));

        public void AddDataModel(string prmTag) => AddDataModel(prmTag, prmModelo: "");
        public void AddDataModel(string prmTag, string prmModelo) => AddDataModel(prmTag, prmModelo, prmMask: "");
        public void AddDataModel(string prmTag, string prmModelo, string prmMask) => Pool.AddDataModel(prmTag, prmModelo, prmMask);

        public void AddDataVariant(string prmTag) => AddDataVariant(prmTag, prmRegra: "");
        public void AddDataVariant(string prmTag, string prmRegra) => Pool.AddDataVariant(prmTag, prmRegra);

        public string GetOutput(string prmTags, eTipoFileFormat prmTipo)
        {

            switch (prmTipo)
            {

                case eTipoFileFormat.csv:
                    return csv(prmTags);

                case eTipoFileFormat.txt:
                    return txt(prmTags);

            }

            return json(prmTags);

        }
        public string txt(string prmTags) => (Pool.txt(prmTags));
        public string csv(string prmTags) => (Pool.csv(prmTags));
        public string json(string prmTags) => (Pool.json(prmTags));

    }

    public class TestDataKeyDriven
    {

        public TestDataPool Pool;

        public TestDataKeyDriven(TestDataPool prmPool)
        {

            Pool = prmPool;

        }

        public void SetMaskDataFluxo(string prmTag, string prmMask) => Pool.SetMaskDataFluxo(prmMask);


    }

    public class ITestDataLocal
    {

        private TestDataLocal _Dados;

        public TestDataLocal Dados
        {
            get
            {
                if (_Dados == null)
                    _Dados = new TestDataLocal();

                return _Dados;

            }

        }

    }
}
