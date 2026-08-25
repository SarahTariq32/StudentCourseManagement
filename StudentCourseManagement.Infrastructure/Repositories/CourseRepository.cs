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
            entity.StudentId = course.StudentId;

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
}