using System;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.Threading.Tasks;

namespace NetBooru.MigrationTool
{
    public class MigrationCommand
    {
        public static Command Command { get; } =
            new Command("migrations",
                "Migration subcommands for NetBooru database providers")
            {
                MigrationAddCommand.Command,
                MigrationListCommand.Command,
                MigrationRemoveCommand.Command,
            };
    }
}
