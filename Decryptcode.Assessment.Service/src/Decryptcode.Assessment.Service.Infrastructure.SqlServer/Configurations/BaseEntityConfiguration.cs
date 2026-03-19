using Decryptcode.Assessment.Service.Domain;
using Decryptcode.Assessment.Service.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Decryptcode.Assessment.Service.Infrastructure.SqlServer.Configurations;

public class BaseEntityConfiguration<T> : IEntityTypeConfiguration<T> where T : BaseEntity
{
    public virtual void Configure(EntityTypeBuilder<T> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasColumnOrder(1)
            .ValueGeneratedNever();

        builder.Property(x => x.CreatedAt)
            .IsRequired();

        builder.Property(x => x.CreatedById)
            .HasMaxLength(DomainPropertiesConstraints.MAX_VALUE_FOR_STRING_IDS);

        builder.Property(x => x.UpdatedAt)
            .IsRequired(false);

        builder.Property(x => x.UpdatedById)
            .HasMaxLength(DomainPropertiesConstraints.MAX_VALUE_FOR_STRING_IDS);

        builder.Property(x => x.DeletedAt)
            .IsRequired(false);

        builder.Property(x => x.DeletedBy)
            .HasMaxLength(DomainPropertiesConstraints.MAX_VALUE_FOR_STRING_IDS);

        builder.Ignore(x => x.DomainEvents);
    }
}
