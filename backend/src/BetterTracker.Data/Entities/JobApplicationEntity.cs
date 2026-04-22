using BetterTracker.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BetterTracker.Data.Entities;

public sealed record JobApplicationEntity : BaseEntity<Guid>
{
    public required Guid UserId { get; set; }
    public required string JobTitle { get; set; }
    public string? Description { get; set; }
    public required string CompanyName { get; set; }
    public string? Requirements { get; set; }
    public string? Benefits { get; set; }
    public string? Link { get; set; }
    public string? Technologies { get; set; }
    public string? Experience { get; set; }
    public required WorkType WorkType { get; set; }
    public required JobApplicationStatus CurrentStatus { get; set; }
    public UserEntity? User { get; set; }

    internal class Configuration : BaseConfiguration<JobApplicationEntity>
    {
        public override void Configure(EntityTypeBuilder<JobApplicationEntity> builder)
        {
            base.Configure(builder);
            builder.ToTable("JobApplications", DatabaseSchemas.Default);
            builder.Property(e => e.Id).HasValueGenerator<GuidV7ValueGenerator>();
            builder.Property(e => e.UserId).IsRequired();
            builder.Property(e => e.JobTitle).HasMaxLength(200).IsRequired();
            builder.Property(e => e.Description);
            builder.Property(e => e.CompanyName).HasMaxLength(200).IsRequired();
            builder.Property(e => e.Requirements);
            builder.Property(e => e.Benefits);
            builder.Property(e => e.Link).HasMaxLength(500);
            builder.Property(e => e.Technologies);
            builder.Property(e => e.Experience);
            builder.Property(e => e.WorkType).IsRequired();
            builder.Property(e => e.CurrentStatus).IsRequired();
            builder.HasOne(e => e.User)
                .WithMany()
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);
            builder.HasIndex(e => new { e.UserId, e.CurrentStatus });
            builder.HasIndex(e => new { e.UserId, e.CreatedAt });
        }
    }
}
