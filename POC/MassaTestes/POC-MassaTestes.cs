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

            //
            // Buscar dados tabela Usuario
            //

            Dados.AddDataModel(prmTag: "Login", prmModelo: @"{'#TABELAS#':'seg.usuario','#CAMPOS#':'cod_usuario as login, ""1234as"" as senha, nom_usuario as usuarioLogado '}");
            Dados.AddDataVariant(prmTag: "=Marli", prmRegra: @"{'#CONDICAO#': 'cod_usuario = ""1016283"" ' }");

            Dados.File.SaveAll(prmTags: "Login", prmNome: "LoginAdm", prmCabecalho: "testLoginAdmValido,login,senha,usuarioLogado"); 
            
            //
            // Buscar dados tabela Aluno
            //

            Dados.AddDataView(
                prmTag: "Aluno",
                prmSQL: "SELECT * FROM (SELECT cod_matricula as matricula, nom_aluno as getNomeAluno FROM sia.aluno, sia.aluno_curso WHERE sia.aluno.num_seq_aluno = '4495769' and cod_matricula = '201903371619') WHERE ROWNUM = 1",
                prmMask: "{ 'matricula': '####.##.#####-#' }");

            Dados.File.SaveAll(prmTags: "Aluno", prmNome: "ArqDadosAtendimentoAoAluno", prmCabecalho: "test01_ValidarInformacoesDoAluno,matricula,getNomeAluno");

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
            Connect.Oracle.port = args.GetValor("port", "1521");

            Connect.Oracle.service = "branch_1085.prod01.redelocal.oraclevcn.com";

            Connect.Oracle.Add(prmTag: "SIA");

        }

    }

}
