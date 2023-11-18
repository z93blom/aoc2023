﻿using System.Diagnostics;
using System.Reflection;
using System.Text.RegularExpressions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using AdventOfCode.Framework;

var usageProvider = new ApplicationUsage();
var assemblies = new List<Assembly>
{
    typeof(Program).Assembly
};

var solverTypes = assemblies.SelectMany(a => a.GetTypes())
    .Where(t => t.GetTypeInfo().IsClass && typeof(ISolver).IsAssignableFrom(t))
    .OrderBy(t => t.FullName)
    .ToArray();

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((_, services) =>
    {
        var yearDaysWithSolvers = new List<YearAndDay>();
        foreach (var solverType in solverTypes)
        {
            var yearAndDay = SolverExtensions.YearAndDay(solverType);
            services.AddKeyedTransient<ISolver>(yearAndDay, (_, _) =>
            {
                var instance = Activator.CreateInstance(solverType);
                if (instance == null)
                {
                    throw new InvalidOperationException($"Unable to create an instance of ISolver for {yearAndDay.Year}-{yearAndDay.Day}");
                }

                return (ISolver)instance;
            });
            yearDaysWithSolvers.Add(yearAndDay);
        }

        services.AddSingleton<IResolver>(s => new Resolver(s, yearDaysWithSolvers));
    })
    .Build();

var solverResolver = host.Services.GetService<IResolver>();
Debug.Assert(solverResolver != null, nameof(solverResolver) + " != null");

var action =
    Command(args, Args("update", "([0-9]+)[/-]([0-9]+)"), m =>
    {
        var year = int.Parse(m[1]);
        var day = int.Parse(m[2]);
        return () => Updater.Update(year, day).Wait();
    }) ??
    Command(args, Args("update", "last"), _ =>
    {
        var now = DateTime.Now;
        if (now is { Month: 12, Day: >= 1 and <= 25 })
        {
            return () => Updater.Update(now.Year, now.Day).Wait();
        }
        else
        {
            throw new Exception("Event is not active. This option works in Dec 1-25 only)");
        }
    }) ??
    Command(args, Args("([0-9]+)[/-]([0-9]+)"), m =>
    {
         var year = int.Parse(m[0]);
         var day = int.Parse(m[1]);
         var solver = solverResolver.GetSolver(new YearAndDay(year, day));
         return () =>
         {
             if (solver != null)
             {
                 Runner.RunAll(solver);
             }
             else
             {
                 Console.WriteLine($"Unable to find a problem solver for {year}-{day}.");
             }
         };
    }) ??
    Command(args, Args("[0-9]+"), m =>
    {
        var year = int.Parse(m[0]);
        var solvers = solverResolver.GetSolvers(year).ToArray();
        return () => Runner.RunAll(solvers);
    }) ??
    Command(args, Args("([0-9]+)[/-]all"), m =>
    {
        var year = int.Parse(m[0]);
        var solvers = solverResolver.GetSolvers(year).ToArray();
        return () => Runner.RunAll(solvers);
    }) ??
    Command(args, Args("all"), _ =>
    {
        var solvers = solverResolver.GetAllSolvers().ToArray();
        return () => Runner.RunAll(solvers);
    }) ??
    Command(args, Args("last"), _ =>
    {
        var solver = solverResolver.GetLatestSolver();
        return () =>
        {
            if (solver != null)
            {
                Runner.RunAll(solver);
            }
            else
            {
                Console.WriteLine($"Unable to find a problem solver.");
            }
        };
    }) ??
    new Action(() =>
    {
        Console.WriteLine(usageProvider.Usage());
    });

action();

Action? Command(IReadOnlyCollection<string> args, IReadOnlyCollection<string> regularExpressions, Func<string[], Action> parse)
{
    if (args.Count != regularExpressions.Count)
    {
        return null;
    }
    var matches = args.Zip(regularExpressions, (arg, regex) => new Regex("^" + regex + "$").Match(arg)).ToArray();
    if (!matches.All(match => match.Success))
    {
        return null;
    }
    try
    {

        return parse(matches.SelectMany(m => m.Groups.Count > 1 ? m.Groups.Cast<Group>().Skip(1).Select(g => g.Value) : new[] { m.Value }).ToArray());
    }
    catch
    {
        return null;
    }
}

string[] Args(params string[] regex)
{
    return regex;
}
