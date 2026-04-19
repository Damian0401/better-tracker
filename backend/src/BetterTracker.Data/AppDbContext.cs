using BetterTracker.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace BetterTracker.Data;

public sealed class AppDbContext : DbContext
{
    private readonly TimeProvider timeProvider;

    public AppDbContext(
        DbContextOptions<AppDbContext> options,
        TimeProvider timeProvider)
        : base(options)
    {
        this.timeProvider = timeProvider;
        this.ChangeTracker.Tracked += (_, args) => HandleStateChange(args, timeProvider);
        this.ChangeTracker.StateChanged += (_, args) => HandleStateChange(args, timeProvider);
    }

    public DbSet<NoteEntity> Notes => this.Set<NoteEntity>();
    public DbSet<UserEntity> Users => this.Set<UserEntity>();
    public DbSet<JobApplicationEntity> JobApplications => this.Set<JobApplicationEntity>();
    public DbSet<JobApplicationSalaryEntity> JobApplicationSalaries => this.Set<JobApplicationSalaryEntity>();
    public DbSet<JobApplicationCommentEntity> JobApplicationComments => this.Set<JobApplicationCommentEntity>();
    public DbSet<JobApplicationStatusHistoryEntity> JobApplicationStatusHistory => this.Set<JobApplicationStatusHistoryEntity>();
    public DbSet<TagEntity> Tags => this.Set<TagEntity>();
    public DbSet<JobApplicationTagEntity> JobApplicationTags => this.Set<JobApplicationTagEntity>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }

    private static void HandleStateChange(
        EntityEntryEventArgs args,
        TimeProvider timeProvider)
    {
        if (args.Entry.Entity is not ITimeTrackable timeTrackable)
        {
            return;
        }

        switch (args.Entry.State)
        {
            case EntityState.Added:
                timeTrackable.CreatedAt = timeProvider.GetUtcNow();
                timeTrackable.UpdatedAt = timeProvider.GetUtcNow();
                break;
            case EntityState.Modified:
                timeTrackable.UpdatedAt = timeProvider.GetUtcNow();
                break;
        }
    }
}
