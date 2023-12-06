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

            var map = new Map(names[0].Value, names[1].Value, mappers.OrderBy(m => m.Source.Start).ToArray());
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

        public IEnumerable<RangeL> MapRange(RangeL range)
        {
            var mappers = GetMappers(range).ToArray();
            if (mappers.Length == 0)
            {
                yield return range;
                yield break;
            }

            var r = range;
            var index = 0;
            while (index < mappers.Length && !r.IsEmpty)
            {
                var mapper = mappers[index];
                if (r.Start > mapper.Source.Last)
                {
                    index++;
                    continue;
                }

                RangeL left;
                if (r.Start < mapper.Source.Start)
                {
                    // The range sticks out to the left of the current range.
                    (left, r) = r.Split(mapper.Source.Start);
                    yield return left;
                    continue;
                }

                (left, r) = r.Split(mapper.Source.Last + 1);
                yield return left with {Start = mapper.MapValue(left.Start)};
            }

            if (!r.IsEmpty)
            {
                yield return r;
            }
        }

        private IEnumerable<Mapper> GetMappers(RangeL range)
        {
            return Mappers.Where(mapper => mapper.Source.Intersects(range));
        }
    }

    private readonly record struct Mapper(long Destination, RangeL Source)
    {
        public long MapValue(long value)
        {
            if (value < Source.Start || value > Source.Last)
            {
                return value;
            }

            return value - Source.Start + Destination;
        }

        public IEnumerable<RangeL> MapRange(RangeL range)
        {
            var totalLength = 0L;
            if (!range.Intersects(Source))
            {
                yield return range;
                totalLength = range.Length;
            }
            else if (range.Start < Source.Start && range.Last > Source.Last)
            {
                var (left, rest) = range.Split(Source.Start);
                var (middle, right) = rest.Split(Source.Last + 1);
                yield return left;
                yield return middle with { Start = MapValue(middle.Start) };
                yield return right;
                totalLength = left.Length + middle.Length + right.Length;
            }
            else if (range.Start < Source.Start)
            {
                var (left, right) = range.Split(Source.Start);
                yield return left;
                yield return right with { Start = MapValue(right.Start) };
                totalLength = left.Length + right.Length;
            }
            else if (range.Last > Source.Last)
            {
                var (left, right) = range.Split(Source.Last + 1);
                yield return left with { Start = MapValue(left.Start) };
                yield return right;
                totalLength = left.Length + right.Length;
            }
            else
            {
                yield return range with { Start = MapValue(range.Start) };
                totalLength = range.Length;
            }

            if (totalLength != range.Length)
            {
                throw new Exception("Implementation error");
            }
        }
    }

    private readonly record struct RangeL(long Start, long Length)
    {
        public static RangeL Empty => new RangeL(0, 0); 
        public long Last => Start + Length - 1;

        public bool IsEmpty => Length <= 0;

        public bool Intersects(RangeL other)
        {
            if (other.Start > Last)
                return false;

            if (other.Last < Start)
                return false;

            return true;
        }

        public (RangeL Left, RangeL Right) Split(long firstRightPosition)
        {
            var actualLast = Math.Min(Last, firstRightPosition - 1);
            var left = this with { Length = actualLast + 1 - Start };
            var right = new RangeL(actualLast + 1, Length - left.Length);
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


        var rangesToTry = new Stack<RangeAndLevel>();
        
        foreach (var seedRange in seedRanges)
        {
            rangesToTry.Push(new RangeAndLevel(seedRange, 0));
        }

        var min = long.MaxValue;
        while (rangesToTry.Count > 0)
        {
            var (range, level) = rangesToTry.Pop();
            if (level >= maps.Count)
            {
                if (range.Start < min)
                {
                    min = range.Start;
                }
            }
            else
            {
                var map = maps[level];
                foreach (var r in map.MapRange(range))
                {
                    rangesToTry.Push(new RangeAndLevel(r, level + 1));
                }
            }
        }
        
        return min;
    }

    private readonly record struct RangeAndLevel(RangeL Range, int Level)
    {
        public void Deconstruct(out RangeL range, out int level) => (range, level) = (Range, Level);
    }
}