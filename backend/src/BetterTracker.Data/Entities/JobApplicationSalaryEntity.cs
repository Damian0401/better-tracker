using BetterTracker.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BetterTracker.Data.Entities;

public sealed record JobApplicationSalaryEntity : BaseEntity<Guid>
{
    public required Guid JobApplicationId { get; set; }
    public required SalaryType SalaryType { get; set; }
    public decimal? SalaryPost { get; set; }
    public decimal? SalaryCandidate { get; set; }
    public string? Currency { get; set; }
    public JobApplicationEntity? JobApplication { get; set; }

    internal class Configuration : BaseConfiguration<JobApplicationSalaryEntity>
    {
        public override void Configure(EntityTypeBuilder<JobApplicationSalaryEntity> builder)
        {
            base.Configure(builder);
            builder.ToTable("JobApplicationSalaries", DatabaseSchemas.Default);
            builder.Property(e => e.Id).HasValueGenerator<GuidV7ValueGenerator>();
            builder.Property(e => e.JobApplicationId).IsRequired();
            builder.Property(e => e.SalaryType).IsRequired();
            builder.Property(e => e.SalaryPost).HasPrecision(18, 2);
            builder.Property(e => e.SalaryCandidate).HasPrecision(18, 2);
            builder.Property(e => e.Currency).HasMaxLength(3);
            builder.HasOne(e => e.JobApplication)
                .WithMany()
                .HasForeignKey(e => e.JobApplicationId)
                .OnDelete(DeleteBehavior.Cascade);
            builder.HasIndex(e => new { e.JobApplicationId, e.SalaryType }).IsUnique();
        }
    }
}
