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

            if (false)
                Dados.AddDataBase(prmTag: "RH", prmConexao: @"Data Source=PC-ENGENHARIA\SQLEXPRESS;Initial Catalog=QA_POC;Integrated Security=True; MultipleActiveResultSets = True");

            if (false)
            {

                Dados.AddDataModel(prmTag: "Candidato", prmModelo: @"{'#ENTIDADES':'Candidatos','#ATRIBUTOS':'nome + sobrenome + email + nascimento'}");

                Dados.AddDataVariant(prmTag: "=Padrao");

                Dados.AddDataVariant(prmTag: "=Primeiro", prmVariacao: @"{'#ORDEM': 'nome'}");
                Dados.AddDataVariant(prmTag: "=Ultimo", prmVariacao: @"{'#ORDEM': 'nome DESC'}");
                Dados.AddDataVariant(prmTag: "+Novo", prmVariacao: @"{'#ORDEM': 'nascimento'}");
                Dados.AddDataVariant(prmTag: "+Velho", prmVariacao: @"{'#ORDEM': 'nascimento DESC'}");

                Dados.AddDataVariant(prmTag: "-Email", prmVariacao: @"{'#REGRAS': 'email is null'}");

            }
            if (false)
            {
                
                //Hub.AddDataView(prmTag: "Candidato=Padrao", prmSQL: "SELECT TOP 1 nome, sobrenome, email FROM Candidatos WHERE id = 10");

                //Hub.AddDataView(prmTag: "Candidato=AnalistaFinanceiro", prmSQL: "SELECT TOP 1 nome, sobrenome, email FROM Candidatos ORDER BY nome ASC WHERE depto = 1");
                //Hub.AddDataView(prmTag: "Candidato=GerenteFinanceiro", prmSQL: "SELECT TOP 1 nome, sobrenome, email FROM Candidatos ORDER BY nome DESC");
                //Hub.AddDataView(prmTag: "Candidato+Novo", prmSQL: "SELECT TOP 1 nome, sobrenome, email FROM Candidatos ORDER BY nascimento");
                //Hub.AddDataView(prmTag: "Candidato+Velho", prmSQL: "SELECT TOP 1 nome, sobrenome, email FROM Candidatos ORDER BY nascimento DESC");

                //Hub.AddDataView(prmTag: "Candidato-Email", prmSQL: "SELECT TOP 1 nome, sobrenome, email FROM Candidatos WHERE email is null");

            }

        }
        public void BUILD()
        {

            this.name = "POC - Automação de Testes";

            this.AddSuite(new SuiteKatalon());
            //this.AddSuite(new SuiteGoogle());

        }
        public void START()
        {

            this.Config.PauseAfterTestCase = 5;

            this.Config.OnlyDATA = true;

            this.Executar(prmTipoDriver: eTipoDriver.ChromeDriver);

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
