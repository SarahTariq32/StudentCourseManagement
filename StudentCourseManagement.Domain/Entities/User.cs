using System;
using StudentCourseManagement.Domain.Enums;

namespace StudentCourseManagement.Domain.Entities;

public class User
{
    public int Id { get; set; }

    public string Username { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public UserRole Role { get; set; }

    public string? RefreshToken { get; set; }

    public DateTime? RefreshTokenExpiryTime { get; set; }
}
