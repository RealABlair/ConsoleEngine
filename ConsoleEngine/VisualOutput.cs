using System;
using System.IO;
using System.Text;

namespace ConsoleEngine
{
    public static class VisualOutput
    {
        public static StringBuilder ScreenBuilder = new StringBuilder();

        static readonly BufferedStream stream;

        static VisualOutput()
        {
            Console.OutputEncoding = Encoding.Unicode;

            stream = new BufferedStream(Console.OpenStandardOutput(), 0x15000);
        }

        public static void WriteLine(string s) => Write(s + "\r\n");

        public static void Write(string s)
        {
            byte[] b = new byte[s.Length << 1];
            Encoding.Unicode.GetBytes(s, 0, s.Length, b, 0);

            lock (stream)
                stream.Write(b, 0, b.Length);
        }

        public static void Flush() { lock(stream) stream.Flush(); }
    }
}
