using System;

namespace Do.API.Models
{
    public class TaskForCreationDto
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTimeOffset Date { get; set; }
        public string ImageUrl { get; set; }
        // public Guid CategoryId { get; set; }
    }
}