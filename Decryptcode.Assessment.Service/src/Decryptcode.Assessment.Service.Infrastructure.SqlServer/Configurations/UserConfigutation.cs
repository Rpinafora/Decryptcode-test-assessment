using Decryptcode.Assessment.Service.Domain;
using Decryptcode.Assessment.Service.Domain.Entities.ReferenceEntities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Decryptcode.Assessment.Service.Infrastructure.SqlServer.Configurations;

public sealed class UserConfigutation : BaseEntityConfiguration<User>
{
    public override void Configure(EntityTypeBuilder<User> builder)
    {
        base.Configure(builder);

        builder.ToTable("Users");

        builder.Property(x => x.Email)
            .HasMaxLength(DomainPropertiesConstraints.MAX_VALUE_EMAIL);

        builder.Property(x => x.Name)
            .HasMaxLength(DomainPropertiesConstraints.MAX_VALUE_SHORT_NAME);

        builder.Property(x => x.Role)
            .HasMaxLength(DomainPropertiesConstraints.MAX_VALUE_ROLE_TEXT);

        builder.Property(x => x.Bio)
            .HasMaxLength(DomainPropertiesConstraints.MAX_VALUE_USER_BIO);

        // Here we are using Delete Cascade because we don't have any requirement to block 
        // Organizations deletion if any user is still assign, so, once we delete the organization
        // all users associate with it should be deleted as well.
        // The second point that secure that we can use delete cascade over here is that one user just exist inside
        // an organization, we won't have one user that exist in Org-1 and Org-2 at the same time. At least by the mock data
        builder.HasOne(e => e.Organization)
            .WithMany(e => e.Users)
            .HasForeignKey(e => e.OrgId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
