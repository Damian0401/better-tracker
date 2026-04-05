using BetterTracker.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BetterTracker.Data.Entities;

public sealed record UserEntity : BaseEntity<Guid>
{
    public required string UserName { get; set; }
    public required string Login { get; set; }
    public required string PasswordHash { get; set; }

    internal class Configuration : BaseConfiguration<UserEntity>
    {
        public override void Configure(EntityTypeBuilder<UserEntity> builder)
        {
            base.Configure(builder);
            builder.ToTable("Users", DatabaseSchemas.Default);
            builder.Property(e => e.Id).HasValueGenerator<GuidV7ValueGenerator>();
            builder.Property(e => e.UserName).HasMaxLength(100).IsRequired();
            builder.Property(e => e.Login).HasMaxLength(100).IsRequired();
            builder.HasIndex(e => e.Login).IsUnique();
            builder.Property(e => e.PasswordHash).HasMaxLength(500).IsRequired();
        }
    }
}
