using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using StudentCourseManagement.Application.Interfaces;
using StudentCourseManagement.Domain.Entities;
using StudentCourseManagement.Infrastructure.AuthEntities;
using StudentCourseManagement.Infrastructure.Data;

namespace StudentCourseManagement.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly ApplicationDbContext _context;

    public UserRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<User?> GetByUsernameAsync(string username)
    {
        var user = await _context.UsersData
            .FirstOrDefaultAsync(u => u.Username == username);

        if (user == null)
            return null;

        return new User
        {
            Id = user.Id,
            Username = user.Username,
            PasswordHash = user.PasswordHash,
            Role = user.Role
        };
    }

    public async Task AddAsync(User user)
    {
        var entity = new UsersDatum
        {
            Username = user.Username,
            PasswordHash = user.PasswordHash,
            Role = user.Role
        };

        await _context.UsersData.AddAsync(entity);
        await _context.SaveChangesAsync();
    }
}