using System.Globalization;
using AdventOfCode.Framework;
using AdventOfCode.Utilities;

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
        var instructions = input.SplitBySingleNewline()
            .Select(s => s.Split(' ').ToArray())
            .Select(a => new Instruction(int.Parse(a[1]), a[0][0] switch
            {
                'R' => CompassDirection.East,
                'D' => CompassDirection.South,
                'L' => CompassDirection.West,
                'U' => CompassDirection.North,
                _ => throw new ArgumentException()
            }));
        
        return ShoelaceArea(instructions);
    }
    
    static double ShoelaceArea(IEnumerable<Instruction> instructions) 
    {
        List<Point2> points = new();
        var current = new Point2(0, 0, YAxisDirection.ZeroAtTop);
        var trenchLength = 0L;
        foreach (var instruction in instructions)
        {
            trenchLength += instruction.Length;

            current = current.Move(instruction.Dir, instruction.Length);
            points.Add(current);
        }

        var res1 = 0L;
        var res2 = 0L;
        for (var i = 0; i < points.Count; i++)
        {
            res1 += points[i].X * points[(i + 1) % points.Count].Y;
            res2 += points[i].Y * points[(i + 1) % points.Count].X;
        }

        return (trenchLength / 2) + Math.Abs((res1 - res2) / 2) + 1;

    }
  
    private readonly record struct Instruction(int Length, CompassDirection Dir);

    static object PartTwo(string input, Func<TextWriter> getOutputFunction)
    {
        var instructions = input.SplitBySingleNewline()
            .Select(s => s.Split(' ').ToArray())
            .Select(a => a[2].Trim('#', '(', ')'))
            .Select(s => new Instruction(int.Parse(s[..5], NumberStyles.HexNumber), s[^1] switch
            {
                '0' => CompassDirection.East,
                '1' => CompassDirection.South,
                '2' => CompassDirection.West,
                '3' => CompassDirection.North,
                _ => throw new ArgumentException()
            }));

        return ShoelaceArea(instructions);
    }
}