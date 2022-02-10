using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Dooggy.Tools.Util
{
    public class myColor
    {

        public Color frente;
        public Color fundo;

        public myColor(Color prmFrente)
        {
            frente = prmFrente;
        }
        public myColor(Color prmFrente, Color prmFundo) { frente = prmFrente; fundo = prmFundo; }

    }
}
