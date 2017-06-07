using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Do.API.Entities
{
    public class BlogPost
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Title { get; set; }
        [MaxLength(500)]
        public string Content { get; set; }
        [MaxLength(500)]
        public string ImageUrl { get; set; }
        [Required]
        public DateTimeOffset Date { get; set; }
        //public ICollection<Task> RelatedTasks { get; set; }
        //    = new List<Task>();
    }

    
    public class BlogPostTask
    {
        public Guid BlogPostId { get; set; }
        public Guid TaskId { get; set; }
        public BlogPost BlogPost { get; set; }
        public Task Task { get; set; }
    }
    
}
