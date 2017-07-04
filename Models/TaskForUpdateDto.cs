using System.ComponentModel.DataAnnotations;

namespace Do.API.Models
{
    public class TaskForUpdateDto : TaskForManipulationDto
    {
        [Required(ErrorMessage = "You should fill out a description")]
        public override string Description
        {
            get => base.Description;
            set => base.Description = value;
        }
    }
}
