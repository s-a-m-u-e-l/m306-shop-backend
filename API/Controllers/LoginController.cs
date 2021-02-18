using System;
using System.Threading.Tasks;
using API.Filters;
using API.Helpers;
using API.Services;
using Core.Models.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;

namespace API.Controllers
{
    [Route ("api/v1")]
    [Authorize]
    [ApiV1ExceptionFilter]
    public sealed class LoginController : Controller {
        private readonly ILoginService _loginService;
        private readonly IPasswordService _passwordService;
        private readonly IUserService _userService;

        private readonly ILogger _logger;

        public LoginController (ILoginService loginService, IPasswordService passwordService, IUserService userService, ILogger<LoginController> logger) {
            _loginService = loginService;
            _passwordService = passwordService;
            _userService = userService;

            _logger = logger;
        }

        /// <summary>
        /// Logs in the specified user
        /// </summary>
        /// <response code="200">Successfully logged in</response>
        /// <response code="400">Invalid request model or already logged in</response>
        /// <response code="403">Password and/or username was incorrect</response>
        /// <response code="500">An internal error occurred</response>
        [HttpPost ("login")]
        [AllowAnonymous]
        [ProducesResponseType (200)]
        [ProducesResponseType (typeof (ErrorResponseModel), 400)]
        [ProducesResponseType (typeof (ErrorResponseModel), 403)]
        [ProducesResponseType (typeof (ErrorResponseModel), 500)]
        public async Task<IActionResult> Login ([FromBody] LoginRequestModel model) {
            if (model != null && ModelState.IsValid) {
                _logger.LogDebug ($"Attempting to login user with email {model.EMail}.");

                try {
                    var error = new ErrorResponseModel ("The combination of password an username is wrong or the user does not exist at all.");

                    // Try to get user from database
                    var user = await _userService.GetByEMailAsync(model.EMail);

                    // Check if user exists.
                    // If the user does not exist, we return 403 to not reveal that it does not exist
                    if (user == null) {
                        _logger.LogWarning ($"Error while logging in user with email {model.EMail}. The user does not exist.");
                        return Unauthorized("Email or password incorrect!");
                    }

                    // Check if password is correct
                    if (_passwordService.CheckPassword(model.Password, user.Password)) {
                        var token = _loginService.Login(user);
                        _logger.LogInformation ($"Successfully logged in user with email {model.EMail}.");

                        return Ok (new {
                                user.Id,
                                user.FirstName,
                                user.LastName,
                                user.Email,
                                user.IsAdmin,
                                token
                        });
                    } else {
                        _logger.LogWarning ($"Error while logging in user with email {model.EMail}: The password is incorrect.");
                        return Unauthorized("Email or password incorrect!");
                    }
                } catch (Exception ex) {
                    _logger.LogError (ex, $"Error while logging in: {ex.Message}");
                    return StatusCode (500, new ErrorResponseModel ("Error while handling request."));
                }
            } else {
                _logger.LogWarning ($"Error while logging in. Validation failed.");
                return BadRequest (ModelState.ToErrorResponseModel ());
            }
        }

        /// <summary>
        /// Logs out the current user
        /// </summary>
        /// <response code="200">Successfully logged out</response>
        [HttpGet ("logout")]
        [ProducesResponseType (200)]
        public IActionResult Logout() {
            if (_loginService.IsLoggedIn ()) {
                return BadRequest (new ErrorResponseModel ("Cannot logout, no user is logged in"));
            }
            _loginService.Logout();
            _logger.LogInformation("Successfully logged out");
            return Ok();
        }
    }
}