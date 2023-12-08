namespace AdventOfCode.Y2023;

class Theme : ITheme
{
    public Dictionary<string, int> Override(Dictionary<string, int> themeColors)
    {
        themeColors["calendar-color-n"] = 0x9b715b;
        themeColors["calendar-color-g"] = 0x00cc00;
        themeColors["calendar-color-k"] = 0x6b4d3b;
        themeColors["calendar-color-s"] = 0xe3b585;
        return themeColors;
    }
}