using AdventOfCode.Framework;
using AdventOfCode.Utilities;

namespace AdventOfCode.Solutions.Y2023.Day04;

class Solution : ISolver
{
    public int Year => 2023;
    public int Day => 4;
    public string GetName() => "Scratchcards";

    public IEnumerable<object> Solve(string input, Func<TextWriter> getOutputFunction)
    {
        // var emptyOutput = () => new NullTextWriter();
        yield return PartOne(input, getOutputFunction);
        yield return PartTwo(input, getOutputFunction);
    }

    static object PartOne(string input, Func<TextWriter> getOutputFunction)
    {
        var numbers = input.Lines()
            .Select(l => l.Integers().Skip(1))
            .ToArray();

        var sum = numbers
            .Select(GetPoints)
            .Sum();
        
        return sum;
    }

    private static double GetPoints(IEnumerable<int> card)
    {
        // Assume that the only numbers that appear twice are both in the first and the second section.  
        var winningPoints = card.GroupBy(i => i)
            .Where(g => g.Count() == 2)
            .ToArray();
        if (winningPoints.Length == 0)
        {
            return 0;
        }
        
        return Math.Pow(2, winningPoints.Length - 1);
    }

    static object PartTwo(string input, Func<TextWriter> getOutputFunction)
    {
        var cards = input.Lines()
            .Select(l => new Card(l.Integers().ToArray()))
            .ToArray();

        // We get one card of each to start with.
        var numberOfCards =
            new Dictionary<int, long>(cards.Select(c => new KeyValuePair<int, long>(c.CardNumber, 1)));

        foreach (var card in cards)
        {
            var cardNumber = card.CardNumber;
            var n = numberOfCards[cardNumber];
            for (var i = 0; i < card.Matches; i++)
            {
                numberOfCards[cardNumber + i + 1] += n;
            }
        }

        var total = numberOfCards.Sum(kvp => kvp.Value);
        
        return total;
    }

    public record struct Card
    {
        public int CardNumber { get; }
        public int Matches { get; }

        public Card(IReadOnlyList<int> numbers)
        {
            CardNumber = numbers[0];
            Matches = numbers
                .Skip(1)    
                .GroupBy(i => i)
                .Count(g => g.Count() == 2);
        }
    }
}