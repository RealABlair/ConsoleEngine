using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;


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

        
        public Sprite(string fileName)
        {
            byte[] bytes = File.ReadAllBytes(fileName);
            this.Width = BitConverter.ToInt32(new byte[] { bytes[0], bytes[1], bytes[2], bytes[3] }, 0);
            this.Height = BitConverter.ToInt32(new byte[] { bytes[4], bytes[5], bytes[6], bytes[7] }, 0);
            this.pixels = new Pixel[this.Width * this.Height];
            for(int i = 0; i < this.Width * this.Height; i++)
            {
                pixels[i] = Pixel.FromDec(BitConverter.ToInt32(new byte[] { bytes[i * 4 + 8], bytes[i * 4 + 9], bytes[i * 4 + 10], bytes[i * 4 + 11] }, 0));
            }
        }

        public void Save(string fileName)
        {
            byte[] bytes = new byte[CalculateTotalBytes()];
            byte[] wdth = BitConverter.GetBytes(Width);
            byte[] hght = BitConverter.GetBytes(Height);
            bytes[0] = wdth[0];
            bytes[1] = wdth[1];
            bytes[2] = wdth[2];
            bytes[3] = wdth[3];
            bytes[4] = hght[0];
            bytes[5] = hght[1];
            bytes[6] = hght[2];
            bytes[7] = hght[3];
            for (int i = 0; i < pixels.Length; i++)
            {
                Pixel p = pixels[i];
                byte[] pixelBytes = BitConverter.GetBytes(GetPixelDec(p));
                bytes[i * 4 + 8] = pixelBytes[0];
                bytes[i * 4 + 9] = pixelBytes[1];
                bytes[i * 4 + 10] = pixelBytes[2];
                bytes[i * 4 + 11] = pixelBytes[3];
            }
            File.WriteAllBytes(fileName, bytes);
        }

        int GetPixelDec(Pixel pixel)
        {
            int dec = 0;
            dec += (int)pixel.R << 16;
            dec += (int)pixel.G << 8;
            dec += (int)pixel.B;
            return dec;
        }

        int CalculateTotalBytes()
        {
            int ret = 0;
            ret += 8;
            ret += pixels.Length * 4;
            return ret;
        }
    }
}
