using Decryptcode.Assessment.Service.Domain;
using Decryptcode.Assessment.Service.Domain.Entities.ReferenceEntities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Decryptcode.Assessment.Service.Infrastructure.SqlServer.Configurations;

public sealed class TimeEntryConfiguration : BaseEntityConfiguration<TimeEntry>
{
    public override void Configure(EntityTypeBuilder<TimeEntry> builder)
    {
        base.Configure(builder);

        builder.ToTable("TimeEntries");

        builder.Property(e => e.Hours)
            .HasMaxLength(DomainPropertiesConstraints.MAX_VALUE_TIME_ENTRY_HOURS);

        builder.Property(t => t.Hours).IsRequired();

        builder.ToTable(t =>
            t.HasCheckConstraint("CK_TimeEntry_Hours_Range", "[Hours] >= 0 AND [Hours] <= 24"));

        builder.Property(e => e.Description)
            .HasMaxLength(DomainPropertiesConstraints.MAX_VALUE_LONG_DESCRIPTION);

        // Diferent from the previous relationships, here we should put the Time Entries deletion as restrcit
        // this entries could be valuable audit data, so should be careful managed.
        builder.HasOne(e => e.User)
            .WithMany(e => e.TimeEntries)
            .HasForeignKey(e => e.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        // Diferent from the previous relationships, here we should put the Time Entries deletion as restrcit
        // this entries could be valuable audit data, so should be careful managed.
        builder.HasOne(e => e.Project)
            .WithMany(e => e.TimeEntries)
            .HasForeignKey(e => e.ProjectId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
