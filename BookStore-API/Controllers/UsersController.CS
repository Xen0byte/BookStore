using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using API.Contracts;
using API.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using JwtRegisteredClaimNames = Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames;

namespace API.Controllers
{
    /// <summary>
    ///     Endpoint For Book Store Authentication
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly ILoggerService _logger;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;

        public UsersController(SignInManager<IdentityUser> signInManager, UserManager<IdentityUser> userManager,
            ILoggerService logger, IConfiguration config)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _logger = logger;
            _config = config;
        }

        /// <summary>
        ///     Register
        /// </summary>
        /// <param name="userDTO"></param>
        /// <returns>User</returns>
        [Route("register")]
        [AllowAnonymous]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status415UnsupportedMediaType)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Register([FromBody] UserDTO userDTO)
        {
            var controllerAction = GetControllerActionNames();

            try
            {
                var emailAddress = userDTO.EmailAddress;
                var password = userDTO.Password;
                var username = userDTO.Username;
                var user = new IdentityUser {Email = emailAddress, UserName = username};

                _logger.LogInfo($"{controllerAction}: Attempted For User With Email Address \"{emailAddress}\"");
                var result = await _userManager.CreateAsync(user, password);

                if (!result.Succeeded)
                {
                    _logger.LogError(
                        $"{controllerAction}: User With Email Address \"{emailAddress}\" Could Not Be Registered");

                    foreach (var error in result.Errors)
                        _logger.LogError($"{controllerAction}: {error.Description}");

                    return StatusCode(500,
                        $"{controllerAction}: User With Email Address \"{emailAddress}\" Could Not Be Registered");
                }

                _logger.LogInfo($"{controllerAction}: Successful For User With Email Address \"{emailAddress}\"");
                return Ok(user);
            }

            catch (Exception e)
            {
                return InternalError($"{controllerAction}: {e.Message} - {e.InnerException}");
            }
        }

        /// <summary>
        ///     Log In
        /// </summary>
        /// <param name="userDTO"></param>
        /// <returns>Session Token</returns>
        [Route("login")]
        [AllowAnonymous]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status415UnsupportedMediaType)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Login([FromBody] UserDTO userDTO)
        {
            var controllerAction = GetControllerActionNames();

            try
            {
                var emailAddress = userDTO.EmailAddress;
                var password = userDTO.Password;

                _logger.LogInfo($"{controllerAction}: Attempted For User With Email Address \"{emailAddress}\"");
                var result = await _signInManager.PasswordSignInAsync(emailAddress, password, false, false);

                if (!result.Succeeded)
                {
                    _logger.LogError(
                        $"{controllerAction}: User With Email Address \"{emailAddress}\" Not Authenticated");
                    return Unauthorized(userDTO);
                }

                _logger.LogInfo($"{controllerAction}: Successful For User With Email Address \"{emailAddress}\"");
                var user = await _userManager.FindByEmailAsync(emailAddress);
                var token = await GenerateJSONWebToken(user);
                return Ok(new {token});
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

        private async Task<string> GenerateJSONWebToken(IdentityUser user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JWT:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Sub, user.Email),
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new(ClaimTypes.NameIdentifier, user.Id)
            };

            var roles = await _userManager.GetRolesAsync(user);

            claims.AddRange(roles.Select(role => new Claim(ClaimsIdentity.DefaultRoleClaimType, role)));

            var token = new JwtSecurityToken(
                _config["JWT:Issuer"],
                _config["JWT:Issuer"],
                claims,
                null,
                DateTime.Now.AddMinutes(5), // Increase Duration In Production
                credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}