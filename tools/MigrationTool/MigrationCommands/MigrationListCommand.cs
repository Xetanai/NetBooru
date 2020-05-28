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
    public class MigrationListCommand : ICommandHandler
    {
        static MigrationListCommand()
        {
            Command = new Command("list",
                "List migrations for NetBooru database providers")
            {
                Handler = new MigrationListCommand()
            };

            Command.Add(SolutionArgument);
        }

        public static Command Command { get; }
        public static Argument SolutionArgument { get; }
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

                    var jsonOutput = new List<string>();
                    var args = "ef migrations list " +
                        $"-p {result.ProjectFilePath} " +
                        $"--msbuildprojectextensionspath {objFolder} " +
                        "--json --prefix-output --no-color";

                    using var process = Process.StartProcess(
                        "dotnet", args,
                        workingDir: Path.GetDirectoryName(
                            result.ProjectFilePath),
                        stdOut: (x) => ProcessStdout(jsonOutput, x));

                    var dotnetEfResult = await process.CompleteAsync();

                    if (dotnetEfResult != 0)
                        return dotnetEfResult;

                    var json = string.Join("", jsonOutput);

                    var migrations = JsonSerializer
                        .Deserialize<List<Migration>>(json)!;

                    foreach (var migration in migrations)
                    {
                        Console.WriteLine(
                            $"{assemblyName}: {migration.Name} " +
                            $"({migration.Id})");
                    }
                }
            }

            return 0;

            static void ProcessStdout(List<string> migrations, string output)
            {
                if (output.StartsWith("data:"))
                {
                    var json = output.Substring(5).TrimStart();
                    migrations.Add(json);
                }
            }
        }

        private class Migration
        {
            [JsonPropertyName("id")]
            public string Id { get; set; } = null!;

            [JsonPropertyName("name")]
            public string Name { get; set; } = null!;

            [JsonPropertyName("safeName")]
            public string SafeName { get; set; } = null!;
        }
    }
}
