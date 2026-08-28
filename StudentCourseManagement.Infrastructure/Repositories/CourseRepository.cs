using Microsoft.EntityFrameworkCore;
using StudentCourseManagement.Application.Interfaces;
using StudentCourseManagement.Domain.Entities;
using StudentCourseManagement.Infrastructure.Data;
using StudentCourseManagement.Infrastructure.Mappings;

using InfrastructureCourse = StudentCourseManagement.Infrastructure.Entities.Course;

namespace StudentCourseManagement.Infrastructure.Repositories;

public class CourseRepository : ICourseRepository
{
    private readonly ApplicationDbContext _context;

    public CourseRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<Course>> GetAllAsync()
    {
        try
        {
            var courses = await _context.Courses.ToListAsync();

            return courses.Select(c => c.ToDomain()).ToList();
        }
        catch (Exception ex)
        {
            throw new Exception($"Error retrieving all courses: {ex.Message}", ex);
        }
    }

    public async Task<Course?> GetByIdAsync(int id)
    {
        try
        {
            var course = await _context.Courses
                .FirstOrDefaultAsync(c => c.Id == id);

            return course?.ToDomain();
        }
        catch (Exception ex)
        {
            throw new Exception($"Error retrieving course with ID {id}: {ex.Message}", ex);
        }
    }

    public async Task<Course> AddAsync(Course course)
    {
        try
        {
            InfrastructureCourse entity = course.ToInfrastructure();

            _context.Courses.Add(entity);
            await _context.SaveChangesAsync();

            return entity.ToDomain();
        }
        catch (Exception ex)
        {
            throw new Exception($"Error adding course: {ex.Message}", ex);
        }
    }

    public async Task UpdateAsync(Course course)
    {
        try
        {
            var entity = await _context.Courses
                .FirstOrDefaultAsync(c => c.Id == course.Id);

            if (entity == null)
                return;

            entity.Name = course.Name;
            entity.Credits = course.Credits;

            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            throw new Exception($"Error updating course with ID {course.Id}: {ex.Message}", ex);
        }
    }

    public async Task DeleteAsync(int id)
    {
        try
        {
            var entity = await _context.Courses
                .FirstOrDefaultAsync(c => c.Id == id);

            if (entity == null)
                return;

            _context.Courses.Remove(entity);
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            throw new Exception($"Error deleting course with ID {id}: {ex.Message}", ex);
        }
    }
    // CourseRepository.cs (Additions to implementation)
    public async Task<int> GetEnrolledStudentCountAsync(int courseId)
    {
        return await _context.StudentCourses.CountAsync(sc => sc.CourseId == courseId);
    }

    public async Task<bool> IsStudentEnrolledAsync(int studentId, int courseId)
    {
        return await _context.StudentCourses.AnyAsync(sc => sc.StudentId == studentId && sc.CourseId == courseId);
    }

    public async Task EnrollStudentAsync(int studentId, int courseId)
    {
        var enrollment = new Infrastructure.Entities.StudentCourse
        {
            StudentId = studentId,
            CourseId = courseId,
            EnrolledOn = DateTime.UtcNow
        };
        await _context.StudentCourses.AddAsync(enrollment);
        await _context.SaveChangesAsync();
    }

    public async Task UnenrollStudentAsync(int studentId, int courseId)
    {
        var enrollment = await _context.StudentCourses
            .FirstOrDefaultAsync(sc => sc.StudentId == studentId && sc.CourseId == courseId);

        if (enrollment != null)
        {
            _context.StudentCourses.Remove(enrollment);
            await _context.SaveChangesAsync();
        }
    }
    public async Task<int> GetStudentEnrolledCoursesCountAsync(int studentId)
    {
        return await _context.StudentCourses.CountAsync(sc => sc.StudentId == studentId);
    }
}