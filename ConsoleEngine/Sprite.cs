using System;
using System.IO;
using System.Collections.Generic;
using ABSoftware;

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
            byteBuilder.Append(BitConverter.GetBytes((uint)Chunks.START));
            byteBuilder.Append(BitConverter.GetBytes((uint)Chunks.SIZE));
            byteBuilder.Append(BitConverter.GetBytes(Width));
            byteBuilder.Append(BitConverter.GetBytes(Height));
            byteBuilder.Append(BitConverter.GetBytes((uint)Chunks.PALETTE));
            List<Pixel> palette = ReadPalette();
            for (int i = 0; i < palette.Count; i++)
            {
                byteBuilder.Append(new byte[] { palette[i].R, palette[i].G, palette[i].B });
            }
            byteBuilder.Append(BitConverter.GetBytes((uint)Chunks.PIXELS));
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    byteBuilder.Append(((UInt24)(UInt32)palette.IndexOf(GetPixel(x, y))).ToByteArray());
                }
            }
            byteBuilder.Append(BitConverter.GetBytes((uint)Chunks.END));
            File.WriteAllBytes(fileName, byteBuilder.ToArray());
        }

        public void Load(string fileName)
        {
            ByteBuilder byteBuilder = new ByteBuilder();
            byteBuilder.Append(File.ReadAllBytes(fileName));
            int startIndex = byteBuilder.IndexOf(BitConverter.GetBytes((uint)Chunks.START));
            int endIndex = byteBuilder.IndexOf(BitConverter.GetBytes((uint)Chunks.END));
            if (startIndex < 0 || endIndex < startIndex)
                return;
            int sizeIndex = byteBuilder.IndexOf(BitConverter.GetBytes((uint)Chunks.SIZE));
            Width = BitConverter.ToInt32(byteBuilder.GetRange(sizeIndex + 4, 4), 0);
            Height = BitConverter.ToInt32(byteBuilder.GetRange(sizeIndex + 8, 4), 0);
            pixels = new Pixel[Width * Height];
            Pixel[] palette = new Pixel[Width * Height];
            int paletteIndex = byteBuilder.IndexOf(BitConverter.GetBytes((uint)Chunks.PALETTE));
            for (int i = 0; i < palette.Length; i++)
            {
                byte[] color = byteBuilder.GetRange(paletteIndex + 4 + (i * 3), 3);
                palette[i] = Pixel.FromRGB(color[0], color[1], color[2]);
            }
            int pixelsIndex = byteBuilder.IndexOf(BitConverter.GetBytes((uint)Chunks.PIXELS));
            int pixelId = 0;
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    SetPixel(x, y, palette[((UInt24)(byteBuilder.GetRange(pixelsIndex + 4 + (3 * pixelId), 3))).Value]);
                    pixelId++;
                }
            }
        }

        public Sprite GetRange(int x, int y, int width, int height)
        {
            Sprite s = new Sprite(width, height);
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    s.SetPixel(i, j, GetPixel(x + i, y + j));
                }
            }
            return s;
        }

        enum Chunks : uint
        {
            START = 0x00413000,
            SIZE = 0x00413500,
            PALETTE = 0x00414000,
            PIXELS = 0x00415000,
            END = 0x00416000
        }

        struct UInt24 //24bit value | 3 bytes
        {
            private Byte b0, b1, b2;

            public UInt24(UInt32 value)
            {
                b0 = (byte)((value) & 0xFF);
                b1 = (byte)((value >> 8) & 0xFF);
                b2 = (byte)((value >> 16) & 0xFF);
            }

            public UInt24(byte[] data)
            {
                b0 = data[2];
                b1 = data[1];
                b2 = data[0];
            }

            public UInt32 Value { get { return (UInt32)(b0 | (b1 << 8) | (b2 << 16)); } }

            public byte[] ToByteArray()
            {
                return new byte[] { b2, b1, b0 };
            }

            public static explicit operator UInt24(byte[] data) => new UInt24(data);
            public static explicit operator UInt24(UInt32 value) => new UInt24(value);
            public static implicit operator UInt32(UInt24 value) => value.Value;
        }

        List<Pixel> ReadPalette()
        {
            List<Pixel> pixs = new List<Pixel>();

            for (int i = 0; i < pixels.Length; i++)
            {
                if (!pixs.Contains(pixels[i]))
                    pixs.Add(pixels[i]);
            }
            return pixs;
        }
    }
}