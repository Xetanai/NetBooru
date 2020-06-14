using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace NetBooru.MagicTool
{
    public class MimeType
    {
        [JsonIgnore]
        public int Priority { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; } = null!;

        [JsonPropertyName("patterns")]
        public List<MimeTypePattern> Patterns { get; set; }
            = new List<MimeTypePattern>();
    }

    public class MimeTypePattern
    {
        [JsonPropertyName("offset")]
        public int OffsetIntoFile { get; set; }

        [JsonPropertyName("mask")]
        public byte[] Mask { get; set; } = null!;

        [JsonPropertyName("value")]
        public byte[] Value { get; set; } = null!;

        [JsonPropertyName("word_size")]
        public int WordSize { get; set; }

        [JsonPropertyName("range")]
        public int RangeLength { get; set; }

        [JsonPropertyName("children")]
        public List<MimeTypePattern> Children { get; set; }
            = new List<MimeTypePattern>();
    }
}
