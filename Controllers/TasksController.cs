using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using Do.API.Services;
using Do.API.Models;
using Do.API.Entities;
using Microsoft.AspNetCore.JsonPatch;
using Do.API.Helpers;
using Microsoft.Extensions.Logging;

namespace Do.API.Controllers
{
    [Route("api/categories/{categoryId}/tasks")]
    public class TasksController : Controller
    {
        private ITaskRepository _taskRepository;
        private ITaskCategoryRepository _categoryRepository;
        private ILogger _logger;

        public TasksController(ITaskRepository taskRepository, ITaskCategoryRepository categoryRepository,
            ILogger<TasksController> logger)
        {
            _taskRepository = taskRepository;
            _categoryRepository = categoryRepository;
            _logger = logger;
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
        [HttpGet("{id}", Name = "GetTaskForCategory")]
        public IActionResult GetTaskForCategory(Guid categoryId, Guid id)
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
            
            if (task.Title == task.Description)
            {
                ModelState.AddModelError(nameof(TaskForCreationDto), "The provided title should be different from the description.");
            }

            if (!ModelState.IsValid)
            {
                return new UnprocessableEntityObjectResult(ModelState); 
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

            return CreatedAtRoute("GetTaskForCategory", new { categoryId = categoryId, id = taskToReturn.Id }, taskToReturn);
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

            _logger.LogInformation(100, $"Task {id} for category {categoryId} was deleted.");

            return NoContent();
        }

        [HttpPut("{id}")]
        public IActionResult UpdateTaskForCategory(Guid categoryId, Guid id,
            [FromBody] TaskForUpdateDto task)
        {
            if (task == null)
            {
                return BadRequest();
            }

            if (task.Title == task.Description)
            {
                ModelState.AddModelError(nameof(TaskForUpdateDto), "The provided title should be different from the description.");
            }

            if (!ModelState.IsValid)
            {
                return new UnprocessableEntityObjectResult(ModelState);
            }

            // this would be enough if our repository would take Id's instead of actual objects 
            if (!_categoryRepository.CategoryExists(categoryId))
            {
                return NotFound();
            }

            var categoryFromRepository = _categoryRepository.GetCategory(categoryId);
            // we do not need to check if it is found because we already checked if it exists

            var taskFromRepository = _taskRepository.GetTaskForCategory(id, categoryId);

            if (taskFromRepository == null)
            {
                // if we do not allow upserting, return NotFound();
                // return NotFound();
                var taskToAdd = Mapper.Map<Task>(task);
                taskToAdd.Id = id;
                _taskRepository.AddTaskToCategory(categoryFromRepository, taskToAdd);

                if (!_taskRepository.Save())
                {
                    throw new Exception($"Upserting task { id } for category { categoryId } failed on save.");
                }

                var taskToReturn = Mapper.Map<TaskDto>(taskToAdd);

                return CreatedAtRoute("GetTaskForCategory", new { categoryId = categoryId,  id = taskToReturn.Id }, taskToReturn);
            }

            // map
            // apply update
            // map back to entity
            Mapper.Map(task, taskFromRepository);
            _taskRepository.UpdateTask(taskFromRepository);

            if (!_taskRepository.Save())
            {
                throw new Exception($"Updating task { id } for category { categoryId } failed on save.");
            }

            // Returning an empty 204 response is a design choice
            // Return 200 OK including the new task data in payload may be more fitting if 
            // e.g. a ModifiedDate has been updated automatically and should be presented to the user
            return NoContent();
        }

        [HttpPatch("{id}")]
        public IActionResult PartiallyUpdateTaskForCategory(Guid categoryId, Guid id,
            [FromBody] JsonPatchDocument<TaskForUpdateDto> patchDoc)
        {
            if (patchDoc == null)
            {
                return BadRequest();
            }

            // this would be enough if our repository would take Id's instead of actual objects 
            if (!_categoryRepository.CategoryExists(categoryId))
            {
                return NotFound();
            }
            var categoryFromRepository = _categoryRepository.GetCategory(categoryId);

            var taskFromRepository = _taskRepository.GetTaskForCategory(id, categoryId);
            
            if (taskFromRepository == null)
            {
                var taskDto = new TaskForUpdateDto();
                patchDoc.ApplyTo(taskDto, ModelState);

                if (taskDto.Title == taskDto.Description)
                {
                    ModelState.AddModelError(nameof(TaskForUpdateDto), "The provided title should be different from the description.");
                }

                // the ModelState is automatically validated against the JsonPatchDocument class
                // the following line will enforce validation rules of the actual Dto
                // (as well as the custom rule title !== description defined above) 
                TryValidateModel(taskDto);

                if (!ModelState.IsValid)
                {
                    return new UnprocessableEntityObjectResult(ModelState);
                }

                var taskToAdd = Mapper.Map<Task>(taskDto);
                taskToAdd.Id = id;
                // keep in mind that if no value is given for a field,
                // the value will be default for that field

                _taskRepository.AddTaskToCategory(categoryFromRepository, taskToAdd);

                if (!_taskRepository.Save())
                {
                    throw new Exception($"Upserting task { id } for category { categoryId } failed on save.");
                }

                var taskToReturn = Mapper.Map<TaskDto>(taskToAdd);

                return CreatedAtRoute("GetTaskForCategory", new { categoryId = categoryId, id = taskToReturn.Id }, taskToReturn);
            }

            var taskToPatch = Mapper.Map<TaskForUpdateDto>(taskFromRepository);

            //patchDoc.ApplyTo(taskToPatch, ModelState);
            // ERROR LOGGING: Use below to check error behaviour (applying invalid patch request to task entity)
            patchDoc.ApplyTo(taskToPatch);

            if (taskToPatch.Title == taskToPatch.Description)
            {
                ModelState.AddModelError(nameof(TaskForUpdateDto), "The provided title should be different from the description.");
            }

            // the ModelState is automatically validated against the JsonPatchDocument class
            // the following line will enforce validation rules of the actual Dto
            // (as well as the custom rule title !== description defined above) 
            TryValidateModel(taskToPatch);

            if (!ModelState.IsValid)
            {
                return new UnprocessableEntityObjectResult(ModelState);
            }

            // add validation becasue this may fail

            Mapper.Map(taskToPatch, taskFromRepository);
            _taskRepository.UpdateTask(taskFromRepository);

            if (!_taskRepository.Save())
            {
                throw new Exception($"Updating task { id } for category { categoryId } failed on save.");
            }

            return NoContent();
        }
    }
}
