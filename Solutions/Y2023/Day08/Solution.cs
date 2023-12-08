using System.Text.RegularExpressions;
using AdventOfCode.Framework;
using AdventOfCode.Utilities;

namespace AdventOfCode.Solutions.Y2023.Day08;

partial class Solution : ISolver
{
    public int Year => 2023;
    public int Day => 8;
    public string GetName() => "Haunted Wasteland";

    public IEnumerable<object> Solve(string input, Func<TextWriter> getOutputFunction)
    {
        // var emptyOutput = () => new NullTextWriter();
        yield return PartOne(input, getOutputFunction);
        yield return PartTwo(input, getOutputFunction);
    }

    public readonly record struct Node(string Name, string Left, string Right);

    static object PartOne(string input, Func<TextWriter> getOutputFunction)
    {
        var lines = input.Lines().ToArray();
        var instructions = lines[0];

        var nodes = lines
            .Skip(1)
            .Select(l => NodeMatcher().Match(l))
            .Select(m => m.Groups.Cast<Group>().Skip(1).ToArray())
            .Select(ga => new Node(ga[0].Value, ga[1].Value, ga[2].Value))
            .ToDictionary(n => n.Name, n => n);


        var steps = 0L;
        var index = 0;
        var current = nodes["AAA"];
        while (current != nodes["ZZZ"])
        {
            var instruction = instructions[index];
            index = (index + 1) % instructions.Length;
            steps++;
            current = instruction switch
            {
                'L' => nodes[current.Left],
                'R' => nodes[current.Right],
                _ => throw new Exception("Invalid direction")
            };
        }

        return steps;
    }

    static object PartTwo(string input, Func<TextWriter> getOutputFunction)
    {
        var lines = input.Lines().ToArray();
        var instructions = lines[0];

        var nodes = lines
            .Skip(1)
            .Select(l => NodeMatcher().Match(l))
            .Select(m => m.Groups.Cast<Group>().Skip(1).ToArray())
            .Select(ga => new Node(ga[0].Value, ga[1].Value, ga[2].Value))
            .ToDictionary(n => n.Name, n => n);


        var startingNodes = nodes.Where(kvp => kvp.Value.Name.EndsWith('A')).Select(kvp => kvp.Value).ToArray();
        var stepsUntilZ = new List<double>();
        foreach (var node in startingNodes)
        {
            var index = 0;
            var steps = 0L;
            var current = node;
            while (!current.Name.EndsWith('Z'))
            {
                var instruction = instructions[index];
                index = (index + 1) % instructions.Length;
                steps++;
                current = instruction switch
                {
                    'L' => nodes[current.Left],
                    'R' => nodes[current.Right],
                    _ => throw new Exception("Invalid direction")
                };
            }
            
            stepsUntilZ.Add(steps);
        }

        var total = stepsUntilZ.Aggregate(1.0, (a, b) => AocMath.lcm(a, b));
        return total;
    }

    [GeneratedRegex(@"(\w\w\w) = \((\w\w\w), (\w\w\w)\)")]
    private static partial Regex NodeMatcher();
}