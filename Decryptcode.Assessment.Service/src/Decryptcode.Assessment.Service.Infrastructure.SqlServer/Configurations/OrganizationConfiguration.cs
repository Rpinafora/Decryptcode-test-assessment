using Decryptcode.Assessment.Service.Domain;
using Decryptcode.Assessment.Service.Domain.Entities.AggregateRoots;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Decryptcode.Assessment.Service.Infrastructure.SqlServer.Configurations;

public sealed class OrganizationConfiguration : BaseEntityConfiguration<Organization>
{
    public override void Configure(EntityTypeBuilder<Organization> builder)
    {
        base.Configure(builder);

        builder.Property(x => x.Name)
            .HasMaxLength(DomainPropertiesConstraints.MAX_VALUE_SHORT_NAME);

        builder.Property(x => x.Slug)
            .HasMaxLength(DomainPropertiesConstraints.MAX_VALUE_SHORT_NAME);

        builder.Property(x => x.Industry)
            .HasMaxLength(DomainPropertiesConstraints.MAX_VALUE_SHORT_NAME);

        builder.Property(x => x.Tier)
            .HasMaxLength(DomainPropertiesConstraints.MAX_VALUE_SHORT_NAME);

        builder.Property(x => x.ContactEmail)
            .HasMaxLength(DomainPropertiesConstraints.MAX_VALUE_EMAIL);

        builder.Property(x => x.Description)
            .HasMaxLength(DomainPropertiesConstraints.MAX_VALUE_LONG_DESCRIPTION);

        builder.OwnsOne(x => x.Settings);
        builder.OwnsOne(x => x.Metadata);

    }
}
