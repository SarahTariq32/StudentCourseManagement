using StudentCourseManagement.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentCourseManagement.Application.Interfaces
{
    public interface ICourseService
    {
        Task<List<CourseDto>> GetAllAsync();

        Task<CourseDto?> GetByIdAsync(int id);

        Task<CourseDto> CreateAsync(CreateCourseDto dto);

        Task<bool> UpdateAsync(int id, UpdateCourseDto dto);

        Task<bool> DeleteAsync(int id);
    }
}
