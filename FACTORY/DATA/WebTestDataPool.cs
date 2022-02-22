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

        public DataBasesConnection Bases;

        public TestDataConnect Connect;

        public TestDataLocal Dados;

        public TestDataVars Vars;
        public TestDataRaws Raws;
        public TestDataViews Views;

        public TestDataTratamento Tratamento;

        public TestConsole Console => Factory.Console;
        public TestTrace Trace => Factory.Trace;

        private bool bloqueado = false;

        public TestDataFlows Flows => (DataViewCorrente.Flows);

        public DataTypesField DataTypes => (Bases.DataTypes);
        public DataBaseConnection DataBaseCorrente => (Bases.Corrente);
        public TestDataView DataViewCorrente => (Views.Corrente);
        public TestDataFlow DataFlowCorrente => (Flows.Corrente);

        public bool IsDbOK => (Bases.IsOK);
        public bool IsDbBlocked => bloqueado;
        public bool IsHaveData => Raws.IsHaveData || Views.IsHaveData;
        private int next_view => Views.Count + 1;

        public TestDataPool(TestFactory prmFactory)
        {

            Factory = prmFactory;

            Bases = new DataBasesConnection(new DataTypesField());

            Dados = new TestDataLocal(this);

            Connect = new TestDataConnect(this);

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

        public void Cleanup()
        {

            Vars = new TestDataVars(this);
            Raws = new TestDataRaws(this);
            Views = new TestDataViews(this);

        }

        public void SetDBStatus(bool prmBloqueado) => bloqueado = prmBloqueado;

        public bool IsSQLDataException(string prmTexto) => Tratamento.IsSQLDataException(prmTexto);
        public string GetTextoTratado(string prmTexto) => Tratamento.GetTextoTratado(prmTexto);
        public string GetNextKeyDataView() => string.Format("view#{0}", next_view);

        public string txt(string prmTags) => output(prmTags, prmTipo: eTipoFileFormat.txt);
        public string csv(string prmTags) => output(prmTags, prmTipo: eTipoFileFormat.csv);
        public string json(string prmTags) => output(prmTags, prmTipo: eTipoFileFormat.json);
        public string output(string prmTags, eTipoFileFormat prmTipo) => Tratamento.GetOutput(prmTags, prmTipo);

    }
    public class TestDataTratamento : TestDataFormat
    {
        private TestDataVars Vars => Pool.Vars;
        private TestDataRaws Raws => (Pool.Raws);
        private TestDataViews Views => (Pool.Views);

        private string varDataFlow = "#(flow)";

        private string varDataInput = "#(input)";
        private string varDataOutput = "#(output)";
        private string varDataFull => varDataInput + "," + varDataOutput;

        private string varDelimitadorInicial = "#(";
        private string varDelimitadorFinal = ")";

        private string fncDelimitadorPrefixo = "$";
        private string fncDelimitadorInicial = "(";
        private string fncDelimitadorFinal = ")";

        public TestDataTratamento(TestDataPool prmPool)
        {
            Pool = prmPool;
        }

        public string GetOutput(string prmTags, eTipoFileFormat prmTipo)
        {

            string data_view = Views.output(prmTags, prmTipo);

            if (Raws.IsON)
                return Raws.GetOutput(data_view, prmTipo);

            return data_view;

        }
        public string GetSQLTratado(string prmSql, TestDataHeader prmHeader)
        {
            string sql = myString.GetSubstituir(prmSql, varDataFlow, varDataFull);

            sql = GetTuplasTratadas(sql, varDataInput, prmHeader.Input);
            sql = GetTuplasTratadas(sql, varDataOutput, prmHeader.Output.GetVariavel());

            sql = GetTextoTratado(sql);

            return sql;
        }
        public string GetTextoTratado(string prmTexto)
        {
            string texto = prmTexto;

            texto = GetVariavelTratada(texto);
            texto = GetFuncaoTratada(texto);

            return (texto);
        }

        private string GetTuplasTratadas(string prmSql, string prmVariavel, myTuplas prmTuplas)
        {
            string sql = myString.GetSubstituir(prmSql, prmVariavel, prmTuplas.sql);

            foreach (myTupla tupla in prmTuplas)
            {
                if (tupla.TemVariavel)
                    Trace.LogConsole.SetValueVariable(tupla.name, tupla.value_sql);

                sql = myString.GetSubstituir(sql, tupla.var_sql, tupla.value_sql);
            }
            return sql;
        }
        
        private string GetVariavelTratada(string prmTexto)
        {
            string sql = prmTexto; string arg; string arg_extendido; string arg_valor;

            while (true)
            {
                arg_extendido = Bloco.GetBloco(sql, varDelimitadorInicial, varDelimitadorFinal, prmPreserve: true);

                arg = Bloco.GetBloco(sql, varDelimitadorInicial, varDelimitadorFinal);

                if (arg == "") break;

                arg_valor = Vars.GetValor(prmTag: arg);

                if (arg_valor == "")
                    Trace.LogConsole.FailFindVariable(arg, prmTexto);

                sql = myString.GetSubstituir(sql, arg_extendido, arg_valor);
            }

            return (sql);
        }

        private string GetFuncaoTratada(string prmSQL)
        {

            string sql = prmSQL; string funcao; string funcao_ext;
            string code; string parametro; string valor;

            while (true)
            {

                funcao = Bloco.GetBloco(sql, fncDelimitadorPrefixo, fncDelimitadorInicial);

                funcao_ext = Bloco.GetBloco(sql, fncDelimitadorPrefixo, fncDelimitadorInicial, prmPreserve: true);

                code = Bloco.GetBloco(sql, prmDelimitadorInicial: funcao_ext, fncDelimitadorFinal, prmPreserve: true);

                parametro = Bloco.GetBloco(sql, prmDelimitadorInicial: funcao_ext, fncDelimitadorFinal);

                if ((myString.IsEmpty(funcao)) || (myString.IsEmpty(parametro))) break;

                valor = GetFuncao(funcao, parametro);

                if (valor == "")
                    Trace.LogConsole.FailFindFunction(funcao, code);

                sql = myString.GetSubstituir(sql, code, valor);

            }

            return (sql);
        }

        private string GetFuncao(string prmFuncao, string prmParametro)
        {

            switch (prmFuncao)
            {
                case "date":
                    return GetDataDinamica(prmParametro);

                case "now":
                case "today":
                    return GetDataDinamica(prmParametro);
            }

            return ("");
        }
        private string GetDataDinamica(string prmParametro)
        {
            return (myDate.View(prmDate: anchor, prmSintaxe: prmParametro));
        }
        private string GetDataEstatica(string prmParametro)
        {
            return (myDate.Static(prmDate: anchor, prmFormato: prmParametro));
        }
    }
    public class TestDataFormat : TestDataException
    {

        public TestConfigCSV CSV => Pool.Console.Config.CSV;

        public DateTime anchor => CSV.anchor;

        public string formatDateDefault => CSV.formatDateDefault;

        public string GetDateAnchor() => GetDateAnchor(formatDateDefault);
        public string GetDateAnchor(string prmFormat) => GetDateFormat(anchor, prmFormat);

        public string GetTextFormat(string prmText, string prmFormat) => CSV.TextToCSV(prmText, prmFormat);

        public string GetDateFormat(DateTime prmDate) => GetDateFormat(prmDate, prmFormat: "");
        public string GetDateFormat(DateTime prmDate, string prmFormat)
        {
            string format = prmFormat;

            if (myString.IsEmpty(format))
                format = formatDateDefault;

            return CSV.DateToCSV(prmDate, format);
        }
        public string GetDoubleFormat(double prmNumber) => GetDoubleFormat(prmNumber, prmFormat: "");
        public string GetDoubleFormat(double prmNumber, string prmFormat) => CSV.DoubleToCSV(prmNumber, prmFormat);



    }
    public class TestDataException
    {

        public TestDataPool Pool;

        private xLista Dominio;

        private string dataSQL_NoFindSQLCommand { get => GetTag("NoFindSQLCommand"); }
        private string dataSQL_ZeroItensSQLResult { get => GetTag("ZeroItensSQLResult"); }


        public bool IsSQLDataException(string prmItem) => (Dominio.GetContido(prmItem) != 0);

        private string GetTag(string prmTexto) => string.Format("<#$#{0}#$#>", prmTexto);

        public TestTrace Trace => (Pool.Trace);

        public TestDataException()
        {

            PopularDominio();

        }

        private void PopularDominio()
        {
            Dominio = new xLista();

            Dominio.Add(dataSQL_ZeroItensSQLResult);
            Dominio.Add(dataSQL_NoFindSQLCommand);

        }

        public string GetZeroItens()
        {

            return (dataSQL_ZeroItensSQLResult);

        }

        public string GetNoCommand()
        {

            Trace.LogData.FailFindSQLCommand();

            return (dataSQL_NoFindSQLCommand);

        }
    }
    public class TestDataLocal
    {

        public TestDataPool Pool;

        public TestDataFile FileINI;
        public TestDataFile FileLOG;
        public TestDataView View { get => Pool.DataViewCorrente; }

        public TestTrace Trace { get => Pool.Trace; }

        public bool IsHaveData => Pool.IsHaveData;

        public TestDataLocal(TestDataPool prmPool)
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
