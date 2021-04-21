using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ABSoftware;

namespace ConsoleEngine
{
    public class Point
    {
        public int x, y;

        public Point(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public static float Distance(Point p1, Point p2)
        {
            int x = Math.Max(p1.x, p2.x) - Math.Min(p1.x, p2.x), y = Math.Max(p1.y, p2.y) - Math.Min(p1.y, p2.y);

            return (float)Math.Sqrt(Maths.PowerNumber(x, 2) + Maths.PowerNumber(y, 2));
        }
    }
}
