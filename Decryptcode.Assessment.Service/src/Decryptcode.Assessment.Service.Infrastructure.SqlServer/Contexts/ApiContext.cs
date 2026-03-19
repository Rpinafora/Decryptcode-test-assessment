using Decryptcode.Assessment.Service.Domain.Entities;
using Decryptcode.Assessment.Service.Domain.Entities.AggregateRoots;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using System.Linq.Expressions;
using System.Security.Claims;

namespace Decryptcode.Assessment.Service.Infrastructure.SqlServer.Contexts;

public sealed class ApiContext : DbContext
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public ApiContext(DbContextOptions<ApiContext> options, IHttpContextAccessor httpContextAccessor)
        : base(options)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public DbSet<Organization> Organizations { get; set; }

    public override int SaveChanges()
    {
        ModifyEntryBeforeSave();
        return base.SaveChanges();
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        ModifyEntryBeforeSave();
        return base.SaveChangesAsync(cancellationToken);
    }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            var primaryKey = entityType.FindPrimaryKey();

            if (primaryKey != null)
            {
                foreach (var property in primaryKey.Properties)
                {
                    property.ValueGenerated = ValueGenerated.Never;
                    property.SetAnnotation("Relational:DefaultValueSql", null);
                    property.SetAnnotation("SqlServer:ValueGenerationStrategy", null);
                }
            }

            if (typeof(BaseEntity).IsAssignableFrom(entityType.ClrType))
            {
                var parameter = Expression.Parameter(entityType.ClrType, "p");

                var deletedDateCheck = Expression.Equal(
                    Expression.Property(parameter, nameof(BaseEntity.DeletedAt)),
                    Expression.Constant(null, typeof(DateTime?)));

                var lambda = Expression.Lambda(deletedDateCheck, parameter);

                modelBuilder.Entity(entityType.ClrType).HasQueryFilter(lambda);
            }

            foreach (var property in entityType.GetProperties())
            {
                if (property.ClrType == typeof(string) && property.GetMaxLength() == null)
                {
                    property.SetMaxLength(1000);
                }
            }
        }

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApiContext).Assembly);

        base.OnModelCreating(modelBuilder);

    }

    private void ModifyEntryBeforeSave()
    {
        var currentUserId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var prev = ChangeTracker.AutoDetectChangesEnabled;
        ChangeTracker.AutoDetectChangesEnabled = false;

        ProcessAddedEntries(currentUserId);
        ProcessModifiedEntries(currentUserId);
        ProcessDeletedEntries(currentUserId);

        ChangeTracker.AutoDetectChangesEnabled = prev;
    }

    private void ProcessAddedEntries(string? currentUserId)
    {
        foreach (var entry in ChangeTracker.Entries<BaseEntity>().Where(e => e.State == EntityState.Added))
        {
            entry.Entity.ChangeCreatedInfo(currentUserId);
        }
    }

    private void ProcessModifiedEntries(string? currentUserId)
    {
        foreach (var entry in ChangeTracker.Entries<BaseEntity>().Where(e => e.State == EntityState.Modified))
        {
            entry.Entity.ChangeUpdatedInfo(currentUserId);
        }
    }

    private void ProcessDeletedEntries(string? currentUserId)
    {
        foreach (var entry in ChangeTracker.Entries<BaseEntity>().Where(e => e.State == EntityState.Deleted))
        {
            entry.Entity.ChangeDeletedInfo(currentUserId);
            entry.State = EntityState.Modified;
        }
    }
}
