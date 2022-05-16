using System;
using System.IO;
using System.Collections.Generic;
using ABSoftware;
using System.Linq;

namespace ConsoleEngine
{
    public class Sprite
    {
        public int Width { get; private set; }
        public int Height { get; private set; }

        Pixel[] pixels;

        public Sprite(int Width = 0, int Height = 0)
        {
            this.Width = Width;
            this.Height = Height;
            pixels = new Pixel[Width * Height];
            for (int i = 0; i < pixels.Length; i++)
            {
                pixels[i] = new Pixel();
            }
        }

        public void SetPixel(int x, int y, Pixel p)
        {
            pixels[y * Width + x] = p;
        }

        public Pixel GetPixel(int x, int y)
        {
            return pixels[y * Width + x];
        }

        public Pixel SamplePixel(float x, float y, float xOffset = 0f, float yOffset = -1f)
        {
            int sx = (int)(x * (Width + xOffset));
            int sy = (int)(y * (Height + yOffset));
            if (sx < 0 || sx > Width || sy < 0 || sy > Height)
                return Pixel.Black;
            else
                return pixels[sy * Width + sx];
        }

        public Sprite(string fileName)
        {
            Load(fileName);
        }

        public void Save(string fileName)
        {
            ByteBuilder byteBuilder = new ByteBuilder();
            byteBuilder.Append(BitConverter.GetBytes((short)Chunks.START));
            byteBuilder.Append(BitConverter.GetBytes((short)Chunks.SIZE));
            byteBuilder.Append(BitConverter.GetBytes(Width));
            byteBuilder.Append(BitConverter.GetBytes(Height));
            byteBuilder.Append(BitConverter.GetBytes((short)Chunks.PALETTE));
            List<Pixel> palette = ReadPalette();
            for (int i = 0; i < palette.Count; i++)
            {
                byteBuilder.Append(new byte[] { palette[i].R, palette[i].G, palette[i].B });
            }
            byteBuilder.Append(BitConverter.GetBytes((short)Chunks.PIXELS));
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    byteBuilder.Append(BitConverter.GetBytes(palette.IndexOf(GetPixel(x, y))));
                }
            }
            byteBuilder.Append(BitConverter.GetBytes((short)Chunks.END));
            File.WriteAllBytes(fileName, byteBuilder.ToArray());
        }

        public void Load(string fileName)
        {
            ByteBuilder byteBuilder = new ByteBuilder();
            byteBuilder.Append(File.ReadAllBytes(fileName));
            int startIndex = byteBuilder.IndexOf(BitConverter.GetBytes((short)Chunks.START));
            int endIndex = byteBuilder.IndexOf(BitConverter.GetBytes((short)Chunks.END));
            if (startIndex < 0 || endIndex < startIndex)
                return;

            Width = BitConverter.ToInt32(byteBuilder.GetRange(startIndex + 4, 4), 0);
            Height = BitConverter.ToInt32(byteBuilder.GetRange(startIndex + 8, 4), 0);
            pixels = new Pixel[Width * Height];
            Pixel[] palette = new Pixel[Width * Height];
            for (int i = 0; i < palette.Length; i++)
            {
                byte[] color = byteBuilder.GetRange(startIndex + 14 + (i * 3), 3);
                palette[i] = Pixel.FromRGB(color[0], color[1], color[2]);
            }
            int pixelsIndex = byteBuilder.IndexOf(BitConverter.GetBytes((short)Chunks.PIXELS));
            int pixelId = 0;
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    SetPixel(x, y, palette[BitConverter.ToInt32(byteBuilder.GetRange(pixelsIndex + 2 + (4 * pixelId), 4), 0)]);
                    pixelId++;
                }
            }
        }

        enum Chunks : short
        {
            START = 0x4130,
            SIZE = 0x4135,
            PALETTE = 0x4140,
            PIXELS = 0x4150,
            END = 0x4160
        }

        List<Pixel> ReadPalette()
        {
            List<Pixel> pixs = new List<Pixel>();

            bool has(Pixel pix)
            {
                return pixs.FirstOrDefault(p => Equals(p, pix)) != null;
            }

            for (int i = 0; i < pixels.Length; i++)
            {
                if (!has(pixels[i]))
                    pixs.Add(pixels[i]);
            }
            return pixs;
        }
    }
}
