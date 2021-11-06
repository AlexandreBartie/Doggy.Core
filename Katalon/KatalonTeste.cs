using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

namespace MeuSeleniumCSharp.Katalon
{
    public class KatalonTeste : QA_WebScript
    {
        public void DATA()
        {

            //Dados.AddSQL(prmSQL: "select alguem que tenha 70 anos ou mais  ");

            Dados.Add(prmFluxo: @"{'Nome':'Manuel','Sobrenome':'Rosa','Sexo':'Homem', 'email':'alexandre_bartie@hotmail.com'}");
            Dados.Add(prmFluxo: @"{'Sexo':'Mulher', 'email':'alexandre.bartie@gmail.com'}");
            Dados.Add(prmFluxo: @"{'Sexo':'Indefinido'}");


            Dados.Save();

            //Dados.AddCSV(prmCSV: "NOME-TST;SOBRENOME-TST;MALE;06-05-1971;R.DAS TURMALINAS, 68, bartie.devops@outlook.com");

        }
        public void SETUP()
        {

            Robot.GoURL(prmUrl: "https://katalon-test.s3.amazonaws.com/aut/html/form.html");

            Robot.Mapping("Nome", "id=first-name");
            Robot.Mapping("Sobrenome", "id=last-name");
            Robot.Mapping("Sexo", "name=gender").Dominio("Homem;Mulher;Indefinido");
            Robot.Mapping("Nascimento", "id=dob");
            Robot.Mapping("Endereco", "id=address");
            Robot.Mapping("Email", "id=email");
            Robot.Mapping("Senha", "id=password");
            Robot.Mapping("Empresa", "id=company");
            Robot.Mapping("Cargo", "id=role");
            Robot.Mapping("Expectativa", "id=expectation").Dominio("Salário; Liderança; Ambiente; Equipe; Matriz; Desafio", "//*");
            Robot.Mapping("Aprendizado", "class=col-sm-10 development-ways").Dominio("Livros; Cursos; Contribuições; Eventos; Blogs; Prática", "//*[@type='checkbox']") ;
            Robot.Mapping("Observacao", "id=comment");

        }
        public void PLAY()
        {

            Robot.SetMap("Nome", "Alexandre");
            Robot.SetMap("Sobrenome", "Bartie");
            Robot.SetMap("Sexo", "Mulher");
            Robot.SetMap("Nascimento", "06-05-1971");
            Robot.SetMap("Endereco", "Rua das Turmalinas, 68");
            Robot.SetMap("Email", "bartie.devops@outlook.com");
            Robot.SetMap("Senha", "123456");
            Robot.SetMap("Empresa", "Yduqs Faculdades Integradas");
            Robot.SetMap("Cargo", "Man");
            Robot.SetMap("Expectativa", "Ambiente + Desafio");
            Robot.SetMap("Aprendizado", "Livros + Eventos + Blogs");
            Robot.SetMap("Observacao", "Maravilhoso ...");

        }
        public void CHECK()
        {

            Robot.Submit();

            Robot.Refresh();
        }
        public void CLEANUP()
        {

            //Robot.Reaload();

        }

   }
}
