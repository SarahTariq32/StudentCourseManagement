using StudentCourseManagement.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentCourseManagement.Application.DTOs
{
    public class RegisterDto
    {
        public string Username { get; set; } = null!;
        public string Password { get; set; } = null!;
        [Required]
        [EnumDataType(typeof(UserRole), ErrorMessage = "Role must be either 'Student' or 'Admin'.")]
        public UserRole Role { get; set; }
    }
}
