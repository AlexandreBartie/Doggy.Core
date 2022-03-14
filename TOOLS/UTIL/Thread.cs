using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;

namespace BlueRocket.CORE.Tools.Util
{

    public abstract class myThread
    {

        Thread thread;

        Exception Error;

        public void Go(string prmName) => Go(prmName, prmBackground: false);
        public void Go(string prmName, bool prmBackground)
        {
            thread = new Thread(new ThreadStart(DoWork));

            thread.Name = prmName;
            thread.IsBackground = prmBackground;
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();

        }

        [STAThread]
        private void DoWork()
        {
            try
            {
                Debug.WriteLine("Try: Espera Work ...");
                Work();
                Debug.WriteLine("Try: Depois Work ...");
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Erro na Tread: " + ex.Message); Error = ex;
            }
            finally
            {
                Debug.WriteLine("Finally: Espera End ...");
                End();
                Debug.WriteLine("Finally: Depois End ...");
            }
        }

        public void WaitEnd() { thread.Join(); }

        protected abstract void Work();
        protected abstract void End();

    }

    public abstract class mySuperThread
    {
        
        readonly ManualResetEvent _complete = new ManualResetEvent(false);

        public void Go(string prmName)
        {
            var thread = new Thread(new ThreadStart(DoWork));
            //{
            //    IsBackground = true;
            //}
            thread.IsBackground = false;
            thread.Name = prmName;
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
        }

        // Thread entry method
        private void DoWork()
        {
            try
            {
                _complete.Reset();
                Work();
            }
            catch (Exception ex)
            {
                if (DontRetryWorkOnFailed)
                {
                    Debug.WriteLine("Erro na Tread: " + ex.Message); throw;
                }
                else
                {
                    try
                    {
                        Thread.Sleep(1000);
                        Work();
                    }
                    catch
                    {
                        Debug.WriteLine("Erro na Tread: " + ex.Message);
                    }
                }
            }
            finally
            {
                _complete.Set();
            }
        }

        public bool DontRetryWorkOnFailed { get; set; }

        // Implemented in base class to do actual work.
        protected abstract void Work();
    }

}
