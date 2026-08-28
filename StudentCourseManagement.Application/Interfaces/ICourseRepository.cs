using StudentCourseManagement.Domain.Entities;

// ICourseRepository.cs
namespace StudentCourseManagement.Application.Interfaces;

public interface ICourseRepository
{
    Task<List<Course>> GetAllAsync();
    Task<Course?> GetByIdAsync(int id);
    Task<Course> AddAsync(Course course);
    Task UpdateAsync(Course course);
    Task DeleteAsync(int id);

    Task<int> GetEnrolledStudentCountAsync(int courseId);
    Task<int> GetStudentEnrolledCoursesCountAsync(int studentId); // NEW
    Task<bool> IsStudentEnrolledAsync(int studentId, int courseId);
    Task EnrollStudentAsync(int studentId, int courseId);
    Task UnenrollStudentAsync(int studentId, int courseId);
}