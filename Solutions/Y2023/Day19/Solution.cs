using System.Net.WebSockets;
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
    
    
    private readonly record struct Workflow2(string Name, Dictionary<string, Condition2[]> Conditions);

    private readonly record struct Condition2(bool DefaultWorkflow, char Property, char Comparer, int Value, string Target)
    {
        public static Condition2 Parse(string arg)
        {
            var colonIndex = arg.IndexOf(':');
            if (colonIndex < 0)
            {
                // default condition
                return new Condition2(true, ' ', ' ', 0, arg);
            }

            var property = arg[0];
            var comparer = arg[1];
            var number = int.Parse(arg[2..colonIndex]);
            var target = arg[(colonIndex + 1)..];
            return new Condition2(false, property, comparer, number, target);
        }
    }

    static object PartTwo(string input, Func<TextWriter> getOutputFunction)
    {
        var a = input.SplitByDoubleNewline().ToArray();
        Dictionary<string, Workflow2> workflows = new();
        foreach (var s in a[0].SplitBySingleNewline())
        {
            var groups = s.Groups(@"(\w+)\{(.*)\}").First();
            var name = groups[0].Value;
            var conditions = groups[1].Value
                .Split(',')
                .Select(Condition2.Parse)
                .ToArray();
            
            workflows.Add(name, new Workflow2(name, 
                conditions.GroupBy(c => c.Target).ToDictionary(g => g.Key, g => g.ToArray())));
        }
        
        // We want to find all the combinations that will end up in state "A".
        var conditionsToReachA = new Dictionary<string, List<ConditionsToReachA>>();
        
        // Start with all the workflows that end up in A, and then work backwards.
        Queue<string> targetWorkflows = new();
        targetWorkflows.Enqueue("A");
        conditionsToReachA["A"] = new List<ConditionsToReachA>(new []{ new ConditionsToReachA(Array.Empty<Condition2>()) });
        while (targetWorkflows.Count > 0)
        {
            var targetWorkflow = targetWorkflows.Dequeue();
            foreach (var wf in workflows.Values.Where(wf => wf.Conditions.ContainsKey(targetWorkflow)))
            {
                if (conditionsToReachA.ContainsKey(wf.Name))
                {
                    continue;
                }
                
                var previousConditions = conditionsToReachA[targetWorkflow];
                if (wf.Conditions[targetWorkflow].Any(c => c.DefaultWorkflow))
                {
                    // No new conditions need to be added.
                    // Just copy the conditions from the target workflow.
                    conditionsToReachA[wf.Name] = new List<ConditionsToReachA>(previousConditions);
                }
                else
                {
                    conditionsToReachA[wf.Name] = new List<ConditionsToReachA>();
                    foreach (var newCondition in wf.Conditions[targetWorkflow])
                    {
                        foreach (var previousCondition in previousConditions)
                        {
                            conditionsToReachA[wf.Name].Add(previousCondition.AddCondition(newCondition));
                        }
                    }
                }
                
                targetWorkflows.Enqueue(wf.Name);
            }
        }

        var conditionsForIn = conditionsToReachA["in"];
        var acceptableConditions = 0L;
        foreach (var cdns in conditionsForIn)
        {
            acceptableConditions += cdns.AcceptableConditions();
        }

        return acceptableConditions;
    }

    private readonly struct ConditionsToReachA(Condition2[] Conditions)
    {
        public ConditionsToReachA AddCondition(Condition2 condition)
        {
            if (condition.DefaultWorkflow)
            {
                return this;
            }

            return new ConditionsToReachA(Conditions.Append(condition).ToArray());
        }

        public long AcceptableConditions()
        {
            var groups = Conditions.GroupBy(c => c.Property);
            var allowedRanges = new List<int>();
            foreach (var group in groups)
            {
                var minValue = 1;
                var maxValue = 4000;
                foreach (var c in group)
                {
                    if (c.Comparer == '<')
                    {
                        maxValue = Math.Min(c.Value, maxValue);
                    }
                    else if (c.Comparer == '>')
                    {
                        minValue = Math.Max(c.Value, minValue);
                    }
                    else
                    {
                        throw new Exception("Invalid comparer");
                    }
                }

                if (minValue <= maxValue)
                {
                    allowedRanges.Add(maxValue - minValue + 1);
                }
                else
                {
                    return 0;
                }
            }

            return allowedRanges.Aggregate(1L, (v, i) => v * i);
        }
    }
}