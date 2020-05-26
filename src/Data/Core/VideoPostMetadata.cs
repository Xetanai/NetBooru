using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetBooru.Data
{
    /// <summary>
    /// Extended metadata which applies only to Videos
    /// </summary>
    public class VideoPostMetadata : PostMetadata
    {
        /// <summary>
        /// Width of the video
        /// </summary>
        public int Width { get; set; }

        /// <summary>
        /// Height of the video
        /// </summary>
        public int Height { get; set; }

        /// <summary>
        /// Length of the video
        /// </summary>
        public TimeSpan Length { get; set; }
    }
}
