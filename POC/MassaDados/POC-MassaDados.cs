using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Text;

namespace Dooggy
{
    public class POC_MassaDados : TestProject
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

            Dados.AddDataModel(prmTag: "Aluno", prmModelo: @"{'#ENTIDADES#':'sia.aluno_curso','#ATRIBUTOS#':'cod_matricula, nom_responsavel_pgto'}");

            Dados.AddDataVariant(prmTag: "=Padrao",prmRegra: @"{'#REGRAS#': 'cod_situacao_aluno = '1''}");

        }
        public void BUILD()
        {

            this.Setup(prmName: "POC - Massa de Dados Dinâmica");

            this.AddSuite(new SuiteAtendimento());

        }
        public void CONFIG()
        {

            this.Config.PathFileSources = @"C:\Users\alexa\OneDrive\Área de Trabalho\";

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

            Trace.Log.SQL(Massa.JSON.log);

        }

    }

}
