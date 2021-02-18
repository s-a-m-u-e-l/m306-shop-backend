using System;
using System.Linq;
using System.Threading.Tasks;
using API.Filters;
using API.Helpers;
using API.Services;
using Core.Models.Requests;
using Data.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace API.Controllers
{
    [Route("api/v1/users")]
    [Authorize]
    [ApiV1ExceptionFilter]
    public sealed class UserController : Controller
    {
        private readonly IPasswordService _passwordService;
        private readonly IUserService _userService;
        private readonly ILoginService _loginService;

        private readonly ILogger _logger;

        public UserController(IPasswordService passwordService, IUserService userService, ILoginService loginService, ILogger<UserController> logger)
        {
            _passwordService = passwordService;
            _userService = userService;
            _loginService = loginService;

            _logger = logger;
        }

        /// <summary>
        /// Returns a list of all users
        /// </summary>
        /// <response code="200">List of users returned</response>
        /// <response code="403">No permission to get all users</response>
        /// <response code="500">An internal error occurred</response>
        [HttpGet]
        [Authorize(Roles = "admin")]
        [ProducesResponseType(200)]
        [ProducesResponseType(403)]
        [ProducesResponseType(typeof(ErrorResponseModel), 500)]
        public async Task<IActionResult> Get()
        {
            _logger.LogDebug($"Attempting to get a list of all users");

            var users = await _userService.GetAll();

            _logger.LogInformation($"Got {users.Count} users from the database.");

            return Json(users.Select(x => new
            {
                x.Id,
                x.FirstName,
                x.LastName,
                x.Email,
                x.IsAdmin
            }));
        }

        /// <summary>
        /// Creates/registers a new user
        /// </summary>
        /// <response code="200">User successfully created</response>
        /// <response code="400">An user with the provided data exists already/Invalid model</response>
        /// <response code="500">An internal error occurred</response>
        [HttpPost]
        [AllowAnonymous]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(ErrorResponseModel), 400)]
        [ProducesResponseType(typeof(ErrorResponseModel), 500)]
        public async Task<IActionResult> CreateNew([FromBody]UserCreateRequestModel model)
        {
            // Check if model is valid
            if (model != null && ModelState.IsValid)
            {
                _logger.LogDebug($"Attempting to create a new user with email {model.Email}.");

                try
                {
                    // Check if a user with this email exists already
                    if (await _userService.DoesEMailExistAsync(model.Email))
                    {
                        _logger.LogWarning($"Error while creating new user. A user with email {model.Email} exists already.");

                        return BadRequest(new ErrorResponseModel($"A user with email {model.Email} exists already."));
                    }

                    // Add new user to database
                    var user = new UserEntity
                    {
                        FirstName = model.FirstName,
                        LastName = model.LastName,
                        Email = model.Email,
                        Password = _passwordService.HashPassword(model.Password),
                        IsAdmin = false
                    };

                    await _userService.AddAsync(user);

                    _logger.LogInformation($"Successfully created new user with email {model.Email}.");


                    return Ok(new
                    {
                        user.Id,
                        user.FirstName,
                        user.LastName,
                        user.Email,
                        user.IsAdmin
                    });
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Error while handling request: {ex.Message}.");
                    return StatusCode(500, new ErrorResponseModel("Error while handling request."));
                }
            }
            else
            {
                _logger.LogWarning($"Error while creating new user. Validation failed.");

                return BadRequest(ModelState.ToErrorResponseModel());
            }
        }

        /// <summary>
        /// Updates the specified user.
        /// Generally the currently logged in user is allowed to update himself.
        /// Admins are allowed to update all users.
        ///
        /// If the password field within the model is left blank, the password will not be updated.
        /// </summary>
        /// <response code="200">User successfully updated</response>
        /// <response code="400">Invalid model</response>
        /// <response code="403">The user does not exist, or no permission to update the user</response>
        /// <response code="500">An internal error occurred</response>
        [HttpPut("{userId}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(ErrorResponseModel), 400)]
        [ProducesResponseType(403)]
        [ProducesResponseType(typeof(ErrorResponseModel), 500)]
        public async Task<IActionResult> Update([FromRoute]Guid userId, [FromBody]UserUpdateRequestModel model)
        {
            // Check if model is valid
            if (model != null && userId != null && ModelState.IsValid)
            {
                _logger.LogDebug($"Attempting to update user with email {model.Email}.");

                try
                {
                    // Check if the user exists
                    var user = await _userService.GetByIdAsync(userId);

                    if (user == null)
                    {
                        return StatusCode(403);
                    }

                    // Check if user updates himself or admin is updating
                    if (!_loginService.IsAdmin() && _loginService.GetLoggedInUserId() != user.Id)
                    {
                        _logger.LogWarning($"Only an admin can update arbitrary users. A user can only update himself.");

                        return StatusCode(403);
                    }

                    // Update the user
                    user.FirstName = model.FirstName;
                    user.LastName = model.LastName;
                    user.Email = model.Email;


                    // Set new password, if provided
                    if (!string.IsNullOrEmpty(model.Password))
                    {
                        user.Password = _passwordService.HashPassword(model.Password);
                    }

                    // Set new admin status, if the current user is admin
                    if (_loginService.IsAdmin())
                    {
                        user.IsAdmin = model.IsAdmin;
                    }

                    await _userService.UpdateAsync(user);

                    _logger.LogInformation($"Successfully updated user with email {model.Email}.");


                    return Ok(new
                    {
                        user.Id,
                        user.FirstName,
                        user.LastName,
                        user.Email,
                        user.IsAdmin
                    });
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Error while handling request: {ex.Message}.");
                    return StatusCode(500, new ErrorResponseModel("Error while handling request."));
                }
            }
            else
            {
                _logger.LogWarning($"Error while updating user. Validation failed.");

                return BadRequest(ModelState.ToErrorResponseModel());
            }
        }

        /// <summary>
        /// Deletes the specified user and all corresponding data.
        /// Generally the currently logged in user is allowed to delete himself.
        /// Admins are allowed to delete all users.
        /// </summary>
        /// <response code="200">User successfully deleted</response>
        /// <response code="400">Invalid model</response>
        /// <response code="403">The user does not exist, or no permission to delete the user</response>
        /// <response code="500">An internal error occurred</response>
        [HttpDelete("{userId}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(ErrorResponseModel), 400)]
        [ProducesResponseType(403)]
        [ProducesResponseType(typeof(ErrorResponseModel), 500)]
        public async Task<IActionResult> Delete([FromRoute]Guid userId)
        {
            // Check if model is valid
            if (userId != null)
            {
                _logger.LogDebug($"Attempting to delete user with id {userId}");

                try
                {
                    // Check if the user exists
                    var user = await _userService.GetByIdAsync(userId);

                    if (user == null)
                    {
                        return StatusCode(403);
                    }

                    // Check if user updates himself or admin is updating
                    if (!_loginService.IsAdmin() && _loginService.GetLoggedInUserId() != user.Id)
                    {
                        _logger.LogWarning($"Only an admin can delete arbitrary users. A user can only delete himself.");

                        return StatusCode(403);
                    }

                    await _userService.DeleteAsync(user);

                    _logger.LogInformation($"Successfully deleted user with id {userId}.");

                    // Logout current user deleted himself
                    if (_loginService.GetLoggedInUserId() == userId)
                    {
                        _loginService.Logout();
                    }

                    return Ok();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Error while handling request: {ex.Message}.");
                    return StatusCode(500, new ErrorResponseModel("Error while handling request."));
                }
            }
            else
            {
                _logger.LogWarning($"Error while deleting user. Validation failed.");

                return BadRequest(ModelState.ToErrorResponseModel());
            }
        }
    }
}