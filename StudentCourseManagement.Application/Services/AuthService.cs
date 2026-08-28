using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using StudentCourseManagement.Application.DTOs;
using StudentCourseManagement.Application.Interfaces;
using StudentCourseManagement.Domain.Entities;

namespace StudentCourseManagement.Application.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IConfiguration _configuration;
    private readonly PasswordHasher<User> _passwordHasher;

    public AuthService(
        IUserRepository userRepository,
        IConfiguration configuration)
    {
        _userRepository = userRepository;
        _configuration = configuration;
        _passwordHasher = new PasswordHasher<User>();
    }

    public async Task<bool> RegisterAsync(RegisterDto dto)
    {
        var existingUser = await _userRepository
            .GetByUsernameAsync(dto.Username);

        if (existingUser != null)
            return false;

        var user = new User
        {
            Username = dto.Username,
            Role = dto.Role
        };

        user.PasswordHash = _passwordHasher.HashPassword(
            user,
            dto.Password);

        await _userRepository.AddAsync(user);

        return true;
    }

    public async Task<AuthResponseDto?> LoginAsync(LoginDto dto)
    {
        var user = await _userRepository
            .GetByUsernameAsync(dto.Username);

        if (user == null)
            return null;

        var result = _passwordHasher.VerifyHashedPassword(
            user,
            user.PasswordHash,
            dto.Password);

        if (result == PasswordVerificationResult.Failed)
            return null;

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Role, user.Role.ToString())
        };

        var jwtKey = _configuration["Jwt:Key"]
            ?? throw new InvalidOperationException("JWT Secret Key 'Jwt:Key' is not configured. Please define it in appsettings.json or as an environment variable.");

        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(jwtKey));

        var credentials = new SigningCredentials(
            key,
            SecurityAlgorithms.HmacSha256);

        // Keep the access-token lifetime at 5 minutes for testing
        var token = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(5),
            signingCredentials: credentials);

        // Generate secure 64-byte refresh token with 7-day lifetime
        var refreshToken = GenerateSecureRefreshToken();
        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);

        await _userRepository.UpdateAsync(user);

        return new AuthResponseDto
        {
            Token = new JwtSecurityTokenHandler().WriteToken(token),
            RefreshToken = user.RefreshToken,
            RefreshTokenExpiryTime = user.RefreshTokenExpiryTime.Value
        };
    }

    public async Task<AuthResponseDto?> RefreshTokenAsync(RefreshTokenDto dto)
    {
        var user = await _userRepository
            .GetByRefreshTokenAsync(dto.RefreshToken);

        if (user == null || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
            return null;

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Role, user.Role.ToString())
        };

        var jwtKey = _configuration["Jwt:Key"]
            ?? throw new InvalidOperationException("JWT Secret Key 'Jwt:Key' is not configured. Please define it in appsettings.json or as an environment variable.");

        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(jwtKey));

        var credentials = new SigningCredentials(
            key,
            SecurityAlgorithms.HmacSha256);

        // Keep the access-token lifetime at 5 minutes for testing
        var token = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(5),
            signingCredentials: credentials);

        // Generate a new secure refresh token (rotation) with a new 7-day lifetime
        var newRefreshToken = GenerateSecureRefreshToken();
        user.RefreshToken = newRefreshToken;
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);

        await _userRepository.UpdateAsync(user);

        return new AuthResponseDto
        {
            Token = new JwtSecurityTokenHandler().WriteToken(token),
            RefreshToken = user.RefreshToken,
            RefreshTokenExpiryTime = user.RefreshTokenExpiryTime.Value
        };
    }

    private static string GenerateSecureRefreshToken()
    {
        var randomNumber = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }
}