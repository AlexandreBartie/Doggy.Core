using Dooggy.LIBRARY;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dooggy.CORE
{
    public class TestDataTratamento : TestDataException
    {
        private DataVars Vars => Pool.Vars;
        private TestDataRaws Raws => (Pool.Raws);
        private TestDataViews Views => (Pool.Views);

        private DateTime dateAnchor => Pool.Connect.Format.dateAnchor;

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

                case "random":
                    return GetNumberRandom(prmParametro);
            }

            return ("");
        }
        private string GetDataDinamica(string prmParametro)
        {
            return myDate.View(prmDate: dateAnchor, prmSintaxe: prmParametro);
        }
        private string GetDataEstatica(string prmParametro)
        {
            return myDate.Static(prmDate: dateAnchor, prmFormato: prmParametro);
        }
        private string GetNumberRandom(string prmParametro)
        {
            return myFormat.RandomToString(prmDate: dateAnchor, prmParametro);
        }
    }
    public class TestDataFormat : TestDataException
    {

        public TestConfigCSV CSV => Pool.Console.Config.CSV;

        public DateTime anchor => CSV.dateAnchor;

        public string formatDateDefault => CSV.maskDateDefault;

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
}
