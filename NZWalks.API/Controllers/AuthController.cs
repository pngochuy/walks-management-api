using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NZWalks.API.CustomActionFilters;
using NZWalks.Core.Domain.Entities;
using NZWalks.Core.DTOs;
using NZWalks.Core.ServicesInterface;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace NZWalks.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<User> userManager;
        private readonly ITokenService tokenService;

        public AuthController(UserManager<User> userManager, ITokenService tokenService)
        {
            this.userManager = userManager;
            this.tokenService = tokenService;
        }

        // POST: /api/auth/register
        [HttpPost]
        [Route("register")]
        [ValidateModel]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDto registerRequestDto)
        {
            if (!await tokenService.RegisterUser(registerRequestDto))
            {
                //return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "User already exists!" });
                return StatusCode(StatusCodes.Status400BadRequest, new ResponseDto { Status = "Failed", Message = "Something went wrong" });
            }
            return StatusCode(StatusCodes.Status200OK, new ResponseDto { Status = "Success", Message = "User was registered successfully!" });
        }

        // POST: /api/Auth/Login
        [HttpPost]
        [Route("login")]
        [ValidateModel]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto loginRequestDto)
        {
            var response = await tokenService.Login(loginRequestDto);

            if (response == null) return BadRequest("Username or password incorrect!");

            return Ok(response);

        }

        // POST: /api/Auth/refresh-token
        [HttpPost]
        [Route("refresh-token")]
        [Authorize(Roles = "Reader,Writer")]
        public async Task<IActionResult> RefreshToken(TokenDto tokenDto)
        {
            if (tokenDto == null) return BadRequest("Invalid client request!");

            ClaimsPrincipal? principal = tokenService.GetTokenPrincipal(tokenDto.AccessToken);

            if (principal == null) return BadRequest("Invalid jwt access token!");

            var emailClaim = principal?.FindFirstValue(ClaimTypes.Email);
            var identityUser = await userManager.FindByEmailAsync(emailClaim);

            if (identityUser is null ||
                identityUser.RefreshToken != tokenDto.RefreshToken ||
                identityUser.RefreshTokenExpiry <= DateTime.Now)
            {
                return BadRequest("Invalid refresh token!");
            }

            var refreshTokenResult = await tokenService.RefreshToken(identityUser);

            return Ok(refreshTokenResult);

            //var authorizationHeader = this.HttpContext.Request.Headers["Authorization"].FirstOrDefault();
            //string jwtTokenString = authorizationHeader.Replace("Bearer ", "");
            //var jwt = new JwtSecurityToken(jwtTokenString);
            //var response = $"Authenticated! {Environment.NewLine}";

            //response += $"{Environment.NewLine}Exp time: {jwt.ValidTo.ToLocalTime().ToLongTimeString()}, " +
            //    $"Time: {DateTime.Now.ToLocalTime()}";

            //return Ok(response);

        }
    }
}
