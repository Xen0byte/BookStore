﻿using System;
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
    ///     Endpoint Used To Interact With The Authors In The Book Store's Database
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
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
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAuthors()
        {
            var controllerAction = GetControllerActionNames();

            try
            {
                _logger.LogInfo($"{controllerAction}: Attempted");
                var authors = await _authorRepository.FindAll();
                var response = _mapper.Map<IList<AuthorDTO>>(authors);
                _logger.LogInfo($"{controllerAction}: Successful");

                return Ok(response);
            }

            catch (Exception e)
            {
                return InternalError($"{controllerAction}: {e.Message} - {e.InnerException}");
            }
        }

        /// <summary>
        ///     Get Author By ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Author</returns>
        [HttpGet("{id:int}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAuthor(int id)
        {
            var controllerAction = GetControllerActionNames();

            try
            {
                _logger.LogInfo($"{controllerAction}: Attempted For ID {id}");
                var author = await _authorRepository.FindById(id);

                if (author == null)
                {
                    _logger.LogWarn($"{controllerAction}: ID {id} Not Found");
                    return NotFound();
                }

                var response = _mapper.Map<AuthorDTO>(author);
                _logger.LogInfo($"{controllerAction}: Successful For ID {id}");

                return Ok(response);
            }

            catch (Exception e)
            {
                return InternalError($"{controllerAction}: {e.Message} - {e.InnerException}");
            }
        }

        /// <summary>
        ///     Create Author
        /// </summary>
        /// <param name="authorCreateDTO"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "Administrator")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status415UnsupportedMediaType)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Create([FromBody] AuthorCreateDTO authorCreateDTO)
        {
            var controllerAction = GetControllerActionNames();

            try
            {
                _logger.LogInfo($"{controllerAction}: Attempted");

                if (authorCreateDTO == null)
                {
                    _logger.LogWarn($"{controllerAction}: Empty Submission Was Made");
                    return BadRequest(ModelState);
                }

                if (!ModelState.IsValid)
                {
                    _logger.LogWarn($"{controllerAction}: Invalid Or Incomplete Data Submitted");
                    return BadRequest(ModelState);
                }

                var author = _mapper.Map<Author>(authorCreateDTO);
                var isSuccess = await _authorRepository.Create(author);

                if (!isSuccess) return InternalError($"{controllerAction}: Failed");

                _logger.LogInfo($"{controllerAction}: Successful");

                return Created("Create", new {author});
            }

            catch (Exception e)
            {
                return InternalError($"{controllerAction}: {e.Message} - {e.InnerException}");
            }
        }

        /// <summary>
        ///     Update Author By ID (https://rapidapi.com/blog/put-vs-patch/)
        /// </summary>
        /// <param name="id"></param>
        /// <param name="authorUpdateDTO"></param>
        /// <returns></returns>
        [HttpPut("{id:int}")]
        [Authorize(Roles = "Administrator")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status415UnsupportedMediaType)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Update(int id, [FromBody] AuthorUpdateDTO authorUpdateDTO)
        {
            var controllerAction = GetControllerActionNames();

            try
            {
                _logger.LogInfo($"{controllerAction}: Attempted For ID {id}");

                if (id < 1 || authorUpdateDTO == null || id != authorUpdateDTO.ID)
                {
                    _logger.LogWarn($"{controllerAction}: Missing Data Or Invalid ID");
                    return BadRequest(ModelState);
                }

                var exists = await _authorRepository.Exists(id);

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

                var author = _mapper.Map<Author>(authorUpdateDTO);
                var isSuccess = await _authorRepository.Update(author);

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
        ///     Delete Author By ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Administrator")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
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

                var exists = await _authorRepository.Exists(id);

                if (!exists)
                {
                    _logger.LogWarn($"{controllerAction}: ID {id} Not Found");
                    return NotFound();
                }

                var author = await _authorRepository.FindById(id);

                var isSuccess = await _authorRepository.Delete(author);

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