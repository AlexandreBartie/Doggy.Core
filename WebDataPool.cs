using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Diagnostics;

namespace MeuSeleniumCSharp
{
    public class QA_DataPool
    {
        private QA_WebRobot Robot;

        public JsonDocument Fonte;

        public JsonElement.ArrayEnumerator Entrada;

        private QA_DataFluxos Fluxos;

        public bool IsOK = true;

        public bool IsOFF = true;

        public QA_DataPool(QA_WebRobot prmRobot)
        {

            Robot = prmRobot;
            
            Fluxos = new QA_DataFluxos(this);
        }

        private JsonElement root
        { get => Fonte.RootElement; }
        private QA_WebDebug Debug
        { get => Robot.Debug; }
        private string fluxoJSON
        { get => Fluxos.JSON(); }
        public void Add(string prmFluxo)
        {
            Fluxos.Add(prmFluxo);
        }
        public bool Save()
        {

            try
            { 

                Fonte = JsonDocument.Parse(fluxoJSON);

                Entrada = root.EnumerateArray();

                IsOK = true;

                IsOFF = false;

            }

            catch (Exception e)
            { Debug.Erro("Erro no JSON:Parse " + e.Message); Debug.Erro(fluxoJSON); IsOK = false; }

            return IsOK;
        }
        public bool Next()
        { 
            return (Entrada.MoveNext()); 
        }

        public string GetValor(string prmKey, string prmValor)
        {

            string valor;

            try
            {
                valor = Entrada.Current.GetProperty(prmKey).GetString();
                return (valor);
            }
            catch
            { Debug.Erro("Propriedade não encontrada no JSON: " + prmKey); }

            return (prmValor);
        }

    }

    public class QA_DataFluxos
    {

        private QA_DataPool Pool;

        public string lista;

        public QA_DataFluxos(QA_DataPool prmPool)
        {
            Pool = prmPool;
        }

        public void Add(string prmFluxo)
        {

            string fluxo = @prmFluxo.Replace("'", "\"");

            Debug.Print(fluxo);
            
            if (lista == null)
                lista = fluxo;
            else
                lista += ", " + fluxo;

        }

        public string JSON()
        { return ("[ " + lista + " ]"); }

    }

}
