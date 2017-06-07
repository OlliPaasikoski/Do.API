using AutoMapper;
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
    [Route("api/blogs")]
    public class BlogPostController : Controller
    {
        private IBlogPostRepository _blogPostRepository;
        public BlogPostController(IBlogPostRepository blogPostRepository)
        {
            _blogPostRepository = blogPostRepository;
        }

        // GET api/categories
        [HttpGet()]
        public IActionResult GetBlogPosts()
        {
            var blogPostsFromRepo = _blogPostRepository.GetBlogPosts();

            var blogPosts = Mapper.Map<IEnumerable<BlogPostDto>>(blogPostsFromRepo);
            return Ok(blogPosts);
        }
    }
}
