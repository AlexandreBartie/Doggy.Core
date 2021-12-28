using Dooggy.Factory;
using Dooggy.Factory.Data;
using Dooggy.Lib.Data;
using Dooggy.Lib.Generic;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dooggy.Factory.Data
{
    public class TestDataView : TestDataMask
    {

        public DataBaseConnection DataBase;

        public string tag;

        public string descricao;

        public string saida;


        public TestDataFluxos Fluxos;

        public string header_txt { get => (descricao + "," + saida); }

        private TestDataPool Pool { get => DataBase.Pool; }

        public TestDataView(string prmTag, DataBaseConnection prmDataBase)
        {

            DataBase = prmDataBase;

            Setup(prmTag);

            Fluxos = new TestDataFluxos(Pool);

        }

        private void Setup(string prmTag)
        {

            string bloco = Blocos.GetBloco(prmTag, prmDelimitador: "|", prmPreserve: true);

            descricao = Blocos.GetBloco(bloco, prmDelimitador: "|").Trim();

            tag = Blocos.GetBlocoAntes(prmTag, bloco, prmTRIM: true);

            saida = Blocos.GetBlocoDepois(prmTag, bloco, prmTRIM: true);

        }

        public void Cleanup()
        {




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

        public string header_txt { get => DataView.header_txt; }

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

        public TestDataPool Pool;

        public TestDataView Corrente;

        private TestTrace Trace { get => Pool.Trace; }

        public TestDataViews(TestDataPool prmPool)
        {

            Pool = prmPool;

        }

        public string Criar(string prmTag, DataBaseConnection prmDataBase) => Criar(prmTag, prmMask: "", prmDataBase);
        public string Criar(string prmTag, string prmMask, DataBaseConnection prmDataBase)
        {

            if (Find(prmTag))

                Corrente.Cleanup();

            else
            {

                Corrente = new TestDataView(prmTag, prmDataBase);

                Add(Corrente);

            }

            Corrente.SetMask(prmMask);

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
        public string Save(string prmTags, eTipoFileFormat prmTipo) 
        {

            TestDataFluxos filtro = new TestDataFluxos(Pool);

            xLista lista = new xLista(prmTags, prmSeparador: "+");

            foreach (TestDataView View in this)
            {

                if (lista.IsEqual(View.tag))
                    filtro.AddItens(View.Fluxos);

            }

            return filtro.Save(prmTipo);

        }
        private TestDataViews GetFiltro(string prmLista)
        {

            TestDataViews filtro = new TestDataViews(Pool);

            foreach (string tag in new xLista(prmLista, prmSeparador: "+"))
            {

                int cont = 0;

                foreach (TestDataView View in this)
                {

                    if (xString.IsEqual(View.tag, tag))
                    {

                        filtro.Add(View); cont++;

                    }

                }

                Trace.LogData.ViewsSelection(tag, prmQtde: cont);

            }

            return filtro;

        }
        private bool Find(string prmTag)
        {

            foreach (TestDataView view in this)

                if (xString.IsEqual(view.tag, prmTag))
                {

                    Corrente = view;

                    return (true);

                }

            return (false);

        }

    }
    public class TestDataFluxos : List<TestDataFluxo>
    {

        private TestDataPool Pool;

        public TestDataFluxo Corrente;

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

        public void AddItens(TestDataFluxos prmFluxos)
        {

            foreach (TestDataFluxo fluxo in prmFluxos)
                Add(fluxo);
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

        public string Save(eTipoFileFormat prmTipo)
        {

            switch (prmTipo)
            {

                case eTipoFileFormat.csv:
                    return csv();

                case eTipoFileFormat.txt:
                    return txt();

            }

            return json();

        }

        private string txt() => txt(prmSeparador: ",", prmColunaExtra: true);
        private string txt(string prmSeparador, bool prmColunaExtra)
        {

            string corpo = ""; string header = ""; string tag_view = "";

            string linha; string coluna_extra = "";

            if (prmColunaExtra)
                coluna_extra = prmSeparador;

            foreach (TestDataFluxo Fluxo in this)
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
        private string csv()
        {

            string csv = "";

            foreach (TestDataFluxo Fluxo in this)
                csv += Fluxo.csv() + Environment.NewLine;


            return (csv);

        }
        private string json()
        {

            string json = ""; string separador = "";

            foreach (TestDataFluxo Fluxo in this)
            {

                json += separador + Fluxo.json(); separador = ", ";

            }

            return (json);

        }

    }

}
