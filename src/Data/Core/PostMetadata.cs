using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Dynamic;

namespace NetBooru.Data
{
    /// <summary>
    /// Represents a post's metadata
    /// </summary>
    public class PostMetadata
    {
        /// <summary>
        /// The post id this metadata applies to
        /// </summary>
        /// <value></value>
        public ulong Id { get; set; }

        /// <summary>
        /// The post this metadata applies to
        /// </summary>
        [ForeignKey(nameof(Id))]
        public virtual Post Post { get; set; } = null!;

        /// <summary>
        /// The size of the uploaded file, in bytes
        /// </summary>
        public uint Filesize { get; set; }

        /// <summary>
        /// The time this file was *initially* uploaded
        /// Should we come to support replacing images, we'll add UpdatedAt
        /// </summary>
        public DateTimeOffset UploadedAt { get; set; }

        /// <summary>
        /// The source URL of the post
        /// </summary>
        public string? Source { get; set; }

        /// <summary>
        /// True if locked to modification, false otherwise
        /// TODO: Once some sort of permissions scheme is implemented, make
        /// it such that administrators bypass this.
        /// </summary>
        public bool Locked { get; set; }

        /// <summary>
        /// The MIME type of the file
        /// </summary>
        public virtual string MimeType { get; set; } = null!;
    }
}
