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
        padrao = 0,
           txt = 1,
           csv = 2,
          json = 3
    }

    public class TestDataPool
    {

        private DataBasesConnection Bases;

        public TestTrace Trace;

        public TestDataConnect Connect;

        public TestDataVars Vars;
        public TestDataRaws Raws;
        public TestDataViews Views;

        public TestDataTratamento Tratamento;

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

            Tratamento = new TestDataTratamento(this);

            Cleanup();

        }

        public bool AddDataBase(string prmTag, string prmConexao) => Bases.Criar(prmTag, prmConexao, this);

        public string AddDataVar(string prmTarget) => Vars.Criar(prmTarget, DataBaseCorrente);

        public string AddDataRaw(string prmTarget) => Vars.Criar(prmTarget, DataBaseCorrente);

        public string AddDataView(string prmTag) => AddDataView(prmTag, prmMask: "");
        public string AddDataView(string prmTag, string prmMask) => Views.Criar(prmTag, prmMask, DataBaseCorrente);

        public bool AddDataFluxo(string prmTag, string prmSQL, string prmMask) => DataViewCorrente.Fluxos.Criar(prmTag, prmSQL, prmMask, DataViewCorrente);

        public void SetDataRaw(string prmArg, string prmInstrucao) => Raws.SetArgumento(prmArg, prmInstrucao);
        public void SetDataVar(string prmArg, string prmInstrucao) => Vars.SetArgumento(prmArg, prmInstrucao);
        public void SetDataView(string prmArg, string prmInstrucao) => Views.SetArgumento(prmArg, prmInstrucao);
        public void SetDataFluxo(string prmArg, string prmInstrucao) => Fluxos.SetArgumento(prmArg, prmInstrucao);
        public void SetMaskDataFluxo(string prmMask) => Fluxos.SetMask(prmMask);

        public void Cleanup()
        {

            Vars = new TestDataVars(this);
            Raws = new TestDataRaws(this);
            Views = new TestDataViews(this);

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
        public void SetAncora(DateTime prmAncora) => Tratamento.SetAncora(prmAncora);

        public bool IsSQLDataException(string prmTexto) => Tratamento.IsSQLDataException(prmTexto);
        public string GetTextoTratado(string prmTexto) => Tratamento.GetTextoTratado(prmTexto);
        public string GetNextKeyDataView() => string.Format("x{0},", Views.Count);
        public string GetPathDestino(string prmSubPath) => PathDataFiles.GetPath(prmSubPath);

        public string txt(string prmTags) => output(prmTags, prmTipo: eTipoFileFormat.txt);
        public string csv(string prmTags) => output(prmTags, prmTipo: eTipoFileFormat.csv);
        public string json(string prmTags) => output(prmTags, prmTipo: eTipoFileFormat.json);
        public string output(string prmTags, eTipoFileFormat prmTipo) => Tratamento.GetOutput(prmTags, prmTipo);

    }

    public class TestDataTratamento : TestDataException
    {

        private DateTime ancora;

        private TestDataVars Vars => Pool.Vars;
        private TestDataRaws Raws => (Pool.Raws);
        private TestDataViews Views => (Pool.Views);

        public TestDataTratamento(TestDataPool prmPool)
        {

            Pool = prmPool;

            ancora = DateTime.Now;

        }

        public void SetAncora(DateTime prmAncora)
        {

            ancora = prmAncora;

        }
        public string GetTextoTratado(string prmTexto)
        {

            string texto = prmTexto;

            texto = GetSQLVariavel(texto);
            texto = GetSQLFuncoes(texto);

            return (texto);

        }
        public string GetOutput(string prmTags, eTipoFileFormat prmTipo)
        {

            return Raws.GetOutput(prmDados: Views.output(prmTags, prmTipo), prmTipo);

        }
        private string GetVariavel(string prmTag)
        {

            if (Vars.Find(prmTag))
                return Vars.Corrente.valor;

            return ("");

        }
        private string GetFuncao(string prmFuncao, string prmParametro)
        {

            switch (prmFuncao)
            {

                case "date":
                    return GetDataDinamica(prmParametro);

            }

            return ("");
        }

        private string GetDataDinamica(string prmParametro)
        {

            DynamicDate Date = new DynamicDate(ancora);

            return (Date.View(prmSintaxe: prmParametro));

        }

        private string GetSQLVariavel(string prmTexto)
        {

            string sql = prmTexto; string var; string var_extendido; string var_valor;

            while (true)
            {

                var_extendido = Bloco.GetBloco(sql, prmDelimitadorInicial: "$(", prmDelimitadorFinal: ")$", prmPreserve: true);

                var = Bloco.GetBloco(sql, prmDelimitadorInicial: "$(", prmDelimitadorFinal: ")$");

                if (var == "") break;

                var_valor = GetVariavel(prmTag: var);

                sql = xString.GetSubstituir(sql, var_extendido, var_valor);

                if (var_valor == "")
                    Trace.LogConsole.FailFindValueVariable(var, prmTexto);

            }

            return (sql);

        }

        private string GetSQLFuncoes(string prmSQL)
        {

            string sql = prmSQL; string funcao; string funcao_ext;
            string prefixo; string parametro; string valor;

            while (true)
            {

                funcao = Bloco.GetBloco(sql, prmDelimitadorInicial: "$", prmDelimitadorFinal: "(");

                funcao_ext = Bloco.GetBloco(sql, prmDelimitadorInicial: "$", prmDelimitadorFinal: "(", prmPreserve: true);

                prefixo = Bloco.GetBloco(sql, prmDelimitadorInicial: funcao_ext, prmDelimitadorFinal: ")$", prmPreserve: true);

                parametro = Bloco.GetBloco(sql, prmDelimitadorInicial: funcao_ext, prmDelimitadorFinal: ")$");

                if ((xString.IsEmpty(funcao)) || (xString.IsEmpty(parametro))) break;

                valor = GetFuncao(funcao, parametro);

                if (valor != "")
                    sql = xString.GetSubstituir(sql, prefixo, valor);

            }

            return (sql);

        }

    }

    public class TestDataException
    {

        public TestDataPool Pool;

        private xLista Dominio;

        private string dataSQL_ZeroItens { get => GetTag("ZeroItensSQL"); }
        private string dataSQL_NoCommand { get => GetTag("NoCommandSQL"); }

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

            Dominio.Add(dataSQL_ZeroItens);
            Dominio.Add(dataSQL_NoCommand);

        }

        public string GetZeroItens()
        {

            return (dataSQL_ZeroItens);

        }

        public string GetNoCommand()
        {

            Trace.LogData.SQLNoCommand();

            return (dataSQL_NoCommand);

        }
    }

    public class TestDataLocal
    {

        private Object Origem;

        public TestDataPool Pool;

        public TestDataFile File;

        public TestDataView View { get => Pool.DataViewCorrente; }

        public TestTrace Trace { get => Pool.Trace; }

        public TestDataLocal()
        {

            File = new TestDataFile(this);

        }

        public void Setup(Object prmOrigem, TestDataPool prmPool)
        {

            Origem = prmOrigem;

            Pool = prmPool;

        }
        public bool AddDataBase(string prmTag, string prmConexao) => (Pool.AddDataBase(prmTag, prmConexao));

        public string AddDataVar(string prmTag) => (Pool.AddDataVar(prmTag));

        public string AddDataRaw(string prmTag) => (Pool.AddDataRaw(prmTag));

        public string AddDataView(string prmTag) => (AddDataView(prmTag, prmMask: ""));
        public string AddDataView(string prmTag, string prmMask) => (Pool.AddDataView(prmTag, prmMask));

        public bool AddDataFluxo(string prmTag) => (AddDataFluxo(prmTag, prmSQL: ""));
        public bool AddDataFluxo(string prmTag, string prmSQL) => (AddDataFluxo(prmTag, prmSQL, prmMask: ""));
        public bool AddDataFluxo(string prmTag, string prmSQL, string prmMask) => (Pool.AddDataFluxo(prmTag, prmSQL, prmMask));

        public string txt(string prmTags) => (Pool.txt(prmTags));
        public string csv(string prmTags) => (Pool.csv(prmTags));
        public string json(string prmTags) => (Pool.json(prmTags));
        public string output(string prmTags, eTipoFileFormat prmTipo) => (Pool.output(prmTags, prmTipo));

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
