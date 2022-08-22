using Microsoft.EntityFrameworkCore;
using Outboxer.Test.API.Context;
using Outboxer.Test.API.Entities;

namespace Outboxer.Test.API.Repository;

public class StudentRepository : IStudentRepository
{
    private DbSet<Student> DbSet { get; }

    public StudentRepository(StudentsContext context)
    {
        DbSet = context.Set<Student>();
    }
    
    public async Task Add(Student student)
    {
        await DbSet.AddAsync(student);
    }
}