using StudentCourseManagement.Application.DTOs;
using StudentCourseManagement.Application.Interfaces;
using StudentCourseManagement.Domain.Entities;

namespace StudentCourseManagement.Application.Services;

public class StudentService : IStudentService
{
    private readonly IStudentRepository _repository;

    public StudentService(IStudentRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<StudentDto>> GetAllAsync()
    {
        var students = await _repository.GetAllAsync();
        return students.Select(s => new StudentDto
        {
            Id = s.Id,
            Name = s.Name,
            Email = s.Email,
            Age = s.Age,
            EnrolledCourses = s.StudentCourses?
                .Where(sc => sc.Course != null)
                .Select(sc => sc.Course.Name)
                .ToList() ?? new List<string>()
        }).ToList();
    }

    public async Task<StudentDto?> GetByIdAsync(int id)
    {
        var student = await _repository.GetByIdAsync(id);
        if (student == null) return null;

        return new StudentDto
        {
            Id = student.Id,
            Name = student.Name,
            Email = student.Email,
            Age = student.Age,
            EnrolledCourses = student.StudentCourses?
                .Where(sc => sc.Course != null)
                .Select(sc => sc.Course.Name)
                .ToList() ?? new List<string>()
        };
    }

    public async Task<StudentDto> CreateAsync(CreateStudentDto dto)
    {
        try
        {
            var student = new Student
            {
                Name = dto.Name,
                Email = dto.Email,
                Age = dto.Age
            };

            var created = await _repository.AddAsync(student);

            return new StudentDto
            {
                Id = created.Id,
                Name = created.Name,
                Email = created.Email,
                Age = created.Age
            };
        }
        catch (Exception ex)
        {
            throw new Exception($"Error creating student: {ex.Message}", ex);
        }
    }

    public async Task<bool> UpdateAsync(int id, UpdateStudentDto dto)
    {
        try
        {
            var student = await _repository.GetByIdAsync(id);

            if (student == null)
                return false;

            student.Name = dto.Name;
            student.Email = dto.Email;
            student.Age = dto.Age;

            await _repository.UpdateAsync(student);

            return true;
        }
        catch (Exception ex)
        {
            throw new Exception($"Error updating student with ID {id}: {ex.Message}", ex);
        }
    }

    public async Task<bool> DeleteAsync(int id)
    {
        try
        {
            var student = await _repository.GetByIdAsync(id);

            if (student == null)
                return false;

            await _repository.DeleteAsync(id);

            return true;
        }
        catch (Exception ex)
        {
            throw new Exception($"Error deleting student with ID {id}: {ex.Message}", ex);
        }
    }
}