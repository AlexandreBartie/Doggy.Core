using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Dooggy
{
    public class Cor
    {

        public Color frente;
        public Color fundo;

        public Cor(Color prmFrente)
        {
            frente = prmFrente;
        }
        public Cor(Color prmFrente, Color prmFundo) { frente = prmFrente; fundo = prmFundo; }

        //public static void SetColor(Control prmControl) { prmControl.ForeColor = frente; prmControl.BackColor = fundo; }

    }
}
