using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ConsoleEngine;
using ABSoftware;

namespace ConsoleEngine2D
{
    public class Thing : Engine
    {
        static int PixelWidth = 8;
        static int PixelHeight = 8;

        static void Main(string[] args)
        {
            Thing t = new Thing();
            t.ChangeFont(PixelWidth, PixelHeight);
            t.Construct(50, 50, -1);
            t.Start();
        }

        public override void OnCreate()
        {
            QuickEditMode(false);
        }

        bool pressed = false;

        public override void OnUpdate()
        {
            Clear(Pixel.Black);
            glyphRenderer.Clear();

            Point loc = new Point(10, 10);
            Point siz = new Point(30, 15);

            FillRect(loc.x, loc.y, siz.x, siz.y, (pressed) ? Pixel.Yellow : Pixel.Amber);

            if (Keyboard.GetAsyncKeyState(Keyboard.Keys.VK_LBUTTON) != 0 && isFocused)
            {
                Point click = ScreenPosToPixelPos(GetCursor());
                if (click.x >= loc.x && click.y >= loc.y && click.x <= loc.x + siz.x && click.y <= loc.y + siz.y)
                    pressed = true;
                else
                    pressed = false;
            }
            else if (isFocused)
            {
                pressed = false;
            }
        }

        public Point ScreenPosToPixelPos(Point point)
        {
            int xoffset = 8;
            int yoffset = 30;


            int x = (point.x - GetWindowRectangle().left - xoffset) / PixelWidth;
            int y = (point.y - GetWindowRectangle().top - yoffset) / PixelHeight;

            return new Point(x, y);
        }
    }
}
