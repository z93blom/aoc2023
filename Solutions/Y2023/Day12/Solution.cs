using AdventOfCode.Framework;
using AdventOfCode.Utilities;
using LanguageExt.UnitsOfMeasure;
using Spectre.Console;

namespace AdventOfCode.Solutions.Y2023.Day12;

class Solution : ISolver
{
    public int Year => 2023;
    public int Day => 12;


    public string GetName() => "Hot Springs";

    public IEnumerable<object> Solve(string input, Func<TextWriter> getOutputFunction)
    {
        // var emptyOutput = () => new NullTextWriter();
        yield return PartOne(input, getOutputFunction);
        yield return PartTwo(input, getOutputFunction);
    }
    
    private readonly record struct Row(char[] Record, int[] Conditions); 

    static object PartOne(string input, Func<TextWriter> getOutputFunction)
    {
        // There's probably some logic to apply to solve this.
        // For part one, I'm just going to try to brute force it.
        var rows = input.SplitBySingleNewline()
            .Select(s => s.Split(" ").ToArray())
            .Select(a => new Row(a[0].ToCharArray(), a[1].Integers().ToArray()))
            .ToArray();

        var arrangements = new List<long>();
        foreach (var row in rows)
        {
            var permutations = GetPermutations(row.Record).ToArray();
            var validArrangements = permutations
                .Where(p => IsValid(p, row.Conditions))
                .ToArray();
            arrangements.Add(validArrangements.Length);
        }
        
        return arrangements.Sum();
    }

    private static readonly char[] Period = { '.' };
    private static readonly char[] Hash = { '#' };

    private static IEnumerable<char[]> GetPermutations(char[] input)
    {
        if (input.Length == 0)
        {
            yield return [];
            yield break;
        }
        
        var endings = GetPermutations(input[1..]).ToArray();
        if (input[0] == '?')
        {
            foreach (var p in endings)
            {
                yield return Period.Concat(p).ToArray();
                yield return Hash.Concat(p).ToArray();
            }
        }
        else
        {
            foreach (var p in endings)
            {
                var chars = input[..1].Concat(p).ToArray();
                yield return chars;
            }
        }
    }

    private static bool IsValid(char[] input, int[] conditions)
    {
        var groupIndex = 0;
        var i = 0;
        var isInGroup = false;
        var groupLength = 0;
        while (i < input.Length)
        {
            if (input[i] == '#' && isInGroup)
            {
                groupLength++;
                if (groupLength > conditions[groupIndex])
                {
                    // Invalid length of this group.
                    return false;
                }
            }
            else if (input[i] == '#')
            {
                isInGroup = true;
                groupLength = 1;
                if (groupIndex == conditions.Length)
                {
                    // Too many groups.
                    return false;
                }
            }
            else if (isInGroup)
            {
                isInGroup = false;
                if (groupLength != conditions[groupIndex])
                {
                    // Invalid length of this group.
                    return false;
                }

                groupIndex++;
            }

            i++;
        }

        if (isInGroup)
        {
            if (groupLength != conditions[groupIndex])
            {
                // Invalid length of this group.
                return false;
            }

            groupIndex++;
        }

        if (groupIndex != conditions.Length)
        {
            // Too few groups.
            return false;
        }

        return true;
    }

    static object PartTwo(string input, Func<TextWriter> getOutputFunction)
    {
        var rows = input.SplitBySingleNewline()
            .Select(s => s.Split(" ").ToArray())
            .Select(a => new Row(a[0].ToCharArray(), a[1].Integers().ToArray()))
            .Select(r => new Row(
                string.Join('?', Enumerable.Range(0, 5).Select(_ => new string(r.Record))).ToCharArray(), 
                Enumerable.Range(0, 5).Aggregate(Array.Empty<int>(), (c, _) => c.Concat(r.Conditions).ToArray())))
            .ToArray();

        var arrangements = new List<long>();
        foreach (var row in rows)
        {
            var validArrangements = GetValidPermutations(row, row.Record, row.Conditions, 0).ToArray();
            arrangements.Add(validArrangements.Length);
        }
        
        return arrangements.Sum();
    }

    private static IEnumerable<char[]> GetValidPermutations(Row row, char[] input, int[] conditions, int index)
    {
        if (row.Record.Length != input.Length)
        {
            yield break;
        }
        
        if (index == input.Length)
        {
            if (IsValid(input, conditions))
            {
                yield return input;
            }

            yield break;
        }

        if (input[index] != '?')
        {
            foreach (var p in GetValidPermutations(row, input, conditions, index + 1))
            {
                yield return p;
            }
        }
        else
        {
            var start = input[..index];
            if (!StillValid(start, conditions))
            {
                yield break;
            }

            var withPeriod = start.Concat(Period).Concat(input[(index + 1)..]).ToArray();
            foreach (var p in GetValidPermutations(row, withPeriod, conditions, index + 1))
            {
                yield return p;
            }

            var withHash = start.Concat(Hash).Concat(input[(index + 1)..]).ToArray();
            foreach (var p in GetValidPermutations(row, withHash, conditions, index + 1))
            {
                yield return p;
            }
        }
    }
    
    private static bool StillValid(char[] input, int[] conditions)
    {
        var groupIndex = 0;
        var i = 0;
        var isInGroup = false;
        var groupLength = 0;
        while (i < input.Length)
        {
            if (input[i] == '#' && isInGroup)
            {
                groupLength++;
                if (groupLength > conditions[groupIndex])
                {
                    // Invalid length of this group.
                    return false;
                }
            }
            else if (input[i] == '#')
            {
                isInGroup = true;
                groupLength = 1;
                if (groupIndex == conditions.Length)
                {
                    // Too many groups.
                    return false;
                }
            }
            else if (isInGroup)
            {
                isInGroup = false;
                if (groupLength != conditions[groupIndex])
                {
                    // Invalid length of this group.
                    return false;
                }

                groupIndex++;
            }

            i++;
        }

        if (isInGroup)
        {
            if (groupLength > conditions[groupIndex])
            {
                // Invalid length of this group.
                return false;
            }
        }

        return true;
    }
}