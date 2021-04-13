using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using API.Contracts;
using API.Data.Models;
using API.DTOs;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    /// <summary>
    ///     Endpoint Used To Interact With The Books In The Store's Database
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class BooksController : ControllerBase
    {
        private readonly IBookRepository _bookRepository;
        private readonly ILoggerService _logger;
        private readonly IMapper _mapper;

        public BooksController(IBookRepository bookRepository, ILoggerService logger, IMapper mapper)
        {
            _bookRepository = bookRepository;
            _logger = logger;
            _mapper = mapper;
        }

        /// <summary>
        ///     Get All Books
        /// </summary>
        /// <returns>List Of Books</returns>
        [HttpGet]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetBooks()
        {
            var controllerAction = GetControllerActionNames();

            try
            {
                _logger.LogInfo($"{controllerAction}: Attempted");
                var books = await _bookRepository.FindAll();
                var response = _mapper.Map<IList<BookDTO>>(books);
                _logger.LogInfo($"{controllerAction}: Successful");

                return Ok(response);
            }

            catch (Exception e)
            {
                return InternalError($"{controllerAction}: {e.Message} - {e.InnerException}");
            }
        }

        /// <summary>
        ///     Get Book By ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Book</returns>
        [HttpGet("{id:int}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetBook(int id)
        {
            var controllerAction = GetControllerActionNames();

            try
            {
                _logger.LogInfo($"{controllerAction}: Attempted For ID {id}");
                var book = await _bookRepository.FindById(id);

                if (book == null)
                {
                    _logger.LogWarn($"{controllerAction}: ID {id} Not Found");
                    return NotFound();
                }

                var response = _mapper.Map<BookDTO>(book);
                _logger.LogInfo($"{controllerAction}: Successful For ID {id}");

                return Ok(response);
            }

            catch (Exception e)
            {
                return InternalError($"{controllerAction}: {e.Message} - {e.InnerException}");
            }
        }

        /// <summary>
        ///     Create Book
        /// </summary>
        /// <param name="bookCreateDTO"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "Administrator")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Create([FromBody] BookCreateDTO bookCreateDTO)
        {
            var controllerAction = GetControllerActionNames();

            try
            {
                _logger.LogInfo($"{controllerAction}: Attempted");

                if (bookCreateDTO == null)
                {
                    _logger.LogWarn($"{controllerAction}: Empty Submission Was Made");
                    return BadRequest(ModelState);
                }

                if (!ModelState.IsValid)
                {
                    _logger.LogWarn($"{controllerAction}: Invalid Or Incomplete Data Submitted");
                    return BadRequest(ModelState);
                }

                var book = _mapper.Map<Book>(bookCreateDTO);
                var isSuccess = await _bookRepository.Create(book);

                if (!isSuccess) return InternalError($"{controllerAction}: Failed");

                _logger.LogInfo($"{controllerAction}: Successful");

                return Created("Create", new {book});
            }

            catch (Exception e)
            {
                return InternalError($"{controllerAction}: {e.Message} - {e.InnerException}");
            }
        }

        /// <summary>
        ///     Update Book By ID (https://rapidapi.com/blog/put-vs-patch/)
        /// </summary>
        /// <param name="id"></param>
        /// <param name="bookUpdateDTO"></param>
        /// <returns></returns>
        [HttpPut("{id:int}")]
        [Authorize(Roles = "Administrator")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Update(int id, [FromBody] BookUpdateDTO bookUpdateDTO)
        {
            var controllerAction = GetControllerActionNames();

            try
            {
                _logger.LogInfo($"{controllerAction}: Attempted For ID {id}");

                if (id < 1 || bookUpdateDTO == null || id != bookUpdateDTO.ID)
                {
                    _logger.LogWarn($"{controllerAction}: Missing Data Or Invalid ID");
                    return BadRequest(ModelState);
                }

                var exists = await _bookRepository.Exists(id);

                if (!exists)
                {
                    _logger.LogWarn($"{controllerAction}: ID {id} Not Found");
                    return NotFound();
                }

                if (!ModelState.IsValid)
                {
                    _logger.LogWarn($"{controllerAction}: Invalid Or Incomplete Data Submitted");
                    return BadRequest(ModelState);
                }

                var book = _mapper.Map<Book>(bookUpdateDTO);
                var isSuccess = await _bookRepository.Update(book);

                if (!isSuccess) return InternalError($"{controllerAction}: Failed");

                _logger.LogInfo($"{controllerAction}: Successful For ID {id}");
                return NoContent();
            }

            catch (Exception e)
            {
                return InternalError($"{controllerAction}: {e.Message} - {e.InnerException}");
            }
        }

        /// <summary>
        ///     Delete Book By ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Administrator")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Delete(int id)
        {
            var controllerAction = GetControllerActionNames();

            try
            {
                _logger.LogInfo($"{controllerAction}: Attempted For ID {id}");
                if (id < 1)
                {
                    _logger.LogWarn($"{controllerAction}: Invalid ID");
                    return BadRequest();
                }

                var exists = await _bookRepository.Exists(id);

                if (!exists)
                {
                    _logger.LogWarn($"{controllerAction}: ID {id} Not Found");
                    return NotFound();
                }

                var book = await _bookRepository.FindById(id);

                var isSuccess = await _bookRepository.Delete(book);

                if (!isSuccess) return InternalError($"{controllerAction}: Failed");

                _logger.LogInfo($"{controllerAction}: Successful For ID {id}");
                return NoContent();
            }

            catch (Exception e)
            {
                return InternalError($"{controllerAction}: {e.Message} - {e.InnerException}");
            }
        }

        private string GetControllerActionNames()
        {
            var controller = ControllerContext.ActionDescriptor.ControllerName;
            var action = ControllerContext.ActionDescriptor.ActionName;

            return $"{controller} - {action}";
        }

        private ObjectResult InternalError(string message)
        {
            _logger.LogError(message);
            return StatusCode(500, "Something Went Wrong... Oops!");
        }
    }
}