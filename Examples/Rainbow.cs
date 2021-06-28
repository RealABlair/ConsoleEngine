using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ConsoleEngine;

namespace ConsoleEngine2D
{
    public class Rainbow : Engine
    {
        static void Main(string[] args)
        {
            Rainbow r = new Rainbow();
            r.Construct(60, 30, -1);
            r.Start();
        }

        public override void OnCreate()
        {
            
        }

        public override void OnUpdate()
        {
            for(int x = 0; x < ScreenWidth; x++)
            {
                for (int y = 0; y < ScreenHeight; y++)
                {
                    Draw(x, y, getRainbow(.5f, 1f, 1f));
                }
            }
        }

        public Pixel getRainbow(float sec, float sat, float bri)
        {
            float hue = ((GetCurrentTimeMillis()) % (int)(sec * 1000f)) / (float)(sec * 1000f);
            return Pixel.FromHSV(hue, sat, bri);
        }

        public Pixel getRainbow(float sec, float sat, float bri, long index)
        {
            float hue = ((GetCurrentTimeMillis() + index) % (int)(sec * 1000f)) / (float)(sec * 1000f);
            return Pixel.FromHSV(hue, sat, bri);
        }
    }
}
