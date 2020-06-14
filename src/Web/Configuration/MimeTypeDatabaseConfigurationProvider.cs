using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Configuration;

namespace NetBooru.Web.Configuration
{
    public class MimeTypeDatabaseConfigurationProvider
        : FileConfigurationProvider
    {
        private readonly string[] _basePath;

        public MimeTypeDatabaseConfigurationProvider(
            FileConfigurationSource source,
            string[] basePath)
            : base(source)
        {
            _basePath = basePath;
        }

        public override void Load(Stream stream)
        {
            var options = new JsonSerializerOptions
            {
                ReadCommentHandling = JsonCommentHandling.Skip,
                AllowTrailingCommas = true
            };

            Data = new SortedDictionary<string, string>();
            var pathStack = new Stack<string>();

            foreach (var component in _basePath)
                pathStack.Push(component);

            RawMimeType[] types;

            try
            {
                using var reader = new StreamReader(stream);
                types = JsonSerializer.Deserialize<RawMimeType[]>(
                    reader.ReadToEnd(), options)!;
            }
            catch
            {
                throw new FormatException("Failed to parse JSON");
            }

            if (types == null)
                throw new FormatException("Invalid MIME type database");

            for (int x = 0; x < types.Length; x++)
            {
                pathStack.Push(x.ToString());

                AddValue("Name", types[x].Name);

                pathStack.Push("Patterns");
                AddPatterns(types[x].Patterns);
                _ = pathStack.Pop();

                _ = pathStack.Pop();
            }

            void AddPatterns(RawMimeTypePattern[] patterns)
            {
                for (int x = 0; x < patterns.Length; x++)
                {
                    pathStack.Push(x.ToString());

                    var pattern = patterns[x];

                    AddValue("OffsetIntoFile",
                        pattern.OffsetIntoFile.ToString());

                    AddValue("Mask", pattern.Mask);
                    AddValue("Value", pattern.Value);
                    AddValue("WordSize",
                        pattern.WordSize.ToString());
                    AddValue("RangeLength",
                        pattern.RangeLength.ToString());

                    pathStack.Push("Children");
                    AddPatterns(pattern.Children);
                    _ = pathStack.Pop();

                    _ = pathStack.Pop();
                }
            }

            void AddValue(string key, string value)
            {
                pathStack.Push(key);

                var fullPath = ConfigurationPath.Combine(pathStack.Reverse());

                Data[fullPath] = value;

                _ = pathStack.Pop();
            }
        }

        private class RawMimeType
        {
            [JsonPropertyName("name")]
            public string Name { get; set; } = null!;

            [JsonPropertyName("patterns")]
            public RawMimeTypePattern[] Patterns { get; set; } = null!;
        }

        private class RawMimeTypePattern
        {
            [JsonPropertyName("offset")]
            public int OffsetIntoFile { get; set; }

            [JsonPropertyName("mask")]
            public string Mask { get; set; } = null!;

            [JsonPropertyName("value")]
            public string Value { get; set; } = null!;

            [JsonPropertyName("word_size")]
            public int WordSize { get; set; }

            [JsonPropertyName("range")]
            public int RangeLength { get; set; }

            [JsonPropertyName("children")]
            public RawMimeTypePattern[] Children { get; set; } = null!;
        }
    }
}
