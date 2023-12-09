using AdventOfCode.Framework;
using AdventOfCode.Utilities;

namespace AdventOfCode.Solutions.Y2023.Day09;

class Solution : ISolver
{
    public int Year => 2023;
    public int Day => 9;
    public string GetName() => "Mirage Maintenance";

    public IEnumerable<object> Solve(string input, Func<TextWriter> getOutputFunction)
    {
        yield return PartOne(input, getOutputFunction);
        yield return PartTwo(input, getOutputFunction);
    }

    static object PartOne(string input, Func<TextWriter> getOutputFunction)
    {
        var lines = input
            .SplitBySingleNewline()
            .Select(l => l.Integers().ToList())
            .ToList();

        var newValues = new List<int>();
        foreach (var line in lines)
        {
            var current = line;
            var next = line;
            var allZeroes = current.All(i => i == 0);
            var s = new Stack<List<int>>();
            s.Push(next);
            while (!allZeroes)
            {
                current = next;
                next = new List<int>();

                for (var i = 0; i < current.Count - 1; i++)
                {
                    next.Add(current[i+1] - current[i]);
                }
                
                s.Push(next);
                allZeroes = next.All(i => i == 0);
            }
            
            // Now go backwards
            current = s.Pop();
            current.Add(0);
            while (s.Count > 0)
            {
                next = s.Pop();
                next.Add(next[^1] + current[^1]);
                current = next;
            }
            
            newValues.Add(current[^1]);
        }

        return newValues.Sum();
    }

    static object PartTwo(string input, Func<TextWriter> getOutputFunction)
    {
        var lines = input
            .SplitBySingleNewline()
            .Select(l => l.Integers().ToList())
            .ToList();

        var newValues = new List<int>();
        foreach (var line in lines)
        {
            var current = line;
            var next = line;
            var allZeroes = current.All(i => i == 0);
            var s = new Stack<List<int>>();
            s.Push(next);
            while (!allZeroes)
            {
                current = next;
                next = new List<int>();

                for (var i = 0; i < current.Count - 1; i++)
                {
                    next.Add(current[i+1] - current[i]);
                }
                
                s.Push(next);
                allZeroes = next.All(i => i == 0);
            }
            
            // Now go backwards
            current = s.Pop();
            current.Insert(0, 0);
            while (s.Count > 0)
            {
                next = s.Pop();
                next.Insert(0, next[0] - current[0]);
                current = next;
            }
            
            newValues.Add(current[0]);
        }

        return newValues.Sum();
    }
}