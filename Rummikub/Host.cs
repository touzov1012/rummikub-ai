using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rummikub
{
    static class Host
    {
        public static List<Chip> GenerateDeck()
        {
            List<Chip> chips = new List<Chip>(Chip.COLORCOUNT * Chip.NUMBERCOUNT * 2);

            for (int i = 0; i < 2; i++)
                for (int j = 0; j < Chip.COLORCOUNT; j++)
                    for (int k = 0; k < Chip.NUMBERCOUNT; k++)
                        chips.Add(new Chip(j, k));

            return chips;
        }

        public static List<Chip> DrawRandom(List<Chip> deck, int cnt, bool remove = true)
        {
            if (deck == null || deck.Count == 0)
                return new List<Chip>();

            cnt = Math.Min(deck.Count, cnt);
            deck.Shuffle();
            List<Chip> sample = deck.Take(cnt).ToList();
            if (remove)
                deck.RemoveRange(0, cnt);
            return sample;
        }
    }
}
