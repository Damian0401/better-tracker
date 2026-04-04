using BetterTracker.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BetterTracker.Data.Entities;

public sealed record NoteEntity : BaseEntity<Guid>
{
    public required string Title { get; set; }
    public required string Content { get; set; }

    internal class Configuration : BaseConfiguration<NoteEntity>
    {
        public override void Configure(EntityTypeBuilder<NoteEntity> builder)
        {
            base.Configure(builder);
            builder.ToTable("Notes", DatabaseSchemas.Default);
            builder.Property(e => e.Id).HasValueGenerator<GuidV7ValueGenerator>();
            builder.Property(e => e.Title).HasMaxLength(100).IsRequired();
            builder.Property(e => e.Content).HasMaxLength(500).IsRequired();
        }
    }
}
