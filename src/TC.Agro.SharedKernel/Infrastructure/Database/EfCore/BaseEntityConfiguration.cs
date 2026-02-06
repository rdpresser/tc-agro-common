namespace TC.Agro.SharedKernel.Infrastructure.Database.EfCore
{
    /// <summary>
    /// Base EF Core configuration for aggregate roots.
    /// Provides standard mapping for Id, CreatedAt, UpdatedAt, and IsActive properties.
    /// </summary>
    /// <typeparam name="TEntity">The aggregate root type to configure.</typeparam>
    [ExcludeFromCodeCoverage]
    public abstract class BaseEntityConfiguration<TEntity> : IEntityTypeConfiguration<TEntity>
        where TEntity : BaseAggregateRoot
    {
        public virtual void Configure(EntityTypeBuilder<TEntity> builder)
        {
            // Primary Key
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id)
                .IsRequired()
                .ValueGeneratedOnAdd();

            // Audit fields
            builder.Property(x => x.CreatedAt)
                .IsRequired()
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamptz")
                .ValueGeneratedOnAdd();

            builder.Property(x => x.UpdatedAt)
                .IsRequired(false)
                .HasColumnType("timestamptz");

            // Soft delete / active flag
            builder.Property(x => x.IsActive)
                .IsRequired()
                .HasDefaultValue(true);

            // Ignore domain events collection - not persisted as separate entity
            builder.Ignore(x => x.UncommittedEvents);
        }
    }
}
