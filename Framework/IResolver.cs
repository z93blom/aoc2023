namespace AdventOfCode.Framework;

interface IResolver
{
    ISolver? GetSolvers(int year, int day);

    IEnumerable<ISolver> GetSolvers(int year);

    IEnumerable<ISolver> GetAllSolvers();

    ISolver? GetLatestSolver();
}
