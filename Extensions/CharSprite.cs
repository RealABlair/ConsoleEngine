using System;
using System.IO;

namespace ConsoleEngine
{
    public class CharSprite
    {
        public int Width { get; private set; }
        public int Height { get; private set; }

        char[] pixels;

        public CharSprite(int Width = 0, int Height = 0)
        {
            this.Width = Width;
            this.Height = Height;
            pixels = new char[Width * Height];
            for (int i = 0; i < pixels.Length; i++)
            {
                pixels[i] = ' ';
            }
        }

        public void Load(string path)
        {
            string[] Lines = File.ReadAllLines(path);
            if (Lines == null || Lines.Length < 1)
                return;
            Height = Lines.Length;
            Width = Lines[0].Length;

            pixels = new char[Width * Height];
            for (int i = 0; i < pixels.Length; i++)
            {
                pixels[i] = ' ';
            }

            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    SetPixel(x, y, Lines[y][x]);
                }
            }
        }

        public void Fill(char p)
        {
            for (int i = 0; i < pixels.Length; i++)
            {
                pixels[i] = p;
            }
        }

        public void SetPixel(int x, int y, char p)
        {
            pixels[y * Width + x] = p;
        }

        public char GetPixel(int x, int y)
        {
            return pixels[y * Width + x];
        }
    }
}
