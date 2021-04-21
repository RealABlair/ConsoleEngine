using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using ABSoftware;

namespace ConsoleEngine
{
    public abstract class Engine : Display
    {
        public int framerate;
        public bool enabled;
        public int ScreenWidth, ScreenHeight;
        Pixel[] pixels;
        Thread gameLoop;
        Timer frameTimer;
        long startTime;
        public string AppName { get { return Console.Title; } set { Console.Title = value; } }




        //User Booleans
        public bool DoubleChar = false;
        public bool UpdateWithoutFocusing = true;



        public void Construct(int Width, int Height, int framerate = 1)
        {
            AppName = GetType().Name;
            ScreenWidth = Width;
            ScreenHeight = Height;
            Console.SetWindowSize(Width, Height);
            this.framerate = framerate;
            frameTimer = new Timer(1000.0f / framerate);
            pixels = new Pixel[Width * Height];
            for (int i = 0; i < pixels.Length; i++)
            {
                pixels[i] = new Pixel();
            }
        }

        public long GetCurrentTimeMillis()
        {
            return startTime;
        }

        public void Start()
        {
            Init();
            Console.CursorVisible = false;
            enabled = true;
            startTime = 0;
            gameLoop = new Thread(GameLoop);
            gameLoop.Start();
        }

        public void GameLoop()
        {
            OnCreate();
            while(enabled)
            {
                while (enabled)
                {
                    if (!frameTimer.Tick() || !UpdateWithoutFocusing && !isFocused)
                        continue;

                    OnUpdate();
                    Redraw();
                    startTime++;
                }

                OnDestroy();
            }
        }

        public void Resize(int width, int height)
        {
            Console.SetWindowSize(width, height);
        }

        public Pixel GetPixel(int x, int y)
        {
            return pixels[y * ScreenWidth + x];
        }

        public void Clear(Pixel pixel)
        {
            for(int i = 0; i < pixels.Length; i++)
            {
                pixels[i] = pixel;
            }
        }

        private void Redraw()
        {
            Console.SetCursorPosition(0, 0);
            Console.CursorVisible = false;
            string scrn = "";
            for (int y = 0; y < ScreenHeight; y++)
            {
                for (int x = 0; x < ScreenWidth; x++)
                {
                    Pixel p = GetPixel(x, y);
                    if(!DoubleChar)
                        scrn += Colorize(" ", p, ColorType.Background);
                    else
                        scrn += Colorize("  ", p, ColorType.Background);

                }
                scrn += "\n";
            }
            Console.WriteLine(scrn);
        }

        public void Draw(int x, int y, Pixel p)
        {
            if (y * ScreenWidth + x >= pixels.Length || x < 0 || y < 0)
                return;
            pixels[y * ScreenWidth + x] = p;
        }

        public void DrawLine(int xStart, int yStart, int xEnd, int yEnd, Pixel p)
        {
            int w = xEnd - xStart;
            int h = yEnd - yStart;
            int dx1 = 0, dy1 = 0, dx2 = 0, dy2 = 0;
            if (w < 0) dx1 = -1; else if (w > 0) dx1 = 1;
            if (h < 0) dy1 = -1; else if (h > 0) dy1 = 1;
            if (w < 0) dx2 = -1; else if (w > 0) dx2 = 1;
            int longest = Math.Abs(w);
            int shortest = Math.Abs(h);
            if (!(longest > shortest))
            {
                longest = Math.Abs(h);
                shortest = Math.Abs(w);
                if (h < 0) dy2 = -1; else if (h > 0) dy2 = 1;
                dx2 = 0;
            }
            int numerator = longest >> 1;
            for (int i = 0; i <= longest; i++)
            {
                Draw(xStart, yStart, p);
                numerator += shortest;
                if (!(numerator < longest))
                {
                    numerator -= longest;
                    xStart += dx1;
                    yStart += dy1;
                }
                else
                {
                    xStart += dx2;
                    yStart += dy2;
                }
            }
        }

        public void DrawRect(int x, int y, int width, int height, Pixel p)
        {
            DrawLine(x, y, x + width - 1, y, p);
            DrawLine(x + width - 1, y, x + width - 1, y + height - 1, p);
            DrawLine(x + width - 1, y + height - 1, x, y + height - 1, p);
            DrawLine(x, y + height - 1, x, y, p);
        }

        public void FillRect(int x, int y, int width, int height, Pixel p)
        {
            for(int X = 0; X < width; X++)
            {
                for (int Y = 0; Y < height; Y++)
                {
                    Draw(x + X, y + Y, p);
                }
            }
        }

        public void DrawTriangle(int x1, int y1, int x2, int y2, int x3, int y3, Pixel p)
        {
            DrawLine(x1, y1, x2, y2, p);
            DrawLine(x2, y2, x3, y3, p);
            DrawLine(x3, y3, x1, y1, p);
        }

        public void DrawCurve(Point[] points, Pixel p)
        {
            for(int i = 0; i < points.Length; i++)
            {
                Draw(points[i].x, points[i].y, p);
            }

            for (int i = 0; i < points.Length - 1; i++)
            {
                DrawLine(points[i].x, points[i].y, points[i + 1].x, points[i + 1].y, p);
            }
        }

        public void DrawCircle(int x, int y, int width, int height, Pixel p)
        {
            for (double angle = 0; angle < Maths.DegreesToRadians(360); angle += 0.05)
            {
                int px = (int)(x + width * Math.Cos(angle));
                int py = (int)(y + height * Math.Sin(angle));

                Draw(px, py, p);
            }
        }

        #region Overrides
        public virtual void OnCreate() { }
        public virtual void OnUpdate() { }
        public virtual void OnDestroy() { }
        #endregion
    }
}
