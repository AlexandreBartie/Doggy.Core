using Katty;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace Dooggy
{

    public enum eTipoFileFormat : int
    {
        padrao = 0,
           txt = 1,
           csv = 2,
          json = 3
    }

    public class DataPool
    {

        public TestFactory Factory;

        public DataLocal Local;

        private DataResume Resume;

        public TestDataTratamento Tratamento;

        public TestConsole Console => Factory.Console;
        public TestTrace Trace => Factory.Trace;

        public DataBases Bases => (Connect.Bases);
        public DataConnect Connect => (Resume.Connect);
        public DataSource Dados => (Resume.Dados);
        public DataGlobal Global => (Resume.Global);

        public TestDataViews Views => (Local.Views);
        public TestDataFlows Flows => (Local.Flows);
        public TestDataRaws Raws => (Local.Raws);
        public DataVars Vars => (Local.Vars);

        public DataBase DataBaseCorrente => (Bases.Corrente);
        public TestDataView DataViewCorrente => (Views.Corrente);

        public bool IsHaveData => Raws.IsHaveData || Views.IsHaveData;
        private int next_view => Views.Count + 1;

        public DataPool(TestFactory prmFactory)
        {

            Factory = prmFactory;

            Local = new DataLocal(this);

            Resume = new DataResume(this);

            Tratamento = new TestDataTratamento(this);

            Cleanup();

        }

        public bool DoConnect() => Bases.DoConnect();

        public bool AddDataBase(string prmTag, string prmConexao) => Bases.Criar(prmTag, prmConexao);

        public string AddGlobalVAR(string prmVar) => Global.Vars.Criar(prmVar);
        public void SetGlobalVAR(string prmArg, string prmInstrucao) => Global.Vars.SetArgumento(prmArg, prmInstrucao);

        public string AddLocalVAR(string prmVar) => Vars.Criar(prmVar);
        public void SetLocalVAR(string prmArg, string prmInstrucao) => Vars.SetArgumento(prmArg, prmInstrucao);


        public string AddDataView(string prmTag) => AddDataView(prmTag, prmMask: "");
        public string AddDataView(string prmTag, string prmMask) => Views.Criar(prmTag, prmMask, DataBaseCorrente);

        public bool AddDataFlow(string prmTag, string prmSQL, string prmMask) => DataViewCorrente.Flows.Criar(prmTag, prmSQL, prmMask, DataViewCorrente);

        public void SetDataRaw(string prmOptions) => Raws.SetOptions(prmOptions);
        public void AddDataRaw(string prmArg, string prmInstrucao) => Raws.SetArgumento(prmArg, prmInstrucao);


        public void SetDataView(string prmArg, string prmInstrucao) => Views.SetArgumento(prmArg, prmInstrucao);
        public void SetDataFlow(string prmArg, string prmInstrucao) => Flows.SetArgumento(prmArg, prmInstrucao);

        public void Cleanup() => Local.Cleanup();

        public bool IsSQLDataException(string prmTexto) => Tratamento.IsSQLDataException(prmTexto);
        public string GetTextoTratado(string prmTexto) => Tratamento.GetTextoTratado(prmTexto);
        public string GetNextKeyDataView() => string.Format("view#{0}", next_view);

        public string txt(string prmTags) => output(prmTags, prmTipo: eTipoFileFormat.txt);
        public string csv(string prmTags) => output(prmTags, prmTipo: eTipoFileFormat.csv);
        public string json(string prmTags) => output(prmTags, prmTipo: eTipoFileFormat.json);
        public string output(string prmTags, eTipoFileFormat prmTipo) => Tratamento.GetOutput(prmTags, prmTipo);

    }

    public class DataResume
    {
        private DataPool Pool;

        public DataConnect Connect;

        public DataSource Dados;

        public DataGlobal Global;

        private TestTrace Trace => Pool.Trace;

        public DataResume(DataPool prmPool)
        {
            Pool = prmPool; 

            Connect = new DataConnect(Trace);

            Dados = new DataSource(prmPool);

            Global = new DataGlobal(prmPool);
        }

        public void Cleanup()
        {
            Global.Cleanup();
        }
    }
    public class DataGlobal
    {

        private DataPool Pool;

        public DataTags Tags;

        public DataVars Vars;

        public DataGlobal(DataPool prmPool)
        {
            Pool = prmPool;  Cleanup();
        }

        public void Cleanup()
        {
            Tags = new DataTags(Pool);

            Vars = new DataVars(Pool);
        }

    }
    public class DataLocal
    {

        private DataPool Pool;

        public DataVars Vars;
        public TestDataRaws Raws;
        public TestDataViews Views;

        public TestDataFlows Flows => (Views.Corrente.Flows);

        public DataLocal(DataPool prmPool)
        {
            Pool = prmPool; Cleanup();
        }

        public void Cleanup()
        {
            Vars = new DataVars(Pool);
            Raws = new TestDataRaws(Pool);
            Views = new TestDataViews(Pool);
        }


    }
    public class DataSource
    {

        public DataPool Pool;

        public TestDataFile FileINI;
        public TestDataFile FileLOG;
        public TestDataView View { get => Pool.DataViewCorrente; }

        public TestTrace Trace { get => Pool.Trace; }

        public bool IsHaveData => Pool.IsHaveData;

        public DataSource(DataPool prmPool)
        {

            Pool = prmPool;

            FileINI = new TestDataFile(this, prmExtensao: "ini");
            FileLOG = new TestDataFile(this, prmExtensao: "log");

        }

        public bool DoConnect() => Pool.DoConnect();

        public string AddDataView(string prmTag) => (AddDataView(prmTag, prmMask: ""));
        public string AddDataView(string prmTag, string prmMask) => (Pool.AddDataView(prmTag, prmMask));

        public bool AddDataFlow(string prmTag) => (AddDataFlow(prmTag, prmSQL: ""));
        public bool AddDataFlow(string prmTag, string prmSQL) => (AddDataFlow(prmTag, prmSQL, prmMask: ""));
        public bool AddDataFlow(string prmTag, string prmSQL, string prmMask) => (Pool.AddDataFlow(prmTag, prmSQL, prmMask));

        public string txt(string prmTags) => (Pool.txt(prmTags));
        public string csv(string prmTags) => (Pool.csv(prmTags));
        public string json(string prmTags) => (Pool.json(prmTags));

        public string output(eTipoFileFormat prmTipo) => output(prmTags: "", prmTipo);
        public string output(string prmTags, eTipoFileFormat prmTipo) => (Pool.output(prmTags, prmTipo));

        public string log => Pool.Bases.log();

    }
}
