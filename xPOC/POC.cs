using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Forms;

namespace MeuSeleniumCSharp
{
    public class POC : TestProject
    {

        public void Executar()
        {

            //Config.PauseAfterTestCase = 5;

            //Hub.AddSuite(new SuiteKatalon());
            ////Hub.AddSuite(new SuiteGoogle());

            //Hub.Executar(prmTipoDriver: eTipoDriver.ChromeDriver);

            //

            BaseDados Base = new BaseDados(prmConexao: @"Data Source=PC-ENGENHARIA\SQLEXPRESS;Initial Catalog=QA_POC;Integrated Security=True");

            if (Base.IsOK())
            {

                if (Base.Executar(prmSQL: "SELECT * FROM Candidatos where senha = '123456'"))
                {

                    while (Base.cursor.Ler())
                    {

                        //Debug.Assert(false);
                        System.Diagnostics.Debug.WriteLine("\t{0}\t{1}\t{2}",
                        Base.cursor.GetValor(0), Base.cursor.GetValor(1), Base.cursor.GetValor(2));
                        // Base.cursor.GetValor("nome"), Base.cursor.GetValor("sobrenome"), Base.cursor.GetValor("genero"));
                    }

                    Base.cursor.Fechar();

                }
                else
                {
                    MessageBox.Show("Erro de Execução SQL ...", "POC");
                }

            }
            else
            {

                MessageBox.Show("Erro de Conexao ...", "POC");

                if (false)
                {



                }

            }

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
