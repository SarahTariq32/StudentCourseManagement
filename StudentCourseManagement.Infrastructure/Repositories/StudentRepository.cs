using Microsoft.EntityFrameworkCore;
using StudentCourseManagement.Application.Interfaces;
using StudentCourseManagement.Domain.Entities;
using StudentCourseManagement.Infrastructure.Data;
using StudentCourseManagement.Infrastructure.Mappings;

using InfrastructureStudent = StudentCourseManagement.Infrastructure.Entities.Student;

namespace StudentCourseManagement.Infrastructure.Repositories;

public class StudentRepository : IStudentRepository
{
    private readonly ApplicationDbContext _context;

    public StudentRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<Student>> GetAllAsync()
    {
        try
        {
            var students = await _context.Students
                .Include(s => s.Courses)
                .ToListAsync();

            return students.Select(s => s.ToDomain()).ToList();
        }
        catch (Exception ex)
        {
            throw new Exception($"Error retrieving all students: {ex.Message}", ex);
        }
    }

    public async Task<Student?> GetByIdAsync(int id)
    {
        try
        {
            var student = await _context.Students
                .Include(s => s.Courses)
                .FirstOrDefaultAsync(s => s.Id == id);

            return student?.ToDomain();
        }
        catch (Exception ex)
        {
            throw new Exception($"Error retrieving student with ID {id}: {ex.Message}", ex);
        }
    }

    public async Task<Student> AddAsync(Student student)
    {
        try
        {
            InfrastructureStudent entity = student.ToInfrastructure();

            _context.Students.Add(entity);
            await _context.SaveChangesAsync();

            return entity.ToDomain();
        }
        catch (Exception ex)
        {
            throw new Exception($"Error adding student: {ex.Message}", ex);
        }
    }

    public async Task UpdateAsync(Student student)
    {
        try
        {
            var entity = await _context.Students
                .FirstOrDefaultAsync(s => s.Id == student.Id);

            if (entity == null)
                return;

            entity.Name = student.Name;
            entity.Email = student.Email;
            entity.Age = student.Age;

            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            throw new Exception($"Error updating student with ID {student.Id}: {ex.Message}", ex);
        }
    }

    public async Task DeleteAsync(int id)
    {
        try
        {
            var entity = await _context.Students
                .FirstOrDefaultAsync(s => s.Id == id);

            if (entity == null)
                return;

            _context.Students.Remove(entity);
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            throw new Exception($"Error deleting student with ID {id}: {ex.Message}", ex);
        }
    }
}