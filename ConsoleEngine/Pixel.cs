using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ABSoftware;

namespace ConsoleEngine
{
    public class Pixel
    {
        public byte R { get; set; }
        public byte G { get; set; }
        public byte B { get; set; }


        public Pixel()
        {
            this.R = 0;
            this.G = 0;
            this.B = 0;
        }

        public static Pixel FromRGB(byte r, byte g, byte b)
        {
            Pixel p = new Pixel();
            p.R = r;
            p.G = g;
            p.B = b;
            return p;
        }

        public static Pixel FromDec(int decimalColor)
        {
            Pixel p = new Pixel();
            p.R = (byte)(decimalColor >> 16 & 255);
            p.G = (byte)(decimalColor >> 8 & 255);
            p.B = (byte)(decimalColor & 255);
            return p;
        }

        public static Pixel FromHSV(float hue, float saturation, float value)
        {
            Pixel pix = new Pixel();
            if (saturation == 0)
            {
                pix.R = pix.G = pix.B = (byte)(value * 255.0f + 0.5f);
            }
            else
            {
                float h = (hue - (float)Math.Floor(hue)) * 6.0f;
                float f = h - (float)Math.Floor(h);
                float p = value * (1.0f - saturation);
                float q = value * (1.0f - saturation * f);
                float t = value * (1.0f - (saturation * (1.0f - f)));
                switch ((int)h)
                {
                    case 0:
                        pix.R = (byte)(value * 255.0f + 0.5f);
                        pix.G = (byte)(t * 255.0f + 0.5f);
                        pix.B = (byte)(p * 255.0f + 0.5f);
                        break;
                    case 1:
                        pix.R = (byte)(q * 255.0f + 0.5f);
                        pix.G = (byte)(value * 255.0f + 0.5f);
                        pix.B = (byte)(p * 255.0f + 0.5f);
                        break;
                    case 2:
                        pix.R = (byte)(p * 255.0f + 0.5f);
                        pix.G = (byte)(value * 255.0f + 0.5f);
                        pix.B = (byte)(t * 255.0f + 0.5f);
                        break;
                    case 3:
                        pix.R = (byte)(p * 255.0f + 0.5f);
                        pix.G = (byte)(q * 255.0f + 0.5f);
                        pix.B = (byte)(value * 255.0f + 0.5f);
                        break;
                    case 4:
                        pix.R = (byte)(t * 255.0f + 0.5f);
                        pix.G = (byte)(p * 255.0f + 0.5f);
                        pix.B = (byte)(value * 255.0f + 0.5f);
                        break;
                    case 5:
                        pix.R = (byte)(value * 255.0f + 0.5f);
                        pix.G = (byte)(p * 255.0f + 0.5f);
                        pix.B = (byte)(q * 255.0f + 0.5f);
                        break;
                }
            }
            return pix;
        }

        public static Pixel Fade(Pixel From, Pixel To, float t)
        {
            Pixel p = new Pixel();
            p.R = (byte)((1f - t) * From.R + t * To.R);
            p.G = (byte)((1f - t) * From.G + t * To.G);
            p.B = (byte)((1f - t) * From.B + t * To.B);
            return p;
        }

        public static Pixel InvertColor(Pixel pix)
        {
            Pixel p = new Pixel();
            p.R = (byte)(255 - pix.R);
            p.G = (byte)(255 - pix.G);
            p.B = (byte)(255 - pix.B);
            return p;
        }

        public static Pixel Random => FromRGB(Randomizer.RandomByte(0, 255), Randomizer.RandomByte(0, 255), Randomizer.RandomByte(0, 255));

        public static Pixel Red => FromRGB(255, 0, 0);
        public static Pixel Green => FromRGB(0, 255, 0);
        public static Pixel Blue => FromRGB(0, 0, 255);
        public static Pixel Cyan => FromRGB(0, 255, 255);
        public static Pixel DarkCyan => FromRGB(0, 55, 55);
        public static Pixel DarkRed => FromRGB(55, 0, 0);
        public static Pixel DarkGreen => FromRGB(0, 55, 0);
        public static Pixel DarkBlue => FromRGB(0, 0, 55);
        public static Pixel Sky => FromRGB(135, 206, 235);
        public static Pixel Amber => FromRGB(255, 191, 0);
        public static Pixel Gold => FromRGB(255, 213, 0);
        public static Pixel Dirt => FromRGB(80, 55, 21);
        public static Pixel Magenta => FromRGB(255, 0, 255);
        public static Pixel ProcessMagenta => FromRGB(255, 0, 144);
        public static Pixel Yellow => FromRGB(255, 255, 0);
        public static Pixel Black => FromRGB(0, 0, 0);
        public static Pixel White => FromRGB(255, 255, 255);
        public static Pixel LightGray => FromRGB(211, 211, 211);
        public static Pixel Gray => FromRGB(169, 169, 169);
        public static Pixel DarkGray => FromRGB(128, 128, 128);
    }
}
