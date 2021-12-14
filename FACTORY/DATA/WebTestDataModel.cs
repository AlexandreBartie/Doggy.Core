using Dooggy.Lib.Data;
using Dooggy.Lib.Files;
using Dooggy.Lib.Generic;
using Dooggy.Lib.Parse;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dooggy.Factory.Data
{
    public class TestDataImport
    {

        private TestDataLocal Dados;

        private xFileTXT File = new xFileTXT();

        public TestTraceLogFile Log { get => Dados.Pool.LogFile; }

        public TestDataImport(TestDataLocal prmDados)
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
    public class TestDataModel : TestDataMask
    {

        public string tag;

        public xJSON Parametros;

        public DataBaseConnection DataBase;

        private TestDataViews Visoes;

        private TestDataVariant Variacao;

        public TestDataPool Pool { get => DataBase.Pool; }

        public TestTrace Trace { get => Pool.Trace; }

        public TestDataModel(string prmTag, string prmModel, string prmMask, DataBaseConnection prmDataBase)
        {

            tag = prmTag;

            Parametros = new xJSON(prmModel);

            SetMask(prmMask);

            DataBase = prmDataBase;

            Variacao = new TestDataVariant(this);

            Visoes = new TestDataViews(Pool);

            if (Parametros.IsErro)
                Trace.LogData.FailSQLNoDataModelConnection(prmTag, prmModel, Parametros.Erro);

        }

        public bool CriarView(string prmTag, string prmSQL)
        {

            if (Pool.AddDataView(prmTag, prmSQL, mask))
            { Visoes.Add(Pool.DataViewCorrente); return (true); }

            return (false);

        }
        public bool CriarVariacao(string prmTag, string prmRegras, int prmQtde) => Variacao.Criar(prmTag, prmRegras, prmQtde);

        public string GetListaTabelas() => (TratarParametros(Parametros.GetValor("#TABELAS#")));

        public string GetListaCampos() => (TratarParametros(Parametros.GetValor("#CAMPOS#", prmPadrao: "*")));

        private string TratarParametros(string prmLista) => new xMemo(prmLista, prmSeparador: "+").memo(", ");

    }
    public class TestDataVariant
    {

        private xJSON Regras;

        public TestDataModel Modelo;

        private string GetTagExtendida(string prmTag) => Modelo.tag + prmTag;

        private bool IsRegraOK { get => (Regras.IsOK); }

        public TestDataVariant(TestDataModel prmModelo)
        {

            Modelo = prmModelo;

        }

        public bool Criar(string prmTag, string prmRegras, int prmQtde)
        {

            Regras = new xJSON(prmRegras);

            return (Modelo.CriarView(GetTagExtendida(prmTag), prmSQL: GetSQL(prmQtde)));

        }

        private string GetSQL(int prmQtde)
        {

            string sql = MontaSELECT(prmQtde);

            if (IsRegraOK)
                sql += " " + MontaEXTENSAO();

            return (MontaSQL(sql, prmQtde));

        }

        private string MontaSQL(string prmSQL, int prmQtde) => (string.Format("SELECT * FROM ({0}) WHERE ROWNUM = {1}", prmSQL, prmQtde));

        private string MontaSELECT(int prmQtde) => (string.Format("SELECT {0} FROM {1}", Modelo.GetListaCampos(), Modelo.GetListaTabelas()));

        private string MontaEXTENSAO() => (string.Format("{0} {1}", MontaWHERE(), MontaORDERBY()).Trim());

        private string MontaWHERE() => (Regras.FindValor("#CONDICAO#", "WHERE {0}"));

        private string MontaORDERBY() => (Regras.FindValor("#ORDEM#", "ORDER BY {0}"));

    }
    public class TestDataModels : List<TestDataModel>
    {

        public TestDataModel Corrente;

        public bool AddItem(string prmTag, string prmModel, string prmMask, DataBaseConnection prmDataBase)
        {

            if (prmDataBase != null)
            {

                Corrente = new TestDataModel(prmTag, prmModel, prmMask, prmDataBase);

                Add(Corrente);

                return (true);
            }

            return (false);

        }

        public bool SetModel(string prmTag)
        {

            foreach (TestDataModel Model in this)
            {

                if (Model.tag == prmTag)
                {

                    Corrente = Model;

                    return (true);

                }

            }

            return (false);

        }

    }
}
