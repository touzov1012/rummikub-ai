using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rummikub
{
    class Chip
    {
        public const int COLORCOUNT = 4;
        public const int NUMBERCOUNT = 13;

        public bool[] color = new bool[COLORCOUNT];
        public bool[] number = new bool[NUMBERCOUNT];

        public Chip(int color, int number)
        {
            this.color[color] = true;
            this.number[number] = true;
        }
    }
}
