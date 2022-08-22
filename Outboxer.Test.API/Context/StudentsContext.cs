using Microsoft.EntityFrameworkCore;
using Outboxer.Context;
using Outboxer.Test.API.Entities;
using Outboxer.Transactional;

namespace Outboxer.Test.API.Context;

public class StudentsContext : OutboxerContext<StudentsContext>
{
    public StudentsContext(DbContextOptions<StudentsContext> options,
        ITransactionalMessageContainer transactionalMessageContainer) : base(options, transactionalMessageContainer)
    {
    }

    public Student Student { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Student>().HasKey(b => b.Id);
        modelBuilder.Entity<Student>().Property(b => b.Name).HasMaxLength(20);
        base.OnModelCreating(modelBuilder);
    }
}