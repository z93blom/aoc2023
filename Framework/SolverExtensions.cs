using System.Reflection;

namespace AdventOfCode.Framework;

public static class SolverExtensions
{
    public static string DayName(this ISolver solver)
    {
        return $"Day {solver.Day}";
    }

#pragma warning disable CS8602 // Dereference of a possibly null reference.

    private static int Year(Type t)
    {
        var namespaceParts = t.Namespace.Split('.');
        var yearPart = namespaceParts.FirstOrDefault(s => s[0] == 'Y' && s.Length == 5 && s[1..].All(char.IsDigit));
        var yearString = yearPart[1..];
        return int.Parse(yearString);
    }

    private static int Day(Type t)
    {
        var namespaceParts = t.Namespace.Split('.');
        var dayPart = namespaceParts.FirstOrDefault(s => s.StartsWith("Day") && s.Length == 5 && s[3..].All(char.IsDigit));
        var dayString = dayPart[3..];
        return int.Parse(dayString);
    }

    public static YearAndDay YearAndDay(Type t)
    {
        return new YearAndDay(Year: Year(t), Day: Day(t));
    }
    
#pragma warning restore CS8602 // Dereference of a possibly null reference.


    public static string WorkingDir(int year)
    {
        return Path.Combine("Solutions", $"Y{year}");
    }

    public static string WorkingDir(int year, int day)
    {
        return Path.Combine(WorkingDir(year), "Day" + day.ToString("00"));
    }

    public static string WorkingDir(this ISolver solver)
    {
        return WorkingDir(solver.Year, solver.Day);
    }

    public static SplashScreen SplashScreen(this ISolver solver)
    {
        var splashScreenType = solver.GetType().Assembly.GetTypes()
             .Where(t => t.GetTypeInfo().IsClass && !t.IsAbstract && typeof(SplashScreen).IsAssignableFrom(t))
             .Single(t => Year(t) == solver.Year);
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
#pragma warning disable CS8603 // Possible null reference return.
        return (SplashScreen)Activator.CreateInstance(splashScreenType);
#pragma warning restore CS8603 // Possible null reference return.
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
    }

    public static ITheme? Theme(int year)
    {
        var themeType = typeof(SolverExtensions).Assembly.GetTypes()
             .Where(t => t.GetTypeInfo().IsClass && !t.IsAbstract && typeof(ITheme).IsAssignableFrom(t))
             .SingleOrDefault(t => Year(t) == year);
        if (themeType == null)
        {
            return null;
        }

#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
        return (ITheme)Activator.CreateInstance(themeType);
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
    }
}
