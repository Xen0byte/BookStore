using System;
using System.Threading.Tasks;
using API.Contracts;
using API.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ILoggerService _logger;

        public UsersController(SignInManager<IdentityUser> signInManager, UserManager<IdentityUser> userManager, ILoggerService logger)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _logger = logger;
        }

        /// <summary>
        ///     Log In
        /// </summary>
        /// <param name="userDTO"></param>
        /// <returns>User</returns>
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Login([FromBody] UserDTO userDTO)
        {
            var controllerAction = GetControllerActionNames();

            try
            {
                var username = userDTO.Username;
                var password = userDTO.Password;

                _logger.LogInfo($"{controllerAction}: Attempted For User \"{username}\"");
                var result = await _signInManager.PasswordSignInAsync(username, password, false, false);

                if (!result.Succeeded)
                {
                    _logger.LogInfo($"{controllerAction}: User \"{username}\" Not Authenticated");
                    return Unauthorized(userDTO);
                }

                _logger.LogInfo($"{controllerAction}: Successful For User \"{username}\"");
                var user = await _userManager.FindByNameAsync(username);
                return Ok(user);
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