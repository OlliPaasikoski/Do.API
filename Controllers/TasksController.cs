using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Do.API.Controllers
{
    [Route("api/tasks")]
    public class TasksController : Controller
    {
        // GET api/tasks
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "task1", "task3" };
        }

        // GET api/tasks/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "task5";
        }
    }
}
