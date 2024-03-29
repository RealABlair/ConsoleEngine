﻿using System;
using System.Threading;
using ABSoftware;
using static ConsoleEngine.Windows;

namespace ConsoleEngine
{
    public abstract class EngineChar : Display
    {
        public int framerate;
        public bool enabled;
        public int ScreenWidth, ScreenHeight;
        CHAR_INFO[] pixels;
        SMALL_RECT rect;
        Thread gameLoop;
        Timer frameTimer;
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
            SetConsoleScreenBufferSize(ConsoleHandleOut, new COORD((short)ScreenWidth, (short)ScreenHeight));
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

        public void Start()
        {
            HandlerInit(ConsoleEventCallback);
            SetCtrlHandler(handler, true);
            Console.CursorVisible = false;
            enabled = true;
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
                switch(inputs[i].EventType)
                {
                    case FOCUS_EVENT:
                        {
                            isFocused = inputs[i].Event.FocusEvent.bSetFocus;
                        }
                        break;
                    case MOUSE_EVENT:
                        {
                            switch(inputs[i].Event.MouseEvent.dwEventFlags)
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
                                case MouseEventFlags.DOUBLE_CLICK:
                                    {
                                        for (int m = 0; m < 5; m++)
                                            mouse.newMouseStamp[m] = ((ushort)inputs[i].Event.MouseEvent.dwButtonState & (1 << m)) > 0;
                                    }
                                    break;
                                case MouseEventFlags.MOUSE_WHEELED:
                                    {
                                        short direction = (short)((int)inputs[i].Event.MouseEvent.dwButtonState >> 16);
                                        mouse.newMwheelUp = direction > 0;
                                        mouse.newMwheelDown = direction < 0;
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

        public ushort GetPixelColor(int x, int y)
        {
            return pixels[y * ScreenWidth + x].Attributes;
        }

        public void Clear(char pixel)
        {
            for (int i = 0; i < pixels.Length; i++)
            {
                pixels[i].UnicodeChar = (short)pixel;
                pixels[i].Attributes = FG_GRAY;
            }
        }

        private void Redraw()
        {
            Console.SetCursorPosition(0, 0);
            Console.CursorVisible = false;

            WriteConsoleOutput(ConsoleHandleOut, pixels, new COORD((short)ScreenWidth, (short)ScreenHeight), new COORD(0, 0), ref rect);
        }

        public void Draw(int x, int y, char p = '█')
        {
            if (x > ScreenWidth - 1 || y > ScreenHeight - 1 || x < 0 || y < 0)
                return;
            pixels[y * ScreenWidth + x].UnicodeChar = (short)p;
            pixels[y * ScreenWidth + x].Attributes = FG_GRAY;
        }

        public void DrawLine(int xStart, int yStart, int xEnd, int yEnd, char p = '█')
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

        public void DrawRect(int x, int y, int width, int height, char p = '█')
        {
            DrawLine(x, y, x + width - 1, y, p);
            DrawLine(x + width - 1, y, x + width - 1, y + height - 1, p);
            DrawLine(x + width - 1, y + height - 1, x, y + height - 1, p);
            DrawLine(x, y + height - 1, x, y, p);
        }

        public void FillRect(int x, int y, int width, int height, char p = '█')
        {
            for (int X = 0; X < width; X++)
            {
                for (int Y = 0; Y < height; Y++)
                {
                    Draw(x + X, y + Y, p);
                }
            }
        }

        public void DrawTriangle(int x1, int y1, int x2, int y2, int x3, int y3, char p = '█')
        {
            DrawLine(x1, y1, x2, y2, p);
            DrawLine(x2, y2, x3, y3, p);
            DrawLine(x3, y3, x1, y1, p);
        }

        public void FillTriangle(int x1, int y1, int x2, int y2, int x3, int y3, char p = '█')
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

        public void DrawCurve(Point[] points, char p = '█')
        {
            for (int i = 0; i < points.Length; i++)
            {
                Draw(points[i].x, points[i].y, p);
            }

            for (int i = 0; i < points.Length - 1; i++)
            {
                DrawLine(points[i].x, points[i].y, points[i + 1].x, points[i + 1].y, p);
            }
        }

        public void DrawCircle(int x, int y, int width, int height, char p = '█')
        {
            float dx, dy, d1, d2, x1, y1;
            x1 = 0;
            y1 = height;

            d1 = (height * height) - (width * width * height) + (0.25f * width * width);
            dx = 2 * height * height * x1;
            dy = 2 * width * width * y1;

            while (dx < dy)
            {
                Draw((int)(x1 + x), (int)(y1 + y), p);
                Draw((int)(-x1 + x), (int)(y1 + y), p);
                Draw((int)(x1 + x), (int)(-y1 + y), p);
                Draw((int)(-x1 + x), (int)(-y1 + y), p);

                if (d1 < 0)
                {
                    x1++;
                    dx = dx + (2 * height * height);
                    d1 = d1 + dx + (height * height);
                }
                else
                {
                    x1++;
                    y1--;
                    dx = dx + (2 * height * height);
                    dy = dy - (2 * width * width);
                    d1 = d1 + dx - dy + (height * height);
                }
            }
            d2 = ((height * height) * ((x1 + 0.5f) * (x1 + 0.5f))) + ((width * width) * ((y1 - 1f) * (y1 - 1f))) - (width * width * height * height);

            while (y1 >= 0)
            {
                Draw((int)(x1 + x), (int)(y1 + y), p);
                Draw((int)(-x1 + x), (int)(y1 + y), p);
                Draw((int)(x1 + x), (int)(-y1 + y), p);
                Draw((int)(-x1 + x), (int)(-y1 + y), p);

                if (d2 > 0)
                {
                    y1--;
                    dy = dy - (2 * width * width);
                    d2 = d2 + (width * width) - dy;
                }
                else
                {
                    y1--;
                    x1++;
                    dx = dx + (2 * height * height);
                    dy = dy - (2 * width * width);
                    d2 = d2 + dx - dy + (width * width);
                }
            }
        }

        public void FillCircle(int x, int y, int width, int height, char p = '█')
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

        #region Math
        public float Sinf(float a) => (float)Math.Sin(a);
        public float Cosf(float a) => (float)Math.Cos(a);
        public float Tanf(float a) => (float)Math.Tan(a);
        public float Asinf(float d) => (float)Math.Asin(d);
        public float Acosf(float d) => (float)Math.Acos(d);
        public float Atanf(float d) => (float)Math.Atan(d);
        public float Atan2f(float y, float x) => (float)Math.Atan2(y, x);
        public float Absf(float value) => Math.Abs(value);
        public float RadToDeg(float radians) => (radians / PI) * 180f;
        public float DegToRad(float degrees) => (degrees / 180f) * PI;
        public float Maxf(float a, float b) => (a > b) ? a : b;
        public float Minf(float a, float b) => (a < b) ? a : b;
        public float PI => (float)Math.PI;
        public float PI2 => PI * 2f;
        #endregion

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