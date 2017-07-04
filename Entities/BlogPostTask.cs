using System;

namespace Do.API.Entities
{
    public class BlogPostTask
    {
        public Guid BlogPostId { get; set; }
        public Guid TaskId { get; set; }
        public BlogPost BlogPost { get; set; }
        public Task Task { get; set; }
    }
    
}
