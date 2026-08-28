//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Mvc;
//using StudentCourseManagement.Application.DTOs;
//using StudentCourseManagement.Application.Interfaces;

//namespace StudentCourseManagement.API.Controllers;

//[Authorize(Roles = "Admin,admin")]
//[ApiController]
//[Route("api/[controller]")]
//public class CoursesController : ControllerBase
//{
//    private readonly ICourseService _service;

//    public CoursesController(ICourseService service)
//    {
//        _service = service;
//    }

//    [HttpGet]
//    public async Task<IActionResult> GetAll()
//    {
//        //throw new Exception("TEST GLOBAL EXCEPTION - Database connection failed!");
//        var courses = await _service.GetAllAsync();
//        return Ok(courses);
//    }

//    [HttpGet("{id}")]
//    public async Task<IActionResult> GetById(int id)
//    {
//        if (id <= 0)
//            return BadRequest("Invalid course ID.");

//        var course = await _service.GetByIdAsync(id);

//        if (course == null)
//            return NotFound($"Course with ID {id} not found.");

//        return Ok(course);
//    }

//    [HttpPost]
//    public async Task<IActionResult> Create(CreateCourseDto dto)
//    {
//        if (!ModelState.IsValid)
//            return BadRequest(ModelState);

//        var course = await _service.CreateAsync(dto);

//        return CreatedAtAction(
//            nameof(GetById),
//            new { id = course.Id },
//            course);
//    }

//    [HttpPut("{id}")]
//    public async Task<IActionResult> Update(int id, UpdateCourseDto dto)
//    {
//        if (id <= 0)
//            return BadRequest("Invalid course ID.");

//        if (!ModelState.IsValid)
//            return BadRequest(ModelState);

//        var updated = await _service.UpdateAsync(id, dto);

//        if (!updated)
//            return NotFound($"Course with ID {id} not found.");

//        return NoContent();
//    }

//    [HttpDelete("{id}")]
//    public async Task<IActionResult> Delete(int id)
//    {
//        if (id <= 0)
//            return BadRequest("Invalid course ID.");

//        var deleted = await _service.DeleteAsync(id);

//        if (!deleted)
//            return NotFound($"Course with ID {id} not found.");

//        return NoContent();
//    }

//    [HttpPost("{courseId}/enroll/{studentId}")]
//    public async Task<IActionResult> EnrollStudent(int courseId, int studentId)
//    {
//        if (courseId <= 0 || studentId <= 0)
//            return BadRequest("Invalid course or student ID.");

//        var result = await _service.EnrollStudentAsync(studentId, courseId);

//        if (!result.Success)
//            return BadRequest(result.Message);

//        return Ok(result.Message);
//    }

//    [HttpDelete("{courseId}/unenroll/{studentId}")]
//    public async Task<IActionResult> UnenrollStudent(int courseId, int studentId)
//    {
//        if (courseId <= 0 || studentId <= 0)
//            return BadRequest("Invalid course or student ID.");

//        var success = await _service.UnenrollStudentAsync(studentId, courseId);

//        if (!success)
//            return NotFound("Enrollment record not found.");

//        return NoContent();
//    }
//}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudentCourseManagement.Application.DTOs;
using StudentCourseManagement.Application.Interfaces;

namespace StudentCourseManagement.API.Controllers;

[Authorize(Roles = "Admin,admin")]
[ApiController]
[Route("api/[controller]")]
public class CoursesController : ControllerBase
{
    private readonly ICourseService _service;

    public CoursesController(ICourseService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var courses = await _service.GetAllAsync();
        return Ok(courses);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        if (id <= 0) return BadRequest("Invalid course ID.");
        var course = await _service.GetByIdAsync(id);
        if (course == null) return NotFound($"Course with ID {id} not found.");
        return Ok(course);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateCourseDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var course = await _service.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = course.Id }, course);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, UpdateCourseDto dto)
    {
        if (id <= 0) return BadRequest("Invalid course ID.");
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var updated = await _service.UpdateAsync(id, dto);
        if (!updated) return NotFound($"Course with ID {id} not found.");
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        if (id <= 0) return BadRequest("Invalid course ID.");
        var deleted = await _service.DeleteAsync(id);
        if (!deleted) return NotFound($"Course with ID {id} not found.");
        return NoContent();
    }

    // --- ADMIN OVERRIDE & ASSISTED ENROLLMENT / UNENROLLMENT ---

    [HttpPost("{courseId}/enroll-student/{studentId}")]
    public async Task<IActionResult> AdminEnrollStudent(int courseId, int studentId)
    {
        if (courseId <= 0 || studentId <= 0)
            return BadRequest("Invalid course ID or student ID.");

        var result = await _service.EnrollStudentAsync(studentId, courseId);
        if (!result.Success)
            return BadRequest(result.Message);

        return Ok($"Admin successfully enrolled Student ID {studentId} into Course ID {courseId}.");
    }

    [HttpDelete("{courseId}/unenroll-student/{studentId}")]
    public async Task<IActionResult> AdminUnenrollStudent(int courseId, int studentId)
    {
        if (courseId <= 0 || studentId <= 0)
            return BadRequest("Invalid course ID or student ID.");

        var success = await _service.UnenrollStudentAsync(studentId, courseId);
        if (!success)
            return NotFound($"Student ID {studentId} is not enrolled in Course ID {courseId}.");

        return NoContent();
    }
}