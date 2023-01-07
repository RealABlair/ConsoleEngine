﻿using System;
using System.Threading;
using ABSoftware;
using static ConsoleEngine.Windows;

namespace ConsoleEngine
{
    public abstract class ColoredEngineChar : Display
    {
        public int framerate;
        public bool enabled;
        public int ScreenWidth, ScreenHeight;
        CHAR_INFO[] pixels;
        SMALL_RECT rect;
        Thread gameLoop;
        Timer frameTimer;
        long startTime;
        DateTime lastUpdateTime = DateTime.Now;
        public string AppName { get { return Console.Title; } set { Console.Title = value; } }

        //User Things
        public bool UpdateWithoutFocusing = true;
        public short mousePosX = 0;
        public short mousePosY = 0;

        public Mouse mouse = new Mouse();
        public Keyboard keyboard = new Keyboard();

        public void Construct(int Width, int Height, int framerate = -1)
        {
            Init();
            keyboard.Initialize();
            AppName = GetType().Name;
            ScreenWidth = Width;
            ScreenHeight = Height;
            Console.SetWindowSize(Width, Height);
            this.framerate = framerate;
            frameTimer = new Timer(1000.0f / framerate);
            lastUpdateTime = DateTime.Now;
            pixels = new CHAR_INFO[Width * Height];

            rect = new SMALL_RECT(0, 0, (short)(ScreenWidth - 1), (short)(ScreenHeight - 1));
            SetConsoleWindowInfo(ConsoleHandleOut, true, ref rect);

            for (int i = 0; i < pixels.Length; i++)
            {
                pixels[i].UnicodeChar = (short)' ';
                pixels[i].Attributes = 0x0000;
            }
        }

        public long GetCurrentTimeMillis()
        {
            return startTime;
        }

        public void Start()
        {
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
            enabled = false;
            gameLoop.Join(100);
        }

        public void GameLoop()
        {
            OnCreate();
            while (enabled)
            {
                while (enabled)
                {
                    if (!frameTimer.Tick() || !UpdateWithoutFocusing && !isFocused)
                        continue;

                    ReadInput();
                    DateTime dt = DateTime.Now;
                    OnUpdate((float)(dt - lastUpdateTime).TotalSeconds);
                    lastUpdateTime = dt;
                    Redraw();
                    startTime++;
                }

                OnDestroy();
            }
        }

        private void ReadInput()
        {
            uint count = 0;
            GetNumberOfConsoleInputEvents(ConsoleHandleIn, out count);
            INPUT_RECORD[] inputs = new INPUT_RECORD[count];
            ReadConsoleInput(ConsoleHandleIn, inputs, count, out count);
            for (int i = 0; i < count; i++)
            {
                switch (inputs[i].EventType)
                {
                    case FOCUS_EVENT:
                        {
                            isFocused = inputs[i].Event.FocusEvent.bSetFocus;
                        }
                        break;
                    case MOUSE_EVENT:
                        {
                            switch (inputs[i].Event.MouseEvent.dwEventFlags)
                            {
                                case MouseEventFlags.MOUSE_MOVED:
                                    {
                                        mousePosX = inputs[i].Event.MouseEvent.dwMousePosition.X;
                                        mousePosY = inputs[i].Event.MouseEvent.dwMousePosition.Y;
                                    }
                                    break;
                                case 0:
                                    {
                                        for (int m = 0; m < 5; m++)
                                            mouse.newMouseStamp[m] = ((ushort)inputs[i].Event.MouseEvent.dwButtonState & (1 << m)) > 0;
                                    }
                                    break;
                                default:
                                    break;
                            }
                        }
                        break;
                    default:
                        break;
                }
            }
            mouse.UpdateStates();
            keyboard.UpdateStates();
        }

        public void Resize(int width, int height)
        {
            pixels = new CHAR_INFO[width * height];
            ScreenWidth = width;
            ScreenHeight = height;
            Console.SetWindowSize(width, height);
            rect = new SMALL_RECT(0, 0, (short)(ScreenWidth - 1), (short)(ScreenHeight - 1));
            SetConsoleWindowInfo(ConsoleHandleOut, true, ref rect);
        }

        public char GetPixel(int x, int y)
        {
            return (char)pixels[y * ScreenWidth + x].UnicodeChar;
        }

        public void Clear(char pixel, ushort color = 0x0000)
        {
            for (int i = 0; i < pixels.Length; i++)
            {
                pixels[i].UnicodeChar = (short)pixel;
                pixels[i].Attributes = color;
            }
        }

        private void Redraw()
        {
            Console.SetCursorPosition(0, 0);
            Console.CursorVisible = false;

            WriteConsoleOutput(ConsoleHandleOut, pixels, new COORD((short)ScreenWidth, (short)ScreenHeight), new COORD(0, 0), ref rect);
        }

        public void Draw(int x, int y, char p = '█', ushort color = 0x0000)
        {
            if (x > ScreenWidth - 1 || y > ScreenHeight - 1 || x < 0 || y < 0)
                return;
            pixels[y * ScreenWidth + x].UnicodeChar = (short)p;
            pixels[y * ScreenWidth + x].Attributes = color;
        }

        public void DrawLine(int xStart, int yStart, int xEnd, int yEnd, char p = '█', ushort color = 0x0000)
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
                Draw(xStart, yStart, p, color);
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

        public void DrawRect(int x, int y, int width, int height, char p = '█', ushort color = 0x0000)
        {
            DrawLine(x, y, x + width - 1, y, p, color);
            DrawLine(x + width - 1, y, x + width - 1, y + height - 1, p, color);
            DrawLine(x + width - 1, y + height - 1, x, y + height - 1, p, color);
            DrawLine(x, y + height - 1, x, y, p, color);
        }

        public void FillRect(int x, int y, int width, int height, char p = '█', ushort color = 0x0000)
        {
            for (int X = 0; X < width; X++)
            {
                for (int Y = 0; Y < height; Y++)
                {
                    Draw(x + X, y + Y, p, color);
                }
            }
        }

        public void DrawTriangle(int x1, int y1, int x2, int y2, int x3, int y3, char p = '█', ushort color = 0x0000)
        {
            DrawLine(x1, y1, x2, y2, p, color);
            DrawLine(x2, y2, x3, y3, p, color);
            DrawLine(x3, y3, x1, y1, p, color);
        }

        public void FillTriangle(int x1, int y1, int x2, int y2, int x3, int y3, char p = '█', ushort color = 0x0000)
        {
            void Swap(ref int a, ref int b) { int t = a; a = b; b = t; }
            void MakeLine(int sx, int ex, int ny) { for (int i = sx; i <= ex; i++) Draw(i, ny, p, color); }

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

        public void DrawCurve(Point[] points, char p = '█', ushort color = 0x0000)
        {
            for (int i = 0; i < points.Length; i++)
            {
                Draw(points[i].x, points[i].y, p, color);
            }

            for (int i = 0; i < points.Length - 1; i++)
            {
                DrawLine(points[i].x, points[i].y, points[i + 1].x, points[i + 1].y, p, color);
            }
        }

        public void DrawCircle(int x, int y, int width, int height, char p = '█', ushort color = 0x0000)
        {
            for (double angle = 0.000001; angle < Maths.DegreesToRadians(360); angle += 0.05)
            {
                int px = (int)(x + width * Math.Cos(angle));
                int py = (int)(y + height * Math.Sin(angle));

                Draw(px, py, p, color);
            }
        }

        public void FillCircle(int x, int y, int width, int height, char p = '█', ushort color = 0x0000)
        {
            for (int Y = -height; Y <= height; Y++)
            {
                for (int X = -width; X <= width; X++)
                {
                    if (X * X * height * height + Y * Y * width * width <= height * height * width * width)
                        Draw(x + X, y + Y, p, color);
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