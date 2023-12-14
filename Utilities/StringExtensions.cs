using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Text.RegularExpressions;

namespace AdventOfCode.Utilities;

public static class StringExtensions
{
    private static readonly Regex _intRegex = new(@"([+-]?\d+)", RegexOptions.Compiled);

    private static readonly string[] LineEndings = { "\r\n", "\r", "\n" };
    private static readonly string[] DoubleLineEndings = { "\r\n\r\n", "\r\r", "\n\n" };

    public static IEnumerable<string> Lines(this string s, StringSplitOptions options = StringSplitOptions.RemoveEmptyEntries)
    {
        return SplitBySingleNewline(s, options);
    }

    public static IEnumerable<string> SplitBySingleNewline(this string s, StringSplitOptions options = StringSplitOptions.RemoveEmptyEntries)
    {
        return s.Split(LineEndings, options);
    }

    public static IEnumerable<string> SplitByDoubleNewline(this string s, StringSplitOptions options = StringSplitOptions.RemoveEmptyEntries)
    {
        return s.Split(DoubleLineEndings, options);
    }

    public static IEnumerable<int> Integers(this string s)
    {
        var matches = _intRegex.Matches(s);
        return matches.SelectMany(m => m.Captures.Select(v => int.Parse(v.Value)));
    }
    
    public static IEnumerable<long> Longs(this string s)
    {
        var matches = _intRegex.Matches(s);
        return matches.SelectMany(m => m.Captures.Select(v => long.Parse(v.Value)));
    }

    public static IEnumerable<int> ParseNumbers(this string t)
    {
        var position = 0;
        while (position < t.Length)
        {
            if (char.IsDigit(t[position]) || 
                t[position] == '-' && char.IsDigit(t[position + 1]))
            {
                var start = position;
                position += 1;
                while (position < t.Length && char.IsDigit(t[position]))
                {
                    position++;
                }

                yield return int.Parse(t[start..position]);
            }
            else
            {
                position++;
            }
        }
    }

    public static bool TryReadNested(this string t, char startCharacter, char endCharacter, int startIndex, out int start, out int end)
    {
        start = startIndex;
        while (start < t.Length)
        {
            if (t[start] == startCharacter)
            {
                break;
            }

            start++;
        }

        if (start >= t.Length)
        {
            end = t.Length;
            return false;
        }

        end = start;
        var nesting = 0;
        while (end < t.Length)
        {
            if (t[end] == startCharacter)
            {
                nesting++;
            }
            else if (t[end] == endCharacter)
            {
                nesting--;
                if (nesting == 0)
                {
                    break;
                }
            }

            end++;
        }

        if (end < t.Length)
        {
            return true;
        }

        return false;
    }

    public static string Replace(this string text, int index, int length, string replacement)
    {
        return new StringBuilder()
            .Append(text[..index])
            .Append(replacement)
            .Append(text.AsSpan(index + length))
            .ToString();
    }

    public static string ReplaceAll(this MatchCollection matches, string source, string replacement)
    {
        return matches.Aggregate(source, (current, match) => match.Replace(current, replacement));
    }

    public static string Replace(this Match match, string source, string replacement)
    {
        return string.Concat(source.AsSpan(0, match.Index), replacement, source.AsSpan(match.Index + match.Length));
    }

    public static string Slice(this string s, int startIndex, int endIndex)
    {
        return s[startIndex..endIndex];
    }

    /// <summary>
    /// Tries to apply the regular expression on the string, and returns the captured groups from it.
    /// </summary>
    /// <param name="s"></param>
    /// <param name="pattern"></param>
    /// <param name="groups"></param>
    /// <returns></returns>
    public static bool TryMatch(this string s, [StringSyntax(StringSyntaxAttribute.Regex)] string pattern, out Group[] groups)
    {
        var match = Regex.Match(s, pattern);
        // Take all the groups from the result, but skip the first one (the one that includes the whole string).
        groups = match.Groups.Cast<Group>().Skip(1).ToArray();
        return match.Success;
    }

    public static IEnumerable<Match> Matches(this string s, string pattern)
    {
        var collection = Regex.Matches(s, pattern);
        return collection;
    }

    /// <summary>
    /// Returns all the captured groups for the indicated pattern. Will only return the groups indicated by parenthesis.
    /// </summary>
    /// <param name="s">The string to apply the regular expression search pattern to.</param>
    /// <param name="pattern">The regular expression search pattern.</param>
    /// <returns></returns>
    public static IEnumerable<Group[]> Groups(this string s, [StringSyntax(StringSyntaxAttribute.Regex)] string pattern)
    {
        var collection = Regex.Matches(s, pattern);
        foreach (Match match in collection)
        {
            yield return match.Groups.Cast<Group>().Skip(1).ToArray();
        }
    }


    /// <summary>
    /// Returns the groups from a regex (the values inside of captured parenthesis), applied to each string in turn.
    /// </summary>
    /// <param name="strings">The list of strings to apply the regex pattern to.</param>
    /// <param name="pattern">The regular expression pattern.</param>
    /// <returns>The list of groups for each string.</returns>
    public static IEnumerable<Group[]> Groups(this IEnumerable<string> strings, [StringSyntax(StringSyntaxAttribute.Regex)] string pattern)
    {
        foreach (var s in strings)
        {
            if (s.TryMatch(pattern, out var groups))
            {
                yield return groups.ToArray();
            }
        }
    }
    
    /// <summary>
    /// Removes all occurrences of a character from a string.
    /// </summary>
    /// <param name="s">The string being changed.</param>
    /// <param name="c">The character to be removed.</param>
    /// <returns>The resulting string.</returns>
    public static string RemoveChar(this string s, char c)
    {
        return s.Replace(new string(c, 1), string.Empty);
    }

    /// <summary>
    /// Removes all occurrences of the characters from a string.
    /// </summary>
    /// <param name="s">The string being changed.</param>
    /// <param name="characters">The characters to be removed from the string.</param>
    /// <returns>The resulting string.</returns>
    public static string RemoveAll(this string s, char[] characters)
    {
        foreach(var c in characters)
        {
            s = s.RemoveChar(c);
        }

        return s;
    }

    /// <summary>
    /// Removes all characters from a string.
    /// </summary>
    /// <param name="s">The string being changed.</param>
    /// <param name="c">A string containing all the characters to be removed..</param>
    /// <returns>The resulting string.</returns>
    public static string RemoveAllChars(this string s, string remove)
    {
        foreach (var c in remove)
        {
            s = s.RemoveChar(c);
        }

        return s;
    }
        
    public static string Indent(this string st, int l)
    {
        return string.Join("\n" + new string(' ', l),
            from line in st.Split('\n')
            select Regex.Replace(line, @"^\s*\|", "")
        );
    }

    /// <summary>
    /// Creates a grid from the (input) string.
    /// </summary>
    /// <param name="s">The (input) string.</param>
    /// <param name="yAxisDirection"></param>
    /// <param name="f">The function to translate from a char to the grid type.</param>
    /// <typeparam name="T">The type that each point in the grid will have.</typeparam>
    /// <returns>The parsed grid.</returns>
    public static Grid<T> ToGrid<T>(this string s, YAxisDirection yAxisDirection, Func<char, T> f)
    {
        var lines = s.SplitBySingleNewline().ToArray();
        var grid = new Grid<T>(lines[0].Length, lines.Length, yAxisDirection);
        for (var y = 0; y < grid.Height; y++)
        {
            for (var x = 0; x < grid.Width; x++)
            {
                grid[x, y] = f(lines[y][x]);
            }
        }

        return grid;
    }

}