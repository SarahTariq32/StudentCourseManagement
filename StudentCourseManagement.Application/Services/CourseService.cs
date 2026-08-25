using StudentCourseManagement.Application.DTOs;
using StudentCourseManagement.Application.Interfaces;
using StudentCourseManagement.Domain.Entities;

namespace StudentCourseManagement.Application.Services;

public class CourseService : ICourseService
{
    private readonly ICourseRepository _repository;

    public CourseService(ICourseRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<CourseDto>> GetAllAsync()
    {
        try
        {
            var courses = await _repository.GetAllAsync();

            return courses.Select(c => new CourseDto
            {
                Id = c.Id,
                Name = c.Name,
                Credits = c.Credits,
                StudentId = c.StudentId
            }).ToList();
        }
        catch (Exception ex)
        {
            throw new Exception($"Error retrieving all courses: {ex.Message}", ex);
        }
    }

    public async Task<CourseDto?> GetByIdAsync(int id)
    {
        try
        {
            var course = await _repository.GetByIdAsync(id);

            if (course == null)
                return null;

            return new CourseDto
            {
                Id = course.Id,
                Name = course.Name,
                Credits = course.Credits,
                StudentId = course.StudentId
            };
        }
        catch (Exception ex)
        {
            throw new Exception($"Error retrieving course with ID {id}: {ex.Message}", ex);
        }
    }

    public async Task<CourseDto> CreateAsync(CreateCourseDto dto)
    {
        try
        {
            var course = new Course
            {
                Name = dto.Name,
                Credits = dto.Credits,
                StudentId = dto.StudentId
            };

            var created = await _repository.AddAsync(course);

            return new CourseDto
            {
                Id = created.Id,
                Name = created.Name,
                Credits = created.Credits,
                StudentId = created.StudentId
            };
        }
        catch (Exception ex)
        {
            throw new Exception($"Error creating course: {ex.Message}", ex);
        }
    }

    public async Task<bool> UpdateAsync(int id, UpdateCourseDto dto)
    {
        try
        {
            var course = await _repository.GetByIdAsync(id);

            if (course == null)
                return false;

            course.Name = dto.Name;
            course.Credits = dto.Credits;
            course.StudentId = dto.StudentId;

            await _repository.UpdateAsync(course);

            return true;
        }
        catch (Exception ex)
        {
            throw new Exception($"Error updating course with ID {id}: {ex.Message}", ex);
        }
    }

    public async Task<bool> DeleteAsync(int id)
    {
        try
        {
            var course = await _repository.GetByIdAsync(id);

            if (course == null)
                return false;

            await _repository.DeleteAsync(id);

            return true;
        }
        catch (Exception ex)
        {
            throw new Exception($"Error deleting course with ID {id}: {ex.Message}", ex);
        }
    }
}