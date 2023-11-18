using AdventOfCode.Generator;

namespace AdventOfCode.Framework;

public class ApplicationUsage : IUsageProvider
{
    public string Usage() => """
               Usage: dotnet run [arguments]
               Supported arguments:

                [year]-[day|all] Solve the specified problems
                [year]                Solve the whole year
                last                  Solve the last problem
                all                   Solve everything
                list                  List possible solvers

               To start working on new problems:
               
                update [year]-[day]   Prepares a folder for the given day, updates the input, 
                                      the readme and creates a solution template.
                update last           Same as above, but for the current day. Works in December only.  
                
               Useful commands during december:
                 dotnet run update last
                 dotnet run last 
               
               """;
}
