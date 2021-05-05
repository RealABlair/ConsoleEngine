using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleEngine
{
    public class GlyphRenderer
    {
        Engine engineInstance;
        int Width, Height;
        public Glyph[] glyphs;

        public void Init(int Width, int Height, Engine instance)
        {
            this.engineInstance = instance;
            this.Width = Width;
            this.Height = Height;
            glyphs = new Glyph[Width * Height];
        }

        public Glyph GetGlyph(int x, int y)
        {
            return glyphs[y * Width + x];
        }

        public void Clear()
        {
            for (int i = 0; i < glyphs.Length; i++)
            {
                glyphs[i] = null;
            }
        }

        public void Draw(int x, int y, Glyph glyph)
        {
            if (y * Width + x >= glyphs.Length || x < 0 || y < 0)
                return;
            glyphs[y * Width + x] = glyph;
        }

        /// <summary>
        /// Transparent background
        /// </summary>
        public void DrawText(int x, int y, string text, Pixel foregroundColor)
        {
            for (int i = 0; i < text.Length; i++)
            {
                Draw(x + i, y, ConstructGlyph(text[i], engineInstance.GetPixel(x + i, y), foregroundColor));
            }
        }

        /// <summary>
        /// Transparent background
        /// </summary>
        public void DrawText(int x, int y, string text, Pixel[] foregroundColors)
        {
            for (int i = 0; i < text.Length; i++)
            {
                Draw(x + i, y, ConstructGlyph(text[i], engineInstance.GetPixel(x + i, y), foregroundColors[i]));
            }
        }

        public void DrawText(int x, int y, string text, Pixel backgroundColor, Pixel foregroundColor)
        {
            for (int i = 0; i < text.Length; i++)
            {
                Draw(x + i, y, ConstructGlyph(text[i], backgroundColor, foregroundColor));
            }
        }

        public void DrawText(int x, int y, string text, Pixel[] backgroundColors, Pixel[] foregroundColors)
        {
            for (int i = 0; i < text.Length; i++)
            {
                Draw(x + i, y, ConstructGlyph(text[i], backgroundColors[i], foregroundColors[i]));
            }
        }

        public void DrawChar(int x, int y, char character, Pixel backgroundColor, Pixel foregroundColor)
        {
            Draw(x, y, ConstructGlyph(character, backgroundColor, foregroundColor));
        }

        Glyph ConstructGlyph(char character, Pixel backColor, Pixel foreColor)
        {
            Glyph g = new Glyph()
            {
                glyph = character,
                backgroundColor = backColor,
                foregroundColor = foreColor
            };
            return g;
        }
    }
}
