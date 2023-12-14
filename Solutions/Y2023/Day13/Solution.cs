using AdventOfCode.Framework;
using AdventOfCode.Utilities;

namespace AdventOfCode.Solutions.Y2023.Day13;

class Solution : ISolver
{
    public int Year => 2023;
    public int Day => 13;
    public string GetName() => "Point of Incidence";

    public IEnumerable<object> Solve(string input, Func<TextWriter> getOutputFunction)
    {
        // var emptyOutput = () => new NullTextWriter();
        yield return PartOne(input, getOutputFunction);
        yield return PartTwo(input, getOutputFunction);
    }

    static object PartOne(string input, Func<TextWriter> getOutputFunction)
    {
        var grids = input
            .SplitByDoubleNewline()
            .Select(gridInput => gridInput.ToGrid(YAxisDirection.ZeroAtTop, c => c switch
            {
                '.' => Feature.Ash,
                '#' => Feature.Mountain,
                _ => throw new Exception("Unknown map feature.")
            }))
            .ToArray();

        var verticals = new List<int>();
        var horizontals = new List<int>();
        foreach (var grid in grids)
        {
            var found = false;
            // Try the vertical slices
            for (var x = 0; x < grid.Width - 1; x++)
            {
                var slice = grid.YSlice(x);
                if (IsPerfectReflection(grid, slice, p => p.Left, p => p.Right))
                {
                    verticals.Add(x + 1);
                    found = true;
                    break;
                }
            }

            if (found)
            {
                continue;
            }
            
            
            // Try the horizontal slices.
            for (var y = 0; y < grid.Height - 1; y++)
            {
                var slice = grid.XSlice(y);
                if (IsPerfectReflection(grid, slice, p => p.Above, p => p.Below))
                {
                    horizontals.Add(y + 1);
                    break;
                }
            }
        }
        
        return verticals.Sum() + horizontals.Sum() * 100;
    }

    private static bool IsPerfectReflection(
        Grid<Feature> grid,
        IEnumerable<Point2> startingPoints, 
        Func<Point2, Point2> pointTranslation,
        Func<Point2, Point2> compareTranslation)
    {
        var current = startingPoints.ToDictionary(p => p, p=> p);
        var compare = current.ToDictionary(p => p.Key, p => compareTranslation(p.Value));

        while (grid.Contains(current.Values.First()) && grid.Contains(compare.Values.First()))
        {
            if (current.Any(p => grid[p.Value] != grid[compare[p.Key]]))
            {
                return false;
            }

            current = current.ToDictionary(kvp => kvp.Key, kvp => pointTranslation(kvp.Value));
            compare = compare.ToDictionary(kvp => kvp.Key, kvp => compareTranslation(kvp.Value));
        }

        return true;
    }

    enum Feature
    {
        Ash,
        Mountain
    }

    static object PartTwo(string input, Func<TextWriter> getOutputFunction)
    {
        var grids = input
            .SplitByDoubleNewline()
            .Select(gridInput => gridInput.ToGrid(YAxisDirection.ZeroAtTop, c => c switch
            {
                '.' => Feature.Ash,
                '#' => Feature.Mountain,
                _ => throw new Exception("Unknown map feature.")
            }))
            .ToArray();

        var verticals = new List<int>();
        var horizontals = new List<int>();
        foreach (var grid in grids)
        {
            var found = false;
            // Try the vertical slices
            for (var x = 0; x < grid.Width - 1; x++)
            {
                var slice = grid.YSlice(x);
                if (IsAlmostPerfectReflection(grid, slice, p => p.Left, p => p.Right))
                {
                    verticals.Add(x + 1);
                    found = true;
                    break;
                }
            }

            if (found)
            {
                continue;
            }
            
            
            // Try the horizontal slices.
            for (var y = 0; y < grid.Height - 1; y++)
            {
                var slice = grid.XSlice(y);
                if (IsAlmostPerfectReflection(grid, slice, p => p.Above, p => p.Below))
                {
                    horizontals.Add(y + 1);
                    break;
                }
            }
        }
        
        return verticals.Sum() + horizontals.Sum() * 100;
    }
    
    private static bool IsAlmostPerfectReflection(
        Grid<Feature> grid,
        IEnumerable<Point2> startingPoints, 
        Func<Point2, Point2> pointTranslation,
        Func<Point2, Point2> compareTranslation)
    {
        var current = startingPoints.ToDictionary(p => p, p=> p);
        var compare = current.ToDictionary(p => p.Key, p => compareTranslation(p.Value));
        var changed = false;
        while (grid.Contains(current.Values.First()) && grid.Contains(compare.Values.First()))
        {
            var count = current.Count(p => grid[p.Value] != grid[compare[p.Key]]);
            if (count != 0)
            {
                if (!changed && count == 1)
                {
                    changed = true;
                }
                else
                {
                    return false;
                }
            }

            current = current.ToDictionary(kvp => kvp.Key, kvp => pointTranslation(kvp.Value));
            compare = compare.ToDictionary(kvp => kvp.Key, kvp => compareTranslation(kvp.Value));
        }

        // The reflection has to have a changed point.
        return changed;
    }
}