using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConsoleEngine;
using ABSoftware;

namespace ConsoleEngine2D
{
    public class CaveGenerator : Engine
    {

        static void Main(string[] args)
        {
            CaveGenerator c = new CaveGenerator();
            c.ChangeFont(12, 12);
            c.Construct(size.x, size.y + 1, -1);
            c.Start();
        }

        static Point size = new Point(96, 64);

        int[,] m;
        Random rand = new Random();

        private int seed;
        private int rfp = 48;
        private int scount = 6;
        private int max = 4;
        private int min = 4;

        int sel = 0;

        public override void OnCreate()
        {

            seed = rand.Next(int.MinValue, int.MaxValue);
            m = Generate(size.x, size.y, rfp, scount, max, min, seed);
        }

        public override void OnUpdate()
        {
            Clear(Pixel.Black);
            glyphRenderer.Clear();

            if (Keyboard.GetAsyncKeyState(Keyboard.Keys.VK_SPACE) != 0 && isFocused)
            {
                seed = rand.Next(int.MinValue, int.MaxValue);
                m = Generate(size.x, size.y, rfp, scount, max, min, seed);
            }


            if (Keyboard.GetAsyncKeyState(Keyboard.Keys.VK_RIGHT) != 0 && isFocused && sel != 3) sel++;
            if (Keyboard.GetAsyncKeyState(Keyboard.Keys.VK_LEFT) != 0 && isFocused && sel != 0) sel--;

            if (Keyboard.GetAsyncKeyState(Keyboard.Keys.VK_UP) != 0 && isFocused) switch (sel)
            {
                    case 0: rfp++; break;
                    case 1: scount++; break;
                    case 2: max++; break;
                    case 3: min++; break;
            }
            if (Keyboard.GetAsyncKeyState(Keyboard.Keys.VK_DOWN) != 0 && isFocused) switch (sel)
            {
                    case 0: rfp--; break;
                    case 1: scount--; break;
                    case 2: max--; break;
                    case 3: min--; break;
            }


            for (int i = 0; i < size.x; i++)
            {
                for (int j = 0; j < size.y; j++)
                {
                    int col = (m[i, j] == 1) ? 6579300 : -1;
                    Draw(i, j, Pixel.FromDec(col));
                }
            }

            glyphRenderer.DrawText(0, size.y, $"S: {seed}", Pixel.FromDec(8 * 20000 + 1000));
            glyphRenderer.DrawText(15, size.y, $"W: {size.ToString()}", Pixel.FromDec(8 * 20000 + 1000));
            glyphRenderer.DrawText(32, size.y, $"RFP: {rfp}", Pixel.FromDec(8 * 20000 + 1000));
            glyphRenderer.DrawText(41, size.y, $"S: {scount}", Pixel.FromDec(8 * 20000 + 1000));
            glyphRenderer.DrawText(48, size.y, $"MX: {max}", Pixel.FromDec(8 * 20000 + 1000));
            glyphRenderer.DrawText(55, size.y, $"MN: {min}", Pixel.FromDec(8 * 20000 + 1000));

            switch (sel)
            {
                case 0: glyphRenderer.DrawText(32, size.y, $"RFP: {rfp}", Pixel.FromDec(12 * 20000 + 1000)); break;
                case 1: glyphRenderer.DrawText(41, size.y, $"S: {scount}", Pixel.FromDec(12 * 20000 + 1000)); break;
                case 2: glyphRenderer.DrawText(48, size.y, $"MX: {max}", Pixel.FromDec(12 * 20000 + 1000)); break;
                case 3: glyphRenderer.DrawText(55, size.y, $"MN: {min}", Pixel.FromDec(12 * 20000 + 1000)); break;

            }
        }

        public int[,] Generate(int width, int height, int randomFillPercent = 45, int smoothCount = 5, int maxNeighbors = 4, int minNeighbors = 4, int seed = -1)
        {

            int[,] map = new int[width, height];

            if (seed == -1) seed = rand.Next(int.MinValue, int.MaxValue);
            Random prng = new Random(seed);

            // Generera
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    if (x == 0 || x == width - 1 || y == 0 || y == height - 1)
                    {
                        map[x, y] = 1;
                    }
                    else
                    {
                        map[x, y] = (prng.Next(0, 100) < randomFillPercent) ? 1 : 0;
                    }
                }
            }

            // Smooth
            int[,] smoothMap = new int[width, height];
            for (int i = 0; i < smoothCount; i++)
            {

                for (int x = 0; x < width; x++)
                {
                    for (int y = 0; y < height; y++)
                    {
                        int neighbors = CountNeighbors(map, x, y, width, height);

                        if (neighbors > maxNeighbors) smoothMap[x, y] = 1;
                        else if (neighbors < minNeighbors) smoothMap[x, y] = 0;
                    }
                }
                map = smoothMap;
            }

            return map;
        }

        public int CountNeighbors(int[,] map, int gridX, int gridY, int w, int h)
        {
            int count = (map[gridX, gridY] == 1) ? -1 : 0;      // exkludera center ifall den är en vägg
            for (int x = gridX - 1; x <= gridX + 1; x++)
            {
                for (int y = gridY - 1; y <= gridY + 1; y++)
                {
                    if (x < 0 || x >= w || y < 0 || y >= h) { count++; continue; }
                    count += map[x, y];
                }
            }

            return count;
        }
    }
}
