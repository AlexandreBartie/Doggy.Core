using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace BlueRocket.CORE.Tools.Util
{
    public class myClipBoard
    {
        private iClipboard clip;
        public void SetText(string prmTexto)
        {
            clip = new iClipboard(prmTexto);
                
            clip.Go(prmName: "Clipboard");
        }
    }
    public class iClipboard : myThread
    {

        private string texto;

        public iClipboard(string prmTexto)
        {
            texto = prmTexto;
        }

        protected override void Work()
        {
            Clipboard.SetText(texto);
        }

        protected override void End() { WaitEnd(); }
    }
}
