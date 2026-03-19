using Decryptcode.Assessment.Service.Domain;
using Decryptcode.Assessment.Service.Domain.Entities.ReferenceEntities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Decryptcode.Assessment.Service.Infrastructure.SqlServer.Configurations;

public sealed class ProjectConfiguration : BaseEntityConfiguration<Project>
{
    public override void Configure(EntityTypeBuilder<Project> builder)
    {
        base.Configure(builder);

        builder.ToTable("Projects");

        builder.Property(e => e.Name)
            .HasMaxLength(DomainPropertiesConstraints.MAX_VALUE_SHORT_NAME);

        builder.Property(e => e.StartDate)
            .IsRequired(false);

        builder.Property(e => e.EndDate)
            .IsRequired(false);

        builder.Property(e => e.Description)
            .HasMaxLength(DomainPropertiesConstraints.MAX_VALUE_LONG_DESCRIPTION);

        // Store enum as string in the database so projections can read the provider value reliably
        builder.Property(e => e.Status)
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();

        // Here we are using Delete Cascade because we don't have any requirement to block 
        // Organizations deletion if any project is still assign, so, once we delete the organization
        // all projects associated with it should be deleted as well.
        // The second point that secure that we can use delete cascade over here is that one project just exist inside
        // an organization, we won't have one project that exist in Org-1 and Org-2 at the same time. At least by the mock data
        builder.HasOne(e => e.Organization)
            .WithMany(e => e.Projects)
            .HasForeignKey(e => e.OrgId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
