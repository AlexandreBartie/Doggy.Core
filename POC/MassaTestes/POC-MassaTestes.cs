using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Text;
using Dooggy.Factory.Robot;
using Dooggy.Factory;
using Dooggy.Factory.Data;

namespace Dooggy.POC.MassaTestes
{
    public class POC_MassaTestes : TestDataProject
    {
        public void DATA()
        {

            //Dados.AddDataModel(prmTag: "Aluno", prmModelo: @"{'#TABELAS#':'sia.aluno_curso','#CAMPOS#':'cod_matricula, nom_responsavel_pgto'}");
            //Dados.AddDataVariant(prmTag: "=Padrao",prmRegra: @"{'#CONDICAO#': 'cod_matricula = 201903371619'}");

            Dados.AddDataView(
                prmTag: "Aluno",
                prmSQL: "SELECT * FROM (SELECT cod_matricula as matricula, nom_aluno as getNomeAluno FROM sia.aluno, sia.aluno_curso WHERE sia.aluno.num_seq_aluno = '4495769' and cod_matricula = '201903371619') WHERE ROWNUM = 1",
                prmMask: "{ 'matricula': '####.##.#####-#' }");
            
            //Dados.File.SaveCSV(prmNome: "ArqDadosAtendimentoAoAluno");

            Dados.File.SaveCSV2(prmNome: "ArqDadosAtendimentoAoAluno", prmCabecalho: "test01_ValidarInformacoesDoAluno,matricula,getNomeAluno");

           // Dados.File.SaveJSON(prmNome: "ArqDadosAtendimentoAoAluno");

        }
        public void CONFIG()
        {

            //Dados.File.SetPathDestino(prmPath: @"C:\Users\alexa\OneDrive\Área de Trabalho\MassaTeste\");

        }
        public void BASE()
        {

            Connect.Oracle.user = "desenvolvedor_sia";
            Connect.Oracle.password = "asdfg";

            Connect.Oracle.host = "10.250.1.35";
            Connect.Oracle.port = "1521";
            Connect.Oracle.service = "branch_1084.prod01.redelocal.oraclevcn.com";

            Connect.Oracle.Add(prmTag: "SIA");

        }

    }

}
