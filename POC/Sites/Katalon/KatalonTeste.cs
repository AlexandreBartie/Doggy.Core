
namespace MeuSeleniumCSharp.Katalon
{
    public class KatalonTeste : TestScript
    {
        public void DATA()
        {

            Dados.AddDataBase(prmTag: "RH", prmConexao: @"Data Source=PC-ENGENHARIA\SQLEXPRESS;Initial Catalog=QA_POC;Integrated Security=True; MultipleActiveResultSets = True");

            Dados.AddDataModel(prmTag: "Candidato", prmModelo: @"{'#ENTIDADES#':'Candidatos','#ATRIBUTOS#':'nome + sobrenome + email + nascimento'}");

            Dados.AddDataVariant(prmTag: "=Padrao");

            Dados.AddDataVariant(prmTag: "=Primeiro", prmVariacao: @"{'#ORDEM#': 'nome'}");
            Dados.AddDataVariant(prmTag: "=Ultimo", prmVariacao: @"{'#ORDEM#': 'nome DESC'}");
            Dados.AddDataVariant(prmTag: "+Novo", prmVariacao: @"{'#ORDEM#': 'nascimento'}");
            Dados.AddDataVariant(prmTag: "+Velho", prmVariacao: @"{'#ORDEM#': 'nascimento DESC'}");

            Dados.AddDataVariant(prmTag: "-Email", prmVariacao: @"{'#REGRAS#': 'email is null'}");

            xLista lista = new xLista("Candidato=Padrao;Candidato=Primeiro;Candidato=Ultimo;Candidato+Novo;Candidato+Velho;Candidato-Email");

            foreach (string visao in lista)
            {

                if (Dados.SetView(prmTag: visao))
                {
                    //Robot.Debug.Console("MASSA: " + Pool.View.GetJSon());
                    Robot.Debug.Console("... " + Dados.View.memo());
                }
                else
                    Robot.Debug.Stop();

            }

            Massa.Add(prmFluxo: @"{'Nome':'Aderson','Sexo':'Homem', 'email':'alexandre_bartie@hotmail.com'}");
            Massa.Add(prmFluxo: @"{'email':'alexandre_bartie@hotmail.com'}");//, "AlunoSemAutorizacao");
            Massa.Add(prmFluxo: @"{'email':'alexandre_bartie@hotmail.com'}");//, "Candidato+Velho");
            Massa.Add(prmFluxo: @"{'Nome':'Lisia','Sexo':'Homem', 'email':'alexandre_bartie@hotmail.com'}");
            Massa.Add(prmFluxo: @"{'Expectativa':'Salário + Desafio'}");// "Candidato=Primeiro");
            Massa.Add(prmFluxo: @"{'Expectativa':'Liderança + Ambiente + Equipe'}");
            Massa.Add(prmFluxo: @"{'Expectativa':'Salário + Desafio'}");
            Massa.Add(prmFluxo: @"{'Expectativa':'Liderança + Ambiente + Equipe'}");

            Massa.Save();

        }
        public void SETUP()
        {

            Robot.GoURL(prmUrl: "https://katalon-test.s3.amazonaws.com/aut/html/form.html");

            Robot.Mapping("Nome", "id=first-name");
            Robot.Mapping("Sobrenome", "id=last-name");
            Robot.Mapping("Genero", "name=gender").SetDomain("Homem + Mulher + Indefinido");
            Robot.Mapping("Nascimento", "id=dob");
            Robot.Mapping("Endereco", "id=address");
            Robot.Mapping("Email", "id=email");
            Robot.Mapping("Senha", "id=password");
            Robot.Mapping("Empresa", "id=company");
            Robot.Mapping("Cargo", "id=role");
            Robot.Mapping("Expectativa", "id=expectation").SetDomain("Salário + Liderança + Ambiente + Equipe + Matriz + Desafio", "//*");
            Robot.Mapping("Aprendizado", "class=col-sm-10 development-ways").SetDomain("Livros + Cursos + Contribuições + Eventos + Blogs + Prática", "//*[@type='checkbox']") ;
            Robot.Mapping("Observacao", "id=comment");

        }
        public void PLAY()
        {

            Robot.Input("Nome", "Marcelo");
            Robot.Input("Sobrenome", "Silva");
            Robot.Input("Genero", "Mulher");
            Robot.Input("Nascimento", "06-05-1971");
            Robot.Input("Endereco", "Rua das Turmalinas, 68");
            Robot.Input("Email", "bartie.devops@outlook.com");
            Robot.Input("Senha", "123456");
            Robot.Input("Empresa", "Yduqs Faculdades Integradas");
            Robot.Input("Cargo", "Man");
            Robot.Input("Expectativa", "Ambiente + Desafio");
            Robot.Input("Aprendizado", "Livros + Eventos + Blogs");
            Robot.Input("Observacao", "Maravilhoso ...");

            Robot.Pause(5);

        }
        public void CHECK()
        {

            Robot.Submit();

        }
        public void CLEANUP()
        {

            Robot.Refresh();

        }

   }
}
