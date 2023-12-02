using AdventOfCode.Framework;
using AdventOfCode.Utilities;

namespace AdventOfCode.Solutions.Y2023.Day02;

class Solution : ISolver
{
    public int Year => 2023;
    public int Day => 2;
    public string GetName() => "Cube Conundrum";

    public IEnumerable<object> Solve(string input, Func<TextWriter> getOutputFunction)
    {
        // var emptyOutput = () => new NullTextWriter();
        yield return PartOne(input, getOutputFunction);
        yield return PartTwo(input, getOutputFunction);
    }

    static object PartOne(string input, Func<TextWriter> getOutputFunction)
    {
        var games = input
            .Lines()
            .Select(l => l.Split(':').ToArray())
            .Select(a => (GameNumber: int.Parse(a[0].Substring(5)), Pulls: a[1].Split(';').Select(ParsePull)))
            .ToArray();

        var sum = games
            .Where(g => g.Pulls.All(p => p.Red <= 12 && p.Green <= 13 && p.Blue <= 14))
            .Select(g => g.GameNumber)
            .Sum();
        return sum;
    }

    private static Pull ParsePull(string s)
    {
        var a = s.Split(",", StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
        var pull = new Pull();
        foreach (var part in a.Select(a => a.Groups(@"(\d+) (.*)").ToArray()[0]))
        {
            var count = int.Parse(part[0].ToString());
            switch (part[1].ToString()) 
            {
                case "red" :
                    pull = pull with { Red = count };
                    break;
                case "green" :
                    pull = pull with { Green = count };
                    break;
                case "blue" :
                    pull = pull with { Blue = count };
                    break;
            }
        }

        return pull;
    }
    
    public record struct Pull(int Red, int Green, int Blue);

    static object PartTwo(string input, Func<TextWriter> getOutputFunction)
    {
        return 0;
    }
}