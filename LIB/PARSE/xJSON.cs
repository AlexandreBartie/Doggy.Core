using System;
using System.Text.Json;
using System.Diagnostics;

namespace Dooggy.LIB.PARSE
{
    public class xJSON
    {

        public IJSONcontrol Controle;

        private bool _IsOK;

        public bool IsOK { get => _IsOK; }

        public bool IsCurrent { get => Controle.IsCurrent; }

        public Exception Erro { get => Controle.erro; }

        public string fluxo { get => Controle.fluxo; }

        public string log { get => Controle.log; }

        public xJSON()
        {
            Controle = new IJSONcontrol(this);

        }
        public xJSON(string prmFluxo)
        {

            Controle = new IJSONcontrol(this);

            Parse(prmFluxo);

        }

        public void Add(string prmFluxo)
        {
            Controle.Add(prmFluxo);
        }

        public void Add(string prmFluxo, string prmMestre)
        {
            Controle.AddCombine(prmFluxo, prmMestre);
        }

        public bool Parse(string prmFluxo)
        {

            Add(prmFluxo);

            return (Save());

        }
        public bool Save()
        {

            _IsOK = Controle.Save();

            return (IsOK);

        }
        public bool Next() => Controle.Next();

        public bool Find(string prmKey) => Controle.Find(prmKey);

        public string FindValor(string prmKey, string prmFormato) => Controle.FindValor(prmKey, prmFormato);
        public string GetValor(string prmKey) => Controle.GetValor(prmKey);
        public string GetValor(string prmKey, string prmPadrao) => Controle.GetValor(prmKey, prmPadrao);

        public JsonProperty GetProperty (string prmKey) => Controle.GetProperty(prmKey);

    }
    public class IJSONcontrol
    {

        private xJSON JSON;

        private JsonDocument doc;

        private JsonElement.ArrayEnumerator Corpo;

        private string lista;

        public bool IsCombineFull;

        public bool IsCurrent;

        public Exception erro;

        private JsonElement root { get => doc.RootElement; }
        private JsonElement item { get => Corpo.Current; }
        private JsonElement.ObjectEnumerator Propriedades { get => item.EnumerateObject(); }

        public IJSONcontrol(xJSON prmJSON)
        {
            JSON = prmJSON;
        }
        public string fluxo { get { if (lista == "") return ("[ ]"); else return ("[ " + lista + " ]"); } }

        public string log { get => (String.Format( "Fluxo: {0}", lista)); }

        public void Add(string prmFluxo)
        {

            string fluxo = @prmFluxo.Replace("'", "\"");

            if (lista == null)
                lista = fluxo;
            else
                lista += ", " + fluxo;

        }

        public void AddCombine(string prmFluxo, string prmMestre)
        {

            string fluxo_combinado = GetCombine(prmFluxo, prmMestre);

            Add(prmFluxo: fluxo_combinado);

        }

        private string GetCombine(string prmFluxo, string prmMestre)
        {

            xJSON Fluxo = new xJSON(prmFluxo);

            xJSON Mestre = new xJSON(prmMestre);

            // Lista acomodara o Fluxo Combinado

            xMemo Memo = new xMemo(";");

            // Sobrepor valores do MESTRE que estão presentes no FLUXO

            foreach (JsonProperty prop in Fluxo.Controle.Propriedades)
            {

                JsonProperty mix = prop;

                if (Mestre.Find(prmKey: prop.Name))
                { mix = Mestre.GetProperty(prmKey: prop.Name); }


                Memo.Add(string.Format("'{0}': '{1}'", mix.Name, mix.Value));

            }

            // Inserir propriedades MESTRE que não aparecem no Fluxo

            foreach (JsonProperty prop in Mestre.Controle.Propriedades)
            {

                if (!Fluxo.Find(prmKey: prop.Name))
                { Memo.Add(string.Format("'{0}': '{1}'", prop.Name, prop.Value)); }

            }


            return ("{ " + Memo.memo(", ") + " }");

        }
        public bool Save()
        {
            try
            {

                doc = JsonDocument.Parse(fluxo);

                Corpo = root.EnumerateArray();

                Next();

                return (true);

            }

            catch (Exception e)
            { Debug.WriteLine("Erro na Formataçao de Dados JSON."); lista = ""; erro = e; return (false); }

        }
        public bool Next()
        {

            IsCurrent = Corpo.MoveNext();

            return (IsCurrent);
        }

        public bool Find(string prmKey)
        {

            JsonProperty prop = GetProperty(prmKey);

            try
            { string x = prop.Name; return (true); }
            catch 
            {  }

            return (false);

        }
        public string FindValor(string prmKey, string prmFormato)
        {

            string vlValor = GetValor(prmKey);

            if (vlValor != "")
                return (String.Format(prmFormato, vlValor));

            return (vlValor);

        }
        public string GetValor(string prmKey) => GetValor(prmKey, prmPadrao: "");

        public string GetValor(string prmKey, string prmPadrao)
        {
            try
            {
                return (GetProperty(prmKey).Value.GetString());

            }
            catch (Exception e)
            { erro = e; }

            return (prmPadrao);
        }

        public JsonProperty GetProperty(string prmKey)
        {

            foreach (JsonProperty propriedade in Propriedades)
            {

                if (propriedade.Name.ToLower() == prmKey.ToLower())
                {
                    return propriedade;
                }
            }

            return(new JsonProperty());
        }
    }

}
