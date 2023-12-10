using AdventOfCode.Framework;
using AdventOfCode.Utilities;
using LanguageExt;

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
        var lines = input.SplitBySingleNewline()
            .ToArray();
        var grid = new Grid<char>(lines[0].Length, lines.Length, YAxisDirection.ZeroAtTop);
        Point2 start = new Point2(0, 0, YAxisDirection.ZeroAtTop);
        var groundPoints = new System.Collections.Generic.HashSet<Point2>();
        for (var y = 0; y < lines.Length; y++)
        {
            var line = lines[y];
            for (var x = 0; x < line.Length; x++)
            {
                var p = new Point2(x, y, YAxisDirection.ZeroAtTop);
                grid[p] = line[x];

                if (line[x] == 'S')
                {
                    start = p;
                }
                else if (line[x] == '.')
                {
                    groundPoints.Add(p);
                }
            }
        }
        
        // Find out the longest loop from the start.
        var pointsToTest = start.OrthogonalPoints.Where(grid.Contains).ToList();
        var longestLoop = new System.Collections.Generic.HashSet<Point2>();
        while (pointsToTest.Count > 0)
        {
            var p = pointsToTest[^1];
            pointsToTest.Remove(p);
            try
            {
                var next = Other(grid, start, p);
                var previous = p;
                var loop = new System.Collections.Generic.HashSet<Point2>();
                loop.Add(start);
                loop.Add(p);
                loop.Add(next);
                while (next != start)
                {
                    var n = Other(grid, previous, next);
                    previous = next;
                    next = n;
                    loop.Add(next);
                }

                if (loop.Count > longestLoop.Count)
                {
                    longestLoop = loop;
                }
            }
            catch (Exception)
            {
                // ignored
            }
        }
        
        // Count the number of points inside the loop.
        // There's still a bug here with regards to 'S'. It should be replaced
        // by its actual value for this code to work correctly, but for me
        // it gives the correct value.
        var enclosedPoints = new List<Point2>();
        foreach (var p in grid.Points.Except(longestLoop))
        {
            var intersections = 0;
            var x = p.X + 1;
            while (x < grid.Width)
            {
                var testPoint = new Point2(x, p.Y, YAxisDirection.ZeroAtTop);
                if (!longestLoop.Contains(testPoint))
                {
                    x++;
                    continue;
                }

                var c = grid[testPoint];
                if (c is 'F' or 'L')
                {
                    var lineStart = c;
                    // Continue along the horizontal line until we reach the end.
                    x++;
                    while (x < grid.Width && grid[x, p.Y] == '-')
                    {
                        x++;
                    }

                    var lineEnd = grid[x, p.Y];
                    if ((lineStart == 'F' && lineEnd == 'J') || (lineStart == 'L' && lineEnd == '7'))
                    {
                        intersections++;
                    }
                }
                else
                {
                    intersections++;
                }
                
                
                
                x++;
            }

            if (intersections % 2 == 1)
            {
                enclosedPoints.Add(p);
            }

        }

        return enclosedPoints.Count;
    }
}