using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Do.API.Models
{
    public class TaskCategoryForCreationDto
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }
        public ICollection<TaskForCreationDto> Tasks { get; set; }
            = new List<TaskForCreationDto>();          
    }
}
