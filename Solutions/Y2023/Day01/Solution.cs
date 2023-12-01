using AdventOfCode.Framework;
using AdventOfCode.Utilities;

namespace AdventOfCode.Solutions.Y2023.Day01;

class Solution : ISolver
{
    public int Year => 2023;
    public int Day => 1;
    public string GetName() => "Trebuchet?!";
    
    private static readonly string[] Numbers = { "one", "two", "three", "four", "five", "six", "seven", "eight", "nine" };

    public IEnumerable<object> Solve(string input, Func<TextWriter> getOutputFunction)
    {
        // var emptyOutput = () => new NullTextWriter();
        yield return PartOne(input, getOutputFunction);
        yield return PartTwo(input, getOutputFunction);
    }

    static object PartOne(string input, Func<TextWriter> getOutputFunction)
    {
        var values = input
            .Lines()
            .Select(l => (l.First(c => char.IsDigit(c)) - '0') * 10 + l.Last(c => char.IsDigit(c)) - '0')
            .ToArray();
        var sum = values.Sum();
        return sum;
    }

    static object PartTwo(string input, Func<TextWriter> getOutputFunction)
    {
        var values = input
            .Lines()
            .Select(l => FirstDigit(l) * 10 + LastDigit(l))
            .ToArray();
        var sum = values.Sum();
        return sum;
    }

    static int FirstDigit(string line)
    {
        var index = 0;
        while (true)
        {
            if (char.IsDigit(line[index]))
            {
                return line[index] - '0';
            }

            foreach (var (s, i) in Numbers.Select((s, i) => (s, i)))
            {
                if (Is(line, index, s))
                    return i + 1;
            }
            
            index++;
        }
    }
    
    static int LastDigit(string line)
    {
        var index = line.Length - 1;
        while (true)
        {
            if (char.IsDigit(line[index]))
            {
                return line[index] - '0';
            }

            foreach (var (s, i) in Numbers.Select((s, i) => (s, i)))
            {
                if (Is(line, index, s))
                    return i + 1;
            }

            index--;
        }
    }
    
    private static bool Is(string line, int index, string s)
    {
        for (var offset = 0; offset < s.Length; offset++)
        {
            if (line.Length <= index + offset)
                return false;
            
            if (line[index + offset] != s[offset])
                return false;
        }

        return true;
    }

}