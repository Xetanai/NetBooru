using System;
using System.Buffers;
using System.Buffers.Binary;
using System.Buffers.Text;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.IO.Pipelines;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace NetBooru.MagicTool
{
    public class MagicCommand : ICommandHandler
    {
        public static async Task<int> Main(string[] args)
        {
            var rootCommand = new RootCommand
            {
                Description = "NetBooru magic(5) tool"
            };

            rootCommand.AddOption(
                new Option<FileInfo>("-i", "MIME magic file")
                {
                    Required = true
                });
            rootCommand.AddOption(
                new Option<FileInfo>("-o", "generated C# file")
                {
                    Required = true
                });
            rootCommand.AddOption(new Option<bool>("-v", "verbose"));

            rootCommand.Handler = new MagicCommand();

            return await rootCommand.InvokeAsync(args);
        }

        public async Task<int> InvokeAsync(InvocationContext context)
        {
            var input = context.ParseResult.ValueForOption<FileInfo>("-i");
            var output = context.ParseResult.ValueForOption<FileInfo>("-o");

            if (context.ParseResult.ValueForOption<bool>("-v"))
                _ = Trace.Listeners.Add(new ConsoleTraceListener());

            try
            {
                using var stream = output.Create();

                var mimeTypes = new List<MimeType>();

                await foreach (var pattern in ParseFileAsync(input))
                {
                    Debug.WriteLine($"{pattern.Name} ({pattern.Priority})");
                    PrintTree(pattern.Patterns);

                    mimeTypes.Add(pattern);
                }

                mimeTypes = mimeTypes
                    .OrderByDescending(x => x.Priority)
                    .ToList();

                await JsonSerializer.SerializeAsync(stream, mimeTypes);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Failed to parse input: {e.Message}");
                Debug.WriteLine(e);
                return 1;
            }

            return 0;

            static void PrintTree(List<MimeTypePattern> nodes)
            {
                Debug.Indent();
                foreach (var node in nodes)
                {
                    var printable = string.Join("", node.Value
                        .Select(x => (char)x)
                        .Select(PrintableCharacter));

                    Debug.WriteLine(
                        $"{node.OffsetIntoFile}(+-{node.RangeLength}) - " +
                        $"{BitConverter.ToString(node.Value)} " +
                        $"({printable})");

                    PrintTree(node.Children);
                }
                Debug.Unindent();

                static char PrintableCharacter(char input)
                {
                    return char.IsControl(input)
                        || char.IsWhiteSpace(input)
                        ? '.'
                        : input;
                }
            }
        }

        private async IAsyncEnumerable<MimeType> ParseFileAsync(FileInfo input)
        {
            using var stream = input.OpenRead();
            var reader = PipeReader.Create(stream);

            ReadResult result;

            var isFileStart = true;
            while (true)
            {
                result = await reader.ReadAsync();

                var buffer = result.Buffer;

                while (true)
                {
                    if (!Reader.TryRead(isFileStart, ref buffer, out var value))
                        break;

                    if (!Reader.TryProcess(ref isFileStart, value,
                        out var mimeType))
                        throw new Exception("Invalid MIME magic file");

                    if (mimeType != null)
                        yield return mimeType;
                }

                reader.AdvanceTo(buffer.Start, buffer.End);

                if (buffer.IsEmpty && result.IsCompleted)
                {
                    reader.Complete();
                    break;
                }
            }
        }
    }
}
