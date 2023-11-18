using Microsoft.Extensions.DependencyInjection;

namespace AdventOfCode.Framework;

class Resolver : IResolver
{
    private readonly IServiceProvider _services;
    private readonly YearAndDay[] _solverDays;

    public Resolver(IServiceProvider services, IEnumerable<YearAndDay> solverDays)
    {
        _services = services;
        _solverDays = solverDays.ToArray();
    }

    public ISolver? GetSolver(YearAndDay yearDay)
    {
        var solver = _services.GetKeyedService<ISolver>(yearDay);
        return solver;
    }

    public IEnumerable<ISolver> GetSolvers(int year)
    {
        foreach (var yd in 
                 _solverDays.Where(yd => yd.Year == year)
                     .OrderBy(yd => yd.Day))
        {
            var solver = GetSolver(yd);
            if (solver != null)
            {
                yield return solver;
            }
        }
    }
    public IEnumerable<ISolver> GetAllSolvers()
    {
        return _services.GetServices<ISolver>();
    }

    public ISolver? GetLatestSolver()
    {
        var yd = _solverDays
            .OrderBy(yd => yd.Year)
            .ThenBy(yd => yd.Day)
            .LastOrDefault();

        return GetSolver(yd);
    }

    public IEnumerable<YearAndDay> GetPossibleDays()
    {
        return _solverDays;
    }
}
