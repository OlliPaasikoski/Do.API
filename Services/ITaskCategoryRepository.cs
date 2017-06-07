using Do.API.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Do.API.Services
{
    public interface ITaskCategoryRepository
    {
        IEnumerable<TaskCategory> GetCategories();
        IEnumerable<TaskCategory> GetCategories(IEnumerable<Guid> categoryIds);
        TaskCategory GetCategory(Guid categoryId);
        bool CategoryExists(Guid categoryId);
        void AddCategory(TaskCategory category);
        void UpdateCategory(TaskCategory category);
        void DeleteCategory(TaskCategory category);
        bool Save();
    }
}
