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

    public class TestDataPool
    {

        private DataBasesConnection Bases;

        public TestTrace Trace;

        public TestDataConnect Connect;

        private TestDataVars Vars;
        private TestDataViews Views;

        private DateTime ancora;

        private Path PathDataFiles;

        public TestDataFluxos Fluxos => (DataViewCorrente.Fluxos);

        public DataBaseConnection DataBaseCorrente => (Bases.Corrente);

        public TestDataView DataViewCorrente => (Views.Corrente);
        public TestDataFluxo DataFluxoCorrente => (Fluxos.Corrente);

        public TestDataPool()
        {

            Trace = new TestTrace();

            PathDataFiles = new Path();

            Bases = new DataBasesConnection();

            Connect = new TestDataConnect(this);

            ancora = DateTime.Now;

            Cleanup();

        }

        public bool AddDataBase(string prmTag, string prmConexao) => Bases.Criar(prmTag, prmConexao, this);

        public string AddDataVar(string prmTarget) => Vars.Criar(prmTarget, DataBaseCorrente);

        public string AddDataView(string prmTag) => AddDataView(prmTag, prmMask: "");
        public string AddDataView(string prmTag, string prmMask) => Views.Criar(prmTag, prmMask, DataBaseCorrente);

        public bool AddDataFluxo(string prmTag, string prmSQL, string prmMask) => DataViewCorrente.Fluxos.Criar(prmTag, prmSQL, prmMask, DataViewCorrente);

        public void SetDataVar(string prmArg, string prmInstrucao) => Vars.SetArgumento(prmArg, prmInstrucao);
        public void SetDataView(string prmArg, string prmInstrucao) => Views.SetArgumento(prmArg, prmInstrucao);
        public void SetDataFluxo(string prmArg, string prmInstrucao) => Fluxos.SetArgumento(prmArg, prmInstrucao);
        public void SetMaskDataFluxo(string prmMask) => Fluxos.SetMask(prmMask);

        public void Cleanup()
        {

            Views = new TestDataViews(this);

            Vars = new TestDataVars(this);

        }

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

        public void SetPathOUT(string prmPath)
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
