using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Do.API.Entities;
using Do.API.Helpers;

namespace Do.API.Services
{
    public class TaskCategoryRepository : ITaskCategoryRepository
    {
        private DoContext _context;

        public TaskCategoryRepository(DoContext context)
        {
            _context = context;
        }

        public void AddCategory(TaskCategory category)
        {
            category.Id = Guid.NewGuid();
            _context.TaskCategories.Add(category);

            if (category.Tasks.Any())
            {
                foreach (var task in category.Tasks)
                {
                    task.Id = Guid.NewGuid();
                }
            }
        }

        public bool CategoryExists(Guid categoryId)
        {
            return _context.TaskCategories.Any(c => c.Id == categoryId);
        }

        public void DeleteCategory(TaskCategory category)
        {
            _context.TaskCategories.Remove(category);
        }

        public PagedList<TaskCategory> GetCategories(CategoriesResourceParameters categoriesResourceParameters)
        {
            var collectionBeforePaging = _context.TaskCategories
                                            .OrderBy(tc => tc.Title)
                                            .AsQueryable();


            // TO DO: Filtering by fields, no suitable field in categories atm

            // Searching

            if (!string.IsNullOrEmpty(categoriesResourceParameters.SearchQuery))
            {
                var searchQueryForWhereClause = categoriesResourceParameters.SearchQuery
                    .Trim().ToLowerInvariant();

                collectionBeforePaging = collectionBeforePaging
                    .Where(a => a.Title.Contains(searchQueryForWhereClause)
                    || a.Description.Contains(searchQueryForWhereClause));
            }

            return PagedList<TaskCategory>.Create(collectionBeforePaging, 
                categoriesResourceParameters.PageNumber, 
                categoriesResourceParameters.PageSize);
        }

        public IEnumerable<TaskCategory> GetCategories(IEnumerable<Guid> categoryIds)
        {
            return _context.TaskCategories.Where(c => categoryIds.Contains(c.Id))
                .OrderBy(c => c.Title)
                .ToList();
        }

        public TaskCategory GetCategory(Guid categoryId)
        {
            return _context.TaskCategories.FirstOrDefault(tc => tc.Id == categoryId);
        }

        public void UpdateCategory(TaskCategory category)
        {
            throw new NotImplementedException();
        }

        public bool Save()
        {
            return (_context.SaveChanges() >= 0);
        }


    }
}
