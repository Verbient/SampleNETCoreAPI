// THIS IS ONLY FOR AID DURING DEVELOPMENT PHASE

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyApp.Common;
using MyApp.DTO;
using MyApp.Services;
using Core = Microsoft.AspNetCore.Authorization;

namespace MyApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MiscellaneousController : ControllerBase
    {
        private readonly IMiscellaneousService miscellaneousService;

        public MiscellaneousController(IMiscellaneousService miscellaneousService)
        {
            this.miscellaneousService = miscellaneousService;
        }

        [HttpPost("ResetDatabase")]
        [Core.AllowAnonymous]
        public IActionResult ResetDatabase()
        {
            miscellaneousService.ResetDatabase();
            return Ok(new { Message = "Success!!! Database reset to initial stage and seeded with dummy data"});
        }

        [HttpGet("CustomException")]
        [Core.AllowAnonymous]
        public IActionResult CustomException()
        {
            miscellaneousService.CustomExceptionExample();
            return NoContent(); // This Return statement is never executed
        }
    }
}
