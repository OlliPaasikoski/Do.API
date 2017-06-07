using AutoMapper;
using Do.API.Entities;
using Do.API.Helpers;
using Do.API.Models;
using Do.API.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Do.API.Controllers
{
    [Route("api/categorycollections")]
    public class CategoryCollectionsController : Controller
    {
        private ITaskCategoryRepository _taskCategoryRepository;
        public CategoryCollectionsController(ITaskCategoryRepository taskCategoryRepository)
        {
            _taskCategoryRepository = taskCategoryRepository;
        }

        [HttpPost()]
        public IActionResult CreateCategoryCollection(
            [FromBody] IEnumerable<TaskCategoryForCreationDto> categoryCollection)
        {
            if (categoryCollection == null)
            {
                return BadRequest();
            }

            var categoryEntities = Mapper.Map<IEnumerable<TaskCategory>>(categoryCollection);

            foreach (var category in categoryEntities)
            {
                _taskCategoryRepository.AddCategory(category);
            }

            if (!_taskCategoryRepository.Save()) throw new Exception("Creating an author failed on save.");

            var categoryCollectionToReturn = Mapper.Map<IEnumerable<TaskCategoryDto>>(categoryEntities);
            var idsAsString = String.Join(",",
                    categoryCollectionToReturn.Select(c => c.Id));

            return CreatedAtRoute("GetCategoryCollection", new { ids = idsAsString }, categoryCollectionToReturn);
        }


        [HttpGet("({ids})", Name = "GetCategoryCollection")]
        public IActionResult GetCategoryCollection(
            [ModelBinder(BinderType = typeof(ArrayModelBinder))] IEnumerable<Guid> ids)
        {
            if (ids == null)
            {
                return BadRequest();
            }

            var categoryEntities = _taskCategoryRepository.GetCategories(ids);

            if (ids.Count() != categoryEntities.Count())
            {
                return NotFound();
            }

            var categoriesToReturn = Mapper.Map<IEnumerable<TaskCategoryDto>>(categoryEntities);

            return Ok(categoriesToReturn);
        }
    }
}
