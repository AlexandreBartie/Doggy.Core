using Dooggy;
using Dooggy.Factory;
using Dooggy.Factory.Console;
using Dooggy.Lib.Data;
using Dooggy.Lib.Files;
using Dooggy.Lib.Generic;
using Dooggy.Lib.Parse;
using Dooggy.Lib.Vars;
using Dooggy.Tools.Calc;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace Dooggy.Factory.Data
{

    public enum eTipoFileFormat : int
    {
        padrao = 0,
           txt = 1,
           csv = 2,
          json = 3
    }

    public class TestDataPool
    {

        public TestFactory Factory;

        public TestDataGlobal Global;

        public TestDataLocal Local;

        public TestDataTratamento Tratamento;

        public TestConsole Console => Factory.Console;
        public TestTrace Trace => Factory.Trace;

        private bool bloqueado = false;

        public DataBasesConnection Bases => (Global.Bases);
        public TestDataConnect Connect => (Global.Connect);
        public TestDataSource Dados => (Global.Dados);
        public TestDataTags Tags => (Global.Tags);

        public TestDataViews Views => (Local.Views);
        public TestDataFlows Flows => (Local.Flows);
        public TestDataRaws Raws => (Local.Raws);
        public TestDataVars Vars => (Local.Vars);

        public DataTypesField DataTypes => (Bases.DataTypes);

        public DataBaseConnection DataBaseCorrente => (Bases.Corrente);
        public TestDataView DataViewCorrente => (Views.Corrente);

        public bool IsDbOK => (Bases.IsOK);
        public bool IsDbBlocked => bloqueado;
        public bool IsHaveData => Raws.IsHaveData || Views.IsHaveData;
        private int next_view => Views.Count + 1;

        public TestDataPool(TestFactory prmFactory)
        {

            Factory = prmFactory;

            Global = new TestDataGlobal(this);

            Local = new TestDataLocal(this);

            Tratamento = new TestDataTratamento(this);

            Cleanup();

        }

        public bool DoConnect() => Bases.DoConnect();

        public bool AddDataBase(string prmTag, string prmConexao) => Bases.Criar(prmTag, prmConexao, this);

        public string AddDataVar(string prmVar) => Vars.Criar(prmVar, DataBaseCorrente);

        public string AddDataView(string prmTag) => AddDataView(prmTag, prmMask: "");
        public string AddDataView(string prmTag, string prmMask) => Views.Criar(prmTag, prmMask, DataBaseCorrente);

        public bool AddDataFlow(string prmTag, string prmSQL, string prmMask) => DataViewCorrente.Flows.Criar(prmTag, prmSQL, prmMask, DataViewCorrente);

        public void SetDataRaw(string prmOptions) => Raws.SetOptions(prmOptions);
        public void AddDataRaw(string prmArg, string prmInstrucao) => Raws.SetArgumento(prmArg, prmInstrucao);

        public void SetDataVar(string prmArg, string prmInstrucao) => Vars.SetArgumento(prmArg, prmInstrucao);
        public void SetDataView(string prmArg, string prmInstrucao) => Views.SetArgumento(prmArg, prmInstrucao);
        public void SetDataFlow(string prmArg, string prmInstrucao) => Flows.SetArgumento(prmArg, prmInstrucao);

        public void Cleanup() => Local.Cleanup();

        public void SetDBStatus(bool prmBloqueado) => bloqueado = prmBloqueado;

        public bool IsSQLDataException(string prmTexto) => Tratamento.IsSQLDataException(prmTexto);
        public string GetTextoTratado(string prmTexto) => Tratamento.GetTextoTratado(prmTexto);
        public string GetNextKeyDataView() => string.Format("view#{0}", next_view);

        public string txt(string prmTags) => output(prmTags, prmTipo: eTipoFileFormat.txt);
        public string csv(string prmTags) => output(prmTags, prmTipo: eTipoFileFormat.csv);
        public string json(string prmTags) => output(prmTags, prmTipo: eTipoFileFormat.json);
        public string output(string prmTags, eTipoFileFormat prmTipo) => Tratamento.GetOutput(prmTags, prmTipo);

    }

    public class TestDataGlobal
    {

        private TestDataPool Pool;

        public DataBasesConnection Bases;

        public TestDataConnect Connect;

        public TestDataSource Dados;

        public TestDataTags Tags;

        public TestDataGlobal(TestDataPool prmPool)
        {
            Pool = prmPool; Cleanup();

            Bases = new DataBasesConnection(new DataTypesField());

            Connect = new TestDataConnect(prmPool);

            Dados = new TestDataSource(prmPool);

        }

        public void Cleanup()
        {
            Tags = new TestDataTags();
        }
    }
    public class TestDataLocal
    {

        private TestDataPool Pool;

        public TestDataVars Vars;
        public TestDataRaws Raws;
        public TestDataViews Views;

        public TestDataFlows Flows => (Views.Corrente.Flows);

        public TestDataLocal(TestDataPool prmPool)
        {
            Pool = prmPool; Cleanup();
        }

        public void Cleanup()
        {

            Vars = new TestDataVars(Pool);
            Raws = new TestDataRaws(Pool);
            Views = new TestDataViews(Pool);

        }


    }
    public class TestDataSource
    {

        public TestDataPool Pool;

        public TestDataFile FileINI;
        public TestDataFile FileLOG;
        public TestDataView View { get => Pool.DataViewCorrente; }

        public TestTrace Trace { get => Pool.Trace; }

        public bool IsHaveData => Pool.IsHaveData;

        public TestDataSource(TestDataPool prmPool)
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
