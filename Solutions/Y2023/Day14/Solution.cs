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
        var load = GetLoad(grid, grid.Points.Where(p => grid[p] == Space.RoundRock));
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

    private static long GetLoad(Grid<Space> grid, IEnumerable<Point2> rockPoints)
    {
        return rockPoints.Sum(p => grid.Height - p.Y);
    }

    private static IEnumerable<Point2> Roll(
        IReadOnlyDictionary<(long, long, Direction), Point2> stops, 
        IEnumerable<Point2> rollingRocks, 
        Direction direction, 
        Func<Point2, Point2> fallbackPosition)
    {
        var newPositions = new HashSet<Point2>();
        foreach (var rock in rollingRocks)
        {
            var p = stops[(rock.X, rock.Y, direction)];
            while (newPositions.Contains(p))
            {
                p = fallbackPosition(p);
            }
            newPositions.Add(p);
        }

        return newPositions;
    }

    private enum Direction
    {
        North,
        West,
        South,
        East
    }

    private readonly record struct PD(Point2 Point, Direction Direction);

    static object PartTwo(string input, Func<TextWriter> getOutputFunction)
    {
        var grid = input.ToGrid(YAxisDirection.ZeroAtTop, c => c switch
        {
            'O' => Space.RoundRock,
            '#' => Space.CubeRock,
            '.' => Space.Empty,
            _ => throw new Exception("Unknown space")
        });
        
        // Calculate the stop point for each non-cube rock, and each direction
        var stops = new Dictionary<(long, long, Direction), Point2>();
        foreach (var p in grid.Points.Where(p => grid[p] != Space.CubeRock))
        {
            stops[(p.X, p.Y, Direction.North)] = GetStop(grid, p, s => s.Above);
            stops[(p.X, p.Y, Direction.West)] = GetStop(grid, p, s => s.Left);
            stops[(p.X, p.Y, Direction.South)] = GetStop(grid, p, s => s.Below);
            stops[(p.X, p.Y, Direction.East)] = GetStop(grid, p, s => s.Right);
        }

        var rollingRocks = grid.Points.Where(p => grid[p] == Space.RoundRock);
        var cycle = 1;
        while (cycle <= 1_000_000_000)
        {
            rollingRocks = Roll(stops, rollingRocks, Direction.North, p => p.Below);
            rollingRocks = Roll(stops, rollingRocks, Direction.West, p => p.Right);
            rollingRocks = Roll(stops, rollingRocks, Direction.South, p => p.Above);
            rollingRocks = Roll(stops, rollingRocks, Direction.East, p => p.Left);
            cycle++;
        }

        var load = GetLoad(grid, rollingRocks);
        return load;
    }

    private static Point2 GetStop(Grid<Space> grid, Point2 p, Func<Point2, Point2> step)
    {
        var current = p;
        var next = step(current);
        while (grid.Contains(next) && grid[next] != Space.CubeRock)
        {
            current = next;
            next = step(current);
        }

        return current;
    }
}