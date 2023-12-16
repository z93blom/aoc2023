using AdventOfCode.Framework;
using AdventOfCode.Utilities;
using LanguageExt.ClassInstances;

namespace AdventOfCode.Solutions.Y2023.Day16;

class Solution : ISolver
{
    public int Year => 2023;
    public int Day => 16;
    public string GetName() => "The Floor Will Be Lava";

    public IEnumerable<object> Solve(string input, Func<TextWriter> getOutputFunction)
    {
        // var emptyOutput = () => new NullTextWriter();
        yield return PartOne(input, getOutputFunction);
        yield return PartTwo(input, getOutputFunction);
    }

    private enum Space
    {
        Empty,
        MirrorNw,
        MirrorNe,
        SplitterNs,
        SplitterEw,
    }

    private enum Dir
    {
        North,
        East,
        South,
        West
    }

    private readonly record struct LocationAndDir
    {
        public long X { get; }
        public long Y { get; }
        public Dir Dir { get; }

        public LocationAndDir(Point2 p, Dir dir)
        {
            Dir = dir;
            X = p.X;
            Y = p.Y;
        }

        public Point2 AsPoint()
        {
            return new Point2(X, Y, YAxisDirection.ZeroAtTop);
        }
    }

    static object PartOne(string input, Func<TextWriter> getOutputFunction)
    {
        var grid = input.ToGrid(YAxisDirection.ZeroAtTop, c => c switch
        {
            '.' => Space.Empty,
            '/' => Space.MirrorNw,
            '\\' => Space.MirrorNe,
            '|' => Space.SplitterNs,
            '-' => Space.SplitterEw,
            _ => throw new ArgumentException("Unknown grid space.")
        });


        var queue = new Queue<LocationAndDir>();
        queue.Enqueue(new LocationAndDir(new Point2(0, 0, YAxisDirection.ZeroAtTop), Dir.East));

        var memory = new HashSet<LocationAndDir>();
        while (queue.Count > 0)
        {
            var pd = queue.Dequeue();
            var p = pd.AsPoint();
            if (!grid.Contains(p))
            {
                continue;
            }
            
            LocationAndDir target1;
            var target2 = new LocationAndDir(new Point2(-1, -1, YAxisDirection.ZeroAtTop), Dir.West);
            var hasTwoTargets = false;
            switch (grid[p])
            {
                case Space.Empty:
                    // continue along the same direction
                    target1 = pd.Dir switch
                    {
                        Dir.North => new LocationAndDir(p.Above, pd.Dir),
                        Dir.East => new LocationAndDir(p.Right, pd.Dir),
                        Dir.South => new LocationAndDir(p.Below, pd.Dir),
                        Dir.West => new LocationAndDir(p.Left, pd.Dir),
                        _ => throw new ArgumentOutOfRangeException()
                    };
                    break;
                case Space.MirrorNw:
                    target1 = pd.Dir switch
                    {
                        Dir.North => new LocationAndDir(p.Right, Dir.East),
                        Dir.East => new LocationAndDir(p.Above, Dir.North),
                        Dir.South => new LocationAndDir(p.Left, Dir.West),
                        Dir.West => new LocationAndDir(p.Below, Dir.South),
                        _ => throw new ArgumentOutOfRangeException()
                    };
                    break;
                case Space.MirrorNe:
                    target1 = pd.Dir switch
                    {
                        Dir.North => new LocationAndDir(p.Left, Dir.West),
                        Dir.East => new LocationAndDir(p.Below, Dir.South),
                        Dir.South => new LocationAndDir(p.Right, Dir.East),
                        Dir.West => new LocationAndDir(p.Above, Dir.North),
                        _ => throw new ArgumentOutOfRangeException()
                    };
                    break;
                case Space.SplitterNs:
                    if (pd.Dir == Dir.East || pd.Dir == Dir.West)
                    {
                        hasTwoTargets = true;
                        target1 = new LocationAndDir(p.Above, Dir.North);
                        target2 = new LocationAndDir(p.Below, Dir.South);
                    }
                    else
                    {
                        target1 = pd.Dir switch
                        {
                            Dir.North => new LocationAndDir(p.Above, pd.Dir),
                            Dir.South => new LocationAndDir(p.Below, pd.Dir),
                            _ => throw new ArgumentOutOfRangeException()
                        };
                    }
                    break;
                case Space.SplitterEw:
                    if (pd.Dir == Dir.North || pd.Dir == Dir.South)
                    {
                        hasTwoTargets = true;
                        target1 = new LocationAndDir(p.Right, Dir.East);
                        target2 = new LocationAndDir(p.Left, Dir.West);
                    }
                    else
                    {
                        target1 = pd.Dir switch
                        {
                            Dir.East => new LocationAndDir(p.Right, pd.Dir),
                            Dir.West => new LocationAndDir(p.Left, pd.Dir),
                            _ => throw new ArgumentOutOfRangeException()
                        };
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            if (!memory.Contains(target1) && grid.Contains(target1.AsPoint()))
            {
                queue.Enqueue(target1);
            }

            if (hasTwoTargets && !memory.Contains(target2) && grid.Contains(target2.AsPoint()))
            {
                queue.Enqueue(target2);
            }

            memory.Add(pd);
        }

        var locationsWithinGrid = memory
            .Select(pd => pd.AsPoint())
            .Distinct()
            .Where(p => grid.Contains(p))
            .ToArray();

        return locationsWithinGrid.Length;
    }

    static object PartTwo(string input, Func<TextWriter> getOutputFunction)
    {
        
        var grid = input.ToGrid(YAxisDirection.ZeroAtTop, c => c switch
        {
            '.' => Space.Empty,
            '/' => Space.MirrorNw,
            '\\' => Space.MirrorNe,
            '|' => Space.SplitterNs,
            '-' => Space.SplitterEw,
            _ => throw new ArgumentException("Unknown grid space.")
        });

        var e = new List<int>();
        // Along the top
        for (var x = 0; x < grid.Width; x++)
        {
            e.Add(GetEnergizedTiles(grid, new LocationAndDir(new Point2(x, 0, YAxisDirection.ZeroAtTop), Dir.South)));
        }

        // Along the bottom
        for (var x = 0; x < grid.Width; x++)
        {
            e.Add(GetEnergizedTiles(grid, new LocationAndDir(new Point2(x, grid.Height - 1, YAxisDirection.ZeroAtTop), Dir.South)));
        }
        
        // Along the left
        for (var y = 0; y < grid.Width; y++)
        {
            e.Add(GetEnergizedTiles(grid, new LocationAndDir(new Point2(0, y, YAxisDirection.ZeroAtTop), Dir.East)));
        }

        // Along the right
        for (var y = 0; y < grid.Width; y++)
        {
            e.Add(GetEnergizedTiles(grid, new LocationAndDir(new Point2(grid.Width - 1, y, YAxisDirection.ZeroAtTop), Dir.East)));
        }

        return e.Max();
    }

    private static int GetEnergizedTiles(Grid<Space> grid, LocationAndDir start)
    {
        var queue = new Queue<LocationAndDir>();
        queue.Enqueue(start);

        var memory = new HashSet<LocationAndDir>();
        while (queue.Count > 0)
        {
            var pd = queue.Dequeue();
            var p = pd.AsPoint();
            if (!grid.Contains(p))
            {
                continue;
            }
            
            LocationAndDir target1;
            var target2 = new LocationAndDir(new Point2(-1, -1, YAxisDirection.ZeroAtTop), Dir.West);
            var hasTwoTargets = false;
            switch (grid[p])
            {
                case Space.Empty:
                    // continue along the same direction
                    target1 = pd.Dir switch
                    {
                        Dir.North => new LocationAndDir(p.Above, pd.Dir),
                        Dir.East => new LocationAndDir(p.Right, pd.Dir),
                        Dir.South => new LocationAndDir(p.Below, pd.Dir),
                        Dir.West => new LocationAndDir(p.Left, pd.Dir),
                        _ => throw new ArgumentOutOfRangeException()
                    };
                    break;
                case Space.MirrorNw:
                    target1 = pd.Dir switch
                    {
                        Dir.North => new LocationAndDir(p.Right, Dir.East),
                        Dir.East => new LocationAndDir(p.Above, Dir.North),
                        Dir.South => new LocationAndDir(p.Left, Dir.West),
                        Dir.West => new LocationAndDir(p.Below, Dir.South),
                        _ => throw new ArgumentOutOfRangeException()
                    };
                    break;
                case Space.MirrorNe:
                    target1 = pd.Dir switch
                    {
                        Dir.North => new LocationAndDir(p.Left, Dir.West),
                        Dir.East => new LocationAndDir(p.Below, Dir.South),
                        Dir.South => new LocationAndDir(p.Right, Dir.East),
                        Dir.West => new LocationAndDir(p.Above, Dir.North),
                        _ => throw new ArgumentOutOfRangeException()
                    };
                    break;
                case Space.SplitterNs:
                    if (pd.Dir == Dir.East || pd.Dir == Dir.West)
                    {
                        hasTwoTargets = true;
                        target1 = new LocationAndDir(p.Above, Dir.North);
                        target2 = new LocationAndDir(p.Below, Dir.South);
                    }
                    else
                    {
                        target1 = pd.Dir switch
                        {
                            Dir.North => new LocationAndDir(p.Above, pd.Dir),
                            Dir.South => new LocationAndDir(p.Below, pd.Dir),
                            _ => throw new ArgumentOutOfRangeException()
                        };
                    }
                    break;
                case Space.SplitterEw:
                    if (pd.Dir == Dir.North || pd.Dir == Dir.South)
                    {
                        hasTwoTargets = true;
                        target1 = new LocationAndDir(p.Right, Dir.East);
                        target2 = new LocationAndDir(p.Left, Dir.West);
                    }
                    else
                    {
                        target1 = pd.Dir switch
                        {
                            Dir.East => new LocationAndDir(p.Right, pd.Dir),
                            Dir.West => new LocationAndDir(p.Left, pd.Dir),
                            _ => throw new ArgumentOutOfRangeException()
                        };
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            if (!memory.Contains(target1) && grid.Contains(target1.AsPoint()))
            {
                queue.Enqueue(target1);
            }

            if (hasTwoTargets && !memory.Contains(target2) && grid.Contains(target2.AsPoint()))
            {
                queue.Enqueue(target2);
            }

            memory.Add(pd);
        }

        var locationsWithinGrid = memory
            .Select(pd => pd.AsPoint())
            .Distinct()
            .Where(p => grid.Contains(p))
            .ToArray();

        return locationsWithinGrid.Length;
    }
}