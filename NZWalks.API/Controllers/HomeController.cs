using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NZWalks.Core.DTOs;
using System.Diagnostics;
using System.Net.Http;

namespace NZWalks.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize]
    public class HomeController : ControllerBase
    {

        // GET: api/home/admin
        [Authorize(Roles = "admin")]
        [HttpGet("admin")]
        public IActionResult Admin() => Ok(new { Message = "Welcome to the Admin API!" });

        // GET: api/home/user
        [Authorize(Roles = "user")]
        [HttpGet("user")]
        public IActionResult User() => Ok(new { Message = "Welcome to the User API!" });

        // GET: api/home/accessdenied
        [AllowAnonymous]
        [HttpGet("accessdenied")]
        public IActionResult AccessDenied() => Unauthorized(new { Message = "Access Denied!" });

        // GET: api/home/error
        [AllowAnonymous]
        [HttpGet("error")]
        public IActionResult Error() =>
            Problem(detail: "An error occurred", statusCode: 500);
    }
}
