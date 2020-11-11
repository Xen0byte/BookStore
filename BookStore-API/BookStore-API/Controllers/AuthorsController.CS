using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using API.Contracts;
using API.Data.Models;
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
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAuthors()
        {
            try
            {
                _logger.LogInfo("Attempted Get All Authors");
                var authors = await _authorRepository.FindAll();
                var response = _mapper.Map<IList<AuthorDTO>>(authors);
                _logger.LogInfo("Successfully Got All Authors");

                return Ok(response);
            }

            catch (Exception e)
            {
                return InternalError($"{e.Message} - {e.InnerException}");
            }
        }

        /// <summary>
        ///     Get Author By ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Author</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAuthor(int id)
        {
            try
            {
                _logger.LogInfo($"Attempted Get Author ID {id}");
                var author = await _authorRepository.FindById(id);

                if (author == null)
                {
                    _logger.LogWarn($"Author ID {id} Was Not Found");
                    return NotFound();
                }

                var response = _mapper.Map<AuthorDTO>(author);
                _logger.LogInfo($"Successfully Got Author ID {id}");

                return Ok(response);
            }

            catch (Exception e)
            {
                return InternalError($"{e.Message} - {e.InnerException}");
            }
        }

        /// <summary>
        ///     Creates An Author
        /// </summary>
        /// <param name="authorCreateDTO"></param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Create([FromBody] AuthorCreateDTO authorCreateDTO)
        {
            try
            {
                _logger.LogInfo("Author Submission Attempted");

                if (authorCreateDTO == null)
                {
                    _logger.LogWarn("Empty Submission Was Made");
                    return BadRequest(ModelState);
                }

                if (!ModelState.IsValid)
                {
                    _logger.LogWarn("Invalid Or Incomplete Author Data Submitted");
                    return BadRequest(ModelState);
                }

                var author = _mapper.Map<Author>(authorCreateDTO);
                var isSuccess = await _authorRepository.Create(author);

                if (!isSuccess) return InternalError("Author Creation Failed");

                _logger.LogInfo("Author Successfully Created");

                return Created("Create", new {author});
            }

            catch (Exception e)
            {
                return InternalError($"{e.Message} - {e.InnerException}");
            }
        }

        /// <summary>
        ///     Updates An Author Via PUT (https://rapidapi.com/blog/put-vs-patch/)
        /// </summary>
        /// <param name="id"></param>
        /// <param name="authorUpdateDTO"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Update(int id, [FromBody] AuthorUpdateDTO authorUpdateDTO)
        {
            try
            {
                _logger.LogInfo($"Author ID {id} Update Attempted");

                if (id < 1 || authorUpdateDTO == null || id != authorUpdateDTO.ID)
                {
                    _logger.LogWarn("Missing Data Or Invalid Author ID");
                    return BadRequest(ModelState);
                }

                if (!ModelState.IsValid)
                {
                    _logger.LogWarn("Invalid Or Incomplete Author Data Submitted");
                    return BadRequest(ModelState);
                }

                var author = _mapper.Map<Author>(authorUpdateDTO);
                var isSuccess = await _authorRepository.Update(author);

                if (!isSuccess) return InternalError("Update Operation Failed");

                return NoContent();
            }

            catch (Exception e)
            {
                return InternalError($"{e.Message} - {e.InnerException}");
            }
        }

        private ObjectResult InternalError(string message)
        {
            _logger.LogError(message);
            return StatusCode(500, "Something Went Wrong... Oops!");
        }
    }
}