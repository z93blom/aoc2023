using AdventOfCode.Framework;
using AdventOfCode.Utilities;

namespace AdventOfCode.Solutions.Y2023.Day05;

class Solution : ISolver
{
    public int Year => 2023;
    public int Day => 5;
    public string GetName() => "If You Give A Seed A Fertilizer";

    public IEnumerable<object> Solve(string input, Func<TextWriter> getOutputFunction)
    {
        // var emptyOutput = () => new NullTextWriter();
        yield return PartOne(input, getOutputFunction);
        yield return PartTwo(input, getOutputFunction);
    }

    static object PartOne(string input, Func<TextWriter> getOutputFunction)
    {
        var lines = input
            .Lines(StringSplitOptions.TrimEntries)
            .ToArray();

        var seeds = lines[0].Longs().ToArray();
        var index = 2;
        List<Map> maps = new();
        while (index < lines.Length)
        {
            var line = lines[index];
            
            // Read the name of the map.
            var names = line.Groups(@"(.*)-to-(.*) map\:").First();
            index++;
            List<Mapper> mappers = new();
            while (index < lines.Length)
            {
                line = lines[index];
                if (string.IsNullOrWhiteSpace(line))
                {
                    break;
                }
                
                var values = line.Longs().ToArray();
                mappers.Add(new Mapper(values[0], values[1], values[2]));
                index++;
            }

            var map = new Map(names[0].Value, names[1].Value, mappers.ToArray());
            maps.Add(map);
            index++;
        }

        // Get the final location for each seed.
        var seedToLocationMap = new Dictionary<long, long>();
        foreach (var seed in seeds)
        {
            var final = maps.Aggregate(seed, (s, map) => map.MapSeed(s));
            seedToLocationMap[seed] = final;
        }

        return seedToLocationMap.Values.Min();
    }

    private record struct Map(string From, string To, Mapper[] maps)
    {
        public long MapSeed(long seed)
        {
            foreach (var map in maps)
            {
                if (seed >= map.Source && seed <= map.Source + map.Range)
                {
                    return seed - map.Source + map.Destination;
                }
            }

            return seed;
        }
    }

    private record struct Mapper(long Destination, long Source, long Range);

    static object PartTwo(string input, Func<TextWriter> getOutputFunction)
    {
        return 0;
    }
}