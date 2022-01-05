using Dooggy.Lib.Generic;
using System;
using System.Collections.Generic;
using System.Text;
using static Dooggy.xInt;

namespace Dooggy
{
    public enum eTipoMarcaData { dia, mes, ano }

    public class DynamicDate
    {

        public DynamicDateRules Rules;

        private DynamicDateExport Export;

        public DateTime ancora;

        public DynamicDate(DateTime prmAncora)
        {

            ancora = prmAncora;

            Rules = new DynamicDateRules(this);

            Export = new DynamicDateExport(this);

        }

        public string View(string prmSintaxe) => Rules.View(prmSintaxe);
        public DateTime Calc(string prmSintaxe) => (Rules.Calc(prmSintaxe));


    }
    public class DynamicDateRules
    {

        private DynamicDate Raiz;

        private DynamicDateImport Import;

        public DynamicDateAnchor Anchor;

        public DynamicDateRelative Relative;

        public DynamicDateRestrict Restrict;

        public DynamicDateFormat Format;

        public DynamicDateRules(DynamicDate prmRaiz)
        {

            Raiz = prmRaiz;

            Import = new DynamicDateImport(Raiz);

            Format = new DynamicDateFormat(Raiz);

            Anchor = new DynamicDateAnchor(Raiz);

            Relative = new DynamicDateRelative(Raiz);

            Restrict = new DynamicDateRestrict(Raiz);
 
        }

        public DateTime Calc(string prmSintaxe)
        {

            Setup(prmSintaxe);

            DateTime data = Raiz.ancora;

            data = Anchor.GetDate(data);
            data = Relative.GetDate(data);

            return (data);

        }
        public string View(string prmSintaxe) => (Format.GetView(prmData: Calc(prmSintaxe)));
        private void Setup(string prmSintaxe)
        {

            Anchor.Setup();

            Relative.Setup();

            Import.Parse(prmSintaxe);

        }

        public bool IsMarcaOK(string prmTipoData, string prmTipoOperador) => (IsTipoDataOK(prmTipoData) && IsTipoOperadorOK(prmTipoOperador));

        public void SetFixo(string prmTipoData, int prmValor)
        {

            Anchor.SetValor(prmTipoData, prmValor: xInt.GetPositivo(prmValor));

        }

        public void SetRelative(string prmTipoData, int prmValor)
        {

            Relative.SetValor(prmTipoData, prmValor);

        }

        public void SetFormat(string prmFormat) => Format.Setup(prmFormat);

        private bool IsTipoDataOK(string prmTipoData) => (xString.IsContem("DMA", prmTipoData));

        private bool IsTipoOperadorOK(string prmTipoOperador) => (xString.IsContem("=+-", prmTipoOperador));

    }
    public class DynamicDateFormat
    {

        private DynamicDate Raiz;

        private string formatacao_data;

        public string formatacao_padrao { get => "DD/MM/AAAA"; }

        public bool IsPadrao() => ((formatacao_data == formatacao_padrao));

        public DynamicDateFormat(DynamicDate prmRaiz)
        {

            Raiz = prmRaiz;

        }

        public void Setup(string prmFormat) => formatacao_data = prmFormat;

        public string GetView(DateTime prmData)
        {

            string formato = GetFormatacaoAtual();

            string retorno = prmData.ToString(formato);

            return (retorno);

        }

        private string GetFormatacaoAtual() 
        {

            string formato;
            
            if (formatacao_data != "") 
                formato = formatacao_data;
            else
               formato = formatacao_padrao;

            return (GetFormatacaoAjustada(formato));
        
        }

        private string GetFormatacaoAjustada(string prmFormato)
        {

            string formato = GetFormatacao(prmFormato);

            formato = xString.GetSubstituir(formato, "A", "y");
            formato = xString.GetSubstituir(formato, "a", "y");

            formato = xString.GetSubstituir(formato, "D", "d");
            formato = xString.GetSubstituir(formato, "m", "M");

            return (formato);

        }
        private string GetFormatacao(string prmFormato)
        {

            if (prmFormato == "")
                return (formatacao_data);

            return (prmFormato);

        }
    }
    public class DynamicDateAnchor : DynamicDateParameters
    {
        public DynamicDateAnchor(DynamicDate prmRaiz)
        {

            Raiz = prmRaiz;

        }
        public DateTime GetDate(DateTime prmDataAncora)
        {

            int _dia; int _mes; int _ano;

            if (dia == 0)
                _dia = prmDataAncora.Day;
            else
                _dia = dia;

            if (mes == 0)
                _mes = prmDataAncora.Month;
            else
                _mes = mes;

            if (ano == 0)
                _ano = prmDataAncora.Year;
            else
                _ano = ano;

            //
            // Manter valores dentro dos intervalos estabelecidos
            //

            _ano = GetIntervalo(prmValor: _ano, prmMinimo: 1, prmMaximo: 2500);

            _mes = GetIntervalo(prmValor: _mes, prmMinimo: 1, prmMaximo: 12);

            _dia = GetIntervalo(prmValor: _dia, prmMinimo: 1, prmMaximo: DateTime.DaysInMonth(_ano, _mes));

            return new System.DateTime(_ano, _mes, _dia);

        }

    }
    public class DynamicDateRelative : DynamicDateParameters
    {

        public DynamicDateRestrict Restrict => Raiz.Rules.Restrict;

        public DynamicDateRelative(DynamicDate prmRaiz)
        {

            Raiz = prmRaiz;

        }
        public DateTime GetDate(DateTime prmDataAncora)
        {

            DateTime data = prmDataAncora;

            data = data.AddMonths(mes);
            data = data.AddYears(ano);

            data = Restrict.GetDate(data, prmDesvio: dia);

            return (data);

        }

    }
    public class DynamicDateParameters
    {

        public DynamicDate Raiz;

        public int dia;
        public int mes;
        public int ano;

        public void Setup()
        {

            ano = 0;
            mes = 0;
            dia = 0;

        }

        public void SetValor(string prmTipoData, int prmValor)
        {

            string tipo = prmTipoData.ToLower();

            switch (tipo)
            {

                case "d":
                    dia = prmValor;
                    break;

                case "m":
                    mes = prmValor;
                    break;

                case "a":
                    ano = prmValor;
                    break;

            }

        }

    }
    public class DynamicDateRestrict
    {

        private DynamicDate Raiz;

        public bool segunda;
        public bool terca;
        public bool quarta;
        public bool quinta;
        public bool sexta;
        public bool sabado;
        public bool domingo;

        public string GetRestringeDomingo() { if (domingo) return "1"; return ""; }
        public string GetRestringeSegunda() { if (segunda) return "2"; return ""; }
        public string GetRestringeTerca() { if (terca) return "3"; return ""; }
        public string GetRestringeQuarta() { if (quarta) return "4"; return ""; }
        public string GetRestringeQuinta() { if (quinta) return "5"; return ""; }
        public string GetRestringeSexta() { if (sexta) return "6"; return ""; }
        public string GetRestringeSabado() { if (sabado) return "7"; return ""; }

        public DynamicDateRestrict(DynamicDate prmRaiz)
        {

            Raiz = prmRaiz;

        }

        public void Importar(string prmParametro)
        {

            string vlParametro = prmParametro;

            vlParametro = xString.GetSubstituir(vlParametro, "UTIL", "17");

            domingo = xString.GetFind(vlParametro, prmParte: "1");
            segunda = xString.GetFind(vlParametro, prmParte: "2");
            terca = xString.GetFind(vlParametro, prmParte: "3");
            quarta = xString.GetFind(vlParametro, prmParte: "4");
            quinta = xString.GetFind(vlParametro, prmParte: "5");
            sexta = xString.GetFind(vlParametro, prmParte: "6");
            sabado = xString.GetFind(vlParametro, prmParte: "7");

        }

        public DateTime GetDate(DateTime prmData, int prmDesvio)
        {

            int deslocamento; int desvio;

            DateTime data = GetNextDate(prmData, prmAvancar: true);

            if (prmDesvio != 0)
            {

                bool avancar = xInt.IsPositivo(prmDesvio);

                desvio = xInt.GetPositivo(prmDesvio);

                deslocamento = xInt.GetInverter(avancar, prmValor: 1);

                for (int loop = 1; loop <= desvio; loop++)
                {

                    data = GetNextDate(prmData: data.AddDays(deslocamento), avancar);

                }

            }

            return (data);

        }

        private DateTime GetNextDate(DateTime prmData, bool prmAvancar) => (GetNextDate(prmData, xInt.GetInverter(prmAvancar, prmValor: 1)));
        private DateTime GetNextDate(DateTime prmData, int prmIncremento)
        {

            if (IsDiaRestricao(prmData))
            {

                return GetNextDate(prmData.AddDays(prmIncremento), prmIncremento);

            }

            return (prmData);

        }

        private bool IsDiaRestricao(DateTime prmData)
        {

            switch (prmData.DayOfWeek)
            {

                case DayOfWeek.Sunday:
                    return (domingo);

                case DayOfWeek.Monday:
                    return (segunda);

                case DayOfWeek.Tuesday:
                    return (terca);

                case DayOfWeek.Wednesday:
                    return (quarta);

                case DayOfWeek.Thursday:
                    return (quinta);

                case DayOfWeek.Friday:
                    return (sexta);

                case DayOfWeek.Saturday:
                    return (sabado);

            }

            return (false);

        }

    }
    public class DynamicDateImport
    {

        public DynamicDate Raiz;

        public DynamicDateRules Rules => Raiz.Rules;

        public DynamicDateImport(DynamicDate prmRaiz)
        {

            Raiz = prmRaiz;

        }

        public void Parse(string prmSintaxe)
        {

            ParseMarcas(prmParametro: ObterParametroAnalise(prmSintaxe));
            ParseFormatacao(prmParametro: ObterParametroFormat(prmSintaxe));
            ParseRestricao( ObterParametroAnalise(prmSintaxe));

        }

        private void ParseMarcas(string prmParametro)
        {

            foreach (string item in new xLista(prmParametro, "|"))

                AtualizarMarca(prmMarca: item);

        }

        private void AtualizarMarca(string prmMarca)
        {

            string marca; string tipo_data; string tipo_operador; string parametro; int valor;

            marca = xString.GetNoBlank(prmMarca);

            tipo_data = xString.GetFirst(marca);
            tipo_operador = xString.GetChar(marca, 2);

            if (Rules.IsMarcaOK(tipo_data, tipo_operador))
            {

                parametro = xString.GetLast(prmMarca, prmTamanho: marca.Length - 2);

                if (tipo_operador == "=")
                {

                    valor = Convert.ToInt32(parametro);

                    Rules.SetFixo(tipo_data, valor);

                }

                else
                {

                    valor = Convert.ToInt32(tipo_operador + parametro);

                    Rules.SetRelative(tipo_data, valor);

                }
            
            }

        }

        public void ParseFormatacao(string prmParametro)
        { Rules.SetFormat(prmFormat: prmParametro); }

        public void ParseRestricao(string prmParametro)
        { ; }

        private string ObterParametroAnalise(string prmSintaxe) => Bloco.GetBlocoAntes(prmSintaxe, prmDelimitador: ":");

        private string ObterParametroFormat(string prmSintaxe) => Bloco.GetBlocoDepois(prmSintaxe, prmDelimitador: ":");

        private string ObterParametroRestricao(string prmSintaxe) => Bloco.GetBlocoDepois(prmSintaxe, prmDelimitador: ":");

    }
    public class DynamicDateExport
    {

        public DynamicDate Raiz;

        public DynamicDateRules Rules => Raiz.Rules;

        public DynamicDateFormat Format => Rules.Format;

        public DynamicDateExport(DynamicDate prmRaiz)
        {

            Raiz = prmRaiz;

        }
        public string GetSintaxeData(string prmSintaxe, string prmFormatacao)
        {

            DateTime data = Raiz.Calc(prmSintaxe);

            return GetSintaxeDataCalculada(data, prmFormatacao);

        }
        private string GetSintaxeDataCalculada(DateTime prmDataAncora, string prmFormatacao) => prmDataAncora.ToString(GetSintaxeDataRelativa());
        private string GetSintaxeDataRelativa()
        {

            string valor = ""; string delimitador = "|";

            valor = AculumularValorAtualizado(ObterReferencia("D", Rules.Relative.dia), valor, delimitador);
            valor = AculumularValorAtualizado(ObterReferencia("M", Rules.Relative.mes), valor, delimitador);
            valor = AculumularValorAtualizado(ObterReferencia("A", Rules.Relative.ano), valor, delimitador);

            valor = AculumularValorAtualizado(ObterExatamente("D", Rules.Anchor.dia), valor, delimitador);
            valor = AculumularValorAtualizado(ObterExatamente("M", Rules.Anchor.mes), valor, delimitador);
            valor = AculumularValorAtualizado(ObterExatamente("A", Rules.Anchor.ano), valor, delimitador);

            if (valor == "")
                valor = "D+0";

            return (valor);

        }
        private string GetSintaxeDataRestricao()
        {

            string valor = ""; string delimitador = "";

            valor = AculumularValorAtualizado(Rules.Restrict.GetRestringeDomingo(), valor, delimitador);
            valor = AculumularValorAtualizado(Rules.Restrict.GetRestringeSegunda(), valor, delimitador);
            valor = AculumularValorAtualizado(Rules.Restrict.GetRestringeTerca(), valor, delimitador);
            valor = AculumularValorAtualizado(Rules.Restrict.GetRestringeQuarta(), valor, delimitador);
            valor = AculumularValorAtualizado(Rules.Restrict.GetRestringeQuinta(), valor, delimitador);
            valor = AculumularValorAtualizado(Rules.Restrict.GetRestringeSexta(), valor, delimitador);
            valor = AculumularValorAtualizado(Rules.Restrict.GetRestringeSabado(), valor, delimitador);

            if (valor == "17")
                valor = "UTIL";

            return (valor);

        }
        private string GetSintaxeCorrente()
        {

            string sintaxe = GetSintaxeDataRelativa();

            if (Format.IsPadrao())
            {

                if (GetSintaxeDataRestricao() != "")
                    sintaxe = sintaxe + ":";

            }
            else
                sintaxe = "";// AdicionarLista(sintaxe, Format.GetFormatacaoAtual(), ":", true);


            return "";//AdicionarLista(sintaxe, GetSintaxeDataRestricao(), ":", true);

        }

        private string AculumularValorAtualizado(string prmNovoValor, string prmValorAcumulado, string prmDelimitador)
        {

            string valor = prmValorAcumulado;

            if (prmNovoValor != "")

                if (prmValorAcumulado == "")
                    valor = prmNovoValor;

                else
                    valor += prmDelimitador + prmNovoValor;


            return valor;

        }
        private string ObterReferencia(string prmPrefixo, int prmValor)
        {

            string retorno = "";

            if (prmValor != 0)
            {

                string txt_valor = Convert.ToString(System.Math.Abs(prmValor));

                if (prmValor > 0)
                    retorno = prmPrefixo + "+" + txt_valor;
                else
                    retorno = prmPrefixo + "-" + txt_valor;

            }

            return (retorno);

        }
        private string ObterExatamente(string prmPrefixo, int prmValor)
        {

            string retorno = "";

            if (prmValor != 0)
            {

                string txt_valor = Convert.ToString(System.Math.Abs(prmValor));

                retorno = prmPrefixo + "=" + txt_valor;

            }

            return (retorno);

        }

    }

}
