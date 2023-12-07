using AdventOfCode.Framework;
using AdventOfCode.Utilities;

namespace AdventOfCode.Solutions.Y2023.Day07;

class Solution : ISolver
{
    public int Year => 2023;
    public int Day => 7;
    public string GetName() => "Camel Cards";

    public IEnumerable<object> Solve(string input, Func<TextWriter> getOutputFunction)
    {
        // var emptyOutput = () => new NullTextWriter();
        yield return PartOne(input, getOutputFunction);
        yield return PartTwo(input, getOutputFunction);
    }

    private record struct HandAndBid(Hand Hand, int Bid);

    static object PartOne(string input, Func<TextWriter> getOutputFunction)
    {
        var handAndBids = input
            .Lines()
            .Select(s => s.Split(" ").ToArray())
            .Select(a => new HandAndBid(new Hand(a[0]), int.Parse(a[1])))
            .OrderBy(hb => hb.Hand)
            .ToArray();

        var sum = 0;
        var multiplier = 1;
        foreach (var hb in handAndBids)
        {
            sum += multiplier * hb.Bid;
            multiplier++;
        }

        return sum;
    }

    static object PartTwo(string input, Func<TextWriter> getOutputFunction)
    {
        return 0;
    }

    private readonly record struct Hand : IComparable<Hand>
    {
        public Card[] Cards { get; }
        public HandType HandType { get; }
        
        public Hand(string input)
        {
            Cards = input.Select(c => new Card(c)).ToArray();
            var groupings = Cards.Select(c => c.Label).GroupBy(l => l).ToArray();
            var maxOfSingle = Cards.Select(c => c.Label).GroupBy(l => l).Max(g => g.Count());
            HandType = maxOfSingle switch
            {
                5 => HandType.FiveOfAKind,
                4 => HandType.FourOfAKind,
                3 => groupings.Length switch
                {
                    2 => HandType.FullHouse,
                    3 => HandType.ThreeOfAKind,
                    _ => throw new ArgumentException("Invalid hand type")
                },
                2 => groupings.Length switch
                {
                    3 => HandType.TwoPair,
                    4 => HandType.OnePair,
                    _ => throw new ArgumentException("Invalid hand type")
                }, 
                1 => HandType.HighCard,
                _ => throw new ArgumentException("Invalid hand type")
            };
        }

        public int CompareTo(Hand other)
        {
            var result = HandType.CompareTo(other.HandType);
            if (result != 0)
            {
                return result;
            }

            for (var i = 0; i < Cards.Length; i++)
            {
                result = Cards[i].Value.CompareTo(other.Cards[i].Value);
                if (result != 0)
                {
                    return result;
                }
            }

            return 0;
        }

        public override string ToString()
        {
            var labels = Cards.Select(c => c.Label).ToString();
            return $" {labels} ({HandType})";
        }
    }

    private record struct Card(char Label)
    {
        public readonly char Label = Label;

        public int Value
        {
            get
            {
                return Label switch
                {
                    'T' => 10,
                    'J' => 11,
                    'Q' => 12,
                    'K' => 13,
                    'A' => 14,
                    _ => Label - '0'
                };
            }
        }
    }
    
    private enum HandType
    {
        HighCard,
        OnePair,
        TwoPair,
        ThreeOfAKind,
        FullHouse,
        FourOfAKind,
        FiveOfAKind,
    }
}