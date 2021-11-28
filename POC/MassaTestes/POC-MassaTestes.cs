using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Text;
using Dooggy.Factory.Robot;
using Dooggy.Lib.Data;

namespace Dooggy
{
    public class POC_MassaTestes : TestProject
    {
        public void BASE()
        {

            DataBaseOracle db = new DataBaseOracle();

            db.user = "desenvolvedor_sia";
            db.password = "asdfg";

            db.host = "10.250.1.35";
            db.port = "1521";
            db.service = "branch_1084.prod01.redelocal.oraclevcn.com";

            Dados.AddDataBase(prmTag: "SIA", prmConexao: db.GetString());

        }
        public void DATA()
        {

            Dados.AddDataModel(prmTag: "Aluno", prmModelo: @"{'#TABELAS#':'sia.aluno_curso','#CAMPOS#':'cod_matricula, nom_responsavel_pgto'}");

            Dados.AddDataVariant(prmTag: "=Padrao",prmRegra: @"{'#CONDICAO#': 'cod_situacao_aluno = 1'}");

            Dados.Export.SaveCSV(Config.PathDataFiles, prmNome: "Texto");

        }
        public void BUILD()
        {

            this.Setup(prmName: "POC - Massa de Dados Dinâmica");

//            this.AddSuite(new SuiteAtendimento());

        }
        public void CONFIG()
        {

            this.Config.PathDataFiles = @"C:\Users\alexa\OneDrive\Área de Trabalho\MassaTeste\";

            this.Config.EncodedDataJUNIT = Encoding.UTF7;

            this.Config.OnlyDATA = true;

            this.Config.PauseAfterTestCase = 0;

            this.Executar(prmTipoDriver: eTipoDriver.ChromeDriver);

        }
    }
    public class SuiteAtendimento : TestSuite
    {
        public SuiteAtendimento()
        {


            AddScript(new AtendimentoAluno_Teste());

        }

    }

    public class AtendimentoAluno_Teste : TestScript
    {


        public void DATA()
        {

            Massa.SetView(prmTag: "Aluno=Padrao");

            Massa.Add(prmFluxo: @"{ 'COD_MATRICULA': 'Alexandre', 'NOM_RESPONSAVEL_PGTO': 'alexandre_bartie@hotmail.com' }");

            Massa.Save();



        }

    }

}
