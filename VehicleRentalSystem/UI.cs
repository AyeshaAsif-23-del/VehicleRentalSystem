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

        // ---------------- TABLE HEADER ----------------
        public static void DrawTable(string[] columns)
        {
            if (columns.Length != 7)
            {
                Console.WriteLine("Invalid table format.");
                return;
            }

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine(TableFmt,
                columns[0], columns[1], columns[2],
                columns[3], columns[4], columns[5], columns[6]);

            DrawTableLine();
            Console.ResetColor();
        }

        // ---------------- TABLE ROW ----------------
        public static void DrawRow(object[] data, ConsoleColor color = ConsoleColor.White)
        {
            if (data.Length != 7)
            {
                Console.WriteLine("Invalid row data.");
                return;
            }

            Console.ForegroundColor = color;
            Console.WriteLine(TableFmt,
                data[0], data[1], data[2],
                data[3], data[4], data[5], data[6]);

            Console.ResetColor();
        }

        // ---------------- LINE ----------------
        public static void DrawTableLine()
        {
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine(new string('-', Width));
            Console.ResetColor();
        }

        // ---------------- SUCCESS MESSAGE ----------------
        public static void Success(string m)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"\n [✔] SUCCESS: {m}");
            Console.ResetColor();

            Pause();
        }

        // ---------------- ERROR MESSAGE ----------------
        public static void Error(string m)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"\n [✘] ERROR: {m}");
            Console.ResetColor();

            Pause();
        }

        // ---------------- COMMON PAUSE ----------------
        public static void Pause()
        {
            Console.WriteLine("\nPress Enter to continue...");
            Console.ReadLine();
        }
    }
}