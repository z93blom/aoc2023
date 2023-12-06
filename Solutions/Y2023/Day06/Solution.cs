using AdventOfCode.Framework;
using AdventOfCode.Utilities;

namespace AdventOfCode.Solutions.Y2023.Day06;

class Solution : ISolver
{
    public int Year => 2023;
    public int Day => 6;
    public string GetName() => "Wait For It";

    public IEnumerable<object> Solve(string input, Func<TextWriter> getOutputFunction)
    {
        // var emptyOutput = () => new NullTextWriter();
        yield return PartOne(input, getOutputFunction);
        yield return PartTwo(input, getOutputFunction);
    }

    static object PartOne(string input, Func<TextWriter> getOutputFunction)
    {
        var lines = input.Lines().ToArray();
        var times = lines[0].Integers().ToArray();
        var distances = lines[1].Integers().ToArray();

        var results = new List<long>();
        for (var i = 0; i < times.Length; i++)
        {
            var wins = 0;
            for (var t = 0; t < times[i]; t++)
            {
                var speed = t;
                var distance = speed * (times[i] - t);
                if (distance > distances[i])
                {
                    wins++;
                }
            }
            
            results.Add(wins);
        }
        
        return results.Aggregate(1L, (cur, next) => cur*next);
    }

    static object PartTwo(string input, Func<TextWriter> getOutputFunction)
    {
        var lines = input.Lines().Select(s => s.RemoveChar(' ')).ToArray();
        var time = lines[0].Longs().ToArray()[0];
        var distance = lines[1].Longs().ToArray()[0];
        
        var startWin = 0L;
        var endWin = long.MaxValue;
        for (var t = 0; t < time; t++)
        {
            var speed = t;
            var d = speed * (time - t);
            if (d > distance)
            {
                startWin = t;
                break;
            }
        }

        for (var t = time; t >= 0; t--)
        {
            var speed = t;
            var d = speed * (time - t);
            if (d > distance)
            {
                endWin = t;
                break;
            }
        }

        return endWin - startWin + 1;
    }
}