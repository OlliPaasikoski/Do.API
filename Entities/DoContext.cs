using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Do.API.Entities
{
    public class DoContext : DbContext
    {
        public DoContext(DbContextOptions<DoContext> options)
           : base(options)
        {
            //Database.Migrate();
        }
    }
}
