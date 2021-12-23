using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using Dooggy.Factory.Robot;
using Dooggy.Factory;
using Dooggy.Factory.Data;

namespace Dooggy.POC.MassaTestes
{

    public class POC_GeracaoMassaConsole : TestDataProject
    {
        public void DATA()
        {

            //''LoginAdm();

            //''ArqDadosAtendimentoAoAluno();

            //ArqDadosConsultaCursos();

            //''ArqDadosBancos();

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
    public class POC_GeracaoMassaScripts : TestDataProject
    {
        public void DATA()
        {

            //''LoginAdm();

            //''ArqDadosAtendimentoAoAluno();

            ArqDadosConsultaCursos();

            //''ArqDadosBancos();

        }

        private void ArqDadosConsultaCursos()
        {

            Dados.AddDataView(
                prmTag: "ConsultaCursosPorSigla|test01_PesquisarCursosPorSigla|nSigla, nCodigoCurso, nomCurso, nTipoCurso");

            Dados.AddDataFluxo(
                prmTag: "Padrao", 
                prmSQL : "select ab.SGL_CURSO as nSigla, ab.COD_CURSO as nCodigoCurso, ab.NOM_CURSO as nomCurso, ac.NOM_TIPO_CURSO as nTipoCurso " +
                         "from sia.curso ab, sia.tipo_curso ac " +
                         "where (ab.COD_TIPO_CURSO = ac.COD_TIPO_CURSO)");

            Dados.AddDataView(
                prmTag: "ConsultaCursosPorTipo|test02_PesquisarCursosPorTipoDeCurso|nCurso,nTipoCurso,nomCurso,nSigla,nCodigoCurso");

            Dados.AddDataFluxo(
                prmTag: "Padrao", 
                prmSQL: "select ad.NOM_AREA as nCurso, ab.NOM_TIPO_CURSO as nTipoCurso, ac.NOM_CURSO as nomCurso, ac.SGL_CURSO as nSigla, ac.COD_CURSO as nCodigoCurso " +
                        "from sia.tipo_curso ab, sia.curso ac, sia.area_conhecimento ad " +
                        "where (ab.COD_TIPO_CURSO = ac.COD_TIPO_CURSO and ac.COD_AREA_CONHECIMENTO = ad.COD_AREA_CONHECIMENTO)");

            SaveFile(prmTags: "ConsultaCursosPorSigla + ConsultaCursosPorTipo");

        }

        private void ArqDadosBancos()
        {

            Dados.AddDataView(prmTag: "ConsultaBancos|test01_ConsultarBanco|codBanco, nBanco, nBancoResultado");

            Dados.AddDataFluxo(prmTag: "PorCodigo", prmSQL: "select '356' as codBanco, nom_banco as nBanco, nom_banco as nBancoResultado from sur.banco_sur where cod_banco = 356");
            Dados.AddDataFluxo(prmTag: "PorDescricao", prmSQL: "select '' as codBanco, 'ABN AMRO' as nBanco, nom_banco as nBancoResultado from sur.banco_sur where nom_banco LIKE 'ABN AMRO%'");

            SaveFile(prmTags: "ConsultaBancos");

        }
        
        private void LoginAdm()
        {

            Dados.AddDataModel(prmTag: "LoginADM|testLoginAdmValido|login, senha, usuarioLogado", prmModelo: @"{'#TABELAS#':'seg.usuario','#CAMPOS#':'cod_usuario as login, ""1234as"" as senha, nom_usuario as usuarioLogado '}");

            Dados.AddDataVariant(prmTag: "=Marli", prmRegra: @"{'#CONDICAO#': 'cod_usuario = ""1016283"" ' }");

            SaveFile(prmTags: "LoginADM");

        }

        private void ArqDadosAtendimentoAoAluno()
        {

            Dados.AddDataView("AtendimentoAluno|test01_ValidarInformacoesDoAluno|matricula,getNomeAluno");

            Dados.AddDataFluxo(
                prmTag: "Padrao",
                prmSQL: "SELECT * FROM (SELECT cod_matricula as matricula, nom_aluno as getNomeAluno FROM sia.aluno, sia.aluno_curso WHERE sia.aluno.num_seq_aluno = '4495769' and cod_matricula = '201903371619') WHERE ROWNUM = 1",
                prmMask: "{ 'matricula': '####.##.#####-#' }");

            SaveFile(prmTags: "AtendimentoAluno"); 

        }

        private void SaveFile(string prmTags, [System.Runtime.CompilerServices.CallerMemberName] string prmNome = "")
        {

            Dados.File.SaveTXT(prmTags, prmNome);

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
