# Architecture Quick Reference

## Cheat Sheet

### Entity Type Classification

| Entity | Type | Repository | Aggregate Root | Has OrgId FK | Can Query Independently |
|--------|------|------------|-----------------|---|---|
| Organization | Root | ✅ Yes | ✅ YES | ❌ No | ✅ YES |
| User | Reference | ✅ Yes | ❌ NO | ✅ Yes | ⚠️ Via Repo Only |
| Project | Reference | ✅ Yes | ❌ NO | ✅ Yes | ⚠️ Via Repo Only |
| TimeEntry | Reference | ✅ Yes | ❌ NO | ❌ No | ⚠️ Via Repo Only |
| Invoice | Reference | ✅ Yes | ❌ NO | ✅ Yes | ⚠️ Via Repo Only |

---

## Quick Decision Tree

### When creating an entity, ask:

```
Is this entity independent?
├─ YES → Aggregate Root (like Organization)
│        └─ Implement IAggregateRoot
│        └─ Create repository
│        └─ Can be queried independently
│
└─ NO → Reference Entity (like User, Project, etc.)
        ├─ Does it have OrgId foreign key?
        │  ├─ YES → Belongs to Organization
        │  │        └─ OrgId establishes ownership
        │  │
        │  └─ NO → Indirectly owned (like TimeEntry)
        │          └─ Owned through navigation properties
        │
        └─ Still has its own repository
           └─ But queried through business logic
           └─ Cannot exist without parent aggregate
```

---

## File Structure Quick Lookup

### Domain Layer
```
src/Decryptcode.Assessment.Service.Domain/
├── Entities/
│   ├── AggregateRoots/
│   │   └── Organization.cs          ◄── AGGREGATE ROOT
│   ├── ReferenceEntities/
│   │   ├── User.cs                  ◄── REFERENCE ENTITY
│   │   ├── Project.cs               ◄── REFERENCE ENTITY
│   │   ├── TimeEntry.cs             ◄── REFERENCE ENTITY
│   │   └── Invoice.cs               ◄── REFERENCE ENTITY
│   ├── ValueObjects/
│   │   ├── Settings.cs              ◄── VALUE OBJECT
│   │   └── Metadata.cs              ◄── VALUE OBJECT
│   └── BaseEntity.cs
├── Repositories/
│   ├── IOrganizationRepository.cs
│   ├── IUserRepository.cs
│   ├── IProjectRepository.cs
│   ├── ITimeEntriesRepository.cs
│   └── IInvoiceRepository.cs
└── Guards/
    └── Guard.cs                      ◄── VALIDATION HELPERS
```

### Infrastructure Layer
```
src/Decryptcode.Assessment.Service.Infrastructure.SqlServer/
├── Contexts/
│   └── ApiContext.cs                ◄── EF CORE DBCONTEXT
├── Repositories/
│   ├── OrganizationRepository.cs     ◄── REPOSITORY IMPL
│   ├── UserRepository.cs             ◄── REPOSITORY IMPL
│   ├── ProjectRepository.cs          ◄── REPOSITORY IMPL
│   ├── TimeEntriesRepository.cs      ◄── REPOSITORY IMPL
│   └── InvoiceRepository.cs          ◄── REPOSITORY IMPL
├── Migrations/
│   ├── 2024...InitialCreate.cs
│   └── DbContextModelSnapshot.cs
└── ServicesInjector.cs              ◄── DEPENDENCY REGISTRATION
```

### Application Layer
```
src/Decryptcode.Assessment.Service.Application/
├── Organizations/
│   ├── Queries/
│   │   ├── GetAllOrganizations/
│   │   │   ├── GetAllOrganizationsQuery.cs
│   │   │   └── GetAllOrganizationsQueryHandler.cs
│   │   └── GetOrganizationById/
│   │       ├── GetOrganizationByIdQuery.cs
│   │       └── GetOrganizationByIdQueryHandler.cs
│   └── Dtos/
│       └── OrganizationDto.cs
├── Users/
├── Projects/
├── TimeEntries/
└── Invoices/
```

### API Layer
```
src/Decryptcode.Assessment.Service.Api/
├── Controllers/
│   ├── OrganizationsController.cs
│   ├── UsersController.cs
│   ├── ProjectsController.cs
│   ├── TimeEntriesController.cs
│   └── InvoicesController.cs
└── Program.cs

src/DecryptCode.Assessment.Service.ApiMinimal/
├── Endpoints/
│   ├── OrganizationsEndpoints.cs
│   ├── UsersEndpoints.cs
│   ├── ProjectsEndpoints.cs
│   ├── TimeEntriesEndpoints.cs
│   ├── InvoicesEndpoints.cs
│   ├── DashboardEndpoints.cs
│   └── HealthEndpoints.cs
└── Program.cs
```

---

## Common Patterns

### Pattern 1: Creating a Reference Entity
```csharp
// WRONG ❌ - Direct instantiation
var user = new User { OrgId = orgId, Email = email, ... };

// CORRECT ✅ - Via Application Service/Handler
public async Task CreateUserCommand(CreateUserCommand cmd, CancellationToken ct)
{
    // 1. Verify Organization exists
    var org = await _orgRepo.GetByIdAsync(cmd.OrgId);
    if (org == null) throw new NotFoundException();

    // 2. Create through domain
    var user = User.Create(id, cmd.OrgId, cmd.Email, cmd.Name, ...);

    // 3. Persist through repository
    await _userRepository.AddAsync(user);
}
```

### Pattern 2: Querying Related Entities
```csharp
// Get all users in an organization
var users = await _userRepository.GetAllByOrgAsync(orgId);

// Get filtered users
var activeAdmins = await _userRepository.GetAllFiltered(
    orgId: orgId,
    role: "Admin",
    active: true
);

// Get time entries for specific user/project
var timeEntries = await _timeEntriesRepository.GetAllFiltered(
    userId: userId,
    projectId: projectId
);
```

### Pattern 3: Updating a Reference Entity
```csharp
// Get entity
var user = await _userRepository.GetByIdAsync(userId);
if (user == null) throw new NotFoundException();

// Update through domain method
user.Deactivate(); // Sets Active = false

// Persist changes
await _userRepository.UpdateAsync(user);
```

### Pattern 4: Soft Delete
```csharp
// Delete (soft)
await _userRepository.DeleteAsync(user);
// Sets: DeletedAt = DateTime.Now, DeletedBy = userId

// Query automatically excludes deleted
var users = await _userRepository.GetAllAsync();
// Only returns users where DeletedAt IS NULL
```

---

## API Endpoint Patterns

### GET (Query - Read Only)
```
GET /api/organizations
GET /api/organizations?industry=Tech&tier=Pro
GET /api/organizations/{id}
GET /api/organizations/{id}/summary

GET /api/users
GET /api/users?orgId=X&role=Admin&active=true
GET /api/users/{id}

GET /api/projects
GET /api/projects?orgId=X&status=Active
GET /api/projects/{id}

GET /api/invoices
GET /api/invoices?orgId=X&status=Paid

GET /api/time-entries
GET /api/time-entries?userId=X&projectId=Y&from=DATE&to=DATE

GET /api/dashboard
GET /health
```

---

## Error Handling

### Common Errors and Handling

| Scenario | Error Code | Response |
|----------|-----------|----------|
| Resource not found | 404 | `{ statusCode: 404, message: "Not found" }` |
| Invalid input | 400 | `{ statusCode: 400, message: "Validation error" }` |
| Unauthorized | 401 | `{ statusCode: 401, message: "Unauthorized" }` |
| Forbidden | 403 | `{ statusCode: 403, message: "Access denied" }` |
| Business rule violated | 400 | `{ statusCode: 400, message: "Rule violation" }` |
| Server error | 500 | `{ statusCode: 500, message: "Internal error" }` |

---

## Database Support

### Automatic Selection
```csharp
// In ServicesInjector.cs

if (connectionString is empty)
    → Use SQLite (default local dev)
else if (SQL Server available)
    → Use SQL Server
else
    → Fallback to SQLite
```

### Connection Strings

**SQL Server** (Production):
```json
"ConnectionStrings": {
  "SqlConnectionString": "Data Source=localhost;Initial Catalog=AssessmentDb;Integrated Security=True;"
}
```

**SQLite** (Local Dev - Automatic):
```
Data/assessmentdb.db (created automatically)
```

---

## Testing

### Unit Testing Domain Entities
```csharp
[Fact]
public void Organization_Create_WithValidData_Succeeds()
{
    // Arrange
    var settings = new Settings("USD", "UTC", "en-US", false);
    var metadata = new Metadata();

    // Act
    var org = Organization.Create("id1", "Acme", "acme", "Tech", "Pro", 
                                  "contact@acme.com", "Desc", settings, metadata);

    // Assert
    Assert.NotNull(org);
    Assert.Equal("Acme", org.Name);
}

[Fact]
public void Organization_Create_WithInvalidName_Throws()
{
    // Arrange
    var settings = new Settings("USD", "UTC", "en-US", false);
    var metadata = new Metadata();

    // Act & Assert
    Assert.Throws<ArgumentException>(() =>
        Organization.Create("id1", "", "slug", "Tech", "Pro", "email", "desc", settings, metadata)
    );
}
```

### Integration Testing Repositories
```csharp
[Fact]
public async Task GetByIdAsync_WithValidId_ReturnsOrganization()
{
    // Arrange
    var org = Organization.Create(...);
    await _repository.AddAsync(org);

    // Act
    var result = await _repository.GetByIdAsync(org.Id);

    // Assert
    Assert.NotNull(result);
    Assert.Equal(org.Id, result.Id);
}

[Fact]
public async Task DeleteAsync_WithValidEntity_SetsDeletedAt()
{
    // Arrange
    var org = Organization.Create(...);
    await _repository.AddAsync(org);

    // Act
    await _repository.DeleteAsync(org);

    // Assert
    var result = await _repository.GetByIdAsync(org.Id);
    Assert.Null(result); // Soft delete filters it out
}
```

---

## Deployment Checklist

- [ ] Update connection string for SQL Server (if using)
- [ ] Run EF Core migrations: `dotnet ef database update`
- [ ] Verify database seeding completed
- [ ] Test both conventional API (`/api/*`) and minimal API (`/api/*`)
- [ ] Test health check: `GET /health`
- [ ] Configure CORS settings in appsettings.json
- [ ] Enable/disable Swagger based on environment
- [ ] Test fallback to SQLite (optional)

---

## Key Takeaways

| Concept | Remember |
|---------|----------|
| **Aggregate Root** | Organization is the boss; all changes go through it |
| **Reference Entities** | User, Project, Invoice, TimeEntry serve the Organization |
| **Repository Pattern** | Always access data through repositories, not DbContext directly |
| **Soft Delete** | Queries automatically exclude deleted records (DeletedAt != null) |
| **Value Objects** | Settings, Metadata are immutable; create new instances to update |
| **DTOs** | APIs return DTOs, not domain entities |
| **CQRS** | Queries and Commands separated; MessageBus routes them |
| **Dependency Injection** | Everything is registered in Program.cs; use interfaces |
| **Validation** | Happens in domain entity constructors via Guard clauses |
| **Error Handling** | RequestResult wrapper provides consistent API responses |

---

## Common Commands

### Scaffold a Migration
```bash
cd src/Decryptcode.Assessment.Service.Infrastructure.SqlServer
dotnet ef migrations add FeatureName --startup-project ../../Decryptcode.Assessment.Service.Api
```

### Apply Migrations
```bash
dotnet ef database update --startup-project src/Decryptcode.Assessment.Service.Api
```

### Remove Last Migration
```bash
dotnet ef migrations remove --startup-project ../../Decryptcode.Assessment.Service.Api
```

### Run Tests
```bash
dotnet test tests/Decryptcode.Assessment.Service.UnitTests
dotnet test tests/Decryptcode.Assessment.Service.IntegrationTests
```

### Build Solution
```bash
dotnet build
```

### Run API
```bash
dotnet run --project src/Decryptcode.Assessment.Service.Api
# or
dotnet run --project src/DecryptCode.Assessment.Service.ApiMinimal
```

---

**Version**: 1.0 Quick Reference  
**Last Updated**: 2024  
**Format**: Domain-Driven Design + Clean Architecture + CQRS
