using AdventOfCode.Framework;
using AdventOfCode.Utilities;

namespace AdventOfCode.Solutions.Y2023.Day15;

class Solution : ISolver
{
    public int Year => 2023;
    public int Day => 15;
    public string GetName() => "Lens Library";

    public IEnumerable<object> Solve(string input, Func<TextWriter> getOutputFunction)
    {
        // var emptyOutput = () => new NullTextWriter();
        yield return PartOne(input, getOutputFunction);
        yield return PartTwo(input, getOutputFunction);
    }

    private static int ToHash(string s)
    {
        return s.Aggregate(0, (current, c) => (current + c) * 17 % 256);
    }

    static object PartOne(string input, Func<TextWriter> getOutputFunction)
    {
        var sum = input.Split(',', StringSplitOptions.TrimEntries)
            .Select(ToHash)
            .Sum();
        return sum;
    }

    private readonly record struct Operation(string Label, int BoxNumber, char OpChar, int FocalLength);

    private readonly record struct Lens(string Label, int FocalLength);

    private class Box(int boxNumber)
    {
        public int BoxNumber { get; } = boxNumber;
        private readonly List<string> Lenses = new();
        public readonly List<int> FocalLengths = new();

        public void Add(Lens lens)
        {
            var index = Lenses.IndexOf(lens.Label);
            if (index >= 0)
            {
                FocalLengths[index] = lens.FocalLength;
            }
            else
            {
                Lenses.Add(lens.Label);
                FocalLengths.Add(lens.FocalLength);
            }
        }

        public void Remove(string label)
        {
            var index = Lenses.IndexOf(label);
            if (index >= 0)
            {
                Lenses.RemoveAt(index);
                FocalLengths.RemoveAt(index);
            }
        }
    }

    static object PartTwo(string input, Func<TextWriter> getOutputFunction)
    {
        var ops = input.Split(',', StringSplitOptions.TrimEntries)
            .Groups(@"(.*)([=-])(\d*)?")
            .Select(g => new Operation(g[0].Value, ToHash(g[0].Value), g[1].Value[0], g[1].Value[0] == '=' ? int.Parse(g[2].Value) : 0));

        var boxes = new List<Box>(Enumerable.Range(0, 256).Select(i => new Box(i)));


        foreach (var op in ops)
        {
            var box = boxes[op.BoxNumber];
            switch (op.OpChar)
            {
                case '=' :
                    box.Add(new Lens(op.Label, op.FocalLength));
                    break;
                case '-':
                    box.Remove(op.Label);
                    break;
                default:
                    throw new ArgumentException("Unknown op.");
            }
        }

        var sum = 0;
        for (var boxIndex = 0; boxIndex < boxes.Count; boxIndex++)
        {
            var box = boxes[boxIndex];
            for (var i = 0; i < box.FocalLengths.Count; i++)
            {
                sum += (boxIndex + 1) * (i + 1) * box.FocalLengths[i];
            }
        }

        return sum;
    }
}