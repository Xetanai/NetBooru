using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Buildalyzer;

namespace NetBooru.MigrationTool
{
    public class MigrationRemoveCommand : ICommandHandler
    {
        static MigrationRemoveCommand()
        {
            Command = new Command("remove",
                "Remove migrations from NetBooru database providers")
            {
                Handler = new MigrationRemoveCommand()
            };

            Command.Add(SolutionArgument);
        }

        public static Command Command { get; }
        private static Argument SolutionArgument { get; }
            = new Argument<string>("solution")
            {
                Arity = ArgumentArity.ExactlyOne
            };

        public async Task<int> InvokeAsync(InvocationContext context)
        {
            var solutionPath = string.Join(" ",
                context.ParseResult
                    .FindResultFor(SolutionArgument)
                    .Tokens
                    .Select(x => x.Value));

            var manager = new AnalyzerManager(solutionPath);;

            foreach (var project in manager.Projects)
            {
                var results = project.Value.Build();

                foreach (var result in results)
                {
                    if (!bool.TryParse(
                        result.GetProperty("IsDatabaseProvider"),
                        out var isDatabaseProvider))
                        continue;
                    if (!bool.TryParse(
                        result.GetProperty("DatabaseSupportsMigration"),
                        out var databaseSupportsMigration))
                        continue;
                    if (!(isDatabaseProvider && databaseSupportsMigration))
                        continue;

                    var objFolder = result.GetProperty(
                        "BaseIntermediateOutputPath");
                    var assemblyName = result.GetProperty("AssemblyName");

                    var args =
                        $"ef migrations remove " +
                        $"-p {result.ProjectFilePath} " +
                        $"--msbuildprojectextensionspath {objFolder} " +
                        "--json --prefix-output --no-color";

                    using var process = Process.StartProcess(
                        "dotnet", args,
                        workingDir: Path.GetDirectoryName(
                            result.ProjectFilePath),
                        stdOut: x => ProcessStdout(x),
                        stdErr: x => Console.WriteLine(x));

                    var dotnetEfResult = await process.CompleteAsync();

                    if (dotnetEfResult != 0)
                        return dotnetEfResult;
                }
            }

            Console.WriteLine("OK");

            return 0;

            static void ProcessStdout(string output)
            {
                if (output.StartsWith("error:"))
                {
                    Console.WriteLine(output);
                }
            }
        }
    }
}
