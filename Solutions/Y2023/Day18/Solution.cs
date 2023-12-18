using System.Drawing;
using System.Globalization;
using AdventOfCode.Framework;
using AdventOfCode.Utilities;
using Color = Spectre.Console.Color;

namespace AdventOfCode.Solutions.Y2023.Day18;

class Solution : ISolver
{
    public int Year => 2023;
    public int Day => 18;
    public string GetName() => "Lavaduct Lagoon";

    public IEnumerable<object> Solve(string input, Func<TextWriter> getOutputFunction)
    {
        // var emptyOutput = () => new NullTextWriter();
        yield return PartOne(input, getOutputFunction);
        yield return PartTwo(input, getOutputFunction);
    }

    static object PartOne(string input, Func<TextWriter> getOutputFunction)
    {
        var yDir = YAxisDirection.ZeroAtTop;
        var pointsDug = new HashSet<Point2>();
        var current = new Point2(0, 0, yDir);
        pointsDug.Add(current);
        var digPlan = input.SplitBySingleNewline()
            .Select(s => s.Split(' ').ToArray());

        foreach (var dig in digPlan)
        {
            var steps = int.Parse(dig[1]);
            for (var i = 0; i < steps; i++)
            {
                current = dig[0] switch
                {
                    "U" => current.Above,
                    "D" => current.Below,
                    "L" => current.Left,
                    "R" => current.Right,
                    _ => throw new Exception("Unknown dig direction.")
                };
                pointsDug.Add(current);
            }
        }

        var minX = pointsDug.MinBy(p => p.X);
        var minY = pointsDug.MinBy(p => p.Y);
        // var offset = new Point2(minX.X, minY.Y, yDir);

        var maxX = pointsDug.MaxBy(p => p.X);
        var maxY = pointsDug.MaxBy(p => p.Y);
        // var width = maxX.X - minX.X;
        // var height = maxY.Y - minY.Y;

        // var grid = new Grid<int>(width, height, offset);
        //
        // foreach (var p in pointsDug.Keys)
        // {
        //     grid[p] = pointsDug[p];
        // }
        

        
        // How to figure out a point on the "inside"?
        // Let's take one point along the top where we do not dig below.
        var insidePoint = pointsDug.Where(p => p.Y == minY.Y).First(p => !pointsDug.Contains(p.Below)).Below;
        // Now we need to flood the inside.
        var floodedPoints = new HashSet<Point2>();
        var toTest = new Queue<Point2>();
        toTest.Enqueue(insidePoint);
        floodedPoints.Add(insidePoint);
        while (toTest.Count > 0)
        {
            var p = toTest.Dequeue();
            if (!pointsDug.Contains(p.Above) && !floodedPoints.Contains(p.Above))
            {
                toTest.Enqueue(p.Above);
                floodedPoints.Add(p.Above);
            }
            if (!pointsDug.Contains(p.Below) && !floodedPoints.Contains(p.Below))
            {
                toTest.Enqueue(p.Below);
                floodedPoints.Add(p.Below);
            }
            if (!pointsDug.Contains(p.Left) && !floodedPoints.Contains(p.Left))
            {
                toTest.Enqueue(p.Left);
                floodedPoints.Add(p.Left);
            }
            if (!pointsDug.Contains(p.Right) && !floodedPoints.Contains(p.Right))
            {
                toTest.Enqueue(p.Right);
                floodedPoints.Add(p.Right);
            }
        }
        
        var w = getOutputFunction();
        for (var y = minY.Y; y <= maxY.Y; y++)
        {
            for (var x = minX.X; x <= maxX.X; x++)
            {
                var p = new Point2(x, y, yDir);
                w.Write(pointsDug.Contains(p) ? $"#" : ".");
            }
            w.WriteLine();
        }
        
        return pointsDug.Count + floodedPoints.Count;
    }

    static object PartTwo(string input, Func<TextWriter> getOutputFunction)
    {
        var yDir = YAxisDirection.ZeroAtTop;
        var pointsDug = new HashSet<Point2>();
        var current = new Point2(0, 0, yDir);
        pointsDug.Add(current);
        var digPlan = input.SplitBySingleNewline()
            .Select(s => s.Split(' ').Last());

        foreach (var dig in digPlan)
        {
            var steps = int.Parse(dig[2..^2], NumberStyles.HexNumber);
            for (var i = 0; i < steps; i++)
            {
                current = dig[^2] switch
                {
                    '0' => current.Right,
                    '1' => current.Below,
                    '2' => current.Left,
                    '3' => current.Above,
                    _ => throw new Exception("Unknown dig direction.")
                };
                pointsDug.Add(current);
            }
        }

        var minY = pointsDug.MinBy(p => p.Y);
        
        // How to figure out a point on the "inside"?
        // Let's take one point along the top where we do not dig below.
        var insidePoint = pointsDug.Where(p => p.Y == minY.Y).First(p => !pointsDug.Contains(p.Below)).Below;
        // Now we need to flood the inside.
        var floodedPoints = new HashSet<Point2>();
        var toTest = new Queue<Point2>();
        toTest.Enqueue(insidePoint);
        floodedPoints.Add(insidePoint);
        while (toTest.Count > 0)
        {
            var p = toTest.Dequeue();
            if (!pointsDug.Contains(p.Above) && !floodedPoints.Contains(p.Above))
            {
                toTest.Enqueue(p.Above);
                floodedPoints.Add(p.Above);
            }
            if (!pointsDug.Contains(p.Below) && !floodedPoints.Contains(p.Below))
            {
                toTest.Enqueue(p.Below);
                floodedPoints.Add(p.Below);
            }
            if (!pointsDug.Contains(p.Left) && !floodedPoints.Contains(p.Left))
            {
                toTest.Enqueue(p.Left);
                floodedPoints.Add(p.Left);
            }
            if (!pointsDug.Contains(p.Right) && !floodedPoints.Contains(p.Right))
            {
                toTest.Enqueue(p.Right);
                floodedPoints.Add(p.Right);
            }
        }
        return pointsDug.Count + floodedPoints.Count;
    }
}