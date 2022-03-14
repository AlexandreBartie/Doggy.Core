using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace BlueRocket.CORE.Tools.Util
{
    public class myFileDialog
    {

        public OpenFileDialog Dialog;

        private Thread Thread;
        private DialogResult Result;

        public myFileDialog()
        {
            Dialog = new OpenFileDialog();
            Thread = new Thread(new ThreadStart(MethodThread));
            Thread.SetApartmentState(ApartmentState.STA);
            Result = DialogResult.None;
        }

        public DialogResult Open()
        {
            Thread.Start();
            Thread.Join();
            return Result;
        }

        private void MethodThread()
        {
            Result = Dialog.ShowDialog();
        }

    }
}
