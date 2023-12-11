using AdventOfCode.Framework;
using AdventOfCode.Utilities;
using LanguageExt.Pretty;
using Spectre.Console;

namespace AdventOfCode.Solutions.Y2023.Day11;

class Solution : ISolver
{
    public int Year => 2023;
    public int Day => 11;
    public string GetName() => "Cosmic Expansion";

    public IEnumerable<object> Solve(string input, Func<TextWriter> getOutputFunction)
    {
        // var emptyOutput = () => new NullTextWriter();
        yield return PartOne(input, getOutputFunction);
        yield return PartTwo(input, getOutputFunction);
    }

    static object PartOne(string input, Func<TextWriter> getOutputFunction)
    {
        var lines = input.Lines().ToArray();
        var grid = new Grid<char>(lines[0].Length, lines.Length, YAxisDirection.ZeroAtTop);
        for (var y = 0; y < grid.Height; y++)
        {
            for (var x = 0; x < grid.Width; x++)
            {
                var p = new Point2(x, y, YAxisDirection.ZeroAtTop);
                grid[p] = lines[y][x];
            }
        }
        
        // Expand the universe
        var xColumnsToExpand = Enumerable.Range(0, (int)grid.Width)
            .Select(x => (x: x, slice: grid.YSlice(x)))
            .Where(t => t.slice.All(p => grid[p] == '.'))
            .Select(t => t.x)
            .ToArray();
        var yRowsToExpand = Enumerable.Range(0, (int)grid.Height)
            .Select(y => (y: y, slice: grid.XSlice(y)))
            .Where(t => t.slice.All(p => grid[p] == '.'))
            .Select(t => t.y)
            .ToArray();

        var expandedGrid = new Grid<char>(
            grid.Width + xColumnsToExpand.Length, 
            grid.Height + yRowsToExpand.Length,
            YAxisDirection.ZeroAtTop);

        var expandedLines =
            lines.Select(line => xColumnsToExpand.Reverse()
                .Aggregate(line, (current, x) => current.Insert(x, ".")))
            .ToList();
        foreach (var y in yRowsToExpand.Reverse())
        {
            expandedLines.Insert(y, new string('.', (int)expandedGrid.Width));
        }


        var galaxyLocations = new List<Point2>();
        for (var y = 0; y < expandedGrid.Height; y++)
        {
            for (var x = 0; x < expandedGrid.Width; x++)
            {
                var p = new Point2(x, y, YAxisDirection.ZeroAtTop);
                expandedGrid[p] = expandedLines[y][x];

                if (expandedGrid[p] == '#')
                {
                    galaxyLocations.Add(p);
                }
            }
        }

        var sum = 0L;
        for (var i = 0; i < galaxyLocations.Count; i++)
        {
            var p = galaxyLocations[i];
            for (var i2 = i + 1; i2 < galaxyLocations.Count; i2++)
            {
                var p2 = galaxyLocations[i2];
                var distance = p.ManhattanDistance(p2);
                sum += distance;
            }
            
        }
        
        return sum;
    }

    static object PartTwo(string input, Func<TextWriter> getOutputFunction)
    {
        var lines = input.Lines().ToArray();
        var grid = new Grid<char>(lines[0].Length, lines.Length, YAxisDirection.ZeroAtTop);
        var galaxyLocations = new List<Point2>();
        for (var y = 0; y < grid.Height; y++)
        {
            for (var x = 0; x < grid.Width; x++)
            {
                var p = new Point2(x, y, YAxisDirection.ZeroAtTop);
                grid[p] = lines[y][x];

                if (grid[p] == '#')
                {
                    galaxyLocations.Add(p);
                }
            }
        }
        
        // Expand the universe
        var xColumnsToExpand = Enumerable.Range(0, (int)grid.Width)
            .Select(x => (x: x, slice: grid.YSlice(x)))
            .Where(t => t.slice.All(p => grid[p] == '.'))
            .Select(t => t.x)
            .ToArray();
        var yRowsToExpand = Enumerable.Range(0, (int)grid.Height)
            .Select(y => (y: y, slice: grid.XSlice(y)))
            .Where(t => t.slice.All(p => grid[p] == '.'))
            .Select(t => t.y)
            .ToArray();

        var expandedLocations = new List<Point2>();
        foreach (var p in galaxyLocations)
        {
            var x = xColumnsToExpand.Count(x => x < p.X) * (1_000_000 - 1) + p.X;
            var y = yRowsToExpand.Count(y => y < p.Y) * (1_000_000 - 1) + p.Y;
            expandedLocations.Add(new Point2(x, y, YAxisDirection.ZeroAtTop));
        }
        
        var sum = 0L;
        for (var i = 0; i < expandedLocations.Count; i++)
        {
            var p = expandedLocations[i];
            for (var i2 = i + 1; i2 < expandedLocations.Count; i2++)
            {
                var p2 = expandedLocations[i2];
                var distance = p.ManhattanDistance(p2);
                sum += distance;
            }
            
        }
        
        return sum;
    }
}