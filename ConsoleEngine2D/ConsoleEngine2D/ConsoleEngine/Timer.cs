using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleEngine
{
    internal class Timer
    {
        public Timer(float interval) => Interval = interval;

        public float Interval { get; private set; }

        private DateTime lastTickTime;

        public bool Tick()
        {
            if ((DateTime.Now - lastTickTime).TotalMilliseconds >= Interval)
            {
                lastTickTime = DateTime.Now;
                return true;
            }
            return false;
        }

        public void Init() => lastTickTime = DateTime.Now;
        public void Init(DateTime dt) => lastTickTime = dt;
    }
}
