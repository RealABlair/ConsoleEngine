using System;
using System.IO;
using ConsoleEngine;
using ABSoftware;

namespace ConsoleEngine.Fonts
{
    public class Font
    {
        ArrayList<Sprite> sprites = new ArrayList<Sprite>();

        public Font(string fileName)
        {
            Load(fileName);
        }

        public Font(Sprite font)
        {
            Load(font);
        }

        public void Load(string fileName)
        {
            Sprite font = new Sprite(fileName);
            for(int i = 0; i < FontRenderer.CHARS.Length; i++)
            {
                sprites.Add(font.GetRange(5*i,0,4,5));
            }
        }

        public void Load(Sprite font)
        {
            for (int i = 0; i < FontRenderer.CHARS.Length; i++)
            {
                sprites.Add(font.GetRange(5 * i, 0, 4, 5));
            }
        }

        public Sprite GetChar(char character)
        {
            int index = FontRenderer.CHARS.IndexOf(character);
            if (index < 0)
                index = FontRenderer.CHARS.IndexOf(' ');
            return sprites.Get(index);
        }

        public int CharWidth { get { return sprites[0].Width; } }
        public int CharHeight { get { return sprites[0].Height; } }
    }
}
