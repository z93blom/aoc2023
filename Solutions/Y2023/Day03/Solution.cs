using AdventOfCode.Framework;
using AdventOfCode.Utilities;
using Spectre.Console;

namespace AdventOfCode.Solutions.Y2023.Day03;

class Solution : ISolver
{
    public int Year => 2023;
    public int Day => 3;
    public string GetName() => "Gear Ratios";

    public IEnumerable<object> Solve(string input, Func<TextWriter> getOutputFunction)
    {
        // var emptyOutput = () => new NullTextWriter();
        yield return PartOne(input, getOutputFunction);
        yield return PartTwo(input, getOutputFunction);
    }

    static object PartOne(string input, Func<TextWriter> getOutputFunction)
    {
        var lines = input
            .Lines()
            .ToArray();

        var grid = new Grid<char>(lines[0].Length, lines.Length, YAxisDirection.ZeroAtTop);
        var numberPoints = new HashSet<Point2>();
        var symbolPoints = new HashSet<Point2>();
        for (var y = 0; y < lines.Length; y++)
        {
            var line = lines[y];
            for (var x = 0; x < line.Length; x++)
            {
                var c = line[x];
                var p = new Point2(x, y, YAxisDirection.ZeroAtTop);
                grid[p] = c;

                if (char.IsDigit(c))
                {
                    numberPoints.Add(p);
                }
                else if (c != '.')
                {
                    symbolPoints.Add(p);
                }
            }
        }

        var numberPointsAdjacentToSymbols = new HashSet<Point2>();
        foreach (var symbolPoint in symbolPoints)
        {
            foreach (var p in symbolPoint.AdjacentPoints)
            {
                if (numberPoints.Contains(p))
                {
                    numberPointsAdjacentToSymbols.Add(p);
                }
            }
        }

        var sum = 0;
        var pointsCovered = new HashSet<Point2>();
        foreach (var p in numberPointsAdjacentToSymbols)
        {
            if (pointsCovered.Contains(p))
                // The number has already been covered.
                continue;
            
            var c = p;
            while(grid.Contains(c.Right) && char.IsDigit(grid[c.Right]))
                c = c.Right;

            var value = grid[c] - '0';
            var factor = 1;
            pointsCovered.Add(c);
            while (grid.Contains(c.Left) && char.IsDigit(grid[c.Left]))
            {
                c = c.Left;
                pointsCovered.Add(c);
                factor *= 10;
                value += factor * (grid[c] - '0');
            }

            sum += value;
        }

        return sum;
    }

    static object PartTwo(string input, Func<TextWriter> getOutputFunction)
    {
        return 0;
    }
}