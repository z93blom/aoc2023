using System.Drawing;
using AdventOfCode.Framework;
using AdventOfCode.Utilities;
using SuperLinq;

namespace AdventOfCode.Solutions.Y2023.Day17;

class Solution : ISolver
{
    public int Year => 2023;
    public int Day => 17;
    public string GetName() => "Clumsy Crucible";

    public IEnumerable<object> Solve(string input, Func<TextWriter> getOutputFunction)
    {
        // var emptyOutput = () => new NullTextWriter();
        yield return PartOne(input, getOutputFunction);
        yield return PartTwo(input, getOutputFunction);
    }

    private enum Dir
    {
        North,
        East,
        South,
        West,
    }

    private readonly record struct Path(Point2 P, int Cost, Dir[] Steps, Point2[] Visited);

    private static IEnumerable<Dir> Allowed(Path path)
    {
        switch (path.Steps[^1])
        {
            case Dir.North:
            case Dir.South:
                yield return Dir.East;
                yield return Dir.West;
                break;
            case Dir.East:
            case Dir.West:
                yield return Dir.North;
                yield return Dir.South;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        if (!LastThreeAreSame(path))
        {
            yield return path.Steps[^1];
        }

    }

    private static bool LastThreeAreSame(Path path)
    {
        var lastThreeAreSame = path.Steps.Length >= 3 && 
                               path.Steps[^3..].All(d => path.Steps[^1] == d);
        return lastThreeAreSame;
    }

    private static Point2 Next(Point2 p, Dir dir)
    {
        return dir switch
        {
            Dir.North => p.Above,
            Dir.East => p.Right,
            Dir.South => p.Below,
            Dir.West => p.Left,
            _ => throw new ArgumentOutOfRangeException(nameof(dir), dir, null)
        };
    }

    static object PartOne(string input, Func<TextWriter> getOutputFunction)
    {
        var grid = input.ToGrid(YAxisDirection.ZeroAtTop, c => c - '0');
        var start = new Point2(0, 0, YAxisDirection.ZeroAtTop);
        var goal = new Point2(grid.Width - 1, grid.Height - 1, YAxisDirection.ZeroAtTop);

        IEnumerable<((Point2 Point2, int length, Dir dir), int)> GetNeighbors(
            (Point2 P, int Length, Dir Dir) state, int cost)
        {
            foreach (var dir in Enum.GetValues<Dir>())
            {
                if (state.Length == 3 && state.Dir == dir)
                {
                    // Can't continue for more than three steps in the same direction.
                    continue;
                }

                if (state.Dir != dir)
                {
                    if (state.Dir switch
                        {
                            Dir.East => dir is Dir.West,
                            Dir.West => dir is Dir.East,
                            Dir.North => dir is Dir.South,
                            Dir.South => dir is Dir.North,
                            _ => throw new ArgumentOutOfRangeException()
                        })
                    {
                        // Can't turn around.
                        continue;
                    }
                }

                var destination = dir switch
                {
                    Dir.North => state.P.Above,
                    Dir.East => state.P.Right,
                    Dir.South => state.P.Below,
                    Dir.West => state.P.Left,
                    _ => throw new ArgumentOutOfRangeException()
                };

                if (!grid.Contains(destination))
                {
                    continue;
                }

                yield return ((
                        destination,
                        dir == state.Dir ? state.Length + 1 : 1,
                        dir),
                    cost + grid[destination]);
            }
        }

        var minimalCost = SuperEnumerable.GetShortestPathCost<(Point2 P, int Length, Dir Dir), int>(
            (start, 0, Dir.East),
            GetNeighbors,
            st => st.P == goal);

        return minimalCost;
    }

    static object PartTwo(string input, Func<TextWriter> getOutputFunction)
    {
        var grid = input.ToGrid(YAxisDirection.ZeroAtTop, c => c - '0');
        var start = new Point2(0, 0, YAxisDirection.ZeroAtTop);
        var goal = new Point2(grid.Width - 1, grid.Height - 1, YAxisDirection.ZeroAtTop);

        IEnumerable<((Point2 Point2, int length, Dir dir), int)> GetNeighbors(
            (Point2 P, int Length, Dir Dir) state, int cost)
        {
            foreach (var dir in Enum.GetValues<Dir>())
            {
                if (state.Length == 10 && state.Dir == dir)
                {
                    continue;
                }

                if (state.Length < 4 && state.Dir != dir)
                {
                    continue;
                }

                if (state.Dir != dir)
                {
                    if (state.Dir switch
                        {
                            Dir.East => dir is Dir.West,
                            Dir.West => dir is Dir.East,
                            Dir.North => dir is Dir.South,
                            Dir.South => dir is Dir.North,
                            _ => throw new ArgumentOutOfRangeException()
                        })
                    {
                        // Can't turn around.
                        continue;
                    }
                }

                var destination = dir switch
                {
                    Dir.North => state.P.Above,
                    Dir.East => state.P.Right,
                    Dir.South => state.P.Below,
                    Dir.West => state.P.Left,
                    _ => throw new ArgumentOutOfRangeException()
                };

                if (!grid.Contains(destination))
                {
                    continue;
                }
                
                yield return ((
                        destination,
                        dir == state.Dir ? state.Length + 1 : 1,
                        dir),
                    cost + grid[destination]);
            }
        }

        var minimalCost = SuperEnumerable.GetShortestPathCost<(Point2 P, int Length, Dir Dir), int>(
            (start, 0, Dir.East),
            GetNeighbors,
            st => st.P == goal && st.Length >= 4);

        return minimalCost;    
    }
}