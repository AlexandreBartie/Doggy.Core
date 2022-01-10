using Dooggy.Factory;
using Dooggy.Factory.Data;
using Dooggy.Lib.Data;
using Dooggy.Lib.Generic;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dooggy.Factory.Data
{
    public class TestDataView : TestDataSQL
    {

        public DataBaseConnection DataBase;

        private string key;

        private string alias;

        public string descricao;

        public string saida;

        public TestDataFluxos Fluxos;

        public string tag { get => GetTag(); }
        public string header_txt { get => (descricao + "," + saida); }

        public TestDataPool Pool { get => DataBase.Pool; }

        private TestTrace Trace { get => Pool.Trace; }

        public TestDataView(string prmTag, DataBaseConnection prmDataBase)
        {

            DataBase = prmDataBase;

            Setup(prmTag);

            Cleanup();

        }

        private void Setup(string prmTag)
        {

            string bloco = Bloco.GetBloco(prmTag, prmDelimitador: "|", prmPreserve: true);

            descricao = Bloco.GetBloco(bloco, prmDelimitador: "|").Trim();

            alias = Bloco.GetBlocoAntes(prmTag, bloco, prmTRIM: true);

            saida = Bloco.GetBlocoDepois(prmTag, bloco, prmTRIM: true);

            key = Pool.GetNextKeyDataView();

        }

        public void Cleanup()
        {

            Fluxos = new TestDataFluxos(Pool);

        }

        private string GetTag()
        {

            if (alias == "")
                return (key);

            return (alias);

        }

    }
    public class TestDataFluxo : TestDataSQL
    {

        public string tag;

        public TestDataView View;

        public DataBaseConnection DataBase { get => View.DataBase; }

        // internal

        private DataCursorConnection _cursor;

        public DataCursorConnection Cursor
        {
            get
            {
                if (_cursor == null)
                    _cursor = new DataCursorConnection(GetSQL(View), GetMask(), DataBase);

                return (_cursor);

            }

        }

        public string tag_view { get => View.tag; }

        public string header_txt { get => View.header_txt; }

        public bool IsOK { get => Cursor.IsOK(); }
        public Exception erro { get => Cursor.erro; }

        public TestDataFluxo(string prmTag, string prmSQL, string prmMask, TestDataView prmDataView)
        {

            tag = prmTag;

            SetSQL(prmSQL);

            SetMask(prmMask);

            View = prmDataView;

        }

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
                return (View.mask);

            return mask;

        }

    }
    public class TestDataSQL
    {

        public string sql;

        public string tabelas;

        public string campos;

        public string relacoes;

        public string filtro;

        public string ordem;

        public string mask;

        private bool TemSQL() => (xString.IsStringOK(sql));
        private bool TemCombinacoes() => TemRelacoes() && TemFiltro();
        private bool TemCondicoes() => TemRelacoes() || TemFiltro();
        private bool TemRelacoes() => (xString.IsStringOK(relacoes));
        private bool TemFiltro() => (xString.IsStringOK(filtro));
        private bool TemOrdem() => (xString.IsStringOK(ordem));
        public void SetSQL(string prmSQL) => sql = prmSQL;
        public void SetTabelas(string prmTabelas) => tabelas = prmTabelas;
        public void SetCampos(string prmCampos) => campos = prmCampos;
        public void SetRelacoes(string prmRelacoes) => relacoes = prmRelacoes;
        public void SetFiltro(string prmFiltro) => filtro = prmFiltro;
        public void SetOrdem(string prmOrdem) => ordem = prmOrdem;
        public void SetMask(string prmMask) => mask = prmMask;

        public string GetSQL(TestDataView prmView)
        {

            string retorno= sql;

            if (!TemSQL())
            {

                tabelas = prmView.tabelas;

                campos = prmView.campos;

                relacoes = prmView.relacoes;

                retorno = GetSQL();

            }

            retorno = prmView.Pool.GetTextoTratado(retorno);

            return (retorno);

        }

        private string GetSQL()
        {

            string sql = String.Format("SELECT {0} FROM {1}", campos, tabelas);

            string condicoes = ""; string ordenacao = "";

            if (TemCombinacoes())
                condicoes = String.Format("({0}) AND ({1})", relacoes, filtro);

            else if (TemRelacoes())
                condicoes = relacoes;

            else if (TemFiltro())
                condicoes = filtro;

            if (TemCondicoes())
                condicoes = String.Format(" WHERE {0}", condicoes);

            if (TemOrdem())
                ordenacao = String.Format(" ORDER BY {0}", ordem);

            sql += condicoes + ordenacao;

            return (sql.Trim());


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

                case "tabelas":
                    Corrente.SetTabelas(prmInstrucao);
                    break;

                case "campos":
                    Corrente.SetCampos(prmInstrucao);
                    break;

                case "relacoes":
                    Corrente.SetRelacoes(prmInstrucao);
                    break;
                
                case "mask":
                    Corrente.SetMask(prmInstrucao);
                    break;

                case "saida":
                    Corrente.saida = prmInstrucao;
                    break;

            }

        }
        public string output(string prmTags, eTipoFileFormat prmTipo) 
        {

            TestDataFluxos filtro = new TestDataFluxos(Pool);

            xLista lista = new xLista(prmTags, prmSeparador: "+");

            foreach (TestDataView View in this)
            {

                if (xString.IsEmpty(prmTags) || lista.IsEqual(View.tag))
                    filtro.AddItens(View.Fluxos);

            }

            return filtro.output(prmTipo);

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

                case "filtro":
                    Corrente.SetFiltro(prmInstrucao);
                    break;

                case "ordem":
                    Corrente.SetOrdem(prmInstrucao);
                    break;

                case "mask":
                    SetSQL(prmInstrucao);
                    break;

            }

        }

        public void SetSQL(string prmSQL) => Corrente.SetSQL(prmSQL);
        public void SetMask(string prmMask) => Corrente.SetMask(prmMask);

        public string output(eTipoFileFormat prmTipo)
        {

            switch (prmTipo)
            {

                case eTipoFileFormat.txt:
                    return txt();

                case eTipoFileFormat.csv:
                    return csv();

                case eTipoFileFormat.json:
                    return json();

            }

            return json();

        }

        private string txt() => txt(prmSeparador: ",", prmColunaExtra: true);
        private string txt(string prmSeparador, bool prmColunaExtra)
        {

            string header = ""; string tag_view = ""; int cont = 0;

            string corpo = ""; string linha; string coluna_extra = "";

             if (prmColunaExtra)
                coluna_extra = prmSeparador;

            foreach (TestDataFluxo Fluxo in this)
            {

                cont++;

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

    public class TestDataRaws
    {

        private xMemo Itens;

        public TestDataPool Pool;

        private TestTrace Trace { get => Pool.Trace; }

        private string output { get => Itens.memo(prmSeparador: Environment.NewLine) + Environment.NewLine ; }

        public TestDataRaws(TestDataPool prmPool)
        {

            Pool = prmPool;

            Itens = new xMemo();

        }

        public void SetArgumento(string prmArg, string prmInstrucao)
        {

            switch (prmArg)
            {

                case "null":
                    Itens.Add(prmInstrucao);
                    break;

                case "*":
                    break;

            }

        }
        public string GetOutput(string prmDados)
        {

            Merge(prmDados);

            return (Pool.GetTextoTratado(output));

        }

        private void Merge(string prmDados)
        {

            int cont = 0;

            xLista Dados = new xLista(prmDados, prmSeparador: Environment.NewLine);

            foreach (string linha in Dados)
            {

                cont++;

                if (Dados.qtde > cont)
                    Itens.Add(cont, linha);

            }

        }

    }

}
