namespace AdventOfCode.Utilities;

public static class AocMath
{
    public static double gcd(double a, double b)
    {
        if (a == 0 || b == 0) return Math.Max(a, b);
        return (a % b == 0) ? b : gcd(b, a % b);
    }

    public static double lcm(double a, double b) => a * b / gcd(a, b);
}