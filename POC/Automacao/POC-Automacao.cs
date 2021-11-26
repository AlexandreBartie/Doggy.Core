using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Forms;
using System.Text;

namespace Dooggy
{
    public class POC_AutomacaoTestes : TestProject
    {
        public void DATA()
        {

            Dados.AddDataBase(prmTag: "RH", prmConexao: @"Data Source=PC-ENGENHARIA\SQLEXPRESS;Initial Catalog=QA_POC;Integrated Security=True; MultipleActiveResultSets = True");

            Dados.AddDataModel(prmTag: "Candidato", prmModelo: @"{'#TABELAS#':'Candidatos','#CAMPOS#':'nome + sobrenome + email + nascimento'}");

            Dados.AddDataVariant(prmTag: "=Padrao");

            Dados.AddDataVariant(prmTag: "=Primeiro", prmRegra: @"{'#ORDEM#': 'nome ASC'}");
            Dados.AddDataVariant(prmTag: "=Ultimo", prmRegra: @"{'#ORDEM#': 'nome DESC'}");
            Dados.AddDataVariant(prmTag: "+Novo", prmRegra: @"{'#ORDEM#': 'nascimento DESC'}");
            Dados.AddDataVariant(prmTag: "+Velho", prmRegra: @"{'#ORDEM#': 'nascimento ASC'}");

            Dados.AddDataVariant(prmTag: "-Email", prmRegra: @"{'#CONDICAO#': 'email is null'}");

        }
        public void BUILD()
        {

            this.Setup(prmName: "POC - Automação de Testes");

            this.AddSuite(new SuiteKatalon());
            //this.AddSuite(new SuiteGoogle());

        }
        public void CONFIG()
        {

            this.Config.EncodedDataJUNIT = Encoding.UTF7;

            this.Config.OnlyDATA = false;

            this.Config.PauseAfterTestCase = 0;

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
