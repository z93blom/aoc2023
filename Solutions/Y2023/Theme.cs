namespace AdventOfCode.Y2023;

class Theme : ITheme
{
    public Dictionary<string, int> Override(Dictionary<string, int> themeColors)
    {
        themeColors["calendar-color-n"] = 0x9b715b;
        return themeColors;
    }
}