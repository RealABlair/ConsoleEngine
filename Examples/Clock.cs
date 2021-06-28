using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ConsoleEngine;
using ABSoftware;

namespace ConsoleEngine2D
{
    public class Clock : Engine
    {
        static void Main(string[] args)
        {
            Clock c = new Clock();
            c.Construct(40, 20, -1);
            c.Start();
        }

        int centerX { get { return ScreenWidth / 2; } }
        int centerY { get { return ScreenHeight / 2; } }

        Pixel ClockColor = Pixel.White;
        Pixel HAColor = Pixel.DarkGray;
        Pixel MAColor = Pixel.Gray;
        Pixel SAColor = Pixel.Red;

        public override void OnCreate()
        {
            
        }

        public override void OnUpdate()
        {
            Clear(Pixel.Black);
            DrawCircle(centerX, centerY, ScreenWidth / 2, ScreenHeight / 2, ClockColor);
            //HOURS
            double hangle = Maths.DegreesToRadians(270) + 0.52359 * DateTime.Now.Hour;
            int hpx = (int)(centerX + (ScreenWidth / 2 - 4) * Math.Cos(hangle));
            int hpy = (int)(centerY + (ScreenHeight / 2 - 4) * Math.Sin(hangle));
            DrawLine(centerX, centerY, hpx, hpy, HAColor);
            //MINUTES
            double mangle = Maths.DegreesToRadians(270) + 0.10471 * DateTime.Now.Minute;
            int mpx = (int)(centerX + (ScreenWidth / 2 - 2) * Math.Cos(mangle));
            int mpy = (int)(centerY + (ScreenHeight / 2 - 2) * Math.Sin(mangle));
            DrawLine(centerX, centerY, mpx, mpy, MAColor);
            //SECONDS
            double sangle = Maths.DegreesToRadians(270) + 0.10471 * DateTime.Now.Second;
            int spx = (int)(centerX + (ScreenWidth / 2 - 1) * Math.Cos(sangle));
            int spy = (int)(centerY + (ScreenHeight / 2 - 1) * Math.Sin(sangle));
            DrawLine(centerX, centerY, spx, spy, SAColor);
        }
    }
}
