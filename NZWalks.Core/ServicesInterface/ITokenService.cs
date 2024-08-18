﻿using Microsoft.AspNetCore.Identity;
using NZWalks.Core.Domain.Entities;
using NZWalks.Core.DTOs;
using System.Security.Claims;

namespace NZWalks.Core.ServicesInterface
{
    public interface ITokenService
    {
        Task<bool> RegisterUser(RegisterRequestDto registerRequestDto); // Thay IdentityUser thành User (đc extend từ IdentityUser)
        Task<LoginResponseDto?> Login(LoginRequestDto loginRequestDto);
        Task<TokenDto?> RefreshToken(User identityUser);
        ClaimsPrincipal? GetTokenPrincipal(string token);
    }
}
