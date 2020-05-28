using System;
using System.CommandLine;
using System.Threading.Tasks;

namespace NetBooru.MigrationTool
{
    public class Program
    {
        public static async Task<int> Main(string[] args)
        {
            var rootCommand = new RootCommand
            {
                Description = "NetBooru database tool"
            };

            rootCommand.AddCommand(MigrationCommand.Command);

            return await rootCommand.InvokeAsync(args);
        }
    }
}
