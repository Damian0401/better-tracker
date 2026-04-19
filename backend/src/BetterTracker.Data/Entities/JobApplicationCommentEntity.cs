using BetterTracker.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BetterTracker.Data.Entities;

public sealed record JobApplicationCommentEntity : BaseEntity<Guid>
{
    public required Guid JobApplicationId { get; set; }
    public required string Content { get; set; }
    public JobApplicationEntity? JobApplication { get; set; }

    internal class Configuration : BaseConfiguration<JobApplicationCommentEntity>
    {
        public override void Configure(EntityTypeBuilder<JobApplicationCommentEntity> builder)
        {
            base.Configure(builder);
            builder.ToTable("JobApplicationComments", DatabaseSchemas.Default);
            builder.Property(e => e.Id).HasValueGenerator<GuidV7ValueGenerator>();
            builder.Property(e => e.JobApplicationId).IsRequired();
            builder.Property(e => e.Content).HasMaxLength(2000).IsRequired();
            builder.HasOne(e => e.JobApplication)
                .WithMany()
                .HasForeignKey(e => e.JobApplicationId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
