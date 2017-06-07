using Do.API.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Do.API.Services
{
    public interface IBlogPostRepository
    {
        IEnumerable<BlogPost> GetBlogPosts();
    }
}
