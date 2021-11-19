using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Forms;

namespace Dooggy
{
    public class ProjetoPOC : TestProject
    {
        public void DATA()
        {

            Dados.AddDataBase(prmTag: "RH", prmConexao: @"Data Source=PC-ENGENHARIA\SQLEXPRESS;Initial Catalog=QA_POC;Integrated Security=True; MultipleActiveResultSets = True");

            Dados.AddDataModel(prmTag: "Candidato", prmModelo: @"{'#ENTIDADES#':'Candidatos','#ATRIBUTOS#':'nome + sobrenome + email + nascimento'}");

            Dados.AddDataVariant(prmTag: "=Padrao");

            Dados.AddDataVariant(prmTag: "=Primeiro", prmRegra: @"{'#ORDEM#': 'nome ASC'}");
            Dados.AddDataVariant(prmTag: "=Ultimo", prmRegra: @"{'#ORDEM#': 'nome DESC'}");
            Dados.AddDataVariant(prmTag: "+Novo", prmRegra: @"{'#ORDEM#': 'nascimento DESC'}");
            Dados.AddDataVariant(prmTag: "+Velho", prmRegra: @"{'#ORDEM#': 'nascimento ASC'}");

            Dados.AddDataVariant(prmTag: "-Email", prmRegra: @"{'#REGRAS#': 'email is null'}");

        }
        public void BUILD()
        {

            this.name = "POC - Automação de Testes";

            //this.AddSuite(new SuiteKatalon());
            this.AddSuite(new SuiteGoogle());

        }
        public void CONFIG()
        {

            this.Config.PathFileSources = @"C:\Users\alexa\OneDrive\Área de Trabalho\";

            this.Config.OnlyDATA = true;

            this.Config.PauseAfterTestCase = 2;

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
