using BetterTracker.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BetterTracker.Data.Entities;

public sealed record TagEntity : BaseEntity<Guid>
{
    public required Guid UserId { get; set; }
    public required string Name { get; set; }
    public UserEntity? User { get; set; }

    internal class Configuration : BaseConfiguration<TagEntity>
    {
        public override void Configure(EntityTypeBuilder<TagEntity> builder)
        {
            base.Configure(builder);
            builder.ToTable("Tags", DatabaseSchemas.Default);
            builder.Property(e => e.Id).HasValueGenerator<GuidV7ValueGenerator>();
            builder.Property(e => e.UserId).IsRequired();
            builder.Property(e => e.Name).HasMaxLength(50).IsRequired();
            builder.HasOne(e => e.User)
                .WithMany()
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);
            builder.HasIndex(e => new { e.UserId, e.Name }).IsUnique();
            builder.HasIndex(e => e.UserId);
        }
    }
}
