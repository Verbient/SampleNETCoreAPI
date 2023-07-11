using Core = Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyApp.DTO;
using MyApp.Services;
using MyApp.API.Filters;
using System.Reflection;
using MyApp.Models;

namespace MyApp.API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AccountController : ControllerBase 
    {
        private readonly IAccountService accountService;

        // This constructor will be used by DI, in case multiple constructors are present
        [ActivatorUtilitiesConstructor] 
        public AccountController(IAccountService accountService)
        {
            this.accountService = accountService;
        }


        [Authorize("admin")]
        [HttpGet("SearchTop5UserByEmailContaining")]
        public IActionResult SearchTop5UserByEmailContaining(string EmailContains)
        {
            return Ok(accountService.SearchUsersByEmailContaining(EmailContains));
        }

        [Authorize("admin")]
        [HttpGet("SearchTop5UserByNameContaining")]
        public IActionResult SearchTop5UserByNameContaining(string NameContains)
        {
            return Ok(accountService.SearchUserByNameContaining(NameContains));
        }

        /// <summary>
        /// Register
        /// </summary>
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        [HttpPost("Register")]
        [Core.AllowAnonymous]
        public IActionResult Register(AppUserRequestDTO dto)
        {
            AppUserModel createdUser = accountService.Register(dto);
            // Return a 201 Created response
            return CreatedAtAction(nameof(Register), createdUser);
        }

        /// <summary>
        /// Change Password
        /// </summary>
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [HttpPost("ChangePassword")]
        [Authorize]
        public IActionResult ChangePassword(PasswordChangeDTO dto)
        {
            accountService.ChangePassword(dto);
            return NoContent();
        }


        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [HttpPost("authenticate-with-email")]
        [Core.AllowAnonymous]
        public IActionResult AuthenticateWithEmail(AuthenticateWithEmailDTO dto)
        {
            var response = accountService.AuthenticateWithEmail(dto);
            return Ok(response);
        }
    }
}
