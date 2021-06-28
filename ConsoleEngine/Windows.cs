using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace ConsoleEngine
{
    public class Windows
    {
        #region Imports
        [DllImport(KERNEL)]
        public static extern bool GetConsoleMode(IntPtr hConsoleHandle, out uint lpMode);

        [DllImport(USER)]
        public static extern int SetWindowLong(IntPtr hWnd, int nIndex, uint dwNewLong);

        [DllImport(KERNEL)]
        public static extern bool SetConsoleMode(IntPtr hConsoleHandle, uint dwMode);

        [DllImport(KERNEL, SetLastError = true)]
        public static extern IntPtr GetStdHandle(int nStdHandle);

        [DllImport(KERNEL)]
        public static extern IntPtr GetConsoleWindow();

        [DllImport(USER)]
        public static extern IntPtr GetForegroundWindow();

        [DllImport(USER, SetLastError = true)]
        public static extern bool MoveWindow(IntPtr hWnd, int X, int Y, int nWidth, int nHeight, bool bRepaint);

        [DllImport(USER, CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall, ExactSpelling = true, SetLastError = true)]
        internal static extern bool GetWindowRect(IntPtr hWnd, ref RECT rect);

        [DllImport(USER)]
        public static extern bool GetClientRect(IntPtr hWnd, out RECT lpRect);

        [DllImport(USER, ExactSpelling = true, CharSet = CharSet.Auto)]
        public static extern int GetSystemMetrics(int nIndex);
        [DllImport(KERNEL, SetLastError = true)]
        public static extern Int32 SetCurrentConsoleFontEx(IntPtr ConsoleOutput, bool MaximumWindow, ref CONSOLE_FONT_INFO_EX ConsoleCurrentFontEx);
        [DllImport(USER, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetCursorPos(out POINT lpPoint);

        #endregion

        #region Consts
        public const string KERNEL = "kernel32.dll";
        public const string USER = "user32.dll";

        public const int STD_INPUT_HANDLE = -10;
        public const int STD_OUTPUT_HANDLE = -11;
        public const int STD_ERROR_HANDLE = -12;
            

        public const uint ENABLE_VIRTUAL_TERMINAL_PROCESSING = 0x0004;
        public const uint ENABLE_QUICK_EDIT = 0x0040;
        public const uint ENABLE_EXTENDED_FLAGS = 0x0080;
        #endregion

        #region Structs

        [StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            public int X;
            public int Y;

            public POINT(int x, int y)
            {
                this.X = x;
                this.Y = y;
            }
        }
        public struct RECT
        {
            public int left;
            public int top;
            public int right;
            public int bottom;
        }
        [StructLayout(LayoutKind.Sequential)]
        public struct COORD
        {
            public short X;
            public short Y;

            public COORD(short X, short Y)
            {
                this.X = X;
                this.Y = Y;
            }
        };
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct CONSOLE_FONT_INFO_EX
        {
            public uint cbSize;
            public uint nFont;
            public COORD dwFontSize;
            public int FontFamily;
            public int FontWeight;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)] // Edit sizeconst if the font name is too big
            public string FaceName;
        }
        #endregion
    }
}
