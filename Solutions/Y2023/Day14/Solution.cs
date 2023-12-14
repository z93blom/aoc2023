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

        // var output = getOutputFunction();
        // PrintGrid(output, grid);
        
        // Count the load
        var load = GetLoad(grid);
        return load;
    }

    private static void PrintGrid(TextWriter output, Grid<Space> grid)
    {
        output.WriteLine(grid.ToString(string.Empty, c => c switch
        {
            Space.RoundRock => "O",
            Space.CubeRock => "#",
            Space.Empty => ".",
            _ => throw new NotImplementedException("Unknown space.")
        }));
    }

    private static long GetLoad(Grid<Space> grid)
    {
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

    private static void RollNorth(Grid<Space> grid)
    {
        for (var x = 0; x < grid.Width; x++)
        {
            var slice = grid.YSlice(x).OrderBy(p => p.Y);
            var firstEmpty = new Point2(x, 0, grid.YAxisDirection);
            foreach (var p in slice)
            {
                switch(grid[p])
                {
                    case Space.RoundRock:
                        if (p != firstEmpty)
                        {
                            grid[firstEmpty] = Space.RoundRock;
                            grid[p] = Space.Empty;
                        }
                        firstEmpty = firstEmpty.Below;
                        break;
                    case Space.CubeRock:
                        firstEmpty = p.Below;
                        break;
                    case Space.Empty:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }
    }
    private static void RollSouth(Grid<Space> grid)
    {
        for (var x = 0; x < grid.Width; x++)
        {
            var slice = grid.YSlice(x).OrderByDescending(p => p.Y);
            var firstEmpty = new Point2(x, grid.Height - 1, grid.YAxisDirection);
            foreach (var p in slice)
            {
                switch(grid[p])
                {
                    case Space.RoundRock:
                        if (p != firstEmpty)
                        {
                            grid[firstEmpty] = Space.RoundRock;
                            grid[p] = Space.Empty;
                        }
                        firstEmpty = firstEmpty.Above;
                        break;
                    case Space.CubeRock:
                        firstEmpty = p.Above;
                        break;
                    case Space.Empty:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }
    }
    
    private static void RollEast(Grid<Space> grid)
    {
        for (var y = 0; y < grid.Height; y++)
        {
            var slice = grid.XSlice(y).OrderByDescending(p => p.X);
            var firstEmpty = new Point2(grid.Width - 1, y, grid.YAxisDirection);
            foreach (var p in slice)
            {
                switch(grid[p])
                {
                    case Space.RoundRock:
                        if (p != firstEmpty)
                        {
                            grid[firstEmpty] = Space.RoundRock;
                            grid[p] = Space.Empty;
                        }
                        firstEmpty = firstEmpty.Left;
                        break;
                    case Space.CubeRock:
                        firstEmpty = p.Left;
                        break;
                    case Space.Empty:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }
    }
    private static void RollWest(Grid<Space> grid)
    {
        for (var y = 0; y < grid.Height; y++)
        {
            var slice = grid.XSlice(y).OrderBy(p => p.X);
            var firstEmpty = new Point2(0, y, grid.YAxisDirection);
            foreach (var p in slice)
            {
                switch(grid[p])
                {
                    case Space.RoundRock:
                        if (p != firstEmpty)
                        {
                            grid[firstEmpty] = Space.RoundRock;
                            grid[p] = Space.Empty;
                        }
                        firstEmpty = firstEmpty.Right;
                        break;
                    case Space.CubeRock:
                        firstEmpty = p.Right;
                        break;
                    case Space.Empty:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }
    }

    static object PartTwo(string input, Func<TextWriter> getOutputFunction)
    {
        var grid = input.ToGrid(YAxisDirection.ZeroAtTop, c => c switch
        {
            'O' => Space.RoundRock,
            '#' => Space.CubeRock,
            '.' => Space.Empty,
            _ => throw new Exception("Unknown space")
        });

        var output = getOutputFunction();
        var cycle = 1;
        while (cycle <= 1_000_000_000)
        {
            RollNorth(grid);
            RollWest(grid);
            RollSouth(grid);
            RollEast(grid);
            
            cycle++;

            if (cycle % 10_000 == 0)
            {
                output.WriteLine(cycle);
            }
        }
        PrintGrid(output, grid);
        
        var load = GetLoad(grid);
        return load;
    }
}