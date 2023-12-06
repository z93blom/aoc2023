using System.Collections;
using AdventOfCode.Framework;
using AdventOfCode.Utilities;
using LanguageExt.UnitsOfMeasure;

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
                mappers.Add(new Mapper(values[0], new RangeL(values[1], values[2])));
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

    private readonly record struct Map(string From, string To, Mapper[] Mappers)
    {
        public long MapSeed(long seed)
        {
            foreach (var map in Mappers)
            {
                if (seed >= map.Source.Start && seed <= map.Source.Last)
                {
                    return seed - map.Source.Start + map.Destination;
                }
            }

            return seed;
        }
    }

    private record struct Mapper(long Destination, RangeL Source)
    {
        public RangeL DestinationRange = Source with { Start = Destination };

        private long MapDestinationToSource(long position)
        {
            return position - Destination + Source.Start;
        }
    }

    private readonly record struct RangeL(long Start, long Length)
    {
        public long Last => Start + Length - 1;

        public bool Intersects(RangeL other)
        {
            if (other.Start > Last)
                return false;

            if (other.Last < Start)
                return false;

            return true;
        }

        public RangeL Intersection(RangeL other)
        {
            var start = other.Start > Start ? other.Start : Start;
            var last = other.Last < Last ? other.Last : Last;
            return new RangeL(start, last - start); // Length might be off by one, but I don't care.
        }

        public (RangeL Left, RangeL Right) Split(long firstRightPosition)
        {
            var left = this with { Length = firstRightPosition - Start };
            var right = new RangeL(firstRightPosition, Length - left.Length);
            return (left, right);
        }
    }

    static object PartTwo(string input, Func<TextWriter> getOutputFunction)
    {
        var lines = input
            .Lines(StringSplitOptions.TrimEntries)
            .ToArray();

        var seeds = lines[0].Longs().ToArray();

        var seedRanges = new List<RangeL>();
        for (int i = 0; i < seeds.Length; i+= 2)
        {
            seedRanges.Add(new RangeL(seeds[i], seeds[i + 1]));
        }

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
                mappers.Add(new Mapper(values[0], new RangeL(values[1], values[2])));
                index++;
            }

            var map = new Map(names[0].Value, names[1].Value, mappers.OrderBy(m => m.Destination).ToArray());
            maps.Add(map);
            index++;
        }
        
        return 0;
    }
}