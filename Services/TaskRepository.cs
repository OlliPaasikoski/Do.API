using Do.API.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Do.API.Services
{
    public class TaskRepository : ITaskRepository
    {
        private DoContext _context;
        public TaskRepository(DoContext context)
        {
            _context = context;
        }

        public Task GetTaskForCategory(Guid taskId, Guid categoryId)
        {
            // old implementation for any task
            // return _context.Tasks.FirstOrDefault(t => t.Id == taskId);

            return _context.Tasks
                .Where(t => t.Id == taskId && t.CategoryId == categoryId).FirstOrDefault();
        }

        public IEnumerable<Task> GetTasksForCategory(Guid categoryId)
        {
            return _context.Tasks
                .Where(t => t.CategoryId == categoryId)
                .OrderBy(t => t.Date).ThenBy(t => t.Title).ToList();
        }

        public void AddTaskToCategory(TaskCategory category, Task task)
        {
            if (task.Id == null)
            {
                task.Id = Guid.NewGuid();
            }
            category.Tasks.Add(task);
        }

        public void DeleteTask(Task task)
        {
            _context.Tasks.Remove(task);
        }

        public void UpdateTask(Task task)
        {
            // no implementation because EF takes care of this for us!
            // remember to call this anyway in case we are mocking the 
            // repository in a test project
        }

        public bool Save()
        {
            return (_context.SaveChanges() >= 0);
        }
    }
}
