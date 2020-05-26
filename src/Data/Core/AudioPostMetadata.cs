using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetBooru.Data
{
    /// <summary>
    /// Extended metadata which applies only to audio
    /// </summary>
    public class AudioPostMetadata : PostMetadata
    {
        /// <summary>
        /// Duration of the audio
        /// </summary>
        public TimeSpan length { get; set; }
    }
}
