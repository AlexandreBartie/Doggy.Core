using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace MeuSeleniumCSharp
{
    public class QA_MassaDados
    {
        private QA_WebRobot Robot;

        public xJSON JSON = new xJSON();

        public QA_MassaDados(QA_WebRobot prmRobot)
        {

            Robot = prmRobot;

        }

        private TestProject Projeto
        { get => Robot.Projeto; }
        private QA_WebDebug Debug
        { get => Robot.Debug; }
        public bool IsOK
        { get => JSON.IsOK; }
        public bool IsONLINE
        { get => JSON.IsON; }
        public void Add(string prmFluxo)
        {
            JSON.Add(prmFluxo);
        }
        public bool Save()
        {

            if (!JSON.Save())
                Debug.Erro("Erro no JSON:Parse ", JSON.Erro); Debug.Erro(JSON.fluxo);

            return (JSON.IsOK);

        }
        public bool Next()
        { 
            return (JSON.Next()); 
        }

        public string GetValor(string prmKey, string prmPadrao)
        {
            return (JSON.GetValor(prmKey, prmPadrao));
        }

    }


}
