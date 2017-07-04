using System;
using System.ComponentModel.DataAnnotations;

namespace Do.API.Models
{
    public abstract class TaskForManipulationDto
    {
        [Required(ErrorMessage = "You should fill out a title")]
        [MaxLength(100, ErrorMessage = "The title should not have more that 100 characters")]
        public string Title { get; set; }
        [MaxLength(500, ErrorMessage = "The description should not have more than 500 characters")]
        public virtual string Description { get; set; }
        [Required]
        public DateTimeOffset Date { get; set; }
        [MaxLength(500, ErrorMessage = "The image url should not have more than 500 characters")]
        public string ImageUrl { get; set; }
    }
}
