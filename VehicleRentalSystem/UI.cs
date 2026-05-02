using System;

namespace VehicleRentalSystem
{
    public static class UI
    {
        private const int Width = 80;
        // Centralized Format String
        public const string TableFmt = "{0,-4} | {1,-12} | {2,-10} | {3,-8} | {4,-10} | {5,-8} | {6,-10}";

        public static void Header(string title)
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(new string('=', Width));
            int spaces = (Width - title.Length) / 2;
            Console.WriteLine(new string(' ', Math.Max(0, spaces)) + title.ToUpper());
            Console.WriteLine(new string('=', Width));
            Console.ResetColor();
        }

        public static void DrawTable(string[] columns)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine(TableFmt, columns[0], columns[1], columns[2], columns[3], columns[4], columns[5], columns[6]);
            DrawTableLine();
            Console.ResetColor();
        }

        
        public static void DrawRow(object[] data, ConsoleColor color = ConsoleColor.White)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(TableFmt, data);
            Console.ResetColor();
        }

        public static void DrawTableLine()
        {
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine(new string('-', Width));
            Console.ResetColor();
        }

        public static void Success(string m)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"\n [✔] SUCCESS: {m}");
            Console.ResetColor();
            Console.WriteLine("\nPress Enter to continue...");
            Console.ReadLine();
        }

        public static void Error(string m)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"\n [✘] ERROR: {m}");
            Console.ResetColor();
            Console.WriteLine("\nPress Enter to return...");
            Console.ReadLine();
        }
    }
}