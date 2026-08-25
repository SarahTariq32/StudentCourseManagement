using DomainStudent = StudentCourseManagement.Domain.Entities.Student;
using InfrastructureStudent = StudentCourseManagement.Infrastructure.Entities.Student;

namespace StudentCourseManagement.Infrastructure.Mappings;

public static class StudentMapping
{
    public static DomainStudent ToDomain(this InfrastructureStudent student)
    {
        return new DomainStudent
        {
            Id = student.Id,
            Name = student.Name,
            Email = student.Email,
            Age = student.Age
        };
    }

    public static InfrastructureStudent ToInfrastructure(this DomainStudent student)
    {
        return new InfrastructureStudent
        {
            Id = student.Id,
            Name = student.Name,
            Email = student.Email,
            Age = student.Age
        };
    }
}