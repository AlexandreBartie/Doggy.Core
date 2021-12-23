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

    public class TestDataProject : TestFactory
    {
        public TestDataConnect Connect { get => Pool.Connect; }

        public TestConsole Console;

        public TestDataProject()
        {

            Console = new TestConsole(Dados);

        }

        public void Start(string prmPathDestino)
        {

            Pool.SetPathDestino(prmPathDestino);

            Call(this, Parameters.GetDataFactoryBlockCode());

        }

        public void Start(string prmPathDestino, string prmPathINI)
        {

            Pool.SetPathDestino(prmPathDestino);

            Console.Start(prmPathINI);

        }

    }
    public class TestDataPool
    {

        private DataBasesConnection Bases;

        public TestTrace Trace;

        public TestDataConnect Connect;

        private TestDataViews Views;
        private TestDataFluxos Fluxos;
        private TestDataModels Modelos;

        private Path PathDataFiles;

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

            Views = new TestDataViews(this);
            Fluxos = new TestDataFluxos(this);
            Modelos = new TestDataModels();

            Connect = new TestDataConnect(this);

        }

        public bool AddDataBase(string prmTag, string prmConexao) => Bases.Criar(prmTag, prmConexao, this);

        public string AddDataView(string prmTag) => Views.Criar(prmTag, DataBaseCorrente);

        public bool AddDataFluxo(string prmTag, string prmSQL) => AddDataFluxo(prmTag, prmSQL, prmMask: "");
        public bool AddDataFluxo(string prmTag, string prmSQL, string prmMask) => Fluxos.Criar(prmTag, prmSQL, prmMask, DataViewCorrente);

        public bool AddDataModel(string prmTag, string prmModelo, string prmMask) => Modelos.Criar(prmTag, prmModelo, prmMask, DataBaseCorrente);
        public bool AddDataVariant(string prmTag, string prmRegra) => AddDataVariant(prmTag, prmRegra, prmQtde: 1);

        public bool AddDataVariant(string prmTag, string prmRegra, int prmQtde) => DataModelCorrente.CriarVariacao(prmTag, prmRegra, prmQtde);



        public void SetDataView(string prmArg, string prmInstrucao) => Views.SetArgumento(prmArg, prmInstrucao);
        public void SetDataFluxo(string prmArg, string prmInstrucao) => Fluxos.SetArgumento(prmArg, prmInstrucao);

        //public void SetDataModel(string prmArg, string prmInstrucao) => Modelos.SetArgumento(prmArg, prmInstrucao);
        //public void SetDataVariant(string prmArg, string prmInstrucao) => Fluxos.SetArgumento(prmArg, prmInstrucao);

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
        public string GetPathDestino(string prmSubPath) => PathDataFiles.GetPath(prmSubPath);

        public string txt(string prmTags) => Fluxos.Save(prmTags, prmTipo: eTipoFileFormat.txt);
        public string csv(string prmTags) => Fluxos.Save(prmTags, prmTipo: eTipoFileFormat.csv);
        public string json(string prmTags) => Fluxos.Save(prmTags, prmTipo: eTipoFileFormat.json);

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

        public string AddDataView(string prmTag) => (Pool.AddDataView(prmTag));

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

    public class TestDataView : TestDataMask
    {

        public string tag;

        public string descricao;

        public string saida;

        public string header_txt { get => (descricao + "," + saida); }

        public DataBaseConnection DataBase;

        public TestDataView(string prmTag, DataBaseConnection prmDataBase)
        {

            Setup(prmTag);

            DataBase = prmDataBase;

        }

        private void Setup(string prmTag)
        {

            string bloco = Blocos.GetBloco(prmTag, prmDelimitador: "|", prmPreserve: true);

            descricao = Blocos.GetBloco(bloco, prmDelimitador: "|").Trim();

            tag = Blocos.GetBlocoAntes(prmTag, bloco, prmTRIM: true);

            saida = Blocos.GetBlocoDepois(prmTag, bloco, prmTRIM: true);

        }

    }

    public class TestDataFluxo : TestDataMask
    {

        public string tag;

        public TestDataView DataView;

        public DataBaseConnection DataBase { get => DataView.DataBase; }

        // internal

        private string sql;

        private DataCursorConnection _cursor;

        public DataCursorConnection Cursor
        {
            get
            {
                if (_cursor == null)
                    _cursor = new DataCursorConnection(sql, GetMask(), DataBase);

                return (_cursor);

            }

        }

        public string tag_view { get => DataView.tag; }

        public string tag_extendida { get => tag_view + tag; }

        public string header_txt { get => DataView.header_txt; }

        public bool IsTagOk(string prmTag) => (GetTagOK(prmTag));

        public bool IsOK { get => Cursor.IsOK(); }
        public Exception erro { get => Cursor.erro; }

        public TestDataFluxo(string prmTag, string prmSQL, string prmMask, TestDataView prmDataView)
        {

            tag = prmTag;

            sql = prmSQL;

            SetMask(prmMask);

            DataView = prmDataView;

        }

        public void SetSQL(string prmSQL) => sql = prmSQL; 

        public bool Next() => Cursor.Next();

        public string GetName(int prmIndice) => Cursor.GetName(prmIndice);
        public string GetValor(int prmIndice) => Cursor.GetValor(prmIndice);
        public string GetValor(string prmNome) => Cursor.GetValor(prmNome);

        public bool Fechar() => Cursor.Fechar();


        public string csv(string prmSeparador) => Cursor.GetCSV(prmSeparador);
        public string csv() => Cursor.GetCSV();
        public string json() => Cursor.GetJSON();

        private bool GetTagOK(string prmTag)
        {

            string target_tag = prmTag.ToLower();

            string fluxo_tag = tag_extendida.ToLower();

            if (fluxo_tag.StartsWith(target_tag))
                return (true);

            return (false);

        }

        private string GetMask()
        {

            if (mask == "")
                return (DataView.mask);

            return mask;

        }

    }

    public class TestDataMask
    {
        private string _mask = "";
        public string mask { get => _mask; }

        public void SetMask(string prmMask)
        {

            _mask = prmMask;

        }


    }

    public class TestDataViews : List<TestDataView>
    {

        private TestDataPool Pool;

        public TestDataView Corrente;

        public TestDataViews(TestDataPool prmPool)
        {

            Pool = prmPool;

        }

        public string Criar(string prmTag, DataBaseConnection prmDataBase)
        {

            Corrente = new TestDataView(prmTag, prmDataBase);

            Add(Corrente);

            return Corrente.tag;

        }
        public void SetArgumento(string prmArg, string prmInstrucao)
        {

            switch (prmArg)
            {

                case "descricao":
                    Corrente.descricao = prmInstrucao;
                    break;

                case "saida":
                    Corrente.saida = prmInstrucao;
                    break;

                case "mask":
                    Corrente.SetMask(prmInstrucao);
                    break;

            }

        }

    }
    public class TestDataFluxos : List<TestDataFluxo>
    {

        private TestDataPool Pool;

        public TestDataFluxo Corrente;

        public TestDataView View { get => Pool.DataViewCorrente; }

        public TestTrace Trace { get => Pool.Trace; }

        public TestDataFluxos(TestDataPool prmPool)
        {

            Pool = prmPool;

        }
        public bool Criar(string prmTag, string prmSQL, string prmMask, TestDataView prmDataView)
        {

            if (prmDataView != null)
            {

                Corrente = new TestDataFluxo(prmTag, prmSQL, prmMask, prmDataView);

                Add(Corrente);

                return (true);
            }

            Trace.LogData.FailNoDataViewDetected(prmTag);

            return (false);

        }

        public void SetArgumento(string prmArg, string prmInstrucao)
        {

            switch (prmArg)
            {

                case "sql":
                    SetSQL(prmInstrucao);
                    break;

                case "mask":
                    SetSQL(prmInstrucao);
                    break;

            }

        }

        public void SetSQL(string prmSQL) => Corrente.SetSQL(prmSQL); 
        public void SetMask(string prmMask) => Corrente.SetMask(prmMask);

        public string Save(string prmTags, eTipoFileFormat prmTipo)
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

        private string txt(string prmTags) => txt(prmTags, prmSeparador: ",", prmColunaExtra: true);
        private string txt(string prmTags, string prmSeparador, bool prmColunaExtra)
        {

            string corpo = ""; string header = ""; string tag_view = "";
             
            string linha; string coluna_extra = "";

            if (prmColunaExtra)
                coluna_extra = prmSeparador;

            foreach (TestDataFluxo Fluxo in GetFiltro(prmTags))
            {

                //
                // Adicionar o Cabeçalho (Header) sempre que o layout da visão for diferente ...
                //

                if (tag_view != Fluxo.tag_view)
                {

                    tag_view = Fluxo.tag_view;

                    header = Fluxo.header_txt;

                    corpo += header + Environment.NewLine;

                    //Trace.LogFile.DataFileFormatTXT(header);

                }

                //
                // Montar o Corpo do arquivo TXT ...
                //

                linha = Fluxo.csv(prmSeparador);

                if (linha != "")
                    corpo += coluna_extra + linha;

                corpo += Environment.NewLine;

                //Trace.LogFile.DataFileFormatTXT(linha);

            }

            return (corpo);

        }
        private string csv(string prmLista)
        {

            string csv = "";

            foreach (TestDataFluxo Fluxo in GetFiltro(prmLista))
                csv += Fluxo.csv() + Environment.NewLine;


            return (csv);

        }
        private string json(string prmLista)
        {

            string json = ""; string separador = "";

            foreach (TestDataFluxo Fluxo in GetFiltro(prmLista))
            {

                json += separador + Fluxo.json(); separador = ", ";

            }

            return (json);

        }
        private TestDataFluxos GetFiltro(string prmLista)
        {

            TestDataFluxos filtro = new TestDataFluxos(Pool);

            foreach (string tag in new xLista(prmLista, prmSeparador: "+"))
            {

                int cont = 0;

                foreach (TestDataFluxo Fluxo in this)
                {

                    if (Fluxo.IsTagOk(tag))
                    {
                    
                        filtro.Add(Fluxo); cont++;

                    }

                }

                Trace.LogData.ViewsSelection(tag, prmQtde: cont);

            }

            return filtro;

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
