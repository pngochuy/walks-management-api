using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using NZWalks.Core.Domain.Entities;
using NZWalks.Core.DTOs;
using NZWalks.Core.ServicesInterface;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace NZWalks.Core.Services
{
    public class TokenService : ITokenService
    {
        private readonly UserManager<User> userManager; // Thay IdentityUser thành User (đc extend từ IdentityUser)
        private readonly IConfiguration configuration;

        // IConfiguration: dùng để truy cập các object trong appsetting.json
        public TokenService(UserManager<User> userManager, IConfiguration configuration)
        {
            this.userManager = userManager;
            this.configuration = configuration;
        }

        public async Task<bool> RegisterUser(RegisterRequestDto registerRequestDto)
        {
            // check user exist by Username 
            var userExists = await userManager.FindByNameAsync(registerRequestDto.Username);
            if (userExists != null)
            {
                return false;
            }


            var identityUser = new User
            {
                // cả Username và Email trong IdentityUser đều là Email (từ client nhập vào) 
                UserName = registerRequestDto.Username,
                Email = registerRequestDto.Username
            };

            var identityResult = await userManager.CreateAsync(identityUser, registerRequestDto.Password);

            if (identityResult.Succeeded)
            {
                // Add role to this user
                if (registerRequestDto.RoleList != null && registerRequestDto.RoleList.Any())
                {
                    // !: Lỗi khi add role ko nằm trong DB mà vẫn add User vào DB đc (nhưng ko có 1-1 giữa User vs Role)
                    identityResult = await userManager.AddToRolesAsync(identityUser, registerRequestDto.RoleList);
                }
            }

            return identityResult.Succeeded;
        }

        public async Task<LoginResponseDto?> Login(LoginRequestDto loginRequestDto)
        {
            // hoặc dùng FindByNameAsync nếu trong jwt có claims "Name"
            var identityUser = await userManager.FindByEmailAsync(loginRequestDto.Username);

            if (identityUser != null)
            {
                var checkPasswordResult = await userManager.CheckPasswordAsync(identityUser, loginRequestDto.Password);

                if (checkPasswordResult)
                {
                    // Get roles for this user to check Authorization
                    var roleList = await userManager.GetRolesAsync(identityUser);

                    if (roleList != null)
                    {
                        // Create Access Token
                        var accessTokenExpiry = DateTime.Now.AddMinutes(Convert.ToDouble(configuration["Jwt:EXPIRATION_MINUTES"]));
                        var accessToken = GenerateAccessToken(identityUser, roleList.ToList(), accessTokenExpiry);

                        // Create Refresh Token (convert to UTC time, it'll saved in PosgresSQL)
                        var refreshTokenExpiry = DateTime.Now.AddMinutes(Convert.ToDouble(configuration["RefreshToken:EXPIRATION_MINUTES"]));
                        var refreshToken = GenerateRefreshToken();

                        // Update User's RefreshToken and RefreshTokenExpiry in DB
                        identityUser.RefreshToken = refreshToken;
                        identityUser.RefreshTokenExpiry = refreshTokenExpiry;

                        await userManager.UpdateAsync(identityUser);

                        var loginResponse = new LoginResponseDto
                        {
                            AccessToken = accessToken,
                            AccessTokenExpiriedTime = accessTokenExpiry,
                            RefreshToken = refreshToken,
                            RefreshTokenExpiriedTime = refreshTokenExpiry
                        };

                        return loginResponse;
                    }

                }
            }

            return null;
        }


        public async Task<TokenDto?> RefreshToken(User identityUser)
        {

            // Find list role of user
            //var identityUserRoles = await nZWalksAuthDbContext.UserRoles
            //    .Where(role => role.UserId.Equals(identityUser.Id))
            //    .ToListAsync();

            //var roleList = await nZWalksAuthDbContext.Roles.ToListAsync();

            //var userRoles = new List<String>();
            //foreach (var userRole in identityUserRoles)
            //{
            //    foreach (var role in roleList)
            //    {
            //        if (userRole.RoleId.Equals(role.Id))
            //        {
            //            userRoles.Add(role.Name);
            //            break;
            //        }
            //    }
            //}

            var response = new TokenDto();

            // Get roles for this user to check Authorization
            var roleList = await userManager.GetRolesAsync(identityUser);

            var accessTokenExpiry = DateTime.Now.AddMinutes(Convert.ToDouble(configuration["Jwt:EXPIRATION_MINUTES"]));
            response.AccessToken = GenerateAccessToken(identityUser, roleList.ToList(), accessTokenExpiry);
            response.RefreshToken = identityUser?.RefreshToken;
            //response.RefreshToken = GenerateRefreshToken();

            //identityUser.RefreshToken = response.RefreshToken;
            //identityUser.RefreshTokenExpiry = DateTime.Now.AddMinutes(3); // or UtcNow

            await userManager.UpdateAsync(identityUser);

            return response;
        }

        public ClaimsPrincipal? GetTokenPrincipal(string token)
        {

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]));

            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = configuration["Jwt:Issuer"],
                ValidAudience = configuration["Jwt:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(configuration["Jwt:Key"]))
            };

            return new JwtSecurityTokenHandler().ValidateToken(token, tokenValidationParameters, out _);
        }

        private string GenerateAccessToken(User user, List<string> roleList, DateTime accessTokenExpiry)
        {
            // Create list claim 
            var claimList = new List<Claim>();

            var iat = DateTimeOffset.Now.ToUnixTimeSeconds().ToString();

            //claimList.Add(new Claim("ClaimTypes.N", user.Email));
            claimList.Add(new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString())); // Subject (user id)
            claimList.Add(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())); // JWT unique id
            claimList.Add(new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.Now.ToUnixTimeSeconds().ToString())); // Issue at (date and time of token generation)
            claimList.Add(new Claim(ClaimTypes.Email, user.Email));

            foreach (var role in roleList)
            {
                claimList.Add(new Claim(ClaimTypes.Role, role));
            }

            // Create a SymmetricSecurityKey object using the key specified in the configuration
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]));

            // Create a SigningCredentials object with the security key and the HmacSha256 algorithm
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);


            var token = new JwtSecurityToken(
                configuration["Jwt:Issuer"],
                configuration["Jwt:Audience"],
                claimList,
                expires: accessTokenExpiry,
                signingCredentials: credentials
                );

            // Create a JwtSecurityToken object and use it to write the token as a string
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        // Create a refresh token (base 64 string of random numbers)
        public string GenerateRefreshToken()
        {
            var randomNumber = new byte[64];

            using (var numberGenerator = RandomNumberGenerator.Create())
            {
                numberGenerator.GetBytes(randomNumber);
            }

            return Convert.ToBase64String(randomNumber);
        }

    }
}
