# Architecture Code Examples

## Domain Model Examples

### 1. Aggregate Root: Organization

**File**: `src/Decryptcode.Assessment.Service.Domain/Entities/AggregateRoots/Organization.cs`

```csharp
public sealed class Organization : BaseEntity, IAggregateRoot
{
    // Public read-only properties (enforced through private setters)
    public string Name { get; private set; }
    public string Slug { get; private set; }
    public string Industry { get; private set; }
    public string Tier { get; private set; }
    public string ContactEmail { get; private set; }
    public string Description { get; private set; }

    // Value Objects (immutable)
    public Settings Settings { get; private set; }
    public Metadata Metadata { get; private set; }

    // Collections of Reference Entities
    public ICollection<User> Users { get; private set; } = new HashSet<User>();
    public ICollection<Project> Projects { get; private set; } = new HashSet<Project>();
    public ICollection<Invoice> Invoices { get; private set; } = new HashSet<Invoice>();

    // Private constructor (EF Core only)
    private Organization() { }

    // Factory method for creating organizations
    [SetsRequiredMembers]
    private Organization(string id, string name, string slug, string industry, 
                        string tier, string contactEmail, string description,
                        Settings settings, Metadata metadata)
    {
        Id = Guard.AgainstNullOrEmpty(id, nameof(id));
        Name = Guard.AgainstNullOrEmpty(name, nameof(name));
        Slug = Guard.AgainstNullOrEmpty(slug, nameof(slug));
        Industry = Guard.AgainstNullOrEmpty(industry, nameof(industry));
        Tier = Guard.AgainstNullOrEmpty(tier, nameof(tier));
        ContactEmail = Guard.AgainstNullOrEmpty(contactEmail, nameof(contactEmail));
        Description = Guard.AgainstNullOrEmpty(description, nameof(description));
        Settings = Guard.AgainstNull(settings, nameof(settings));
        Metadata = Guard.AgainstNull(metadata, nameof(metadata));
    }

    // Static factory for cleaner API
    public static Organization Create(string id, string name, string slug, string industry,
                                     string tier, string contactEmail, string description,
                                     Settings settings, Metadata metadata)
    {
        return new Organization(id, name, slug, industry, tier, contactEmail, description, settings, metadata);
    }
}
```

**Key Points:**
- ✅ Implements `IAggregateRoot` interface (marker interface)
- ✅ Private setters prevent unauthorized state changes
- ✅ Factory method `Create()` for consistent creation
- ✅ Guard clauses validate inputs
- ✅ Contains collections of reference entities

---

### 2. Reference Entity: User (NOT an Aggregate Root)

**File**: `src/Decryptcode.Assessment.Service.Domain/Entities/ReferenceEntities/User.cs`

```csharp
public sealed class User : BaseEntity
{
    // Note: NO IAggregateRoot interface!

    // Foreign key to Organization (establishes ownership)
    public string OrgId { get; private set; }

    public string Email { get; private set; }
    public string Name { get; private set; }
    public string Role { get; private set; }
    public bool Active { get; private set; }
    public string Bio { get; private set; }

    // Navigation property to parent Organization
    public Organization? Organization { get; private set; }

    // Collection of owned TimeEntries
    public ICollection<TimeEntry> TimeEntries { get; private set; } = new HashSet<TimeEntry>();

    private User() { }

    [SetsRequiredMembers]
    private User(string id, string orgId, string email, string name, 
                string role, bool active, string bio)
    {
        Id = Guard.AgainstNullOrEmpty(id, nameof(id));
        OrgId = Guard.AgainstNullOrEmpty(orgId, nameof(orgId));
        Email = Guard.AgainstNullOrEmpty(email, nameof(email));
        Name = Guard.AgainstNullOrEmpty(name, nameof(name));
        Role = Guard.AgainstNullOrEmpty(role, nameof(role));
        Active = active;
        Bio = Guard.AgainstNullOrEmpty(bio, nameof(bio));
    }

    public static User Create(string id, string orgId, string email, string name,
                             string role, bool active, string bio)
    {
        // Additional business rule validation
        if (!email.Contains("@"))
            throw new ArgumentException("Invalid email format", nameof(email));

        return new User(id, orgId, email, name, role, active, bio);
    }

    // Business logic method
    public void Deactivate()
    {
        if (!Active)
            throw new InvalidOperationException("User is already inactive");

        Active = false;
    }
}
```

**Key Points:**
- ❌ Does NOT implement `IAggregateRoot`
- ✅ Has `OrgId` foreign key (establishes parent relationship)
- ✅ Cannot be created or modified independently
- ✅ Must belong to an Organization
- ✅ Contains its own collection (TimeEntries) but is itself a reference

---

### 3. Value Objects: Settings and Metadata

**File**: `src/Decryptcode.Assessment.Service.Domain/Entities/ValueObjects/Settings.cs`

```csharp
namespace Decryptcode.Assessment.Service.Domain.Entities.ValueObjects;

public record Settings
{
    // Immutable properties (records are immutable by default)
    public string Currency { get; init; }
    public string Timezone { get; init; }
    public string DefaultLocale { get; init; }
    public bool AllowOvertime { get; init; }

    // Constructor with validation
    public Settings(string currency, string timezone, string defaultLocale, bool allowOvertime)
    {
        Currency = Guard.AgainstNullOrEmpty(currency, nameof(currency));
        Timezone = Guard.AgainstNullOrEmpty(timezone, nameof(timezone));
        DefaultLocale = Guard.AgainstNullOrEmpty(defaultLocale, nameof(defaultLocale));
        AllowOvertime = allowOvertime;
    }

    // Cannot be changed after creation
    // To update: create new instance with `with` operator
    // var newSettings = existingSettings with { Currency = "EUR" };
}

public record Metadata
{
    public string? LegacyId { get; init; }
    public DateTime? MigratedAt { get; init; }
    public string? Source { get; init; }

    public Metadata(string? legacyId = null, DateTime? migratedAt = null, string? source = null)
    {
        LegacyId = legacyId;
        MigratedAt = migratedAt;
        Source = source;
    }
}
```

**Key Points:**
- ✅ Implemented as `record` (immutable by default)
- ✅ All properties use `init` (can only be set during construction)
- ✅ Validation in constructor
- ✅ Changes require creating new instance
- ✅ Compared by value, not reference

---

## Repository Pattern Examples

### 4. Repository Interface (Domain Layer)

**File**: `src/Decryptcode.Assessment.Service.Domain/Repositories/IOrganizationRepository.cs`

```csharp
public interface IOrganizationRepository : IRepository<Organization>
{
    // Inherited from IRepository<T>
    // Task AddAsync(Organization entity);
    // Task UpdateAsync(Organization entity);
    // Task DeleteAsync(Organization entity);
    // Task<Organization?> GetByIdAsync(string id);
    // Task<IEnumerable<Organization>> GetAllAsync();

    // Specialized queries
    Task<Organization?> GetBySlugAsync(string slug);
    Task<IEnumerable<Organization>> GetAllFiltered(string? industry, string? tier);
    Task<DashboardDto> GetDashboardAsync(string orgId);
}
```

**Key Points:**
- ✅ Abstraction for data access
- ✅ Defines contract for infrastructure layer
- ✅ Located in Domain layer (not Infrastructure)
- ✅ Infrastructure implements this interface

---

### 5. Repository Implementation (Infrastructure Layer)

**File**: `src/Decryptcode.Assessment.Service.Infrastructure.SqlServer/Repositories/OrganizationRepository.cs`

```csharp
public sealed class OrganizationRepository : IOrganizationRepository
{
    private readonly ApiContext _dbContext;

    public OrganizationRepository(ApiContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddAsync(Organization entity)
    {
        Guard.AgainstNull(entity, nameof(entity));

        await _dbContext.Organizations.AddAsync(entity);
        await _dbContext.SaveChangesAsync();
    }

    public async Task UpdateAsync(Organization entity)
    {
        Guard.AgainstNull(entity, nameof(entity));

        _dbContext.Organizations.Update(entity);
        await _dbContext.SaveChangesAsync();
    }

    public async Task DeleteAsync(Organization entity)
    {
        Guard.AgainstNull(entity, nameof(entity));

        // Soft delete: set DeletedAt instead of removing
        entity.SoftDelete(); // Sets DeletedAt and DeletedBy

        _dbContext.Organizations.Update(entity);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<Organization?> GetByIdAsync(string id)
    {
        Guard.AgainstNullOrEmpty(id, nameof(id));

        // Soft delete filter: automatically excludes deleted records
        return await _dbContext.Organizations
            .FirstOrDefaultAsync(x => x.Id == id && x.DeletedAt == null);
    }

    public async Task<IEnumerable<Organization>> GetAllAsync()
    {
        return await _dbContext.Organizations
            .Where(x => x.DeletedAt == null) // Soft delete filter
            .ToListAsync();
    }

    public async Task<Organization?> GetBySlugAsync(string slug)
    {
        Guard.AgainstNullOrEmpty(slug, nameof(slug));

        return await _dbContext.Organizations
            .FirstOrDefaultAsync(x => x.Slug == slug && x.DeletedAt == null);
    }

    public async Task<IEnumerable<Organization>> GetAllFiltered(string? industry, string? tier)
    {
        var query = _dbContext.Organizations.AsQueryable();

        // Soft delete filter always applied
        query = query.Where(x => x.DeletedAt == null);

        if (!string.IsNullOrEmpty(industry))
            query = query.Where(x => x.Industry == industry);

        if (!string.IsNullOrEmpty(tier))
            query = query.Where(x => x.Tier == tier);

        return await query.ToListAsync();
    }

    public async Task<DashboardDto> GetDashboardAsync(string orgId)
    {
        Guard.AgainstNullOrEmpty(orgId, nameof(orgId));

        return new DashboardDto
        {
            TotalOrganizations = await _dbContext.Organizations.CountAsync(x => x.DeletedAt == null),
            TotalUsers = await _dbContext.Users.CountAsync(x => x.DeletedAt == null),
            TotalProjects = await _dbContext.Projects.CountAsync(x => x.DeletedAt == null),
            TotalTimeEntries = await _dbContext.TimeEntries.CountAsync(x => x.DeletedAt == null),
            TotalInvoices = await _dbContext.Invoices.CountAsync(x => x.DeletedAt == null),
        };
    }
}
```

**Key Points:**
- ✅ Implements `IOrganizationRepository` interface
- ✅ Encapsulates EF Core operations
- ✅ Applies soft delete filter on all queries
- ✅ Manages transactions (SaveChangesAsync)
- ✅ Guards against null inputs

---

## CQRS Query Handler Example

### 6. Query (Application Layer)

**File**: `src/Decryptcode.Assessment.Service.Application/Organizations/Queries/GetAllOrganizations/GetAllOrganizationsQuery.cs`

```csharp
namespace Decryptcode.Assessment.Service.Application.Organizations.Queries.GetAllOrganizations;

public sealed class GetAllOrganizationsQuery
{
    public string? Industry { get; set; }
    public string? Tier { get; set; }
}

// Query Handler
public sealed class GetAllOrganizationsQueryHandler
{
    private readonly IOrganizationRepository _organizationRepository;

    public GetAllOrganizationsQueryHandler(IOrganizationRepository organizationRepository)
    {
        _organizationRepository = organizationRepository;
    }

    public async Task<IEnumerable<OrganizationDto>> Handle(
        GetAllOrganizationsQuery query,
        CancellationToken cancellationToken)
    {
        // Fetch filtered organizations from repository
        var organizations = await _organizationRepository
            .GetAllFiltered(query.Industry, query.Tier);

        // Map to DTOs (for API response)
        return organizations.Select(x => new OrganizationDto
        {
            Id = x.Id,
            Name = x.Name,
            Slug = x.Slug,
            Industry = x.Industry,
            Tier = x.Tier,
            ContactEmail = x.ContactEmail,
            Description = x.Description
        });
    }
}
```

**Key Points:**
- ✅ Query object defines request parameters
- ✅ Query handler executes business logic
- ✅ Returns DTOs, not domain entities
- ✅ Invoked through MessageBus

---

## API Endpoint Examples

### 7. Minimal API Endpoint

**File**: `src/DecryptCode.Assessment.Service.ApiMinimal/Endpoints/OrganizationsEndpoints.cs`

```csharp
public static class OrganizationsEndpoints
{
    public static WebApplication MapOrganizationsEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/organizations")
            .WithTags("Organizations")
            .WithOpenApi();

        group.MapGet("/", GetOrganizations)
            .WithName("GetOrganizations")
            .WithDescription("Return all Organizations with optional filters");

        group.MapGet("/{id}", GetOrganizationById)
            .WithName("GetOrganizationById")
            .WithDescription("Return organization by ID");

        return app;
    }

    private static async Task<IResult> GetOrganizations(
        IMessageBus messageBus,        // Injected dependency
        string? industry,               // Query parameter
        string? tier,                   // Query parameter
        CancellationToken cancellationToken)
    {
        // Create query
        var query = new GetAllOrganizationsQuery 
        { 
            Industry = industry, 
            Tier = tier 
        };

        // Send through Wolverine MessageBus (mediator pattern)
        var result = await messageBus.InvokeAsync<dynamic>(query, cancellationToken);

        // Return result
        return Results.Ok(result);
    }

    private static async Task<IResult> GetOrganizationById(
        IMessageBus messageBus,
        string id,                      // Route parameter
        CancellationToken cancellationToken)
    {
        var query = new GetOrganizationByIdQuery { Id = id };
        var result = await messageBus.InvokeAsync<dynamic>(query, cancellationToken);

        return result == null 
            ? Results.NotFound() 
            : Results.Ok(result);
    }
}
```

**Key Points:**
- ✅ Dependency injection (IMessageBus)
- ✅ Query parameter binding (industry, tier)
- ✅ Route parameter binding (id)
- ✅ Returns IResult (type-safe response)
- ✅ Cleaner than traditional controllers

---

### 8. Conventional API Controller (Alternative)

**File**: `src/Decryptcode.Assessment.Service.Api/Controllers/OrganizationsController.cs`

```csharp
[ApiController]
[Route("api/organizations")]
[Produces("application/json")]
public sealed class OrganizationsController : BaseController
{
    public OrganizationsController(IMessageBus messageBus) : base(messageBus)
    {
    }

    [HttpGet]
    [SwaggerOperation(
        Summary = "Return all Organizations",
        Tags = ["Organizations"])
    ]
    public async Task<IActionResult> GetOrganizationsAsync(
        [FromQuery] string? industry,
        [FromQuery] string? tier,
        CancellationToken cancellationToken)
    {
        var query = new GetAllOrganizationsQuery { Industry = industry, Tier = tier };
        return await SendAsync(query, cancellationToken); // Base class helper
    }

    [HttpGet("{id}")]
    [SwaggerOperation(
        Summary = "Return organization by ID",
        Tags = ["Organizations"])
    ]
    public async Task<IActionResult> GetOrganizationByIdAsync(
        [FromRoute] string id,
        CancellationToken cancellationToken)
    {
        var query = new GetOrganizationByIdQuery { Id = id };
        return await SendAsync(query, cancellationToken);
    }
}

// BaseController helper
public abstract class BaseController : ControllerBase
{
    protected readonly IMessageBus _messageBus;

    protected BaseController(IMessageBus messageBus)
    {
        _messageBus = messageBus;
    }

    protected async Task<IActionResult> SendAsync(object message, CancellationToken cancellationToken)
    {
        var result = await _messageBus.InvokeAsync<IRequestResult>(message, cancellationToken);
        return StatusCode(result.StatusCode, result);
    }
}
```

**Key Points:**
- ✅ Traditional MVC/REST pattern
- ✅ Attribute-based routing
- ✅ Same query/command execution through MessageBus
- ✅ Uses BaseController for common logic
- ✅ Swagger documentation

---

## Dependency Injection Configuration

### 9. Service Registration

**File**: `src/Decryptcode.Assessment.Service.Api/Program.cs`

```csharp
// Add Application Services
builder.Services.AddApplicationServices(builder.Host);

// Add Infrastructure Services
builder.Services.AddSqlServerInfrastructure(builder.Configuration, builder.Environment.IsProduction());
```

**File**: `src/Decryptcode.Assessment.Service.Infrastructure.SqlServer/ServicesInjector.cs`

```csharp
public static void AddSqlServerInfrastructure(this IServiceCollection services, IConfiguration configuration, bool isProduction = false)
{
    var connectionString = configuration.GetConnectionString(ApiConstants.SQL_CONNECTION_STRING_NAME);

    // Determine which database to use
    if (string.IsNullOrEmpty(connectionString))
    {
        AddSqliteDatabase(services);
    }
    else if (IsSqlServerAvailable(connectionString))
    {
        AddSqlServerDatabase(services, connectionString);
    }
    else
    {
        Console.WriteLine("⚠️ SQL Server unavailable. Falling back to SQLite.");
        AddSqliteDatabase(services);
    }

    // Register repositories
    services.AddScoped<IOrganizationRepository, OrganizationRepository>();
    services.AddScoped<IUserRepository, UserRepository>();
    services.AddScoped<IProjectRepository, ProjectRepository>();
    services.AddScoped<ITimeEntriesRepository, TimeEntriesRepository>();
    services.AddScoped<IInvoiceRepository, InvoiceRepository>();
}

private static void AddSqlServerDatabase(IServiceCollection services, string connectionString)
{
    services.AddDbContext<ApiContext>(options =>
        options.UseSqlServer(connectionString,
            sqlOptions => sqlOptions.EnableRetryOnFailure(2, TimeSpan.FromSeconds(30), null)));

    Console.WriteLine("✅ Using SQL Server database");
}

private static void AddSqliteDatabase(IServiceCollection services)
{
    var sqlitePath = Path.Combine(AppContext.BaseDirectory, "Data");
    Directory.CreateDirectory(sqlitePath);
    var connectionString = $"Data Source={Path.Combine(sqlitePath, "assessmentdb.db")}";

    services.AddDbContext<ApiContext>(options =>
        options.UseSqlite(connectionString,
            sqliteOptions => sqliteOptions.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery)));

    Console.WriteLine($"✅ Using SQLite database at: {Path.Combine(sqlitePath, "assessmentdb.db")}");
}
```

**Key Points:**
- ✅ Automatic database selection (SQL Server → SQLite fallback)
- ✅ All repositories registered as scoped
- ✅ DbContext configured per database type
- ✅ Retry policy for resilience

---

## Summary Diagram

```
┌─────────────────────────────────────────────────┐
│         HTTP Request                            │
│  GET /api/organizations?industry=Tech&tier=Pro  │
└────────────────┬────────────────────────────────┘
                 │
    ┌────────────▼─────────────────────────┐
    │ Minimal API Endpoint (GetOrganizations)    │
    │ • Extract query parameters                 │
    │ • Create GetAllOrganizationsQuery         │
    │ • Invoke via IMessageBus                   │
    └────────────┬─────────────────────────┘
                 │
    ┌────────────▼─────────────────────────┐
    │ CQRS Query Handler                        │
    │ • GetAllOrganizationsQueryHandler        │
    │ • Call repository method                 │
    │ • Transform to DTOs                      │
    └────────────┬─────────────────────────┘
                 │
    ┌────────────▼─────────────────────────┐
    │ Repository Layer                         │
    │ • IOrganizationRepository                │
    │ • GetAllFiltered("Tech", "Pro")          │
    │ • Soft delete filters applied            │
    └────────────┬─────────────────────────┘
                 │
    ┌────────────▼─────────────────────────┐
    │ Entity Framework Core                    │
    │ • DbContext.Organizations               │
    │ • SQL translation                       │
    │ • Query execution                       │
    └────────────┬─────────────────────────┘
                 │
    ┌────────────▼─────────────────────────┐
    │ Database (SQL Server or SQLite)          │
    │ SELECT * FROM Organizations             │
    │ WHERE Industry='Tech' AND Tier='Pro'    │
    │ AND DeletedAt IS NULL                   │
    └────────────┬─────────────────────────┘
                 │
    ┌────────────▼─────────────────────────┐
    │ Response                                 │
    │ 200 OK                                   │
    │ [ Organization DTOs ]                   │
    └─────────────────────────────────────┘
```

---

**Remember the Architecture Principles:**
1. ✅ **Organization** = Aggregate Root (single point of entry)
2. ✅ **User, Project, Invoice, TimeEntry** = Reference Entities (managed through repositories)
3. ✅ **Repositories** = Data access abstraction
4. ✅ **Queries/Commands** = Business logic (CQRS pattern)
5. ✅ **DTOs** = Data transfer objects (API responses)
6. ✅ **Value Objects** = Immutable, defined by value (Settings, Metadata)
7. ✅ **Soft Delete** = Logical deletion with audit trail
8. ✅ **Dependency Injection** = Loose coupling, testability
