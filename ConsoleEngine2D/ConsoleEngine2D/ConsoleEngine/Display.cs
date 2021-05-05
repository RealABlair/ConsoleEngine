using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

using static ConsoleEngine.Windows;

namespace ConsoleEngine
{
    public class Display
    {
        public bool isFocused { get { return (GetConsoleWindow() == GetForegroundWindow()); } }

        public const int PIXEL_HEIGHT = 16;
        public const int PIXEL_WIDTH = 8;

        public int SCREEN_WIDTH { get { return GetSystemMetrics(0); } }
        public int SCREEN_HEIGHT { get { return GetSystemMetrics(1); } }

        public void Init()
        {
            var iStdOut = GetStdHandle(STD_OUTPUT_HANDLE); GetConsoleMode(iStdOut, out var outConsoleMode); SetConsoleMode(iStdOut, outConsoleMode | ENABLE_VIRTUAL_TERMINAL_PROCESSING);
        }

        public string Colorize(string text, Pixel color, ColorType type, Pixel EXTENDED = null)
        {
            if(type == ColorType.BackForeground && EXTENDED != null)
                return $"\x1b[{48};2;{EXTENDED.R};{EXTENDED.G};{EXTENDED.B}m\x1b[{38};2;{color.R};{color.G};{color.B}m{text}\x1b[0m";
            return $"\x1b[{(byte)type};2;{color.R};{color.G};{color.B}m{text}\x1b[0m";
        }

        public void ChangeFont(int Width, int Height, string FontName = "Consolas")
        {
            CONSOLE_FONT_INFO_EX ConsoleFontInfo = new CONSOLE_FONT_INFO_EX();
            ConsoleFontInfo.cbSize = (uint)Marshal.SizeOf(ConsoleFontInfo);
            ConsoleFontInfo.FaceName = FontName;
            ConsoleFontInfo.dwFontSize.X = (short)Width;
            ConsoleFontInfo.dwFontSize.Y = (short)Height;
            SetCurrentConsoleFontEx(GetStdHandle(STD_OUTPUT_HANDLE), false, ref ConsoleFontInfo);
        }

        public void QuickEditMode(bool Enable)
        {
            IntPtr consoleHandle = GetStdHandle(STD_INPUT_HANDLE);
            UInt32 consoleMode;

            GetConsoleMode(consoleHandle, out consoleMode);
            if (Enable)
                consoleMode |= ((uint)ENABLE_QUICK_EDIT);
            else
                consoleMode &= ~((uint)ENABLE_QUICK_EDIT);

            consoleMode |= ((uint)ENABLE_EXTENDED_FLAGS);

            SetConsoleMode(consoleHandle, consoleMode);
        }

        public void Move(int x, int y, int width, int height)
        {
            MoveWindow(GetConsoleWindow(), x, y, width, height, false);
        }

        public RECT GetWindowRectangle()
        {
            RECT tmp = new RECT();
            GetWindowRect(GetConsoleWindow(), ref tmp);
            return tmp;
        }

        public Point GetCursor()
        {
            POINT c;
            GetCursorPos(out c);
            return new Point(c.X, c.Y);
        }

        public int GetConsoleWidth()
        {
            return GetWindowRectangle().right - GetWindowRectangle().left;
        }

        public int GetConsoleHeight()
        {
            return GetWindowRectangle().bottom - GetWindowRectangle().top;
        }

        public enum ColorType : byte
        {
            Foreground = 38,
            Background = 48,
            BackForeground = 48 | 38
        }
    }
}
