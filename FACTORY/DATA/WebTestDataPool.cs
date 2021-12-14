using Dooggy;
using Dooggy.Factory;
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

    public enum eTipoFileSave : int
    {
        json = 0,
        csv = 1,
        txt = 2
    }

    public class TestDataProject : TestFactory
    {

        public TestDataConnect Connect { get => Pool.Connect; }

        public void Start(string prmPath)
        {

            Pool.SetPathDestino(prmPath);

            Call(this, Parameters.GetDataFactoryBlockCode());

        }

    }
    public class TestDataPool
    {

        public TestTrace Trace;

        public TestDataConnect Connect;

        private DataBasesConnection Bases;
        private TestDataViews Visoes;
        private TestDataModels Modelos;

        private string pathDataFiles;

        public DataBaseConnection DataBaseCorrente { get => (Bases.Corrente); }

        public TestDataView DataViewCorrente { get => (Visoes.Corrente); }
        public TestDataModel DataModelCorrente { get => (Modelos.Corrente); }

        public TestTraceLogData LogData { get => (Trace.LogData); }
        public TestTraceLogFile LogFile { get => (Trace.LogFile); }

        private bool IsBaseCorrente { get => (DataBaseCorrente != null); }
        private bool IsModelCorrente { get => (DataModelCorrente != null); }

        public TestDataPool()
        {

            Trace = new TestTrace();

            Bases = new DataBasesConnection();

            Visoes = new TestDataViews(this);
            Modelos = new TestDataModels();

            Connect = new TestDataConnect(this);

        }

        public bool AddDataBase(string prmTag, string prmConexao) => Bases.AddItem(prmTag, prmConexao, this);

        public bool AddDataView(string prmTag, string prmSQL) => AddDataView(prmTag, prmSQL, prmMask: "");
        public bool AddDataView(string prmTag, string prmSQL, string prmMask) => Visoes.AddItem(prmTag, prmSQL, prmMask, DataBaseCorrente);

        public void SetMaskDataView(string prmMask) => Visoes.SetMask(prmMask);

        public bool AddDataModel(string prmTag, string prmModelo, string prmMask) => Modelos.AddItem(prmTag, prmModelo, prmMask, DataBaseCorrente);

        public bool AddDataVariant(string prmTag, string prmRegra) => AddDataVariant(prmTag, prmRegra, prmQtde: 1);

        public bool AddDataVariant(string prmTag, string prmRegra, int prmQtde) => DataModelCorrente.CriarVariacao(prmTag, prmRegra, prmQtde);

        public bool SetView(string prmTag) => Visoes.SetView(prmTag);

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

            pathDataFiles = prmPath;

            LogFile.SetPath(prmContexto: "MassaTestes", prmPath);

        }

        public string GetPath(string prmSubPath)
        {
            if (prmSubPath != "")
                return (pathDataFiles + @"\" + prmSubPath + @"\"); 

            return (pathDataFiles);
        }

        public string csv(string prmTags) => Visoes.csv(prmTags);
        public string json(string prmTags) => Visoes.json(prmTags);
        public string txt(string prmTags, string prmSeparador, bool prmColunaExtra) => Visoes.txt(prmTags, prmSeparador, prmColunaExtra);

    }
    public class TestDataLocal
    {

        private Object Origem;

        public TestDataPool Pool;

        public TestDataFile File;

        public TestDataKeyDriven KeyDriven;

        public TestDataView View { get => Pool.DataViewCorrente; }

        public string tagView { get => View.tag; }

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

        public bool AddDataView(string prmTag, string prmSQL) => (AddDataView(prmTag, prmSQL, prmMask: ""));
        public bool AddDataView(string prmTag, string prmSQL, string prmMask) => (Pool.AddDataView(prmTag, prmSQL, prmMask));

        public void AddDataModel(string prmTag, string prmModelo) => AddDataModel(prmTag, prmModelo, prmMask: "");
        public void AddDataModel(string prmTag, string prmModelo, string prmMask) => Pool.AddDataModel(prmTag, prmModelo, prmMask);

        public void AddDataVariant(string prmTag) => AddDataVariant(prmTag, prmRegra: "");
        public void AddDataVariant(string prmTag, string prmRegra) => Pool.AddDataVariant(prmTag, prmRegra);

        public string Save(string prmTags, eTipoFileSave prmTipo)
        {

            switch (prmTipo)
            {

                case eTipoFileSave.csv:
                    return csv(prmTags);

                case eTipoFileSave.txt:
                    return txt(prmTags);

            }

            return json(prmTags);

        }

        public string csv(string prmTags) => (Pool.csv(prmTags));
        public string json(string prmTags) => (Pool.json(prmTags));

        public string txt(string prmTags) => txt(prmTags, prmColunaExtra: true);
        public string txt(string prmTags, bool prmColunaExtra) => txt(prmTags, prmSeparador: ",", prmColunaExtra);
        public string txt(string prmTags, string prmSeparador, bool prmColunaExtra) => Pool.txt(prmTags, prmSeparador, prmColunaExtra);

    }

    public class TestDataKeyDriven
    {

        public TestDataPool Pool;

        public TestDataKeyDriven(TestDataPool prmPool)
        {

            Pool = prmPool;

        }

        public void SetMaskDataView(string prmTag, string prmMask) => Pool.SetMaskDataView(prmMask);


    }
    public class TestDataView : TestDataMask
    {


        public string tag;

        public DataBaseConnection DataBase;

        // internal

        private string sql;

        private DataCursorConnection _cursor;

        public DataCursorConnection Cursor
        {
            get
            {
                if (_cursor == null)
                    _cursor = new DataCursorConnection(sql, mask, DataBase);

                return (_cursor);

            }


        }

        public bool IsOK { get => Cursor.IsOK(); }
        public Exception erro { get => Cursor.erro; }

        public TestDataView(string prmTag, string prmSQL, string prmMask, DataBaseConnection prmDataBase)
        {

            tag = prmTag;

            sql = prmSQL;

            SetMask(prmMask);

            DataBase = prmDataBase;

        }

        public bool Next() => Cursor.Next();

        public string GetName(int prmIndice) => Cursor.GetName(prmIndice);
        public string GetValor(int prmIndice) => Cursor.GetValor(prmIndice);
        public string GetValor(string prmNome) => Cursor.GetValor(prmNome);

        public bool Fechar() => Cursor.Fechar();


        public string csv(string prmSeparador) => Cursor.GetCSV(prmSeparador);
        public string csv() => Cursor.GetCSV();
        public string json() => Cursor.GetJSON();

        public string log_sql() => String.Format("VIEW:[{0,25}] SQL: {1}", tag, sql);
        public string log_data() => String.Format("DATA:[{0,25}] Fluxo: {1}", tag, json());

    }

    public class TestDataMask
    {

        private string _mask;

        public string mask { get => _mask; }

        public void SetMask(string prmMask) { _mask = prmMask; }

    }
    public class TestDataViews : List<TestDataView>
    {

        private TestDataPool Pool;

        public TestDataView Corrente;

        public TestTrace Trace { get => Pool.Trace; }

        public TestDataViews(TestDataPool prmPool)
        {

            Pool = prmPool;

        }

        public bool AddItem(string prmTag, string prmSQL, string prmMask, DataBaseConnection prmDataBase)
        {

            if (prmDataBase != null)
            {

                Corrente = new TestDataView(prmTag, prmSQL, prmMask, prmDataBase);

                Add(Corrente);

                return (true);
            }

            return (false);

        }

        public void SetMask(string prmMask) { Corrente.SetMask(prmMask); }

        public bool SetView(string prmTag)
        {

            foreach (TestDataView Visao in this)
            {

                if (Visao.tag == prmTag)
                {

                    Corrente = Visao;

                    return (true);

                }

            }

            return (false);

        }
        public string txt(string prmTags, string prmSeparador, bool prmColunaExtra)
        {

            string tags;
            string header;
             
            string txt = "xxx" + Environment.NewLine;

            string linha; string extra = "";

            if (prmColunaExtra)
                extra = prmSeparador;

            foreach (TestDataView Visao in GetFiltro(prmTags))
            {

                linha = Visao.csv(prmSeparador);

                if (linha != "")
                    txt += extra + linha;

                txt += Environment.NewLine;

            }

            return (txt);

        }
        public string csv(string prmLista)
        {

            string csv = "";

            foreach (TestDataView Visao in GetFiltro(prmLista))
                csv += Visao.csv() + Environment.NewLine;


            return (csv);

        }
        public string json(string prmLista)
        {

            string json = ""; string separador = "";

            foreach (TestDataView Visao in GetFiltro(prmLista))
            {

                json += separador + Visao.json(); separador = ", ";

            }

            return (json);

        }
        private TestDataViews GetFiltro(string prmLista)
        {

            TestDataViews filtro = new TestDataViews(Pool);

            xLista lista = new xLista(prmLista);

            foreach (TestDataView view in this)
            {

                if (lista.IsContem(view.tag))
                    filtro.Add(view);

            }

            return (GetTrace(lista, filtro));

        }

        private TestDataViews GetTrace(xLista prmLista, TestDataViews prmViews)
        {

            foreach (string item in prmLista)
            {

                int cont = 0;

                foreach (TestDataView view in this)
                {

                    if (view.tag.ToLower().StartsWith(item.ToLower())) cont++;

                }

                Trace.LogData.ViewsSelection(prmTag: item, prmQtde: cont);

            }
            return (prmViews);

        }

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
