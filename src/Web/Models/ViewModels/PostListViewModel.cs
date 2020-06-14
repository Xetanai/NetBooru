using System.Collections.Generic;

namespace NetBooru.Web.Models
{
    public class PostListViewModel
    {
        public string? Query { get; set; }
        public List<PostListPost> Posts { get; set; } = null!;

        public class PostListPost
        {
            public string PostUrl { get; set; } = null!;
            public string ThumbnailUrl { get; set; } = null!;
        }
    }
}
