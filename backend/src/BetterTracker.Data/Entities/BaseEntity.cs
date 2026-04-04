using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace BetterTracker.Data.Entities;

public abstract record BaseEntity<TKey> : ITimeTrackable
    where TKey : struct
{
    public TKey Id { get; init; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }

    public abstract class BaseConfiguration<TEntity> : IEntityTypeConfiguration<TEntity>
        where TEntity : BaseEntity<TKey>
    {
        public virtual void Configure(EntityTypeBuilder<TEntity> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.CreatedAt).HasConversion(new DateTimeOffsetToBinaryConverter());
            builder.Property(x => x.UpdatedAt).HasConversion(new DateTimeOffsetToBinaryConverter());
        }
    }
}
