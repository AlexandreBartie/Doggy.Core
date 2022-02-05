using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace Dooggy
{
    public static class myClipBoard
    {
        public static void SetText(string prmTexto)
        {
            xClipboard clip = new xClipboard(prmTexto);
                
            clip.Go(prmName: "Clipboard");
        }
    }
    internal class xClipboard : xSuperThread
    {

        private string texto;

        public xClipboard(string prmTexto)
        {
            texto = prmTexto;
        }

        protected override void Work()
        {
            
            
            Clipboard.SetText(texto);
        }

    }
}
