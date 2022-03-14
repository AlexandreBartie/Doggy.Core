using System;
using System.Text.Json;
using System.Diagnostics;
using BlueRocket.CORE;
using BlueRocket.CORE.Lib.Generic;

namespace BlueRocket.CORE.Lib.Parse
{
    public class myJSON
    {

        public JSON_Control Controle;

        private bool _IsOK;

        public bool IsOK { get => _IsOK; }
        public bool IsErro { get => (Erro != null); }
        public bool IsCurrent { get => Controle.IsCurrent; }

        public Exception Erro { get => Controle.erro; }

        public string Flow { get => Controle.Flow; }

        public string tuplas { get => Controle.tuplas; }

        public myJSON()
        {
            Controle = new JSON_Control(this);

        }
        public myJSON(string prmFlow)
        {

            Controle = new JSON_Control(this);

            Parse(prmFlow);

        }

        public void Add(string prmFlow)
        {
            Controle.Add(prmFlow);
        }

        public void Add(string prmFlow, string prmMestre)
        {
            Controle.AddCombine(prmFlow, prmMestre);
        }
        
        public bool Parse(string prmFlow)
        {

            Add(prmFlow);

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
    public class JSON_Control
    {

        private myJSON JSON;

        private JsonDocument doc;

        private JsonElement.ArrayEnumerator Corpo;

        public bool IsCombineFull;

        public bool IsCurrent;

        public Exception erro;

        private JSON_Flows Flows;

        private JsonElement root { get => doc.RootElement; }

        //private JsonElement.ArrayEnumerator Corpo { get => root.EnumerateArray(); }


        private JsonElement item { get => Corpo.Current; }
        private JsonElement.ObjectEnumerator Propriedades { get => item.EnumerateObject(); }


        public JSON_Control(myJSON prmJSON)
        {
            
            JSON = prmJSON;

            Setup();

        }

        private void Setup()
        {

            Flows = new JSON_Flows(JSON);

        }

        public string Flow { get => (Flows.output); } 

        public string tuplas { get => (GetTuplas()); }

        public void Add(string prmFlow)
        {

            string linha = @prmFlow;

            //Flow = Flow.Replace(@"\'", @"#""");
            linha = linha.Replace(@"'", "\"");

            Flows.Add(linha);

        }

        public void AddCombine(string prmFlow, string prmMestre)
        {

            string Flow_combinado = GetCombine(prmFlow, prmMestre);

            Add(prmFlow: Flow_combinado);

        }

        private string GetCombine(string prmFlow, string prmMestre)
        {

            myJSON Flow = new myJSON(prmFlow);

            myJSON Mestre = new myJSON(prmMestre);

            // Lista acomodara o Flow Combinado

            xMemo Memo = new xMemo(";");

            // Sobrepor valores do MESTRE que estão presentes no Flow

            foreach (JsonProperty prop in Flow.Controle.Propriedades)
            {

                JsonProperty mix = prop;

                if (Mestre.Find(prmKey: prop.Name))
                { mix = Mestre.GetProperty(prmKey: prop.Name); }


                Memo.Add(string.Format("'{0}': '{1}'", mix.Name, mix.Value));

            }

            // Inserir propriedades MESTRE que não aparecem no Flow

            foreach (JsonProperty prop in Mestre.Controle.Propriedades)
            {

                if (!Flow.Find(prmKey: prop.Name))
                { Memo.Add(string.Format("'{0}': '{1}'", prop.Name, prop.Value)); }

            }

            return ("{ " + Memo.csv + " }");

        }
        public bool Save()
        {
            try
            {

                doc = JsonDocument.Parse(Flows.output);

                Corpo = root.EnumerateArray();

                Next();

                return (true);

            }

            catch (Exception e)
            { 
                
                Debug.WriteLine("Flow JSON: " + Flow);
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
    internal class JSON_Flows : xMemo
    {

        private myJSON JSON;

        public JSON_Flows(myJSON prmJSON)
        {

            JSON = prmJSON;

        }

        public string output
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