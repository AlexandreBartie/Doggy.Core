using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Forms;

namespace MeuSeleniumCSharp
{
    public class ProjetoPOC : TestProject
    {
        public void DATA()
        {

            //Hub.AddDataBase(prmTag: "RH", prmConexao: @"Data Source=PC-ENGENHARIA\SQLEXPRESS;Initial Catalog=QA_POC;Integrated Security=True");
            Hub.AddDataBase(prmTag: "RH", prmConexao: @"Data Source=PC-ENGENHARIA\SQLEXPRESS;Initial Catalog=QA_POC;Integrated Security=True; MultipleActiveResultSets = True");

            if (true)
            {

                Hub.AddDataModel(prmTag: "Candidato", prmModelo: @"{'#ENTIDADES':'Candidatos','#ATRIBUTOS':'nome + sobrenome + email'}");

                Hub.AddDataVariant(prmTag: "=Padrao");

                Hub.AddDataVariant(prmTag: "=Primeiro", prmVariacao: @"{'#ORDEM':' nome'}");
                Hub.AddDataVariant(prmTag: "=Ultimo", prmVariacao: @"{'#ORDEM':' nome DESC'}");
                Hub.AddDataVariant(prmTag: "+Novo", prmVariacao: @"{'#ORDEM':' nascimento}");
                Hub.AddDataVariant(prmTag: "+Velho", prmVariacao: @"{'#ORDEM':' nascimento DESC'}");

                Hub.AddDataVariant(prmTag: "-Email", prmVariacao: @"{'#REGRA':' email is null'}");

            }
            else
            {
                Hub.AddDataView(prmTag: "Candidato=Padrao", prmSQL: "SELECT TOP 1 nome, sobrenome, email FROM Candidatos");

                Hub.AddDataView(prmTag: "Candidato=Primeiro", prmSQL: "SELECT TOP 1 nome, sobrenome, email FROM Candidatos ORDER BY nome ASC");
                Hub.AddDataView(prmTag: "Candidato=Ultimo", prmSQL: "SELECT TOP 1 nome, sobrenome, email FROM Candidatos ORDER BY nome DESC");
                Hub.AddDataView(prmTag: "Candidato+Novo", prmSQL: "SELECT TOP 1 nome, sobrenome, email FROM Candidatos ORDER BY nascimento");
                Hub.AddDataView(prmTag: "Candidato+Velho", prmSQL: "SELECT TOP 1 nome, sobrenome, email FROM Candidatos ORDER BY nascimento DESC");

                Hub.AddDataView(prmTag: "Candidato-Email", prmSQL: "SELECT TOP 1 nome, sobrenome, email FROM Candidatos WHERE email is null");

            }

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
