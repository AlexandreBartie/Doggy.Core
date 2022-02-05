using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;

namespace Dooggy
{

    public abstract class xThread
    {



    }

    public abstract class xSuperThread
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
