namespace AdventOfCode.Framework;

interface IResolver
{
    ISolver? GetSolver(YearAndDay yd);

    IEnumerable<ISolver> GetSolvers(int year);

    IEnumerable<ISolver> GetAllSolvers();

    ISolver? GetLatestSolver();

    IEnumerable<YearAndDay> GetPossibleDays();
}
