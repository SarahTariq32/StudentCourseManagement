using Microsoft.AspNetCore.Mvc;
using StudentCourseManagement.Application.DTOs;
using StudentCourseManagement.Application.Interfaces;

namespace StudentCourseManagement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StudentsController : ControllerBase
{
    private readonly IStudentService _service;

    public StudentsController(IStudentService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        try
        {
            var students = await _service.GetAllAsync();
            return Ok(students);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error retrieving students: {ex.Message}");
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        try
        {
            if (id <= 0)
                return BadRequest("Invalid student ID.");

            var student = await _service.GetByIdAsync(id);

            if (student == null)
                return NotFound($"Student with ID {id} not found.");

            return Ok(student);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error retrieving student: {ex.Message}");
        }
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateStudentDto dto)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var student = await _service.CreateAsync(dto);

            return CreatedAtAction(
                nameof(GetById),
                new { id = student.Id },
                student);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error creating student: {ex.Message}");
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, UpdateStudentDto dto)
    {
        try
        {
            if (id <= 0)
                return BadRequest("Invalid student ID.");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var updated = await _service.UpdateAsync(id, dto);

            if (!updated)
                return NotFound($"Student with ID {id} not found.");

            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error updating student: {ex.Message}");
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            if (id <= 0)
                return BadRequest("Invalid student ID.");

            var deleted = await _service.DeleteAsync(id);

            if (!deleted)
                return NotFound($"Student with ID {id} not found.");

            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error deleting student: {ex.Message}");
        }
    }
}