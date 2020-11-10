using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using API.Contracts;
using API.DTOs;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    /// <summary>
    ///     Endpoint Used To Interact With The Authors In The Book Store's Database
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public class AuthorsController : ControllerBase
    {
        private readonly IAuthorRepository _authorRepository;
        private readonly ILoggerService _logger;
        private readonly IMapper _mapper;

        public AuthorsController(IAuthorRepository authorRepository, ILoggerService logger, IMapper mapper)
        {
            _authorRepository = authorRepository;
            _logger = logger;
            _mapper = mapper;
        }

        /// <summary>
        ///     Get All Authors
        /// </summary>
        /// <returns>List Of Authors</returns>
        [HttpGet]
        public async Task<IActionResult> GetAuthors()
        {
            try
            {
                _logger.LogInfo("Attempted Get All Authors");
                var authors = await _authorRepository.FindAll();
                var response = _mapper.Map<IList<AuthorDTO>>(authors);
                _logger.LogInfo("Successfully Retrieved All Authors");
                return Ok(response);
            }

            catch (Exception e)
            {
                _logger.LogError($"{e.Message} - {e.InnerException}");
                return StatusCode(500, "Something Went Wrong... Oops!");
            }
        }
    }
}