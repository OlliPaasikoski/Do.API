using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Do.API.Entities;

namespace Do.API.Services
{
    public class BlogPostRepository : IBlogPostRepository
    {
        private DoContext _context;
        public BlogPostRepository(DoContext context)
        {
            _context = context;
        }

        public IEnumerable<BlogPost> GetBlogPosts()
        {
            return _context.BlogPosts
                .OrderByDescending(b => b.Date)
                .ThenBy(b => b.Title).ToList();          
        }
    }
}
