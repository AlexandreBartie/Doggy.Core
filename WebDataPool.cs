using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Diagnostics;

namespace MeuSeleniumCSharp
{
    public class QA_MassaDados
    {
        private QA_WebRobot Robot;

        public JsonDocument JSON;

        public JsonElement.ArrayEnumerator Entrada;

        private QA_FluxosDados Fluxos;

        public bool IsOK = true;

        public bool IsOFF = true;

        public QA_MassaDados(QA_WebRobot prmRobot)
        {

            Robot = prmRobot;

            Fluxos = new QA_FluxosDados(this);
        }

        private TestProject Projeto
        { get => Robot.Projeto; }
        private JsonElement root
        { get => JSON.RootElement; }
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

                JSON = JsonDocument.Parse(fluxoJSON);

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
    public class QA_FluxosDados
    {

        private QA_MassaDados Entrada;

        public string lista;

        public QA_FluxosDados(QA_MassaDados prmMassaDados)
        {
            Entrada = prmMassaDados;
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
