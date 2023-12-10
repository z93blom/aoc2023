using AdventOfCode.Framework;
using AdventOfCode.Utilities;

namespace AdventOfCode.Solutions.Y2023.Day10;

class Solution : ISolver
{
    public int Year => 2023;
    public int Day => 10;
    public string GetName() => "Pipe Maze";

    public IEnumerable<object> Solve(string input, Func<TextWriter> getOutputFunction)
    {
        // var emptyOutput = () => new NullTextWriter();
        yield return PartOne(input, getOutputFunction);
        yield return PartTwo(input, getOutputFunction);
    }

    static object PartOne(string input, Func<TextWriter> getOutputFunction)
    {
        var lines = input.SplitBySingleNewline()
            .ToArray();
        var grid = new Grid<char>(lines[0].Length, lines.Length, YAxisDirection.ZeroAtTop);
        Point2 start = new Point2(0, 0, YAxisDirection.ZeroAtTop);
        for (var y = 0; y < lines.Length; y++)
        {
            var line = lines[y];
            for (var x = 0; x < line.Length; x++)
            {
                grid[x, y] = line[x];

                if (line[x] == 'S')
                {
                    start = new Point2(x, y, YAxisDirection.ZeroAtTop);
                }
            }
        }
        
        // Find out the longest loop from the start.
        var pointsToTest = start.OrthogonalPoints.Where(grid.Contains).ToList();
        var maxLength = int.MinValue;
        while (pointsToTest.Count > 0)
        {
            var p = pointsToTest[^1];
            pointsToTest.Remove(p);
            try
            {
                var next = Other(grid, start, p);
                var length = 1;
                var previous = p;
                while (next != start)
                {
                    var n = Other(grid, previous, next);
                    previous = next;
                    next = n;
                    length++;
                }

                if (length > maxLength)
                {
                    maxLength = length;
                }
            }
            catch (Exception)
            {
                // ignored
            }
        }
        
        return (maxLength + 1) / 2;
    }
    
    private static Point2 Other(Grid<char> grid, Point2 previous, Point2 current)
    {
        var (one, two) = PointsTo(grid[current], current);
        return one == previous ? two : one;
    }

    private static (Point2 one, Point2 two) PointsTo(char c, Point2 p)
    {
        return c switch
        {
            '|' => (p.Above, p.Below),
            '-' => (p.Left, p.Right),
            'L' => (p.Above, p.Right),
            'J' => (p.Above, p.Left),
            '7' => (p.Left, p.Below),
            'F' => (p.Right, p.Below),
            _ => throw new Exception($"Invalid direction: '{c}'")
        };
    }
    
    static object PartTwo(string input, Func<TextWriter> getOutputFunction)
    {
        return 0;
    }
}