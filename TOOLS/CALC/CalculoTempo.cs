using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Dooggy.Tools.Calc
{
    public class Cronometro
    {

        public TimeElapsed Elapsed;

        internal Stopwatch clock = new Stopwatch();

        public Cronometro()
        {
            Elapsed = new TimeElapsed(this);
        }

        public void Start() => clock.Start();
        public void Stop() => clock.Stop();

    }
    public class TimeElapsed
    {

        private Cronometro Cronos;

        public double seconds => milliseconds / 1000;
        public long milliseconds => Cronos.clock.ElapsedMilliseconds;

        public TimeElapsed(Cronometro prmCronos)
        {
            Cronos = prmCronos;
        }
    }
}
