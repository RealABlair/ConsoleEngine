using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using ABSoftware;


namespace ConsoleEngine
{
    public abstract class EngineChar : Display
    {
        public int framerate;
        public bool enabled;
        public int ScreenWidth, ScreenHeight;
        char[] pixels;
        Thread gameLoop;
        Timer frameTimer;
        long startTime;
        DateTime lastUpdateTime;
        public string AppName { get { return Console.Title; } set { Console.Title = value; } }



        //User Things
        public bool UpdateWithoutFocusing = true;


        public void Construct(int Width, int Height, int framerate = -1)
        {
            AppName = GetType().Name;
            ScreenWidth = Width;
            ScreenHeight = Height;
            Console.SetWindowSize(Width, Height);
            this.framerate = framerate;
            frameTimer = new Timer(1000.0f / framerate);
            pixels = new char[Width * Height];
            this.lastUpdateTime = DateTime.Now;

            for (int i = 0; i < pixels.Length; i++)
            {
                pixels[i] = ' ';
            }
        }

        public long GetCurrentTimeMillis()
        {
            return startTime;
        }

        public void Start()
        {
            Init();
            HandlerInit(ConsoleEventCallback);
            SetCtrlHandler(handler, true);
            Console.CursorVisible = false;
            enabled = true;
            startTime = 0;
            gameLoop = new Thread(GameLoop);
            gameLoop.Start();
        }

        public void Stop()
        {
            gameLoop.Join(100);
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

                    DateTime dt = DateTime.Now;
                    OnUpdate((float)(dt - lastUpdateTime).TotalSeconds);
                    lastUpdateTime = dt;
                    Redraw();
                    startTime++;
                }

                OnDestroy();
            }
        }

        public void Resize(int width, int height)
        {
            pixels = new char[width * height];
            ScreenWidth = width;
            ScreenHeight = height;
            Console.SetWindowSize(width, height);
        }

        public char GetPixel(int x, int y)
        {
            return pixels[y * ScreenWidth + x];
        }

        public void Clear(char pixel)
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
            VisualOutput.ScreenBuilder.Clear();
            for (int y = 0; y < ScreenHeight; y++)
            {
                for (int x = 0; x < ScreenWidth; x++)
                {
                    char p = GetPixel(x, y);
                    VisualOutput.ScreenBuilder.Append(p);
                }
                VisualOutput.ScreenBuilder.AppendLine();
            }
            Console.WriteLine(VisualOutput.ScreenBuilder.ToString());
        }

        public void Draw(int x, int y, char p)
        {
            if (x > ScreenWidth - 1 || y > ScreenHeight - 1 || x < 0 || y < 0)
                return;
            pixels[y * ScreenWidth + x] = p;
        }

        public void DrawLine(int xStart, int yStart, int xEnd, int yEnd, char p)
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

        public void DrawRect(int x, int y, int width, int height, char p)
        {
            DrawLine(x, y, x + width - 1, y, p);
            DrawLine(x + width - 1, y, x + width - 1, y + height - 1, p);
            DrawLine(x + width - 1, y + height - 1, x, y + height - 1, p);
            DrawLine(x, y + height - 1, x, y, p);
        }

        public void FillRect(int x, int y, int width, int height, char p)
        {
            for(int X = 0; X < width; X++)
            {
                for (int Y = 0; Y < height; Y++)
                {
                    Draw(x + X, y + Y, p);
                }
            }
        }

        public void DrawTriangle(int x1, int y1, int x2, int y2, int x3, int y3, char p)
        {
            DrawLine(x1, y1, x2, y2, p);
            DrawLine(x2, y2, x3, y3, p);
            DrawLine(x3, y3, x1, y1, p);
        }

        public void FillTriangle(int x1, int y1, int x2, int y2, int x3, int y3, char p)
        {
            void Swap(ref int a, ref int b) { int t = a; a = b; b = t; }
            void MakeLine(int sx, int ex, int ny) { for (int i = sx; i <= ex; i++) Draw(i, ny, p); }

            int t1x, t2x, y, minx, maxx, t1xp, t2xp;
            bool changed1 = false;
            bool changed2 = false;
            int signx1, signx2, dx1, dy1, dx2, dy2;
            int e1, e2;
            // Sort vertices
            if (y1 > y2) { Swap(ref y1, ref y2); Swap(ref x1, ref x2); }
            if (y1 > y3) { Swap(ref y1, ref y3); Swap(ref x1, ref x3); }
            if (y2 > y3) { Swap(ref y2, ref y3); Swap(ref x2, ref x3); }

            t1x = t2x = x1; y = y1;   // Starting points
            dx1 = x2 - x1; if (dx1 < 0) { dx1 = -dx1; signx1 = -1; }
            else signx1 = 1;
            dy1 = y2 - y1;

            dx2 = x3 - x1; if (dx2 < 0) { dx2 = -dx2; signx2 = -1; }
            else signx2 = 1;
            dy2 = y3 - y1;

            if (dy1 > dx1)
            {   // swap values
                Swap(ref dx1, ref dy1);
                changed1 = true;
            }
            if (dy2 > dx2)
            {   // swap values
                Swap(ref dy2, ref dx2);
                changed2 = true;
            }

            e2 = dx2 >> 1;
            // Flat top, just process the second half
            if (y1 == y2) goto next;
            e1 = dx1 >> 1;

            for (int i = 0; i < dx1;)
            {
                t1xp = 0; t2xp = 0;
                if (t1x < t2x) { minx = t1x; maxx = t2x; }
                else { minx = t2x; maxx = t1x; }
                // process first line until y value is about to change
                while (i < dx1)
                {
                    i++;
                    e1 += dy1;
                    while (e1 >= dx1)
                    {
                        e1 -= dx1;
                        if (changed1) t1xp = signx1;//t1x += signx1;
                        else goto next1;
                    }
                    if (changed1) break;
                    else t1x += signx1;
                }
            // Move line
            next1:
                // process second line until y value is about to change
                while (true)
                {
                    e2 += dy2;
                    while (e2 >= dx2)
                    {
                        e2 -= dx2;
                        if (changed2) t2xp = signx2;//t2x += signx2;
                        else goto next2;
                    }
                    if (changed2) break;
                    else t2x += signx2;
                }
            next2:
                if (minx > t1x) minx = t1x; if (minx > t2x) minx = t2x;
                if (maxx < t1x) maxx = t1x; if (maxx < t2x) maxx = t2x;
                MakeLine(minx, maxx, y);    // Draw line from min to max points found on the y
                                            // Now increase y
                if (!changed1) t1x += signx1;
                t1x += t1xp;
                if (!changed2) t2x += signx2;
                t2x += t2xp;
                y += 1;
                if (y == y2) break;

            }
        next:
            // Second half
            dx1 = x3 - x2; if (dx1 < 0) { dx1 = -dx1; signx1 = -1; }
            else signx1 = 1;
            dy1 = y3 - y2;
            t1x = x2;

            if (dy1 > dx1)
            {   // swap values
                Swap(ref dy1, ref dx1);
                changed1 = true;
            }
            else changed1 = false;

            e1 = dx1 >> 1;

            for (int i = 0; i <= dx1; i++)
            {
                t1xp = 0; t2xp = 0;
                if (t1x < t2x) { minx = t1x; maxx = t2x; }
                else { minx = t2x; maxx = t1x; }
                // process first line until y value is about to change
                while (i < dx1)
                {
                    e1 += dy1;
                    while (e1 >= dx1)
                    {
                        e1 -= dx1;
                        if (changed1) { t1xp = signx1; break; }//t1x += signx1;
                        else goto next3;
                    }
                    if (changed1) break;
                    else t1x += signx1;
                    if (i < dx1) i++;
                }
            next3:
                // process second line until y value is about to change
                while (t2x != x3)
                {
                    e2 += dy2;
                    while (e2 >= dx2)
                    {
                        e2 -= dx2;
                        if (changed2) t2xp = signx2;
                        else goto next4;
                    }
                    if (changed2) break;
                    else t2x += signx2;
                }
            next4:

                if (minx > t1x) minx = t1x; if (minx > t2x) minx = t2x;
                if (maxx < t1x) maxx = t1x; if (maxx < t2x) maxx = t2x;
                MakeLine(minx, maxx, y);
                if (!changed1) t1x += signx1;
                t1x += t1xp;
                if (!changed2) t2x += signx2;
                t2x += t2xp;
                y += 1;
                if (y > y3) return;
            }
        }

        public void DrawCurve(Point[] points, char p)
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

        public void DrawCircle(int x, int y, int width, int height, char p)
        {
            for (double angle = 0; angle < Maths.DegreesToRadians(360); angle += 0.05)
            {
                int px = (int)(x + width * Math.Cos(angle));
                int py = (int)(y + height * Math.Sin(angle));

                Draw(px, py, p);
            }
        }

        public void FillCircle(int x, int y, int width, int height, char p)
        {
            for (int Y = -height; Y <= height; Y++)
            {
                for (int X = -width; X <= width; X++)
                {
                    if (X * X * height * height + Y * Y * width * width <= height * height * width * width)
                        Draw(x + X, y + Y, p);
                }
            }
        }


        bool ConsoleEventCallback(int eventType)
        {
            if (eventType == 2)
            {
                OnDestroy();
            }
            return false;
        }

        #region Overrides
        public virtual void OnCreate() { }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="elapsedTime">The time since last update in seconds</param>
        public virtual void OnUpdate(float elapsedTime) { }
        public virtual void OnDestroy() { }
        #endregion
    }
}
