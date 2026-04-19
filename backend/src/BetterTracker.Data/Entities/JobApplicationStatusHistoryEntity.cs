using BetterTracker.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BetterTracker.Data.Entities;

public sealed record JobApplicationStatusHistoryEntity : BaseEntity<Guid>
{
    public required Guid JobApplicationId { get; set; }
    public JobApplicationStatus? PreviousStatus { get; set; }
    public required JobApplicationStatus NewStatus { get; set; }
    public JobApplicationEntity? JobApplication { get; set; }

    internal class Configuration : BaseConfiguration<JobApplicationStatusHistoryEntity>
    {
        public override void Configure(EntityTypeBuilder<JobApplicationStatusHistoryEntity> builder)
        {
            base.Configure(builder);
            builder.ToTable("JobApplicationStatusHistory", DatabaseSchemas.Default);
            builder.Property(e => e.Id).HasValueGenerator<GuidV7ValueGenerator>();
            builder.Property(e => e.JobApplicationId).IsRequired();
            builder.Property(e => e.PreviousStatus);
            builder.Property(e => e.NewStatus).IsRequired();
            builder.HasOne(e => e.JobApplication)
                .WithMany()
                .HasForeignKey(e => e.JobApplicationId)
                .OnDelete(DeleteBehavior.Cascade);
            builder.HasIndex(e => new { e.JobApplicationId, e.CreatedAt });
        }
    }
}
