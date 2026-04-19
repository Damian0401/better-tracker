using BetterTracker.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BetterTracker.Data.Entities;

public sealed record JobApplicationTagEntity : BaseEntity<Guid>
{
    public required Guid JobApplicationId { get; set; }
    public required Guid TagId { get; set; }
    public JobApplicationEntity? JobApplication { get; set; }
    public TagEntity? Tag { get; set; }

    internal class Configuration : BaseConfiguration<JobApplicationTagEntity>
    {
        public override void Configure(EntityTypeBuilder<JobApplicationTagEntity> builder)
        {
            base.Configure(builder);
            builder.ToTable("JobApplicationTags", DatabaseSchemas.Default);
            builder.Property(e => e.Id).HasValueGenerator<GuidV7ValueGenerator>();
            builder.Property(e => e.JobApplicationId).IsRequired();
            builder.Property(e => e.TagId).IsRequired();
            builder.HasOne(e => e.JobApplication)
                .WithMany()
                .HasForeignKey(e => e.JobApplicationId)
                .OnDelete(DeleteBehavior.Cascade);
            builder.HasOne(e => e.Tag)
                .WithMany()
                .HasForeignKey(e => e.TagId)
                .OnDelete(DeleteBehavior.Cascade);
            builder.HasIndex(e => new { e.JobApplicationId, e.TagId }).IsUnique();
        }
    }
}
