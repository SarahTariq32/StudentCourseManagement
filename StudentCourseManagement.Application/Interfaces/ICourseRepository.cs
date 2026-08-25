using StudentCourseManagement.Domain.Entities;

namespace StudentCourseManagement.Application.Interfaces;

public interface ICourseRepository
{
    Task<List<Course>> GetAllAsync();

    Task<Course?> GetByIdAsync(int id);

    Task<Course> AddAsync(Course course);

    Task UpdateAsync(Course course);

    Task DeleteAsync(int id);
}