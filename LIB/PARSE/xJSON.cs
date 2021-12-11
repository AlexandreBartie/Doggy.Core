using System;
using System.Text.Json;
using System.Diagnostics;
using Dooggy.Lib.Generic;

namespace Dooggy.Lib.Parse
{
    public class xJSON
    {

        public xJSON_Control Controle;

        private bool _IsOK;

        public bool IsOK { get => _IsOK; }
        public bool IsErro { get => (Erro != null); }
        public bool IsCurrent { get => Controle.IsCurrent; }

         public Exception Erro { get => Controle.erro; }

        public string fluxo { get => Controle.fluxo; }

        public string tuplas { get => Controle.tuplas; }

        public xJSON()
        {
            Controle = new xJSON_Control(this);

        }
        public xJSON(string prmFluxo)
        {

            Controle = new xJSON_Control(this);

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
    public class xJSON_Control
    {

        private xJSON JSON;

        private JsonDocument doc;

        private JsonElement.ArrayEnumerator Corpo;

        public bool IsCombineFull;

        public bool IsCurrent;

        public Exception erro;

        private xJSON_Fluxos Fluxos;

        private JsonElement root { get => doc.RootElement; }

        //private JsonElement.ArrayEnumerator Corpo { get => root.EnumerateArray(); }


        private JsonElement item { get => Corpo.Current; }
        private JsonElement.ObjectEnumerator Propriedades { get => item.EnumerateObject(); }


        public xJSON_Control(xJSON prmJSON)
        {
            
            JSON = prmJSON;

            Setup();

        }

        private void Setup()
        {

            Fluxos = new xJSON_Fluxos(JSON);

        }

        public string fluxo { get => (Fluxos.txt); } 

        public string tuplas { get => (GetTuplas()); }

        public void Add(string prmFluxo)
        {

            string linha = @prmFluxo;

            //fluxo = fluxo.Replace(@"\'", @"#""");
            linha = linha.Replace(@"'", "\"");

            Fluxos.Add(linha);

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

                doc = JsonDocument.Parse(Fluxos.txt);

                Corpo = root.EnumerateArray();

                Next();

                return (true);

            }

            catch (Exception e)
            { 
                
                Debug.WriteLine("Fluxo JSON: " + fluxo);
                Debug.WriteLine("Erro  JSON: " + e.Message);

                Setup();  erro = e; return (false); }

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
        private string GetTuplas()
        {

            xMemo linhas = new xMemo();

            foreach (JsonElement item in Corpo)

                foreach (JsonProperty propriedade in item.EnumerateObject())

                {

                    string parametro = propriedade.Name;

                    string linha = string.Format("[{0}]: '{1}'", parametro, GetValor(parametro));

                    linhas.Add(linha);

                }

            return (linhas.csv);

        }

    }

    public class xJSON_Fluxos : xMemo
    {

        private xJSON JSON;

        public xJSON_Fluxos(xJSON prmJSON)
        {

            JSON = prmJSON;

        }

        public string txt
        {
            get

            {

                if (IsVazio)
                    return ("[ ]");
                else
                    return ("[ " + csv + " ]");

            }

        }

    }

}