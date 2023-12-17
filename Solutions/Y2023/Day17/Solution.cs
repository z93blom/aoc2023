using System.Drawing;
using AdventOfCode.Framework;
using AdventOfCode.Utilities;

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
        var stack = new Stack<Path>();
        var rightPath = new Path(start.Right, grid[start.Right], new[] { Dir.East }, new[]{start, start.Right});
        stack.Push(rightPath);
        var downPath = new Path(start.Below, grid[start.Below], new[] { Dir.South }, new[]{start, start.Below});
        stack.Push(downPath);
        List<Path> validPaths = new();

        Dictionary<(Point2, Dir, int), int> minimalCosts = new();
        while (stack.Count > 0)
        {
            var path = stack.Pop();

            if (path.P == goal)
            {
                validPaths.Add(path);
            }
            
            var key = (path.P, path.Steps[^1], path.Steps.Reverse().Count(d => d == path.Steps[^1]));
            if (minimalCosts.TryGetValue(key, out var value) && value < path.Cost)
            {
                continue;
            }

            minimalCosts[key] = path.Cost;

            foreach (var dir in Allowed(path))
            {
                var destination = Next(path.P, dir);
                if (path.Visited.Contains(destination) || !grid.Contains(destination))
                {
                    continue;
                }

                var newPath = new Path(
                    destination,
                    path.Cost + grid[destination],
                    path.Steps.Concat(new[] { dir }).ToArray(),
                    path.Visited.Concat(new[] { destination }).ToArray());
                stack.Push(newPath);
            }
        }

        var bestPath = validPaths.MinBy(p => p.Cost);
        return bestPath.Cost;
    }

    static object PartTwo(string input, Func<TextWriter> getOutputFunction)
    {
        return 0;
    }
}