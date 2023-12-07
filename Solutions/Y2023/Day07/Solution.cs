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

    private record struct HandAndBid2(Hand2 Hand, int Bid);
    static object PartTwo(string input, Func<TextWriter> getOutputFunction)
    {
        var handAndBids = input
            .Lines()
            .Select(s => s.Split(" ").ToArray())
            .Select(a => new HandAndBid2(new Hand2(a[0]), int.Parse(a[1])))
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

    private readonly record struct Hand2 : IComparable<Hand2>
    {
        public Card2[] Cards { get; }
        public HandType HandType { get; }
        
        public Hand2(string input)
        {
            Cards = input.Select(c => new Card2(c)).ToArray();
            var cardsWithoutJokers = Cards.Where(c => c.Label != 'J').ToArray();
            var jokers = 5 - cardsWithoutJokers.Length;
            var groupings = cardsWithoutJokers.Length == 0 ? null : cardsWithoutJokers.Select(c => c.Label).GroupBy(l => l).ToArray();
            var maxOfSingle = cardsWithoutJokers.Length == 0 ? 0 : cardsWithoutJokers.Select(c => c.Label).GroupBy(l => l).Max(g => g.Count());

            HandType = jokers switch
            {
                0 => maxOfSingle switch
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
                },
                1 => maxOfSingle switch
                {
                    4 => HandType.FiveOfAKind,
                    3 => HandType.FourOfAKind,
                    2 => groupings.Length switch
                    {
                        2 => HandType.FullHouse,
                        3 => HandType.ThreeOfAKind,
                        _ => throw new ArgumentException("Invalid hand type")
                    },
                    1 => HandType.OnePair,
                    _ => throw new ArgumentException("Invalid hand type")
                },
                2 => maxOfSingle switch
                {
                    3 => HandType.FiveOfAKind,
                    2 => HandType.FourOfAKind,
                    1 => HandType.ThreeOfAKind,
                    _ => throw new ArgumentException("Invalid hand type")
                },
                3 => maxOfSingle switch
                {
                    2 => HandType.FiveOfAKind,
                    1 => HandType.FourOfAKind,
                    _ => throw new ArgumentException("Invalid hand type")
                },
                4 => HandType.FiveOfAKind,
                5 => HandType.FiveOfAKind,
                _ => throw new ArgumentException("Invalid hand type")
            };
        }

        public int CompareTo(Hand2 other)
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
    
    private readonly record struct Card2(char Label)
    {
        public readonly char Label = Label;

        public int Value
        {
            get
            {
                return Label switch
                {
                    'T' => 10,
                    'J' => 1,
                    'Q' => 12,
                    'K' => 13,
                    'A' => 14,
                    _ => Label - '0'
                };
            }
        }
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

    private readonly record struct Card(char Label)
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