//using System.Security.Claims;
//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Mvc;
//using StudentCourseManagement.Application.DTOs;
//using StudentCourseManagement.Application.Interfaces;

//namespace StudentCourseManagement.API.Controllers;

//[Authorize]
//[ApiController]
//[Route("api/[controller]")]
//public class StudentsController : ControllerBase
//{
//    private readonly IStudentService _service;

//    public StudentsController(IStudentService service)
//    {
//        _service = service;
//    }

//    [HttpGet]
//    [Authorize(Roles = "Admin,admin")]
//    public async Task<IActionResult> GetAll()
//    {
//        var students = await _service.GetAllAsync();
//        return Ok(students);
//    }

//    [HttpGet("{id}")]
//    public async Task<IActionResult> GetById(int id)
//    {
//        if (id <= 0)
//            return BadRequest("Invalid student ID.");

//        var student = await _service.GetByIdAsync(id);

//        if (student == null)
//            return NotFound($"Student with ID {id} not found.");

//        if (!IsAdmin())
//        {
//            var loggedInUsername = User.FindFirst(ClaimTypes.Name)?.Value;

//            if (!AreNamesMatching(student.Name, loggedInUsername))
//            {
//                return Forbid();
//            }
//        }

//        return Ok(student);
//    }

//    [HttpPost]
//    [Authorize(Roles = "Admin,admin")]
//    public async Task<IActionResult> Create(CreateStudentDto dto)
//    {
//        if (!ModelState.IsValid)
//            return BadRequest(ModelState);

//        var student = await _service.CreateAsync(dto);

//        return CreatedAtAction(
//            nameof(GetById),
//            new { id = student.Id },
//            student);
//    }

//    [HttpPut("{id}")]
//    public async Task<IActionResult> Update(int id, UpdateStudentDto dto)
//    {
//        if (id <= 0)
//            return BadRequest("Invalid student ID.");

//        if (!ModelState.IsValid)
//            return BadRequest(ModelState);

//        var existingStudent = await _service.GetByIdAsync(id);

//        if (existingStudent == null)
//            return NotFound($"Student with ID {id} not found.");

//        if (!IsAdmin())
//        {
//            var loggedInUsername = User.FindFirst(ClaimTypes.Name)?.Value;

//            if (!AreNamesMatching(existingStudent.Name, loggedInUsername))
//            {
//                return Forbid();
//            }
//        }

//        var updated = await _service.UpdateAsync(id, dto);

//        if (!updated)
//            return NotFound($"Student with ID {id} not found.");

//        return NoContent();
//    }

//    [HttpDelete("{id}")]
//    [Authorize(Roles = "Admin,admin")]
//    public async Task<IActionResult> Delete(int id)
//    {
//        if (id <= 0)
//            return BadRequest("Invalid student ID.");

//        var deleted = await _service.DeleteAsync(id);

//        if (!deleted)
//            return NotFound($"Student with ID {id} not found.");

//        return NoContent();
//    }

//    private bool IsAdmin()
//    {
//        return User.IsInRole("Admin") || User.IsInRole("admin");
//    }

//    private static bool AreNamesMatching(string? name1, string? name2)
//    {
//        if (string.IsNullOrWhiteSpace(name1) || string.IsNullOrWhiteSpace(name2))
//            return false;

//        var cleaned1 = new string(name1.Where(c => !char.IsWhiteSpace(c)).ToArray());
//        var cleaned2 = new string(name2.Where(c => !char.IsWhiteSpace(c)).ToArray());

//        return string.Equals(cleaned1, cleaned2, StringComparison.OrdinalIgnoreCase);
//    }
//}

using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudentCourseManagement.Application.DTOs;
using StudentCourseManagement.Application.Interfaces;

namespace StudentCourseManagement.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class StudentsController : ControllerBase
{
    private readonly IStudentService _studentService;
    private readonly ICourseService _courseService;

    public StudentsController(IStudentService studentService, ICourseService courseService)
    {
        _studentService = studentService;
        _courseService = courseService;
    }

    [HttpGet]
    [Authorize(Roles = "Admin,admin")]
    public async Task<IActionResult> GetAll()
    {
        var students = await _studentService.GetAllAsync();
        return Ok(students);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        if (id <= 0) return BadRequest("Invalid student ID.");

        var student = await _studentService.GetByIdAsync(id);
        if (student == null) return NotFound($"Student with ID {id} not found.");

        if (!IsAdmin())
        {
            var loggedInUsername = User.FindFirst(ClaimTypes.Name)?.Value;
            if (!AreNamesMatching(student.Name, loggedInUsername))
                return Forbid();
        }

        return Ok(student);
    }

    [HttpPost]
    [Authorize(Roles = "Admin,admin")]
    public async Task<IActionResult> Create(CreateStudentDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var student = await _studentService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = student.Id }, student);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, UpdateStudentDto dto)
    {
        if (id <= 0) return BadRequest("Invalid student ID.");
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var existingStudent = await _studentService.GetByIdAsync(id);
        if (existingStudent == null) return NotFound($"Student with ID {id} not found.");

        if (!IsAdmin())
        {
            var loggedInUsername = User.FindFirst(ClaimTypes.Name)?.Value;
            if (!AreNamesMatching(existingStudent.Name, loggedInUsername))
                return Forbid();
        }

        var updated = await _studentService.UpdateAsync(id, dto);
        if (!updated) return NotFound($"Student with ID {id} not found.");

        return NoContent();
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin,admin")]
    public async Task<IActionResult> Delete(int id)
    {
        if (id <= 0) return BadRequest("Invalid student ID.");
        var deleted = await _studentService.DeleteAsync(id);
        if (!deleted) return NotFound($"Student with ID {id} not found.");

        return NoContent();
    }

    // --- STUDENT BROWSING & SELF-ENROLLMENT ---

    [HttpGet("available-courses")]
    public async Task<IActionResult> GetAvailableCourses()
    {
        var courses = await _courseService.GetAvailableCoursesForStudentsAsync();
        return Ok(courses);
    }

    [HttpPost("enroll/{courseId}")]
    [Authorize(Roles = "Student,student")]
    public async Task<IActionResult> SelfEnroll(int courseId)
    {
        if (courseId <= 0) return BadRequest("Invalid course ID.");

        var loggedInUsername = User.FindFirst(ClaimTypes.Name)?.Value;
        if (string.IsNullOrEmpty(loggedInUsername)) return Unauthorized();

        var allStudents = await _studentService.GetAllAsync();
        var student = allStudents.FirstOrDefault(s => AreNamesMatching(s.Name, loggedInUsername));

        if (student == null)
            return BadRequest($"No student profile found for user '{loggedInUsername}'. Please ask an Admin to assist using your Student ID.");

        var result = await _courseService.EnrollStudentAsync(student.Id, courseId);
        if (!result.Success)
            return BadRequest(result.Message);

        return Ok(result.Message);
    }

    private bool IsAdmin() => User.IsInRole("Admin") || User.IsInRole("admin");

    private static bool AreNamesMatching(string? name1, string? name2)
    {
        if (string.IsNullOrWhiteSpace(name1) || string.IsNullOrWhiteSpace(name2)) return false;
        var cleaned1 = new string(name1.Where(c => !char.IsWhiteSpace(c)).ToArray());
        var cleaned2 = new string(name2.Where(c => !char.IsWhiteSpace(c)).ToArray());
        return string.Equals(cleaned1, cleaned2, StringComparison.OrdinalIgnoreCase);
    }
}