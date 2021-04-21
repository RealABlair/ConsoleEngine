using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static ConsoleEngine.Windows;

namespace ConsoleEngine
{
    public class Display
    {
        public bool isFocused { get { return (GetConsoleWindow() == GetForegroundWindow()); } }

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

        public string SetDefaultColor()
        {
            return $"\x1b[0m";
        }

        public enum ColorType : byte
        {
            Foreground = 38,
            Background = 48,
            BackForeground = 48 | 38
        }
    }
}
