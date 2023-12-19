using AdventOfCode.Framework;
using AdventOfCode.Utilities;
using LanguageExt;

namespace AdventOfCode.Solutions.Y2023.Day19;

class Solution : ISolver
{
    public int Year => 2023;
    public int Day => 19;
    public string GetName() => "Aplenty";

    public IEnumerable<object> Solve(string input, Func<TextWriter> getOutputFunction)
    {
        // var emptyOutput = () => new NullTextWriter();
        yield return PartOne(input, getOutputFunction);
        yield return PartTwo(input, getOutputFunction);
    }
    
    private readonly record struct Workflow(string Name, Condition[] Conditions);

    private readonly record struct Condition(Func<Part, bool> Comparison, string Target);

    private readonly record struct Part(int x, int m, int a, int s)
    {
        public int TotalValue => x + m + a + s;
    }
    
    static object PartOne(string input, Func<TextWriter> getOutputFunction)
    {
        var a = input.SplitByDoubleNewline().ToArray();
        Dictionary<string, Workflow> workflows = new();
        foreach (var s in a[0].SplitBySingleNewline())
        {
            var groups = s.Groups(@"(\w+)\{(.*)\}").First();
            var name = groups[0].Value;
            var conditions = groups[1].Value
                .Split(',')
                .Select(ParseCondition)
                .ToArray();
            
            workflows.Add(name, new Workflow(name, conditions));
        }
        
        workflows.Add("A", new Workflow("A", Array.Empty<Condition>()));
        workflows.Add("R", new Workflow("R", Array.Empty<Condition>()));

        var parts = a[1].SplitBySingleNewline().Select(ToPart).ToArray();

        var acceptedParts = new List<Part>();

        foreach (var part in parts)
        {
            var wf = workflows["in"];
            while (wf.Name is not ("R" or "A"))
            {
                foreach (var c in wf.Conditions)
                {
                    if (c.Comparison(part))
                    {
                        wf = workflows[c.Target];
                        break;
                    }
                }
            }

            if (wf.Name == "A")
            {
                acceptedParts.Add(part);
            }
        }
        
        return acceptedParts.Sum(p => p.TotalValue);
    }

    private static Part ToPart(string arg)
    {
        var v = arg.Integers().ToArray();
        var part = new Part(v[0], v[1], v[2], v[3]);
        return part;
    }

    private static Condition ParseCondition(string arg)
    {
        var colonIndex = arg.IndexOf(':');
        if (colonIndex < 0)
        {
            // default condition
            return new Condition(_ => true, arg);
        }

        var property = arg[0];
        var comparer = arg[1];
        var number = int.Parse(arg[2..colonIndex]);
        var target = arg[(colonIndex + 1)..];
        Func<Part, bool> comparison = property switch
        {
            'a' => comparer switch
            {
                '<' => part => part.a < number,
                '>' => part => part.a > number,
                _ => throw new Exception("Unknown comparer"),
            },
            'm' => comparer switch
            {
                '<' => part => part.m < number,
                '>' => part => part.m > number,
                _ => throw new Exception("Unknown comparer"),
            },
            's' => comparer switch
            {
                '<' => part => part.s < number,
                '>' => part => part.s > number,
                _ => throw new Exception("Unknown comparer"),
            },
            'x' => comparer switch
            {
                '<' => part => part.x < number,
                '>' => part => part.x > number,
                _ => throw new Exception("Unknown comparer"),
            },
            _ => throw new Exception("Unknown part property")
        };

        return new Condition(comparison, target);
    }
    
    

    static object PartTwo(string input, Func<TextWriter> getOutputFunction)
    {
        return 0;
    }
}