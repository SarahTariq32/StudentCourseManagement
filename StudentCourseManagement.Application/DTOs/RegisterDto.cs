using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StudentCourseManagement.Domain.Enums;

namespace StudentCourseManagement.Application.DTOs
{
    public class RegisterDto
    {
        public string Username { get; set; } = null!;
        public string Password { get; set; } = null!;
        public UserRole Role { get; set; } = UserRole.Student;
    }
}
