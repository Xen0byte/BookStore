using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    /// <summary>
    ///     This Is A Test API Controller
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class HomeController : ControllerBase
    {
        /// <summary>
        ///     GET: api/controller
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new[] {"first-placeholder", "second-placeholder"};
        }

        /// <summary>
        ///     GET api/controller/id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return $"Got {id}";
        }

        /// <summary>
        ///     POST api/controller
        /// </summary>
        /// <param name="value"></param>
        [HttpPost]
        public string Post([FromBody] string value)
        {
            return $"Posted {value}";
        }

        /// <summary>
        ///     PUT api/controller/id
        /// </summary>
        /// <param name="id"></param>
        /// <param name="value"></param>
        [HttpPut("{id}")]
        public string Put(int id, [FromBody] string value)
        {
            return $"Put {value} Into {id}";
        }

        /// <summary>
        ///     DELETE api/controller/id
        /// </summary>
        /// <param name="id"></param>
        [HttpDelete("{id}")]
        public string Delete(int id)
        {
            return $"Deleted {id}";
        }
    }
}