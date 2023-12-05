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

        public IEnumerable<RangeLAndInfo> MapDestinationRangeToSource(RangeL range)
        {
            if (range.Start < DestinationRange.Start && range.Last > DestinationRange.Last)
            {
                // Three ranges - left, mapped, right
                var (left, _) = range.Split(DestinationRange.Start);
                yield return new RangeLAndInfo(left, true);

                yield return new RangeLAndInfo(Source, false);
                
                var (_, right) = range.Split(DestinationRange.Last + 1);
                yield return new RangeLAndInfo(right, true);
            }
            else if (range.Start < DestinationRange.Start)
            {
                // Two - left and mapped
                var (left, _) = range.Split(DestinationRange.Start);
                yield return new RangeLAndInfo(left, true);
                yield return new RangeLAndInfo(new RangeL(MapDestinationToSource(DestinationRange.Start), range.Length - left.Length), false);
            }
            else if (range.Last < DestinationRange.Last)
            {
                // Two - mapped and right
                var (_, right) = range.Split(DestinationRange.Last + 1);
                yield return new RangeLAndInfo(new RangeL(MapDestinationToSource(range.Start), range.Length - right.Length), false);
                yield return new RangeLAndInfo(right, true);;
            }
            else
            {
                // completely within
                yield return new RangeLAndInfo(range with { Start = MapDestinationToSource(range.Start) }, false);
            }
        }

        private long MapDestinationToSource(long position)
        {
            return position - Destination + Source.Start;
        }
    }

    private readonly record struct RangeLAndInfo(RangeL Range, bool IsFallthrough);

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
        
        // Start from the end, work your way back up to the top, and see if there are any seeds that fall into
        // the correct bucket.

        var toTry = new Stack<RangeAndLevel>();
        foreach (var mapper in maps[^1].Mappers.Reverse())
        {
            toTry.Push(new RangeAndLevel(mapper.Source, 5));
        }
        
        if (maps[^1].Mappers[0].Destination != 0)
        {
            toTry.Push(new RangeAndLevel(new RangeL(0, maps[^1].Mappers[0].Destination), 5));
        }

        var startingSeed = -1L;
        while (toTry.Count > 0)
        {
            var rl = toTry.Pop();

            if (rl.Level < 0)
            {
                // try the seeds.
                var seedRangesThatIntersect = seedRanges
                    .Where(r => rl.Range.Intersects(r))
                    .OrderBy(r => r.Start)
                    .ToArray();
                if (seedRangesThatIntersect.Length > 0)
                {
                    var intersection = rl.Range.Intersection(seedRangesThatIntersect[0]);
                    startingSeed = intersection.Start;
                    break;
                }
            }
            else
            {
                // map another level up.
                var mappers = maps[rl.Level].Mappers
                    .Where(m => rl.Range.Intersects(m.DestinationRange))
                    .OrderByDescending(m => m.Destination)
                    .ToArray();
                if (mappers.Length == 0)
                {
                    // No mappers on this level intersect the current destination range.
                    // Just go up a level.
                    toTry.Push(rl with { Level = rl.Level - 1 });
                }
                else
                {
                    foreach (var mapper in mappers)
                    {
                        // Here's a bug - we can only map a value at most once. Right now we're
                        // taking all ranges from all mappers, even those that might be mapped
                        // with another mapper.
                        foreach (var sr in mapper.MapDestinationRangeToSource(rl.Range).Reverse())
                        {
                            toTry.Push(new RangeAndLevel(sr.Range, rl.Level - 1));
                        }
                    }
                }
            }
        }

        if (startingSeed < 0)
        {
            throw new Exception("Unable to solve this.");
        }
        
        // Get the final location for the seed.
        var final = maps.Aggregate(startingSeed, (s, map) => map.MapSeed(s));
        return final;
    }

    private readonly record struct RangeAndLevel(RangeL Range, int Level);
}