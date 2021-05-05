using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleEngine
{
    public class FrameTimer
    {
        long initialFrame;

        Engine instance;

        public FrameTimer(long initialFrame, Engine instance)
        {
            this.initialFrame = initialFrame;
            this.instance = instance;
        }

        public void Init(long initialFrame, Engine instance) { this.initialFrame = initialFrame; this.instance = instance; }
    
        
        public bool Tick(long period)
        {
            if(instance.GetCurrentTimeMillis() - initialFrame >= period)
            {
                initialFrame = instance.GetCurrentTimeMillis();
                return true;
            }
            return false;
        }
    }
}
