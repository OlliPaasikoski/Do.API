using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Do.API.Services;
using AutoMapper;
using Do.API.Models;
using Do.API.Entities;
using Microsoft.AspNetCore.Http;
using Do.API.Helpers;

namespace Do.API.Controllers
{
    [Route("api/categories")]
    public class CategoriesController : Controller
    {
        private ITaskCategoryRepository _categoryRepository;
        private const int maxCategoryPageSize = 20;
        private IUrlHelper _urlHelper;

        public CategoriesController(ITaskRepository taskRepository, 
            ITaskCategoryRepository categoryRepository,
            IUrlHelper urlHelper)
        {
            _categoryRepository = categoryRepository;
            _urlHelper = urlHelper;
        }

        [HttpPost]
        public IActionResult CreateCategory([FromBody] TaskCategoryForCreationDto category)
        {
            if (category == null)
            {
                return BadRequest();
            }

            var categoryEntity = Mapper.Map<TaskCategory>(category);
            _categoryRepository.AddCategory(categoryEntity);

            if (!_categoryRepository.Save()) throw new Exception($"Creating new category failed on save.");

            var categoryToReturn = Mapper.Map<TaskCategoryDto>(categoryEntity);

            return CreatedAtRoute("GetCategory", new { id = categoryToReturn.Id }, categoryToReturn);
        }

        // unsupported route: sample implementation that still provides 
        // a correct status code if category with id already exists
        [HttpPost("{id}")]
        public IActionResult BlockCategoryCreation(Guid id)
        {
            if (_categoryRepository.CategoryExists(id))
            {
                return new StatusCodeResult(StatusCodes.Status409Conflict);
            }
            // Not found means resource not found, 
            // be it for a given id or the entire route
            return NotFound();
        }


        // GET api/categories
        // inline query parameters
        // public IActionResult GetCategories([FromQuery(Name = "page")] int pageNumber = 1, int pageSize = 10)
        [HttpGet(Name = "GetCategories")]
        public IActionResult GetCategories(CategoriesResourceParameters categoriesResourceParameters)
        {
            // inline query parameters check
            // pageSize = (pageSize > maxCategoryPageSize) ? maxCategoryPageSize : pageSize;

            // throw new Exception("Random exception for testing purposes");
            var categoriesFromRepo = _categoryRepository.GetCategories(categoriesResourceParameters);


            var previousPageLink = categoriesFromRepo.HasPrevious ?
                CreateCategoriesResourceUri(categoriesResourceParameters,
                ResourceUriType.PreviousPage) : null;

            var nextPageLink = categoriesFromRepo.HasNext ?
                CreateCategoriesResourceUri(categoriesResourceParameters,
                ResourceUriType.NextPage) : null;

            var paginationMetadata = new
            {
                totalCount = categoriesFromRepo.TotalCount,
                pageSize = categoriesFromRepo.PageSize,
                currentPage = categoriesFromRepo.CurrentPage,
                totalPages = categoriesFromRepo.TotalPages,
                previousPageLink = previousPageLink,
                nextPageLink = nextPageLink
            };

            Response.Headers.Add("X-Pagination", 
                Newtonsoft.Json.JsonConvert.SerializeObject(paginationMetadata));

            var categories = Mapper.Map<IEnumerable<TaskCategoryDto>>(categoriesFromRepo);
            return Ok(categories);
            // see Startup.cs for global exception handling
            /*
            try
            {

            }
            catch (Exception ex)
            {
                // Here "fault" means a non-userdependent error
                return StatusCode(500, "An unexpected fault happened. Try again later."); 
            }
            */
        }

        private string CreateCategoriesResourceUri(
            CategoriesResourceParameters categoriesResourceParameters,
            ResourceUriType type)
        {
            switch (type)
            {
                case ResourceUriType.NextPage:
                    return _urlHelper.Link("GetCategories",
                        new
                        {
                            pageNumber = categoriesResourceParameters.PageNumber + 1,
                            pageSize = categoriesResourceParameters.PageSize,
                            searchQuery = categoriesResourceParameters.SearchQuery
                        });
                case ResourceUriType.PreviousPage:
                    return _urlHelper.Link("GetCategories",
                        new
                        {
                            pageNumber = categoriesResourceParameters.PageNumber - 1,
                            pageSize = categoriesResourceParameters.PageSize,
                            searchQuery = categoriesResourceParameters.SearchQuery
                        });               
                default:
                    return _urlHelper.Link("GetCategories",
                        new
                        {
                            pageNumber = categoriesResourceParameters.PageNumber,
                            pageSize = categoriesResourceParameters.PageSize,
                            searchQuery = categoriesResourceParameters.SearchQuery
                        });
            }
        }

        // GET api/categories/5
        [HttpGet("{id}", Name = "GetCategory")]
        public IActionResult GetCategory(Guid id)
        {
            var categoryFromRepo = _categoryRepository.GetCategory(id);

            if (categoryFromRepo == null) return NotFound();

            var category = Mapper.Map<TaskCategoryDto>(categoryFromRepo);

            return Ok(category);
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteCategory(Guid id)
        {
            var categoryFromRepository = _categoryRepository.GetCategory(id);

            if (categoryFromRepository == null)
            {
                return NotFound();
            }

            _categoryRepository.DeleteCategory(categoryFromRepository);

            if (!_categoryRepository.Save()) throw new Exception($"Deleting category { id } failed on save.");

            return NoContent();
        }
    }
}
