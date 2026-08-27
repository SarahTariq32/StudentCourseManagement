using System;
using System.Collections.Generic;

namespace StudentCourseManagement.Infrastructure.AuthEntities;

public partial class UsersDatum
{
    public int Id { get; set; }

    public string Username { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public string Role { get; set; } = null!;
}
