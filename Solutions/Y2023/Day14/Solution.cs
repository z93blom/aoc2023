using System.ComponentModel;
using AdventOfCode.Framework;
using AdventOfCode.Utilities;

namespace AdventOfCode.Solutions.Y2023.Day14;

class Solution : ISolver
{
    public int Year => 2023;
    public int Day => 14;
    public string GetName() => "Parabolic Reflector Dish";

    public IEnumerable<object> Solve(string input, Func<TextWriter> getOutputFunction)
    {
        // var emptyOutput = () => new NullTextWriter();
        yield return PartOne(input, getOutputFunction);
        yield return PartTwo(input, getOutputFunction);
    }

    private enum Space
    {
        [Description("O")]
        RoundRock,

        [Description("#")]
        CubeRock,
        
        [Description(".")]
        Empty
    }

    static object PartOne(string input, Func<TextWriter> getOutputFunction)
    {

        var grid = input.ToGrid(YAxisDirection.ZeroAtTop, c => c switch
        {
            'O' => Space.RoundRock,
            '#' => Space.CubeRock,
            '.' => Space.Empty,
            _ => throw new Exception("Unknown space")
        });
        
        // Make all the rounded rocks roll as far north as they can.
        for (var x = 0; x < grid.Width; x++)
        {
            var y = 0;
            while (y < grid.Height)
            {
                switch (grid[x, y])
                {
                    case Space.Empty:
                    {
                        // Move all the rocks below this up to this point.
                        var yb = y;
                        var rocks = 0;
                        while (yb < grid.Height && grid[x, yb] != Space.CubeRock)
                        {
                            if (grid[x, yb] == Space.RoundRock)
                            {
                                rocks++;
                            }

                            yb++;
                        }

                        for (var yr = y; yr < y + rocks; yr++)
                        {
                            grid[x, yr] = Space.RoundRock;
                        }

                        for (var yr = rocks + y; yr < yb; yr++)
                        {
                            grid[x, yr] = Space.Empty;
                        }

                        y = yb;
                        break;
                    }

                    case Space.CubeRock:
                    case Space.RoundRock:
                        y++;
                        break;
                    default:
                        throw new NotImplementedException("Unknown space.");
                }
            }
        }

        getOutputFunction().WriteLine(grid.ToString(string.Empty, c => c switch
        {
            Space.RoundRock => "O",
            Space.CubeRock => "#",
            Space.Empty => ".",
            _ => throw new NotImplementedException("Unknown space.")
        }));
        
        // Count the load
        var load = 0L;
        foreach (var p in grid.Points)
        {
            if (grid[p] == Space.RoundRock)
            {
                load += grid.Height - p.Y;
            }
        }

        return load;
    }

    static object PartTwo(string input, Func<TextWriter> getOutputFunction)
    {
        return 0;
    }
}