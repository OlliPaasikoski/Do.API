using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Do.API.Entities;

namespace Do.API.Services
{
    public interface ITaskRepository
    {
        IEnumerable<Task> GetTasksForCategory(Guid categoryId);
        Task GetTaskForCategory(Guid taskId, Guid categoryId);
        void AddTaskToCategory(TaskCategory category, Task task);
        void DeleteTask(Task task);
        void UpdateTask(Task task);
        bool Save();
    }
}
