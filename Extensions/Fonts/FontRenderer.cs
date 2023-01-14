using System;
using ABSoftware.Structures;
using ConsoleEngine;

namespace Fonts
{
    public class FontRenderer
    {
        public static string CHARS = "0123456789abcdefghijklmnopqrstuvwxyz?!,.-+=/\\*%:; ()[]";

        public static Font currentFont { get; private set; }
        public static int charsGap = 1;

        static FontRenderer()
        {
            fontMatrix = new Matrix3();
            fontMatrix.Identity();
        }

        #region Matrix
        static Matrix3 fontMatrix;

        static void Rotate(float angle)
        {
            fontMatrix = new Matrix3().Rotate_M(angle) * fontMatrix;
        }

        static void Shear(float x, float y)
        {
            fontMatrix = new Matrix3().Shear_M(x, y) * fontMatrix;
        }

        static void Scale(float x, float y)
        {
            fontMatrix = new Matrix3().Scale_M(x, y) * fontMatrix;
        }

        static void Translate(float x, float y)
        {
            fontMatrix = new Matrix3().Translate_M(x, y) * fontMatrix;
        }
        #endregion

        public static void ApplyFont(Font font, int charsGap = 1)
        {
            currentFont = font;
            FontRenderer.charsGap = charsGap;
        }

        public static void DrawText(int x, int y, string text, char p, ushort color = 0x000F)
        {
            if (currentFont == null)
                return;
            text = text.ToLower();
            for(int i = 0; i < text.Length; i++)
            {
                Sprite character = currentFont.GetChar(text[i]);
                for(int cx = 0; cx < character.Width; cx++)
                {
                    for (int cy = 0; cy < character.Height; cy++)
                    {
                        Pixel pix = character.GetPixel(cx, cy);
                        if(pix.R > 0 && pix.G > 0 && pix.B > 0)
                        {
                            if(Draw != null)
                            {
                                Draw.Invoke(x + cx + (i * character.Width) + (i * charsGap), y + cy, p);
                            }
                            else if(ColoredDraw != null)
                            {
                                ColoredDraw.Invoke(x + cx + (i * character.Width) + (i * charsGap), y + cy, p, color);
                            }
                        }
                    }
                }
            }
        }

        public static void DrawText(float x, float y, string text, char p, ushort color = 0x000F, float scaleX = 1f, float scaleY = 1f)
        {
            if (currentFont == null)
                return;
            text = text.ToLower();
            for (int i = 0; i < text.Length; i++)
            {
                Sprite character = currentFont.GetChar(text[i]);
                Translate(-x, -y);
                Translate(x + (i * character.Width) + (i * charsGap), y);
                Scale(scaleX, scaleY);
                DrawSprite(x, y, character, p, color);
                fontMatrix.Identity();
            }
        }

        static void DrawSprite(float x, float y, Sprite sprite, char p, ushort color = 0x000F)
        {
            Translate(x, y);
            Matrix3 matFinalInv = fontMatrix.Invert();

            float ex, ey;
            float sx, sy;
            float px, py;

            fontMatrix.Forward(0f, 0f, out px, out py);
            sx = px; sy = py;
            ex = px; ey = py;

            fontMatrix.Forward((float)sprite.Width, (float)sprite.Height, out px, out py);
            sx = Math.Min(sx, px); sy = Math.Min(sy, py);
            ex = Math.Max(ex, px); ey = Math.Max(ey, py);

            fontMatrix.Forward(0f, (float)sprite.Height, out px, out py);
            sx = Math.Min(sx, px); sy = Math.Min(sy, py);
            ex = Math.Max(ex, px); ey = Math.Max(ey, py);

            fontMatrix.Forward((float)sprite.Width, 0f, out px, out py);
            sx = Math.Min(sx, px); sy = Math.Min(sy, py);
            ex = Math.Max(ex, px); ey = Math.Max(ey, py);

            for (int ix = (int)sx; ix < ex; ix++)
            {
                for (int iy = (int)sy; iy < ey; iy++)
                {
                    float nx, ny;
                    matFinalInv.Forward((float)ix, (float)iy, out nx, out ny);
                    if (ny < 0 || nx < 0 || nx >= sprite.Width || ny >= sprite.Height || (float.IsNaN(nx) || float.IsNaN(ny)))
                        continue;
                    Pixel pix = sprite.GetPixel((int)(nx), (int)(ny));
                    if (pix.R > 0 && pix.G > 0 && pix.B > 0)
                    {
                        if (Draw != null)
                        {
                            Draw.Invoke(ix, iy, p);
                        }
                        else if (ColoredDraw != null)
                        {
                            ColoredDraw.Invoke(ix, iy, p, color);
                        }
                    }
                }
            }
        }

        public static int GetStringWidth(string text, float scaleX = 1f)
        {
            return (int)(currentFont.CharWidth * text.Length * scaleX) + (charsGap * text.Length);
        }

        public static Action<int,int,char,ushort> ColoredDraw { get; set; }
        public static Action<int,int,char> Draw { get; set; }
    }
}
