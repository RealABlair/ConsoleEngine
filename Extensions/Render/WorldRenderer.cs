using System;
using ABSoftware.Structures;
using ConsoleEngine;

namespace Render
{
    public class WorldRenderer
    {
        public Matrix3 worldMatrix { get; private set; }

        public WorldRenderer(Action<int, int, char, ushort> DrawFunction)
        {
            this.ColoredDraw = DrawFunction;
            worldMatrix = new Matrix3();
            worldMatrix.Identity();
        }

        public WorldRenderer(Action<int, int, char> DrawFunction)
        {
            this.Draw = DrawFunction;
            worldMatrix = new Matrix3();
            worldMatrix.Identity();
        }

        public void Rotate(float angle)
        {
            worldMatrix = new Matrix3().Rotate_M(angle) * worldMatrix;
        }

        public void Shear(float x, float y)
        {
            worldMatrix = new Matrix3().Shear_M(x, y) * worldMatrix;
        }

        public void Scale(float x, float y)
        {
            worldMatrix = new Matrix3().Scale_M(x, y) * worldMatrix;
        }

        public void Translate(float x, float y)
        {
            worldMatrix = new Matrix3().Translate_M(x, y) * worldMatrix;
        }

        public void ResetMatrix()
        {
            worldMatrix.Identity();
        }

        public Vector2 TransformPoint(Vector2 point)
        {
            Translate(point.x, point.y);
            Matrix3 matFinalInv = worldMatrix.Invert();

            float sx, sy;
            float px, py;

            worldMatrix.Forward(0f, 0f, out px, out py);
            sx = px; sy = py;

            return new Vector2(sx, sy);
        }

        public void DrawChar(float x, float y, char c, ushort color = 0x000F)
        {
            Translate(x, y);
            Matrix3 matFinalInv = worldMatrix.Invert();

            float ex, ey;
            float sx, sy;
            float px, py;

            worldMatrix.Forward(0f, 0f, out px, out py);
            sx = px; sy = py;
            ex = px; ey = py;

            worldMatrix.Forward(1f, 1f, out px, out py);
            sx = Math.Min(sx, px); sy = Math.Min(sy, py);
            ex = Math.Max(ex, px); ey = Math.Max(ey, py);

            worldMatrix.Forward(0f, 1f, out px, out py);
            sx = Math.Min(sx, px); sy = Math.Min(sy, py);
            ex = Math.Max(ex, px); ey = Math.Max(ey, py);

            worldMatrix.Forward(1f, 0f, out px, out py);
            sx = Math.Min(sx, px); sy = Math.Min(sy, py);
            ex = Math.Max(ex, px); ey = Math.Max(ey, py);

            for (int ix = (int)sx; ix < ex; ix++)
            {
                for (int iy = (int)sy; iy < ey; iy++)
                {
                    float nx, ny;
                    matFinalInv.Forward((float)ix, (float)iy, out nx, out ny);
                    if (ny < 0 || nx < 0 || nx >= 1f || ny >= 1f || (float.IsNaN(nx) || float.IsNaN(ny)))
                        continue;
                    if (Draw != null)
                    {
                        Draw.Invoke(ix, iy, c);
                    }
                    else if (ColoredDraw != null)
                    {
                        ColoredDraw.Invoke(ix, iy, c, color);
                    }
                }
            }
        }

        public void DrawRect(float x, float y, float width, float height, char c, ushort color = 0x000F)
        {
            Translate(x, y);
            Matrix3 matFinalInv = worldMatrix.Invert();

            float ex, ey;
            float sx, sy;
            float px, py;

            worldMatrix.Forward(0f, 0f, out px, out py);
            sx = px; sy = py;
            ex = px; ey = py;

            worldMatrix.Forward(width, height, out px, out py);
            sx = Math.Min(sx, px); sy = Math.Min(sy, py);
            ex = Math.Max(ex, px); ey = Math.Max(ey, py);

            worldMatrix.Forward(0f, height, out px, out py);
            sx = Math.Min(sx, px); sy = Math.Min(sy, py);
            ex = Math.Max(ex, px); ey = Math.Max(ey, py);

            worldMatrix.Forward(width, 0f, out px, out py);
            sx = Math.Min(sx, px); sy = Math.Min(sy, py);
            ex = Math.Max(ex, px); ey = Math.Max(ey, py);

            for (int ix = (int)sx; ix < ex; ix++)
            {
                for (int iy = (int)sy; iy < ey; iy++)
                {
                    float nx, ny;
                    matFinalInv.Forward((float)ix, (float)iy, out nx, out ny);
                    if (ny < 0 || nx < 0 || nx >= width || ny >= height || (float.IsNaN(nx) || float.IsNaN(ny)))
                        continue;
                    if (Draw != null)
                    {
                        Draw.Invoke(ix, iy, c);
                    }
                    else if (ColoredDraw != null)
                    {
                        ColoredDraw.Invoke(ix, iy, c, color);
                    }
                }
            }
        }

        public void DrawSprite(float x, float y, CharSprite sprite, ushort color = 0x000F)
        {
            Translate(x, y);
            Matrix3 matFinalInv = worldMatrix.Invert();

            float ex, ey;
            float sx, sy;
            float px, py;

            worldMatrix.Forward(0f, 0f, out px, out py);
            sx = px; sy = py;
            ex = px; ey = py;

            worldMatrix.Forward((float)sprite.Width, (float)sprite.Height, out px, out py);
            sx = Math.Min(sx, px); sy = Math.Min(sy, py);
            ex = Math.Max(ex, px); ey = Math.Max(ey, py);

            worldMatrix.Forward(0f, (float)sprite.Height, out px, out py);
            sx = Math.Min(sx, px); sy = Math.Min(sy, py);
            ex = Math.Max(ex, px); ey = Math.Max(ey, py);

            worldMatrix.Forward((float)sprite.Width, 0f, out px, out py);
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
                    char p = sprite.GetPixel((int)(nx), (int)(ny));
                    if (p != ' ')
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

        public Action<int, int, char, ushort> ColoredDraw { get; private set; }
        public Action<int, int, char> Draw { get; private set; }
    }
}
