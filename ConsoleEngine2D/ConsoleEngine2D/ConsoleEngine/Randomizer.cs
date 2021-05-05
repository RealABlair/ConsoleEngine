using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleEngine
{
    public class Randomizer
    {
        static Random rnd = new Random();

        public static byte RandomByte(byte min, byte max) => (byte)rnd.Next(min, max + 1);

        public static int RandomInt(int min, int max) => rnd.Next(min, max + 1);

        public static double RandomDouble(double min, double max) => min + rnd.NextDouble() * max;

        public static float RandomFloat(float min, float max) => (float)(min + rnd.NextDouble() * max);
    }
}
