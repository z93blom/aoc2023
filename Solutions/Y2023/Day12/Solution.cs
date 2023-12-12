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

    private static readonly SpringState[] SingleUnknownState = { SpringState.Unknown};
    
    static object PartTwo(string input, Func<TextWriter> getOutputFunction)
    {
        SpringState ToSpringState(char c)
        {
            return c switch
            {
                '.' => SpringState.Operational,
                '#' => SpringState.Damaged,
                '?' => SpringState.Unknown,
                _ => throw new Exception("Unknown spring state.")
            };
        }
        
        var rows = input.SplitBySingleNewline()
            .Select(s => s.Split(" ").ToArray())
            .Select(a => new SpringRow(a[0].Select(ToSpringState).ToArray(), a[1].Integers().ToArray()))
            .Select(r => new SpringRow(
                Enumerable.Range(0, 5).Aggregate(Enumerable.Empty<SpringState>(), (a, _) => a.Concat(SingleUnknownState).Concat(r.Springs)).Skip(1).ToArray(), 
                Enumerable.Range(0, 5).Aggregate(Array.Empty<int>(), (c, _) => c.Concat(r.DamagedSequence).ToArray())))
            .ToArray();

        var arrangements = new List<long>();
        foreach (var row in rows)
        {
            var cache = new Dictionary<(int, int), long>();
            var numberOfArrangements = row.GetValidArrangements(0, 0, cache);
            arrangements.Add(numberOfArrangements);
        }
        
        return arrangements.Sum();
    }

    private enum SpringState
    {
        Operational,
        Damaged,
        Unknown,
    }

    private readonly record struct SpringRow(SpringState[] Springs, int[] DamagedSequence)
    {
        public long GetValidArrangements(int springIndex, int damageSequenceIndex, Dictionary<(int, int), long> cache)
        {
            if (cache.ContainsKey((springIndex, damageSequenceIndex)))
            {
                return cache[(springIndex, damageSequenceIndex)];
            }
        
            // The next sequence can either start here, or not start here.
            var arrangementCount = 0L;
            if (Springs[springIndex] != SpringState.Damaged && springIndex + 1 < Springs.Length)
            {
                // Try at a later point (allow the Unknowns to start later)
                arrangementCount += GetValidArrangements(springIndex + 1, damageSequenceIndex, cache);
            }

            var endIndex = springIndex + DamagedSequence[damageSequenceIndex];
            if (endIndex <= Springs.Length &&
                Springs[springIndex..endIndex].All(s => s != SpringState.Operational) &&
                (endIndex == Springs.Length || Springs[endIndex] != SpringState.Damaged))
            {
                // This point can start a damaged sequence.
                if (damageSequenceIndex + 1 == DamagedSequence.Length)
                {
                    // We are at the last damaged sequence. Are there any damaged springs left in the list?
                    arrangementCount += Springs.Skip(endIndex + 1).Any(s => s == SpringState.Damaged) ? 0 : 1;
                }
                else if (endIndex + 1 < Springs.Length)
                {
                    // Go on to the next damaged sequence.
                    arrangementCount += GetValidArrangements(endIndex + 1, damageSequenceIndex + 1, cache);
                }
            }

            cache[(springIndex, damageSequenceIndex)] = arrangementCount;
            return arrangementCount;
        }
    }

    
}