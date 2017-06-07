using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Do.API.Entities
{
    public class Task
    {
        [Key]
        public Guid Id { get; set; }
        [Required]
        [MaxLength(100)]
        public string Title { get; set; }
        [MaxLength(500)]
        public string Description { get; set; }
        [MaxLength(500)]
        public string ImageUrl { get; set; }
        [Required]
        public DateTimeOffset Date { get; set; }
        [Required]
        public bool Success { get; set; }
        [ForeignKey("CategoryId")]
        public TaskCategory Category { get; set; }
        public Guid CategoryId { get; set; }
    }
}
