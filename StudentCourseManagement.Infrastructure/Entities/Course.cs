using System;
using System.Collections.Generic;

namespace StudentCourseManagement.Infrastructure.Entities;

public partial class Course
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public int Credits { get; set; }

    public int StudentId { get; set; }

    public virtual Student Student { get; set; } = null!;
}
