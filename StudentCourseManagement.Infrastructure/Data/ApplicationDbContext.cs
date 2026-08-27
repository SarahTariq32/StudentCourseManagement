using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using StudentCourseManagement.Infrastructure.Entities;
using StudentCourseManagement.Infrastructure.AuthEntities;
namespace StudentCourseManagement.Infrastructure.Data;

public partial class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Course> Courses { get; set; }

    public virtual DbSet<Student> Students { get; set; }

    public virtual DbSet<UsersDatum> UsersData { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Course>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Courses__3214EC0737508876");

            entity.Property(e => e.Name).HasMaxLength(100);

            entity.HasOne(d => d.Student).WithMany(p => p.Courses)
                .HasForeignKey(d => d.StudentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Courses_Students");
        });

        modelBuilder.Entity<Student>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Students__3214EC07F473CF4A");

            entity.Property(e => e.Email).HasMaxLength(150);
            entity.Property(e => e.Name).HasMaxLength(100);
        });

        modelBuilder.Entity<UsersDatum>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.HasIndex(e => e.Username)
                  .IsUnique();

            entity.Property(e => e.Username)
                  .HasMaxLength(100);

            entity.Property(e => e.PasswordHash)
                  .HasMaxLength(255);

            entity.Property(e => e.Role)
                  .HasMaxLength(50);

            entity.Property(e => e.RefreshToken)
                  .HasMaxLength(500);

            entity.Property(e => e.RefreshTokenExpiryTime);
        });
        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
