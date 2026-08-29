using Microsoft.EntityFrameworkCore;
using StudentCourseManagement.Application.DTOs;
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

    // CourseRepository.cs

    public async Task EnrollStudentAsync(int studentId, int courseId)
    {
        var enrollment = new StudentCourseManagement.Infrastructure.Entities.StudentCourse
        {
            StudentId = studentId,
            CourseId = courseId,
            EnrolledOn = DateTime.UtcNow
        };

        await _context.StudentCourses.AddAsync(enrollment);

        var pendingRequests = await _context.EnrollmentRequests
            .Where(r => r.StudentId == studentId && r.CourseId == courseId && r.Status == "Pending")
            .ToListAsync();

        if (pendingRequests.Any())
        {
            _context.EnrollmentRequests.RemoveRange(pendingRequests);
        }

        await _context.SaveChangesAsync();
    }

    public async Task UnenrollStudentAsync(int studentId, int courseId)
    {
        var enrollment = await _context.StudentCourses
            .FirstOrDefaultAsync(sc => sc.StudentId == studentId && sc.CourseId == courseId);

        if (enrollment != null)
        {
            _context.StudentCourses.Remove(enrollment);
        }

        var pendingRequests = await _context.EnrollmentRequests
            .Where(r => r.StudentId == studentId && r.CourseId == courseId && r.Status == "Pending")
            .ToListAsync();

        if (pendingRequests.Any())
        {
            _context.EnrollmentRequests.RemoveRange(pendingRequests);
        }

        await _context.SaveChangesAsync();
    }
    public async Task<int> GetStudentEnrolledCoursesCountAsync(int studentId)
    {
        return await _context.StudentCourses.CountAsync(sc => sc.StudentId == studentId);
    }


    public async Task<(bool Success, string Message)> CreateEnrollmentRequestAsync(int studentId, int courseId, string requestType, string? reason)
    {
        var course = await _context.Courses.FirstOrDefaultAsync(c => c.Id == courseId);
        if (course == null)
            return (false, "Course does not exist.");

        bool isEnrolled = await _context.StudentCourses.AnyAsync(sc => sc.StudentId == studentId && sc.CourseId == courseId);

        if (requestType == "Enroll" && isEnrolled)
            return (false, "You are already enrolled in this course.");

        if (requestType == "Unenroll" && !isEnrolled)
            return (false, "You cannot request unenrollment from a course you are not enrolled in.");

        var existingPending = await _context.EnrollmentRequests
            .AnyAsync(r => r.StudentId == studentId && r.CourseId == courseId && r.RequestType == requestType && r.Status == "Pending");

        if (existingPending)
            return (false, $"You already have a pending {requestType.ToLower()}ment request for this course.");

        var request = new StudentCourseManagement.Infrastructure.Entities.EnrollmentRequest
        {
            StudentId = studentId,
            CourseId = courseId,
            RequestType = requestType,
            Reason = reason ?? $"Requesting to {requestType.ToLower()} course.",
            Status = "Pending",
            RequestedOn = DateTime.UtcNow
        };

        await _context.EnrollmentRequests.AddAsync(request);
        await _context.SaveChangesAsync();

        return (true, $"{requestType}ment request submitted successfully to the Admin.");
    }

    public async Task<List<EnrollmentRequestResponseDto>> GetPendingEnrollmentRequestsAsync()
    {
        return await _context.EnrollmentRequests
            .Include(r => r.Student)
            .Include(r => r.Course)
            .Where(r => r.Status == "Pending")
            .Select(r => new EnrollmentRequestResponseDto
            {
                RequestId = r.Id,
                StudentId = r.StudentId,
                StudentName = r.Student.Name,
                CourseId = r.CourseId,
                CourseName = r.Course.Name,
                // Explicitly label both request types clearly
                RequestType = r.RequestType == "Unenroll" ? "Unenrollment Request" : "Enrollment Request",
                Reason = r.Reason ?? string.Empty,
                Status = r.Status,
                RequestedOn = r.RequestedOn
            }).ToListAsync();
    }

    public async Task<(bool Success, string Message)> ProcessEnrollmentRequestAsync(int requestId, bool approve)
    {
        // 1. Find the request ticket
        var request = await _context.EnrollmentRequests.FirstOrDefaultAsync(r => r.Id == requestId);
        if (request == null)
            return (false, $"Request ticket #{requestId} was not found or has already been processed.");

        // 2. Handle Rejection: Simply delete the request ticket and inform the Admin
        if (!approve)
        {
            _context.EnrollmentRequests.Remove(request);
            await _context.SaveChangesAsync();
            return (true, $"Request #{requestId} was rejected and removed from the queue.");
        }

        // 3. Handle Approval: Auto-detect RequestType and execute appropriate action
        if (request.RequestType == "Enroll")
        {
            // Check duplication
            bool isEnrolled = await _context.StudentCourses.AnyAsync(sc => sc.StudentId == request.StudentId && sc.CourseId == request.CourseId);
            if (isEnrolled)
            {
                _context.EnrollmentRequests.Remove(request);
                await _context.SaveChangesAsync();
                return (true, "Student was already enrolled. Request ticket cleared from queue.");
            }

            // Check Max 7 Courses Limit
            int studentCourseCount = await _context.StudentCourses.CountAsync(sc => sc.StudentId == request.StudentId);
            if (studentCourseCount >= 7)
                return (false, "Cannot approve: Student has already reached the maximum limit of 7 enrolled courses.");

            // Check Max 50 Capacity Limit
            int courseStudentCount = await _context.StudentCourses.CountAsync(sc => sc.CourseId == request.CourseId);
            if (courseStudentCount >= 50)
                return (false, "Cannot approve: Course capacity has reached the maximum of 50 students.");

            // Add enrollment record
            var enrollment = new StudentCourseManagement.Infrastructure.Entities.StudentCourse
            {
                StudentId = request.StudentId,
                CourseId = request.CourseId,
                EnrolledOn = DateTime.UtcNow
            };
            await _context.StudentCourses.AddAsync(enrollment);
        }
        else if (request.RequestType == "Unenroll")
        {
            var enrollment = await _context.StudentCourses
                .FirstOrDefaultAsync(sc => sc.StudentId == request.StudentId && sc.CourseId == request.CourseId);

            if (enrollment != null)
            {
                _context.StudentCourses.Remove(enrollment);
            }
        }

        // 4. Clean up ticket: Remove satisfied request from DB so it leaves the queue
        _context.EnrollmentRequests.Remove(request);
        await _context.SaveChangesAsync();

        return (true, $"Request #{requestId} approved! Student successfully {request.RequestType.ToLower()}ed.");
    }
}