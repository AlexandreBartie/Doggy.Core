using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace Dooggy
{
    public static class xObject
    {
        public static void TurnOnOff(bool prmON, Control prmObjectA, Control prmObjectB)
        {
            prmObjectA.Visible = prmON; prmObjectB.Visible = !prmON;
        }
    }
}
