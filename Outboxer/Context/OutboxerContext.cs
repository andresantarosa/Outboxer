using Microsoft.EntityFrameworkCore;
using Outboxer.Enums;
using Outboxer.Models;
using Outboxer.Transactional;

namespace Outboxer.Context;

public class OutboxerContext<TContext> : DbContext where TContext : DbContext
{
    private readonly ITransactionalMessageContainer _transactionalMessageContainer;

    protected OutboxerContext(DbContextOptions<TContext> options,
        ITransactionalMessageContainer transactionalMessageContainer) : base(options)
    {
        _transactionalMessageContainer = transactionalMessageContainer;
    }

    public Entry Outbox { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        ConfigureOutbox(modelBuilder);
        base.OnModelCreating(modelBuilder);
    }

    private void ConfigureOutbox(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Entry>()
            .HasKey(b => b.Id);

        modelBuilder.Entity<Entry>()
            .Property(b => b.Destination)
            .IsRequired()
            .HasDefaultValue("");
        
        modelBuilder.Entity<Entry>()
            .Property(b => b.Content)
            .IsRequired()
            .HasDefaultValue("");
        
        modelBuilder.Entity<Entry>()
            .Property(b => b.Retries)
            .IsRequired()
            .HasDefaultValue(5);
        
        modelBuilder.Entity<Entry>()
            .Property(b => b.RetriesCount)
            .IsRequired()
            .HasDefaultValue(0);
        
        modelBuilder.Entity<Entry>()
            .Property(b => b.DateCreated)
            .IsRequired()
            .HasDefaultValueSql("getdate()");
        
        modelBuilder.Entity<Entry>()
            .Property(b => b.LastInteraction)
            .IsRequired()
            .HasDefaultValueSql("getdate()");
        
        modelBuilder.Entity<Entry>()
            .Property(b => b.Status)
            .IsRequired()
            .HasDefaultValue(StatusEnum.PENDING);
        
        
        modelBuilder.Entity<Entry>().ToTable("Outbox");
    }

    /// <summary>
    /// Overrides EF method to be able to dispatch messages to Channel when commit is successful
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
    {
        SetEnqueued();

        var saveChangesResult = await base.SaveChangesAsync(cancellationToken);

        try
        {
            await _transactionalMessageContainer.Dispatch();
        }
        catch (Exception e)
        {
            throw new Exception(
                "Outboxer - One or more messages were not dispatched but they are saved at Outboxer outbox table as enqueued.");
        }

        return saveChangesResult;
    }

    /// <summary>
    /// Overrides EF method to be able to dispatch messages to Channel when commit is successful
    /// </summary>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public override int SaveChanges()
    {
        SetEnqueued();
        var saveChangesResult = base.SaveChanges();
        try
        {
            _transactionalMessageContainer.Dispatch().GetAwaiter().GetResult();
        }
        catch (Exception e)
        {
            throw new Exception(
                "Outboxer - One or more messages were not dispatched but they are saved at Outboxer outbox table as enqueued.");
        }

        return saveChangesResult;
    }

    /// <summary>
    /// Reads the tracked Entries and set them as PENDING so when the SaveChanges ocurrs they will be at the correct status
    /// </summary>
    private void SetEnqueued()
    {
        foreach (var entry in ChangeTracker.Entries<Entry>())
        {
            if (entry.Entity.Status == StatusEnum.PENDING)
                entry.Entity.SetEnqueued();
        }
    }
}