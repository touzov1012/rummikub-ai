# Rummikub AI

In Rummikub, we have colored chips with values 1-13 and 4 colors. Given a collection of chips, our goal is to find the maximum number of chips we can lay down.

Chips are layed down in *words*. A word must be at least 3 chips long and be one of two types:

* A color word: contains chips of unique colors and no duplicate numbers.
* An order word: contains chips in natural counting order, all of the same color.

This is a simple console application which can determine the maximal number of chips we can lay down for any given hand.

## Usage and Example

We start by generating a deck and drawing 30 random chips

```cs
List<Chip> deck = Host.GenerateDeck();
List<Chip> chips = Host.DrawRandom(deck, 30);
```

To find the words which we can lay down, we simply pass the hand of chips to the solver with an optional parameter to indicate how many chips are already layed down.

```cs
List<Chip[]> words = Solver.Solve(chips, 0);
```

The solver returns a list of words made from the chips, as well as the left over chips in the last word.
