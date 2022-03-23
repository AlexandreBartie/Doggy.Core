using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Forms;
using System.Text;
using Dooggy.CORE;

namespace Dooggy.CORE.POC.AutomacaoTestes
{
    public class POC_AutomacaoTestesScripts : TestRobotProject
    {

        public void BASE()
        {

            //Dados.AddDataBase(prmTag: "RH", prmConexao: @"Data Source=PC-ENGENHARIA\SQLEXPRESS;Initial Catalog=QA_POC;Integrated Security=True; MultipleActiveResultSets = True");

        }

        public void DATA()
        {

  
            //Dados.AddDataModel(prmTag: "Candidato", prmModelo: @"{'#TABELAS#':'Candidatos','#CAMPOS#':'nome + sobrenome + email + nascimento'}");

            //Dados.AddDataVariant(prmTag: "=Padrao");

            //Dados.AddDataVariant(prmTag: "=Primeiro", prmRegra: @"{'#ORDEM#': 'nome ASC'}");
            //Dados.AddDataVariant(prmTag: "=Ultimo", prmRegra: @"{'#ORDEM#': 'nome DESC'}");
            //Dados.AddDataVariant(prmTag: "+Novo", prmRegra: @"{'#ORDEM#': 'nascimento DESC'}");
            //Dados.AddDataVariant(prmTag: "+Velho", prmRegra: @"{'#ORDEM#': 'nascimento ASC'}");

            //Dados.AddDataVariant(prmTag: "-Email", prmRegra: @"{'#CONDICAO#': 'email is null'}");

        }
        public void BUILD()
        {

            this.name = "POC - Automação de Testes";

            this.AddSuite(new SuiteKatalon());
            //Projeto.AddSuite(new SuiteGoogle());

        }
        public void CONFIG()
        {

            Config.EncodedDataJUNIT = Encoding.UTF7;

            Config.onlyDATA = false;

            Config.pauseAfterTestCase = 0;

        }
    }
   public class SuiteKatalon : TestRobotSuite
   {
        public SuiteKatalon()
        {
            AddScript(new Katalon.KatalonTeste());
        }

    }
    public class SuiteGoogle : TestRobotSuite
    {
        public SuiteGoogle()
        {
            AddScript(new GoogleSearch.GoogleSearchTextoTeste());
            AddScript(new GoogleSearch.GoogleSearchImagemTeste());
        }

    }
}
