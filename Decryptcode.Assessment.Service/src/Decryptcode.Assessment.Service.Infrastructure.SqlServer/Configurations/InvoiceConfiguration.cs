using Decryptcode.Assessment.Service.Domain;
using Decryptcode.Assessment.Service.Domain.Entities.ReferenceEntities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Decryptcode.Assessment.Service.Infrastructure.SqlServer.Configurations;

public sealed class InvoiceConfiguration : BaseEntityConfiguration<Invoice>
{
    public override void Configure(EntityTypeBuilder<Invoice> builder)
    {
        base.Configure(builder);

        builder.Property(e => e.Currency)
            .HasMaxLength(DomainPropertiesConstraints.MAX_VALUE_CURRENCY_TEXT);

        builder.Property(e => e.DueDate)
            .IsRequired(false);

        builder.Property(e => e.IssuedAt)
            .IsRequired(false);

        builder.Property(e => e.Description)
            .HasMaxLength(DomainPropertiesConstraints.MAX_VALUE_LONG_DESCRIPTION);

        builder.Property(e => e.Status)
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();

        // Diferent from the previous relationships, here we should put the Invoices deletion as restrcit
        // this invoices could be valuable audit data, so should be careful managed.
        builder.HasOne(e => e.Organization)
            .WithMany(e => e.Invoices)
            .HasForeignKey(e => e.OrgId)
            .OnDelete(DeleteBehavior.Restrict);

        // Diferent from the previous relationships, here we should put the Invoices deletion as restrcit
        // this invoices could be valuable audit data, so should be careful managed.
        builder.HasOne(e => e.Project)
            .WithMany(e => e.Invoices)
            .HasForeignKey(e => e.ProjectId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
