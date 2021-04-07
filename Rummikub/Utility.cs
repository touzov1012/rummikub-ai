using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rummikub
{
    static class Utility
    {
        public static Random r = new Random(1337);

        public static void Log(object s, bool condition = true, ConsoleColor background = ConsoleColor.Black, ConsoleColor foreground = ConsoleColor.White, bool newline = true)
        {
            Console.ForegroundColor = foreground;
            Console.BackgroundColor = background;

            if (condition)
            {
                if (newline)
                    Console.WriteLine(s.ToString());
                else
                    Console.Write(s.ToString());
            }

            Console.ResetColor();
        }

        public static void Warning(object s, bool condition = true)
        {
            Log(string.Format("WARNING: {0}", s.ToString()), condition, ConsoleColor.Black, ConsoleColor.Yellow);
        }

        public static void Error(object s, bool condition = true)
        {
            Log(string.Format("ERROR: {0}", s.ToString()), condition, ConsoleColor.Black, ConsoleColor.Red);
        }

        public static void Continue()
        {
            Log("Press any key to continue...");
            Console.ReadKey(true);
        }

        public static void Log(this Chip chip)
        {
            ConsoleColor chipColorback = ConsoleColor.Black;
            ConsoleColor chipColorfront = ConsoleColor.White;
            if (Chip.COLORCOUNT > 0 && chip.color[0]) { chipColorback = ConsoleColor.Red; chipColorfront = ConsoleColor.White; };
            if (Chip.COLORCOUNT > 1 && chip.color[1]) { chipColorback = ConsoleColor.Green; chipColorfront = ConsoleColor.Black; };
            if (Chip.COLORCOUNT > 2 && chip.color[2]) { chipColorback = ConsoleColor.Blue; chipColorfront = ConsoleColor.White; };
            if (Chip.COLORCOUNT > 3 && chip.color[3]) { chipColorback = ConsoleColor.Yellow; chipColorfront = ConsoleColor.Black; };
            if (Chip.COLORCOUNT > 4 && chip.color[4]) { chipColorback = ConsoleColor.Magenta; chipColorfront = ConsoleColor.Black; };
            if (Chip.COLORCOUNT > 5 && chip.color[5]) { chipColorback = ConsoleColor.White; chipColorfront = ConsoleColor.Black; };
            if (Chip.COLORCOUNT > 6) Error("Not enough colors to draw chips!");

            int chipNumber = 0;
            for (int i = 0; i < Chip.NUMBERCOUNT; i++)
                if (chip.number[i])
                    chipNumber = i + 1;

            Log(chipNumber, true, chipColorback, chipColorfront, false);
            Log(" ", newline: false);
        }

        public static void Log(this IEnumerable<Chip> chips)
        {
            foreach (Chip chip in chips)
                chip.Log();
            Console.WriteLine();
        }

        public static void Shuffle<T>(this IList<T> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = r.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }
    }
}
