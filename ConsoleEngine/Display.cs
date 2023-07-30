using System;
using System.Runtime.InteropServices;
using System.Text;
using static ConsoleEngine.Windows;

namespace ConsoleEngine
{
    public class Display
    {
        public bool isFocused = true;

        public const int PIXEL_HEIGHT = 16;
        public const int PIXEL_WIDTH = 8;

        public int SCREEN_WIDTH { get { return GetSystemMetrics(0); } }
        public int SCREEN_HEIGHT { get { return GetSystemMetrics(1); } }

        public ConsoleEventDelegate handler;

        public IntPtr ConsoleHandleIn = IntPtr.Zero;
        public IntPtr ConsoleHandleOut = IntPtr.Zero;
        public IntPtr ConsoleHWND = IntPtr.Zero;

        public void HandlerInit(Func<int, bool> myMethodName)
        {
            handler = new ConsoleEventDelegate(myMethodName);
        }

        public void Init()
        {
            this.ConsoleHandleIn = GetStdHandle(STD_INPUT_HANDLE);
            this.ConsoleHandleOut = GetStdHandle(STD_OUTPUT_HANDLE);
            this.ConsoleHWND = GetConsoleWindow();
            var iStdOut = GetStdHandle(STD_OUTPUT_HANDLE); GetConsoleMode(iStdOut, out var outConsoleMode); SetConsoleMode(iStdOut, outConsoleMode | ENABLE_VIRTUAL_TERMINAL_PROCESSING | 0x0001);
            SetWindowLong(GetConsoleWindow(), (int)GWL.GWL_STYLE, (uint)((GetWindowLong(GetConsoleWindow(), (int)GWL.GWL_STYLE)) & ~(int)WS.WS_MAXIMIZEBOX & ~(int)WS.WS_SIZEFRAME));
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

        public bool ChangePaletteHex(byte paletteColorIndex, uint hexColor)
        {
            return ChangePalette(paletteColorIndex, RGB(hexColor));
        }

        public bool ChangePaletteHex(uint[] newHexPalette)
        {
            for (int i = 0; i < newHexPalette.Length; i++) newHexPalette[i] = RGB(newHexPalette[i]);
            return ChangePalette(newHexPalette);
        }

        public bool ChangePalette(byte paletteColorIndex, uint color)
        {
            CONSOLE_SCREEN_BUFFER_INFO_EX csbix = new CONSOLE_SCREEN_BUFFER_INFO_EX();
            csbix.cbSize = (uint)Marshal.SizeOf(csbix);
            if (!GetConsoleScreenBufferInfoEx(this.ConsoleHandleOut, ref csbix)) return false;
            csbix.srWindow.Bottom++;
            
            csbix.ColorTable[paletteColorIndex] = color;

            if (!SetConsoleScreenBufferInfoEx(this.ConsoleHandleOut, ref csbix)) return false;

            return true;
        }

        public bool ChangePalette(uint[] newPalette)
        {
            CONSOLE_SCREEN_BUFFER_INFO_EX csbix = new CONSOLE_SCREEN_BUFFER_INFO_EX();
            csbix.cbSize = (uint)Marshal.SizeOf(csbix);
            if (!GetConsoleScreenBufferInfoEx(this.ConsoleHandleOut, ref csbix)) return false;
            csbix.srWindow.Bottom++;
            csbix.ColorTable = newPalette;

            if (!SetConsoleScreenBufferInfoEx(this.ConsoleHandleOut, ref csbix)) return false;

            return true;
        }

        public bool GetScreenBufferInfo(out CONSOLE_SCREEN_BUFFER_INFO_EX buffer)
        {
            buffer = new CONSOLE_SCREEN_BUFFER_INFO_EX();
            buffer.cbSize = (uint)Marshal.SizeOf(buffer);
            bool flag = GetConsoleScreenBufferInfoEx(this.ConsoleHandleOut, ref buffer);
            buffer.srWindow.Bottom++;
            return flag;
        }

        public bool SetScreenBufferInfo(CONSOLE_SCREEN_BUFFER_INFO_EX buffer)
        {
            return SetConsoleScreenBufferInfoEx(this.ConsoleHandleOut, ref buffer);
        }

        public void QuickEditMode(bool Enable)
        {
            IntPtr consoleHandle = ConsoleHandleIn;
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
            MoveWindow(this.ConsoleHWND, x, y, width, height, true);
        }

        public void SetCtrlHandler(ConsoleEventDelegate callback, bool add)
        {
            SetConsoleCtrlHandler(callback, add);
        }

        public RECT GetClientRectangle()
        {
            RECT tmp = new RECT();
            GetClientRect(this.ConsoleHWND, out tmp);
            return tmp;
        }

        public RECT GetWindowRectangle()
        {
            RECT tmp = new RECT();
            GetWindowRect(this.ConsoleHWND, ref tmp);
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

        #region Colors
        public const ushort FG_BLACK = 0x0000;
        public const ushort FG_DARK_BLUE = 0x0001;
        public const ushort FG_DARK_GREEN = 0x0002;
        public const ushort FG_DARK_CYAN = 0x0003;
        public const ushort FG_DARK_RED = 0x0004;
        public const ushort FG_DARK_MAGENTA = 0x0005;
        public const ushort FG_DARK_YELLOW = 0x0006;
        public const ushort FG_GRAY = 0x0007;
        public const ushort FG_DARK_GRAY = 0x0008;
        public const ushort FG_BLUE = 0x0009;
        public const ushort FG_GREEN = 0x000A;
        public const ushort FG_CYAN = 0x000B;
        public const ushort FG_RED = 0x000C;
        public const ushort FG_MAGENTA = 0x000D;
        public const ushort FG_YELLOW = 0x000E;
        public const ushort FG_WHITE = 0x000F;

        public const ushort BG_BLACK = 0x0000;
        public const ushort BG_DARK_BLUE = 0x0010;
        public const ushort BG_DARK_GREEN = 0x0020;
        public const ushort BG_DARK_CYAN = 0x0030;
        public const ushort BG_DARK_RED = 0x0040;
        public const ushort BG_DARK_MAGENTA = 0x0050;
        public const ushort BG_DARK_YELLOW = 0x0060;
        public const ushort BG_GRAY = 0x0070;
        public const ushort BG_DARK_GRAY = 0x0080;
        public const ushort BG_BLUE = 0x0090;
        public const ushort BG_GREEN = 0x00A0;
        public const ushort BG_CYAN = 0x00B0;
        public const ushort BG_RED = 0x00C0;
        public const ushort BG_MAGENTA = 0x00D0;
        public const ushort BG_YELLOW = 0x00E0;
        public const ushort BG_WHITE = 0x00F0;

        public const byte PALETTE_BLACK = 0x00;
        public const byte PALETTE_DARK_BLUE = 0x01;
        public const byte PALETTE_DARK_GREEN = 0x02;
        public const byte PALETTE_DARK_CYAN = 0x03;
        public const byte PALETTE_DARK_RED = 0x04;
        public const byte PALETTE_DARK_MAGENTA = 0x05;
        public const byte PALETTE_DARK_YELLOW = 0x06;
        public const byte PALETTE_GRAY = 0x07;
        public const byte PALETTE_DARK_GRAY = 0x08;
        public const byte PALETTE_BLUE = 0x09;
        public const byte PALETTE_GREEN = 0x0A;
        public const byte PALETTE_CYAN = 0x0B;
        public const byte PALETTE_RED = 0x0C;
        public const byte PALETTE_MAGENTA = 0x0D;
        public const byte PALETTE_YELLOW = 0x0E;
        public const byte PALETTE_WHITE = 0x0F;

        public uint RGB(byte r, byte g, byte b) => (uint)(r | (g << 8) | (b << 16));
        public uint RGB(uint hex) => (uint)((hex >> 16) & 0xFF | (hex & 0xFF) << 16 | hex & 0xFF00);

        public uint[] DefaultPaletteHEX { get { return new uint[] { 0x000000, 0x000080, 0x008000, 0x008080, 0x800000, 0x800080, 0x808000, 0xC0C0C0, 0x808080, 0x0000FF, 0x00FF00, 0x00FFFF, 0xFF0000, 0xFF00FF, 0xFFFF00, 0xFFFFFF }; } }
        public uint[] DefaultPaletteRGB { get { return new uint[] { 0x000000, 0x800000, 0x008000, 0x808000, 0x000080, 0x800080, 0x008080, 0xC0C0C0, 0x808080, 0xFF0000, 0x00FF00, 0xFFFF00, 0x0000FF, 0xFF00FF, 0x00FFFF, 0xFFFFFF }; } }
        #endregion
    }
}