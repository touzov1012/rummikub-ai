using ILOG.CPLEX;
using ILOG.Concert;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rummikub
{
    class Solver
    {
        /// <summary>
        /// return a list of words that can be played, left over letters are the last word
        /// </summary>
        /// <param name="chips">All of the chips to conisder, first ones are on the board</param>
        /// <param name="numberOnBoard">The number of leading chips are on the board already</param>
        /// <returns></returns>
        public static List<Chip[]> Solve(List<Chip> chips, int numberOnBoard, bool suppress = true)
        {
            if (chips == null || chips.Count == 0)
            {
                Utility.Warning("Chips is null or empty!");
                return null;
            }

            int chip_count = chips.Count;
            int word_count = chip_count / 3 + 1;

            Cplex model = new Cplex();

            IIntVar[] X = model.BoolVarArray(chip_count);       // should we place the ith chip
            IIntVar[][] Y = new IIntVar[chip_count][];          // which word do we place the ith chip on
            IIntVar[] WO = model.BoolVarArray(word_count);      // flag for if the jth word is an order word or not
            IIntVar[] WC = model.BoolVarArray(word_count);      // flag for if the jth word is a color word or not
            IIntVar[] WN = model.BoolVarArray(word_count);      // flag for if the jth word is not used or is used
            IIntVar[][] UO = new IIntVar[word_count][];         // what is the number associated with this word (used only if a color word)
            IIntVar[][] UC = new IIntVar[word_count][];         // what is the color associated with this word (used only if a order word)
            IIntVar[][] PC = new IIntVar[word_count][];         // if i maps to word j and j is a color word, this will be 1 at [j][i]
            IIntVar[][] PO = new IIntVar[word_count][];         // if i maps to word j and j is a order word, this will be 1 at [j][i] (if and only if)

            // Initialize
            for (int i = 0; i < chip_count; i++)
                Y[i] = model.BoolVarArray(word_count);

            for (int i = 0; i < word_count; i++)
                UC[i] = model.BoolVarArray(Chip.COLORCOUNT);

            for (int i = 0; i < word_count; i++)
                UO[i] = model.BoolVarArray(Chip.NUMBERCOUNT);

            for (int i = 0; i < word_count; i++)
                PC[i] = model.BoolVarArray(chip_count);

            for (int i = 0; i < word_count; i++)
                PO[i] = model.BoolVarArray(chip_count);

            // for each word which chips map to it
            IIntVar[][] Yt = new IIntVar[word_count][];
            for (int i = 0; i < word_count; i++)
            {
                Yt[i] = new IIntVar[chip_count];
                for (int j = 0; j < chip_count; j++)
                    Yt[i][j] = Y[j][i];
            }

            // maximize the number of placed chips
            model.AddMaximize(model.Sum(X));

            // if we place i, we need to map it to some word
            for(int i = 0; i < chip_count; i++)
                model.AddLe(X[i], model.Sum(Y[i]));

            // we can map to at most 1 value
            for (int i = 0; i < chip_count; i++)
                model.AddLe(model.Sum(Y[i]), 1);

            // if any chip maps to word j, turn the not used flag off
            for (int j = 0; j < word_count; j++)
                for (int i = 0; i < chip_count; i++)
                    model.AddLe(Y[i][j], model.Diff(1, WN[j]));

            // if a word is used, make sure it has at least 3 chips
            for (int j = 0; j < word_count; j++)
                model.AddLe(model.Prod(3, model.Diff(1, WN[j])), model.Sum(Yt[j]));

            // first 'numberOnBoard' chips are on the board and thus must be placed
            for (int i = 0; i < numberOnBoard; i++)
                model.AddEq(X[i], 1);

            // each word is either an order word or a color word
            for (int i = 0; i < word_count; i++)
                model.AddEq(model.Sum(WO[i], WC[i], WN[i]), 1);

            // if i maps to word j and j is a color word make sure PC[j][i] is 1
            for (int j = 0; j < word_count; j++)
                for (int i = 0; i < chip_count; i++)
                    model.AddGe(model.Sum(1, PC[j][i]), model.Sum(WC[j], Y[i][j]));

            // if i maps to word j and j is a order word, PO will be 1 at [j][i] (if and only if)
            for (int j = 0; j < word_count; j++)
                for (int i = 0; i < chip_count; i++)
                {
                    model.AddGe(model.Sum(1, PO[j][i]), model.Sum(WO[j], Y[i][j]));
                    model.AddLe(PO[j][i], WO[j]);
                    model.AddLe(PO[j][i], Y[i][j]);
                }

            // ************************************************
            // ************ TYPE CONSTRAINTS ******************
            // ************************************************

            for (int i = 0; i < chip_count; i++)
                for (int j = 0; j < word_count; j++)
                {
                    // if this is a color word and a chip maps to it then the numbers of each chip on this word must match
                    for (int k = 0; k < Chip.NUMBERCOUNT; k++)
                    {
                        var bnd = model.Sum(WO[j], model.Diff(1, Y[i][j]));
                        model.AddLe(model.Diff(UO[j][k], chips[i].number[k] ? 1 : 0), bnd);
                        model.AddLe(model.Diff(chips[i].number[k] ? 1 : 0, UO[j][k]), bnd);
                    }

                    // if this is a order word and a chip maps to it then the colors of each chip on this word must match
                    for (int k = 0; k < Chip.COLORCOUNT; k++)
                    {
                        var bnd = model.Sum(WC[j], model.Diff(1, Y[i][j]));
                        model.AddLe(model.Diff(UC[j][k], chips[i].color[k] ? 1 : 0), bnd);
                        model.AddLe(model.Diff(chips[i].color[k] ? 1 : 0, UC[j][k]), bnd);
                    }
                }

            // if this is a color word, ensure that at most one of each color is allowed
            for(int j = 0; j < word_count; j++)
                for(int k = 0; k < Chip.COLORCOUNT; k++)
                {
                    int[] colstack = new int[chip_count];
                    for (int i = 0; i < chip_count; i++)
                        colstack[i] = chips[i].color[k] ? 1 : 0;

                    model.Add(model.IfThen(model.Eq(WC[j], 1), model.Le(model.ScalProd(colstack, PC[j]), 1)));
                }

            // ensure that the numbers in an order word are sequential
            for (int j = 0; j < word_count; j++)
            {
                IIntVar[] V = model.BoolVarArray(Chip.NUMBERCOUNT);
                for (int k = 0; k < Chip.NUMBERCOUNT; k++)
                {
                    int[] numstack = new int[chip_count];
                    for (int i = 0; i < chip_count; i++)
                        numstack[i] = chips[i].number[k] ? 1 : 0;

                    // if this is an order word put the binary numbers into a vector of flags for those numbers
                    model.Add(model.IfThen(model.Eq(WO[j], 1), model.Eq(model.ScalProd(numstack, PO[j]), V[k])));
                }

                // for each number either the next flag is strictly larger, meaning the start of the sequence
                // or every flag after a decrease is 0, the end of a sequence
                for(int i = 0; i < Chip.NUMBERCOUNT - 1; i++)
                {
                    IIntVar Z = model.BoolVar();
                    model.AddLe(model.Diff(V[i], V[i + 1]), Z);
                    
                    for (int k = i + 1; k < Chip.NUMBERCOUNT; k++)
                    {
                        model.AddLe(V[k], model.Diff(1, Z));
                    }
                }
            }

            List<Chip[]> words = new List<Chip[]>();

            if (suppress)
                model.SetOut(null);

            Utility.Log("Thinking...");

            if (model.Solve())
            {
                for(int j = 0; j < word_count; j++)
                {
                    if (model.GetValue(WN[j]) > 0.5)
                        continue;

                    List<Chip> word = new List<Chip>();
                    double[] flags = model.GetValues(Yt[j]);
                    for (int i = 0; i < chip_count; i++)
                        if (flags[i] > 0.5)
                            word.Add(chips[i]);

                    // if this is a color word else it is an order word
                    if (model.GetValue(WC[j]) > 0.5)
                        words.Add(word.OrderBy(p => Array.IndexOf(p.color, true)).ToArray());
                    else
                        words.Add(word.OrderBy(p => Array.IndexOf(p.number, true)).ToArray());
                }
            }
            else
                Utility.Warning("No possible moves found!");

            model.Dispose();

            Chip[] notplayed = chips.Where(p => !words.Any(q => q.Contains(p))).ToArray();
            words.Add(notplayed);

            return words;
        }
    }
}
