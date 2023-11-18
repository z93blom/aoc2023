using Microsoft.Extensions.DependencyInjection;

namespace AdventOfCode.Framework;

class Resolver : IResolver
{
    private readonly IServiceProvider _services;
    private readonly Type[] _solverTypes;

    public Resolver(IServiceProvider services, Type[] solverTypes)
    {
        _services = services;
        _solverTypes = solverTypes;
    }

    public ISolver? GetSolvers(int year, int day)
    {
        var yearDay = new YearAndDay(year, day);
        var solver = _services.GetKeyedService<ISolver>(yearDay);
        return solver;
    }

    public IEnumerable<ISolver> GetSolvers(int year)
    {
        foreach (var day in Enumerable.Range(1, 25))
        {
            var solver = GetSolvers(year, day);
            if (solver != null)
            {
                yield return solver;
            }
        }
    }
    public IEnumerable<ISolver> GetAllSolvers()
    {
        var solvers = _services.GetServices(typeof(ISolver))
            .Where(s => s != null)
            .Cast<ISolver>();
        return solvers;
    }

    public ISolver? GetLatestSolver()
    {
        var solverType = _solverTypes
            .OrderBy(SolverExtensions.Year)
            .ThenBy(SolverExtensions.Day)
            .LastOrDefault();

        if (solverType == null)
        {
            return null;
        }

        var solver = _services.GetService(solverType) as ISolver;
        return solver;
    }
}
