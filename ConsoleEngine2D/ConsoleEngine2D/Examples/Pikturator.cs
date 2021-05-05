using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using ABSoftware;
using ConsoleEngine;

namespace ConsoleEngine2D
{
    public class Pikturator : Engine
    {
        static void Main(string[] args)
        {
            Pikturator t = new Pikturator();
            t.ChangeFont(8, 8);
            t.Construct(100, 100, -1);
            t.Start();
            t.AppName = "Pikturator";
        }

        public static string PICTURE_PATH = "Picture.txt";
        public static string PALETTE_PATH = "Palette.txt";

        bool FilesExist = false;

        List<Pixel> Palette = new List<Pixel>();
        string[] Picture;
        int PictureWidth;
        int PictureHeight;

        public override void OnCreate()
        {
            FilesExist = CheckFiles();
            if (FilesExist)
            {
                LoadPicture();
                LoadPalette();
            }
            else
                glyphRenderer.DrawText(ScreenWidth / 2 - "FILES NOT FOUND!".Length / 2, ScreenHeight / 2, "FILES NOT FOUND!", Pixel.Red);
        }

        public override void OnUpdate()
        {
            try
            {
                if (!FilesExist)
                    return;

                Clear(Pixel.Sky);
                glyphRenderer.Clear();
                for (int y = 0; y < PictureHeight; y++)
                {
                    for (int x = 0; x < PictureWidth; x++)
                    {
                        Draw(x, y, getPaletteColor(x, y));
                    }
                }
            }
            catch(Exception ex)
            {
                
            }
        }

        public bool CheckFiles()
        {
            if (File.Exists(PALETTE_PATH) && File.Exists(PICTURE_PATH))
                return true;

            return false;
        }

        public void LoadPalette()
        {
            string[] Lines = File.ReadAllLines(PALETTE_PATH);
            for(int i = 0; i < Lines.Length; i++)
            {
                string Line = Lines[i];
                if (Line.Length < 3)
                    continue;
                string colors = Line.Split('=')[1];
                byte r = byte.Parse(colors.Split(',')[0]);
                byte g = byte.Parse(colors.Split(',')[1]);
                byte b = byte.Parse(colors.Split(',')[2]);
                Palette.Add(Pixel.FromRGB(r, g, b));
            }
        }

        public void LoadPicture()
        {
            Picture = File.ReadAllLines(PICTURE_PATH);
            string[] Lines = File.ReadAllText(PICTURE_PATH).Split('\n');
            PictureWidth = Lines[0].Length;
            PictureHeight = Lines.Length;
            ChangeFont((int)((float)PictureWidth / 12.5f), (int)((float)PictureHeight / 12.5f));
            Resize(PictureWidth, PictureHeight);
        }

        public Pixel getPaletteColor(int x, int y)
        {
            char currFacePart = Picture[y][x];
            return Palette[currFacePart - 48];
        }
    }
}