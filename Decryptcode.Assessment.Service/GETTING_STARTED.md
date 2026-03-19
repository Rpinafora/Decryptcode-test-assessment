# Complete Architecture Documentation - Summary

## 📚 Documentation Files

This project includes comprehensive architecture documentation:

1. **ARCHITECTURE.md** - Main architecture guide with detailed explanations
2. **ARCHITECTURE_VISUAL_GUIDE.md** - Visual diagrams and flowcharts
3. **ARCHITECTURE_CODE_EXAMPLES.md** - Real code examples from the project
4. **ARCHITECTURE_QUICK_REFERENCE.md** - Quick lookup and cheat sheet

---

## 🏗️ The Core Architecture

### Single Aggregate Root: Organization

```
Organization (AGGREGATE ROOT)
├── Users (Reference Entities)
├── Projects (Reference Entities)
├── Invoices (Reference Entities)
└── TimeEntries (Reference Entities - via User/Project)
```

**Key Principle**: Everything is accessed through and owned by Organization.

---

## ✅ Aggregate Roots vs Reference Entities - THE KEY DIFFERENCE

### Organization: ✅ AGGREGATE ROOT
- Implements `IAggregateRoot` interface
- Has repository with independent access
- Can exist on its own
- Is a transaction boundary
- **File**: `src/Decryptcode.Assessment.Service.Domain/Entities/AggregateRoots/Organization.cs`

### User, Project, Invoice, TimeEntry: ❌ NOT AGGREGATE ROOTS (Reference Entities)
- Do NOT implement `IAggregateRoot` interface  
- Have repositories but are managed through Organization
- Cannot logically exist without Organization
- Part of Organization's aggregate boundary
- **Files**: `src/Decryptcode.Assessment.Service.Domain/Entities/ReferenceEntities/`

---

## 📊 Architecture Layers

```
┌─────────────────────────┐
│  Presentation Layer     │  Controllers / Minimal API Endpoints
│                         │  • OrganizationsController
│                         │  • OrganizationsEndpoints
├─────────────────────────┤
│  Application Layer      │  CQRS Queries & Commands
│                         │  • GetAllOrganizationsQuery
│                         │  • GetAllOrganizationsQueryHandler
│                         │  • DTOs (Data Transfer Objects)
├─────────────────────────┤
│  Domain Layer           │  Business Logic & Entities
│                         │  • Organization (Aggregate Root)
│                         │  • User, Project, etc. (Reference)
│                         │  • Value Objects (Settings, Metadata)
│                         │  • Repository Interfaces
├─────────────────────────┤
│  Infrastructure Layer   │  Data Access Implementation
│                         │  • EF Core DbContext
│                         │  • Repository Implementations
│                         │  • Migrations
│                         │  • SQL Server / SQLite
└─────────────────────────┘
```

---

## 🔗 Entity Relationships

```
┌──────────────────────┐
│   ORGANIZATION       │ ◄── AGGREGATE ROOT
│                      │     (Can be queried independently)
└──────────────────────┘
      │1..N    │1..N    │1..N
      ▼        ▼        ▼
   USER     PROJECT  INVOICE
(Reference)(Reference)(Reference)
      │         │
      └────┬────┘
           │1..N
           ▼
       TIMEENTRY
      (Reference)

Legend:
  ─── Foreign Key Relationship
  1..N ─── One-to-Many (One Organization owns many Users/Projects/etc)
```

### Ownership Rules
- Organization owns Users (OrgId foreign key)
- Organization owns Projects (OrgId foreign key)
- Organization owns Invoices (OrgId foreign key)
- User creates TimeEntries (UserId foreign key)
- Project has TimeEntries (ProjectId foreign key)
- Both User and Project must be in same Organization

---

## 📝 File Organization

### Domain Layer
```
Domain/Entities/
├── AggregateRoots/
│   └── Organization.cs              ◄── AGGREGATE ROOT
│       └── Implements IAggregateRoot
│
├── ReferenceEntities/
│   ├── User.cs                      ◄── NO IAggregateRoot
│   ├── Project.cs                   ◄── NO IAggregateRoot
│   ├── Invoice.cs                   ◄── NO IAggregateRoot
│   └── TimeEntry.cs                 ◄── NO IAggregateRoot
│
├── ValueObjects/
│   ├── Settings.cs                  ◄── Immutable Record
│   └── Metadata.cs                  ◄── Immutable Record
│
└── Repositories/
    ├── IOrganizationRepository.cs
    ├── IUserRepository.cs
    ├── IProjectRepository.cs
    ├── ITimeEntriesRepository.cs
    └── IInvoiceRepository.cs
```

### Infrastructure Layer
```
Infrastructure.SqlServer/
├── Contexts/
│   └── ApiContext.cs                ◄── EF Core DbContext
│
├── Repositories/
│   ├── OrganizationRepository.cs     ◄── Implements IOrganizationRepository
│   ├── UserRepository.cs            ◄── Implements IUserRepository
│   ├── ProjectRepository.cs         ◄── Implements IProjectRepository
│   ├── TimeEntriesRepository.cs     ◄── Implements ITimeEntriesRepository
│   └── InvoiceRepository.cs         ◄── Implements IInvoiceRepository
│
├── Migrations/
│   └── [Database schema migrations]
│
└── ServicesInjector.cs              ◄── Dependency Registration
    ├── AddSqlServerDatabase()       ◄── SQL Server config
    └── AddSqliteDatabase()          ◄── SQLite fallback
```

### Application Layer
```
Application/
├── Organizations/
│   ├── Queries/
│   │   ├── GetAllOrganizations/
│   │   ├── GetOrganizationById/
│   │   └── GetOrganizationSummary/
│   └── Dtos/
│       └── OrganizationDto.cs
│
├── Users/
│   ├── Queries/
│   │   ├── GetAllUsers/
│   │   └── GetUserById/
│   └── Dtos/
│
├── Projects/
├── Invoices/
└── TimeEntries/
```

### API Layer
```
Api/ (Conventional Controllers)
├── Controllers/
│   ├── OrganizationsController.cs
│   ├── UsersController.cs
│   ├── ProjectsController.cs
│   ├── TimeEntriesController.cs
│   └── InvoicesController.cs
└── Program.cs

ApiMinimal/ (Minimal API - Alternative)
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

## 🔄 Request Flow Example: GET /api/organizations/{id}

```
1. HTTP Request
   └─ GET /api/organizations/org-123

2. Endpoint/Controller
   └─ GetOrganizationById(IMessageBus, string id)
   └─ Create GetOrganizationByIdQuery { Id = "org-123" }
   └─ messageBus.InvokeAsync<dynamic>(query)

3. Query Handler (Application Layer)
   └─ GetOrganizationByIdQueryHandler.Handle()
   └─ Call _organizationRepository.GetByIdAsync("org-123")

4. Repository (Infrastructure Layer)
   └─ OrganizationRepository.GetByIdAsync(id)
   └─ Query: WHERE Id == "org-123" AND DeletedAt IS NULL
   └─ Return Organization entity

5. Query Handler (continued)
   └─ Map Organization → OrganizationDto
   └─ Return DTO

6. HTTP Response
   └─ 200 OK
   └─ {
        "id": "org-123",
        "name": "Acme Corp",
        "industry": "Technology",
        ...
      }
```

---

## 🎯 Key Design Patterns

| Pattern | Purpose | Implementation |
|---------|---------|-----------------|
| **Aggregate Root** | Maintains consistency of entity group | Organization is single root |
| **Repository** | Abstracts data access | IRepository<T> interface |
| **CQRS** | Separates read/write logic | Wolverine MessageBus |
| **DTO** | Data transfer objects | *Dto classes in Application layer |
| **Value Object** | Immutable, value-based | Settings, Metadata records |
| **Soft Delete** | Logical deletion | DeletedAt, DeletedBy fields |
| **Guard Clause** | Input validation | Guard.AgainstNull(), etc. |
| **Dependency Injection** | Loose coupling | ConfigureServices in Program.cs |

---

## 💾 Database Support

So the Sql Server approach can be used, the SQL server should be installed
### Automatic Selection Logic
```
if (ConnectionString is empty or null)
    └─ Use SQLite (local development)
else if (SQL Server is available)
    └─ Use SQL Server (production)
else
    └─ Fallback to SQLite
```

### SQLite Location
```
Data/assessmentdb.db (created automatically in application directory)
```

### SQL Server Connection String
```json
{
  "ConnectionStrings": {
    "SqlConnectionString": "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=DecryptCodeAssessment;Integrated Security=True;"
  }
}
```

---

## 🧪 Testing Approach

### Unit Tests
- Test domain entities and business logic
- Test validation and guard clauses
- **Location**: `tests/Decryptcode.Assessment.Service.UnitTests/`

### Integration Tests
- Test repository operations
- Test API endpoints (conventional and minimal)
- Test data persistence
- **Location**: `tests/Decryptcode.Assessment.Service.IntegrationTests/`
- **Status**: ✅ 41/41 Tests Passing

---

## 📋 Query Examples

### Get All Organizations
```
GET /api/organizations
Response:
[
  { id, name, industry, tier, ... },
  ...
]
```

### Get Organizations with Filters
```
GET /api/organizations?industry=Technology&tier=Enterprise
```

### Get Specific Organization
```
GET /api/organizations/org-123
Response:
{ id, name, industry, tier, ... }
```

### Get Users in Organization
```
GET /api/users?orgId=org-123&role=Admin&active=true
```

### Get Organization Dashboard
```
GET /api/dashboard
Response:
{
  totalOrganizations: 10,
  totalUsers: 150,
  totalProjects: 45,
  totalTimeEntries: 2500,
  totalInvoices: 300
}
```

---

## 🚀 Getting Started

### Install Dependencies
```bash
dotnet restore
```

### Run Tests
```bash
dotnet test
# Result: ✅ 41/41 tests passing
```

### Run API (Conventional)
```bash
dotnet run --project src/Decryptcode.Assessment.Service.Api
# http://localhost:5000
```

### Run API (Minimal)
```bash
dotnet run --project src/DecryptCode.Assessment.Service.ApiMinimal
# http://localhost:5001
```

### Access Swagger Documentation
```
http://localhost:5000/swagger  (Conventional API)
http://localhost:5001/openapi  (Minimal API)
```

---

## ✨ Key Features

✅ **Domain-Driven Design**: Business logic in domain layer  
✅ **Clean Architecture**: Layered separation of concerns  
✅ **CQRS Pattern**: Command Query Responsibility Segregation  
✅ **Repository Pattern**: Abstract data access  
✅ **Aggregate Root Pattern**: Single entry point (Organization)  
✅ **Soft Delete**: Audit trail and data preservation  
✅ **Value Objects**: Immutable, reusable objects  
✅ **Dependency Injection**: Loose coupling, testability  
✅ **SQL Server + SQLite**: Flexible database support  
✅ **Conventional API**: Traditional MVC controllers  
✅ **Minimal API**: Modern endpoint mapping  
✅ **Comprehensive Tests**: 41/41 integration tests passing  

---

## 📚 Documentation Map

```
START HERE:
  ├─ This file (Summary)
  │
UNDERSTAND ARCHITECTURE:
  ├─ ARCHITECTURE.md (Detailed explanations)
  │  └─ Entity relationships
  │  └─ Aggregate root pattern
  │  └─ Repository interfaces
  │
VISUALIZE THE STRUCTURE:
  ├─ ARCHITECTURE_VISUAL_GUIDE.md (Diagrams and flowcharts)
  │  └─ Entity diagrams
  │  └─ Flow diagrams
  │  └─ State machines
  │
LEARN BY EXAMPLE:
  ├─ ARCHITECTURE_CODE_EXAMPLES.md (Real code snippets)
  │  └─ Domain entities
  │  └─ Repositories
  │  └─ Query handlers
  │  └─ API endpoints
  │
QUICK LOOKUP:
  └─ ARCHITECTURE_QUICK_REFERENCE.md (Cheat sheet)
     └─ Decision trees
     └─ Common patterns
     └─ Common commands
```

---

## 🔑 Remember

### The Core Concept
- **Organization** = Aggregate Root (the boss)
- **User, Project, Invoice, TimeEntry** = Reference Entities (employees)
- Reference entities serve Organization
- All changes go through repositories
- Consistency enforced at domain level

### The Pattern
1. **Request** hits endpoint
2. **Endpoint** creates query/command
3. **Handler** executes business logic
4. **Repository** persists to database
5. **Response** returned as DTO

### The Safety
- ✅ Private setters prevent invalid states
- ✅ Guard clauses validate inputs
- ✅ Repositories abstract data access
- ✅ Soft delete maintains audit trail
- ✅ CQRS separates concerns
- ✅ Dependency injection enables testing

---

## 📞 Need Help?

1. **Understand architecture**: Read ARCHITECTURE.md
2. **See visual diagrams**: Check ARCHITECTURE_VISUAL_GUIDE.md
3. **Find code examples**: Look in ARCHITECTURE_CODE_EXAMPLES.md
4. **Quick answer**: Search ARCHITECTURE_QUICK_REFERENCE.md

---

**Architecture Version**: 1.0  
**Last Updated**: 2024  
**Pattern**: Domain-Driven Design + Clean Architecture + CQRS  
**.NET Version**: .NET 10  
**Test Coverage**: ✅ 41/41 tests passing

---

## Summary Table

| Aspect | Details |
|--------|---------|
| **Aggregate Root** | Organization (1) |
| **Reference Entities** | User, Project, Invoice, TimeEntry (4) |
| **Value Objects** | Settings, Metadata (2) |
| **Repositories** | 5 (Organization, User, Project, Invoice, TimeEntry) |
| **API Versions** | 2 (Conventional Controllers + Minimal API) |
| **Database Support** | SQL Server + SQLite (automatic fallback) |
| **Design Patterns** | DDD, Clean Architecture, CQRS, Repository, DTO |
| **Testing** | 41/41 integration tests passing ✅ |
| **Documentation** | 4 comprehensive guides |

---

**Welcome to a well-architected, maintainable, and scalable application! 🎉**
