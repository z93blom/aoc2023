namespace AdventOfCode.Y2023;

class Theme : ITheme
{
    public Dictionary<string, int> Override(Dictionary<string, int> themeColors)
    {
        return themeColors;
    }
}