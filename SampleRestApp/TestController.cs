using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PostgRest.net;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace SampleRestApp
{
    [Route("api/[controller]")]
    public class TestController : Controller
    {
        private readonly IPgDataContentService content;

        public TestController(IPgDataContentService content)
        {
            this.content = content;
        }

        [HttpGet]
        public async Task<ContentResult> Get()
        {
            var c = await content.GetContentAsync("select rest__get_values_json(@_query::json)", p => p.AddWithValue("_query", "{}"));
            return c;
        }
    }
}
