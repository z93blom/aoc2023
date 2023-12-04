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
        return 0;
    }
}