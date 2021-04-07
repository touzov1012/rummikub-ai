using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rummikub
{
    class Program
    {
        static void Main(string[] args)
        {
            // number of chips in play, making this too large may slow down solver.
            int N = 30;

            // generate a standard deck and randomly draw n cards
            List<Chip> deck = Host.GenerateDeck();
            List<Chip> chips = Host.DrawRandom(deck, N);

            // draw the chips
            chips.Log();

            Utility.Continue();

            // find the optimal placement
            List<Chip[]> words = Solver.Solve(chips, 0);

            Utility.Log("Words played:");
            for (int i = 0; i < words.Count - 1; i++)
                words[i].Log();

            Utility.Log("Chips left over:");
            words[words.Count - 1].Log();

            Utility.Continue();
        }
    }
}
