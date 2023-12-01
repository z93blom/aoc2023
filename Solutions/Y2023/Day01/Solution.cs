using AdventOfCode.Framework;
using AdventOfCode.Utilities;

namespace AdventOfCode.Solutions.Y2023.Day01;

class Solution : ISolver
{
    public int Year => 2023;
    public int Day => 1;
    public string GetName() => "Trebuchet?!";

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

            if (Is(line, index,"one"))
                return 1;
            
            if (Is(line, index, "two"))
                return 2;
            if (Is(line, index, "three"))
                return 3;
            if (Is(line, index, "four"))
                return 4;
            if (Is(line, index, "five"))
                return 5;
            if (Is(line, index, "six"))
                return 6;
            if (Is(line, index, "seven"))
                return 7;
            if (Is(line, index, "eight"))
                return 8;
            if (Is(line, index, "nine"))
                return 9;
            index++;
        }

        throw new ArgumentException();
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

            if (Is(line, index,"one"))
                return 1;
            
            if (Is(line, index, "two"))
                return 2;
            if (Is(line, index, "three"))
                return 3;
            if (Is(line, index, "four"))
                return 4;
            if (Is(line, index, "five"))
                return 5;
            if (Is(line, index, "six"))
                return 6;
            if (Is(line, index, "seven"))
                return 7;
            if (Is(line, index, "eight"))
                return 8;
            if (Is(line, index, "nine"))
                return 9;
            index--;
        }

        throw new ArgumentException();
    }
    
    private static bool Is(string line, int index, string s)
    {
        for (int offset = 0; offset < s.Length; offset++)
        {
            if (line.Length <= index + offset)
                return false;
            if (line[index + offset] != s[offset])
                return false;
        }

        return true;
    }
}