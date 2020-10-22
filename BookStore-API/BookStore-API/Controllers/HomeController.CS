using System.Collections.Generic;
using API.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    /// <summary>
    ///     The Book Store's Home API Controller
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class HomeController : ControllerBase
    {
        private readonly ILoggerService _logger;

        public HomeController(ILoggerService logger)
        {
            _logger = logger;
        }

        /// <summary>
        ///     Gets The Controller
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            _logger.LogInfo("Accessed Home Controller");
            return new[] {"first-placeholder", "second-placeholder"};
        }

        /// <summary>
        ///     Gets The ID At The Controller
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public string Get(int id)
        {
            _logger.LogDebug("Got A Value");
            return $"Got {id}";
        }

        /// <summary>
        ///     Posts A Value To The Controller
        /// </summary>
        /// <param name="value"></param>
        [HttpPost]
        public string Post([FromBody] string value)
        {
            _logger.LogError("This Is An Error");
            return $"Posted {value}";
        }

        /// <summary>
        ///     Puts A Value To The ID At The Controller
        /// </summary>
        /// <param name="id"></param>
        /// <param name="value"></param>
        [HttpPut("{id}")]
        public string Put(int id, [FromBody] string value)
        {
            _logger.LogWarn("This Is A Warning");
            return $"Put {value} Into {id}";
        }

        /// <summary>
        ///     Deletes The ID At The Controller
        /// </summary>
        /// <param name="id"></param>
        [HttpDelete("{id}")]
        public string Delete(int id)
        {
            return $"Deleted {id}";
        }
    }
}