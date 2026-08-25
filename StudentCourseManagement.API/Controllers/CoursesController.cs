using Microsoft.AspNetCore.Mvc;
using StudentCourseManagement.Application.DTOs;
using StudentCourseManagement.Application.Interfaces;

namespace StudentCourseManagement.API.Controllers;

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
        try
        {
            var courses = await _service.GetAllAsync();
            return Ok(courses);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error retrieving courses: {ex.Message}");
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        try
        {
            if (id <= 0)
                return BadRequest("Invalid course ID.");

            var course = await _service.GetByIdAsync(id);

            if (course == null)
                return NotFound($"Course with ID {id} not found.");

            return Ok(course);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error retrieving course: {ex.Message}");
        }
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateCourseDto dto)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var course = await _service.CreateAsync(dto);

            return CreatedAtAction(
                nameof(GetById),
                new { id = course.Id },
                course);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error creating course: {ex.Message}");
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, UpdateCourseDto dto)
    {
        try
        {
            if (id <= 0)
                return BadRequest("Invalid course ID.");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var updated = await _service.UpdateAsync(id, dto);

            if (!updated)
                return NotFound($"Course with ID {id} not found.");

            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error updating course: {ex.Message}");
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            if (id <= 0)
                return BadRequest("Invalid course ID.");

            var deleted = await _service.DeleteAsync(id);

            if (!deleted)
                return NotFound($"Course with ID {id} not found.");

            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error deleting course: {ex.Message}");
        }
    }
}
