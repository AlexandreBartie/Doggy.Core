using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MeuSeleniumCSharp
{
    public partial class frmHubTeste : Form
    {

        private QA_WebHub HubTeste = new QA_WebHub();

        public frmHubTeste()
        {
            InitializeComponent();
        }

        private void cmdExecutar_Click(object sender, EventArgs e)
        {
            HubTeste.Executar(prmTipoDriver: eTipoDriver.ChromeDriver);
        }

    }

}




