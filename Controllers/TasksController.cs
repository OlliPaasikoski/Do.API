using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using Do.API.Services;
using Do.API.Models;
using Do.API.Entities;

namespace Do.API.Controllers
{
    [Route("api/categories/{categoryId}/tasks")]
    public class TasksController : Controller
    {
        private ITaskRepository _taskRepository;
        private ITaskCategoryRepository _categoryRepository;

        public TasksController(ITaskRepository taskRepository, ITaskCategoryRepository categoryRepository)
        {
            _taskRepository = taskRepository;
            _categoryRepository = categoryRepository;
        }

        // GET api/categories/{categoryId}/tasks
        [HttpGet()]
        public IActionResult GetTasks(Guid categoryId)
        {
            if (!_categoryRepository.CategoryExists(categoryId))
            {
                return NotFound();
            }

            var tasksForCategoryFromRepo = _taskRepository.GetTasksForCategory(categoryId);

            var tasksForCategory = Mapper.Map<IEnumerable<TaskDto>>(tasksForCategoryFromRepo);
            return Ok(tasksForCategory);
        }

        // GET api/categories/{categoryId}/tasks/{taskId}
        [HttpGet("{id}", Name = "GetTask")]
        public IActionResult GetTask(Guid categoryId, Guid id)
        {
            if (!_categoryRepository.CategoryExists(categoryId))
            {
                return NotFound();
            }

            var taskFromRepo = _taskRepository.GetTaskForCategory(id, categoryId);

            if (taskFromRepo == null) return NotFound();

            var task = Mapper.Map<TaskDto>(taskFromRepo);
            return Ok(task);
        }

        [HttpPost]
        public IActionResult CreateTaskForCategory(Guid categoryId, [FromBody] TaskForCreationDto task)
        {
            if (task == null)
            {
                return BadRequest();
            }

            var category = _categoryRepository.GetCategory(categoryId);

            if (category == null)
            {
                return NotFound();
            }

            var taskEntity = Mapper.Map<Task>(task);
            _taskRepository.AddTaskToCategory(category, taskEntity);

            if (!_taskRepository.Save()) throw new Exception($"Creating an task for category {categoryId} failed on save.");

            var taskToReturn = Mapper.Map<TaskDto>(taskEntity);

            return CreatedAtRoute("GetTask", new { categoryId = categoryId, id = taskToReturn.Id }, taskToReturn);
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteTaskForCategory(Guid categoryId, Guid id)
        {
            if (!_categoryRepository.CategoryExists(categoryId))
            {
                return NotFound();
            }

            var taskFromRepository = _taskRepository.GetTaskForCategory(id, categoryId);

            if (taskFromRepository == null)
            {
                return NotFound();
            }

            _taskRepository.DeleteTask(taskFromRepository);

            if (!_taskRepository.Save()) throw new Exception($"Deleting task { id } for category { categoryId } failed on save.");

            return NoContent();
        } 
    }
}
