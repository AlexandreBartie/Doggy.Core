using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Forms;

namespace MeuSeleniumCSharp
{
    public class POC : TestProject
    {
        public void DATA()
        {

            Hub.AddDataBase(prmTag: "RH", prmConexao: @"Data Source=PC-ENGENHARIA\SQLEXPRESS;Initial Catalog=QA_POC;Integrated Security=True;MultipleActiveResultSets=True");

            Hub.AddDataView(prmTag: "CandidatoNovo", prmSQL: "SELECT nome, sobrenome, genero FROM Candidatos ORDER BY nascimento");
            Hub.AddDataView(prmTag: "Candidatos", prmSQL: "SELECT nome, sobrenome, genero FROM Candidatos");
            Hub.AddDataView(prmTag: "CandidatosOrdenadosASC", prmSQL: "SELECT nome, sobrenome, genero FROM Candidatos ORDER BY nome ASC");
            Hub.AddDataView(prmTag: "CandidatosOrdenadosDESC", prmSQL: "SELECT nome, sobrenome, genero FROM Candidatos ORDER BY nome DESC");

        }
        public void BUILD()
        {

            Hub.name = "POC - Automação de Testes";

            Hub.AddSuite(new SuiteKatalon());
            //Hub.AddSuite(new SuiteGoogle());

        }
        public void START()
        {

            Hub.Config.PauseAfterTestCase = 5;

            Hub.Config.OnlyDATA = true;

            Hub.Executar(prmTipoDriver: eTipoDriver.ChromeDriver);

        }
    }
   public class SuiteKatalon : TestSuite
   {
        public SuiteKatalon()
        {
            AddScript(new Katalon.KatalonTeste());
        }

    }
    public class SuiteGoogle : TestSuite
    {
        public SuiteGoogle()
        {
            AddScript(new GoogleSearch.GoogleSearchTextoTeste());
            AddScript(new GoogleSearch.GoogleSearchImagemTeste());
        }

    }
}
