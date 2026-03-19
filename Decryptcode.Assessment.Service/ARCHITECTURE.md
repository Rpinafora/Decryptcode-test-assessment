# Decryptcode Assessment Service - Architecture Documentation

## Overview

The **Decryptcode Assessment Service** is a backend platform built with **.NET 10** following enterprise-grade architectural principles. It manages organizations, users, projects, time entries, and invoices in a consulting/professional services domain.

### Key Characteristics
- **Target Framework**: .NET 10
- **Architecture Style**: Clean Architecture
- **Design Patterns**: CQRS, DDD, Repository, Dependency Injection
- **Database**: SQL Server with Entity Framework Core
- **API Style**: RESTful with minimal APIs option



This document describes the layered architecture, domain model, and design patterns used in the Decryptcode Assessment Service. The application follows **Domain-Driven Design (DDD)**, **Clean Architecture**, and **CQRS** (Command Query Responsibility Segregation) principles.

---

## Domain Model Architecture

### Aggregate Roots (Root Entities)

An **Aggregate Root** is a domain-driven design pattern that acts as the entry point for accessing related entities within an aggregate. Each aggregate root is responsible for maintaining consistency of its data.

#### 1. **Organization** (Primary Aggregate Root)
- **Location**: `src/Decryptcode.Assessment.Service.Domain/Entities/AggregateRoots/Organization.cs`
- **Responsibility**: Manages organizational data and acts as the parent aggregate for users, projects, and invoices
- **Key Properties**:
  - `Id`: Unique identifier
  - `Name`: Organization name
  - `Slug`: URL-friendly identifier
  - `Industry`: Industry classification
  - `Tier`: Service tier (e.g., enterprise, professional)
  - `ContactEmail`: Contact email
  - `Description`: Organization description
  - `Settings`: Value object for organization settings (currency, timezone, etc.)
  - `Metadata`: Value object for tracking (legacy ID, migration date, source)
  - **Navigation Properties**: Users, Projects, Invoices

```
┌─────────────────────────────────┐
│    AGGREGATE ROOT               │
│        Organization             │
├─────────────────────────────────┤
│  • Id                           │
│  • Name                         │
│  • Slug                         │
│  • Industry                     │
│  • Tier                         │
│  • ContactEmail                 │
│  • Description                  │
│  • Settings (Value Object)      │
│  • Metadata (Value Object)      │
│  • CreatedAt / UpdatedAt        │
│  • DeletedAt (Soft Delete)      │
└─────────────────────────────────┘
         ▲     ▲      ▲
         │     │      │
    ┌────┴─┬───┴──┬───┴─────────┐
    │      │      │             │
    v      v      v             v
  Users Projects Invoices  TimeEntries
(Reference) (Reference) (Reference) (Reference)
```

---

### Reference Entities

**Reference Entities** are entities that are NOT aggregate roots. They are owned or referenced by an aggregate root but cannot exist independently in the domain model. They are managed through their parent aggregate.

#### 1. **User** (Reference Entity)
- **Location**: `src/Decryptcode.Assessment.Service.Domain/Entities/ReferenceEntities/User.cs`
- **Parent Aggregate**: Organization
- **Responsibility**: Represents users within an organization
- **Key Properties**:
  - `Id`: Unique identifier
  - `OrgId`: Foreign key to Organization (establishes parent relationship)
  - `Email`: User email
  - `Name`: User full name
  - `Role`: User role (e.g., admin, editor, viewer)
  - `Active`: Account active status
  - `Bio`: User biography
  - **Navigation Properties**: 
    - `Organization`: Reference to parent Organization
    - `TimeEntries`: Collection of time entries created by this user

**Business Rules**:
- Cannot be created without an Organization
- Deletion of Organization cascades to all Users
- Time entries must reference a valid User

#### 2. **Project** (Reference Entity)
- **Location**: `src/Decryptcode.Assessment.Service.Domain/Entities/ReferenceEntities/Project.cs`
- **Parent Aggregate**: Organization
- **Responsibility**: Represents projects within an organization
- **Key Properties**:
  - `Id`: Unique identifier
  - `OrgId`: Foreign key to Organization (establishes parent relationship)
  - `Name`: Project name
  - `Status`: Project status (enum: Active, OnHold, Completed, Archived)
  - `BudgetHours`: Budgeted hours for the project
  - `StartDate`: Project start date
  - `EndDate`: Project end date
  - `Description`: Project description
  - **Navigation Properties**:
    - `Organization`: Reference to parent Organization
    - `Invoices`: Collection of invoices related to this project
    - `TimeEntries`: Collection of time entries for this project

**Business Rules**:
- Cannot be created without an Organization
- Status transitions follow defined workflow (Active → OnHold → Completed → Archived)
- Budget hours must be positive
- End date must be after start date (if both provided)
- Deletion of Organization cascades to all Projects

#### 3. **TimeEntry** (Reference Entity)
- **Location**: `src/Decryptcode.Assessment.Service.Domain/Entities/ReferenceEntities/TimeEntry.cs`
- **Parent Aggregate**: Organization (through Project and User)
- **Responsibility**: Tracks time spent on project work
- **Key Properties**:
  - `Id`: Unique identifier
  - `UserId`: Foreign key to User
  - `ProjectId`: Foreign key to Project
  - `Duration`: Duration in hours or minutes
  - `LogDate`: Date time entry was logged
  - `Description`: Work description
  - `BillingStatus`: Billing status (billable, non-billable)
  - **Navigation Properties**:
    - `User`: Reference to the user who logged the entry
    - `Project`: Reference to the project

**Business Rules**:
- Must reference both a valid User and Project
- Both referenced User and Project must belong to the same Organization
- Duration must be positive
- Cannot be backdated beyond organization's creation date

#### 4. **Invoice** (Reference Entity)
- **Location**: `src/Decryptcode.Assessment.Service.Domain/Entities/ReferenceEntities/Invoice.cs`
- **Parent Aggregate**: Organization
- **Responsibility**: Represents billing invoices
- **Key Properties**:
  - `Id`: Unique identifier
  - `OrgId`: Foreign key to Organization
  - `ProjectId`: Foreign key to Project (optional)
  - `InvoiceNumber`: Invoice number
  - `Amount`: Invoice amount
  - `Status`: Invoice status (enum: Draft, Sent, Paid, Overdue, Cancelled)
  - `DueDate`: Due date for payment
  - `IssuedDate`: Date invoice was issued
  - `Description`: Invoice description
  - **Navigation Properties**:
    - `Organization`: Reference to parent Organization
    - `Project`: Reference to related Project (optional)

**Business Rules**:
- Cannot be created without an Organization
- Project reference must belong to the same Organization (if provided)
- Status transitions follow: Draft → Sent → Paid (or Overdue, Cancelled)
- Amount must be positive
- Due date should be after issued date
- Deletion of Organization cascades to all Invoices

---

## Value Objects

**Value Objects** are immutable objects that are defined by their attributes, not by their identity.

### 1. **Settings** (Value Object)
```csharp
public record Settings
{
    public string Currency { get; init; }      // e.g., USD, EUR
    public string Timezone { get; init; }      // e.g., UTC, EST
    public string DefaultLocale { get; init; } // e.g., en-US
    public bool AllowOvertime { get; init; }   // Overtime policy
}
```

### 2. **Metadata** (Value Object)
```csharp
public record Metadata
{
    public string? LegacyId { get; init; }     // ID from old system
    public DateTime? MigratedAt { get; init; } // Migration timestamp
    public string? Source { get; init; }       // Data source
}
```

---

## Entity Relationship Diagram

```
┌──────────────────────────┐
│   ORGANIZATION           │ ◄──────────────── AGGREGATE ROOT
│   (Aggregate Root)       │
├──────────────────────────┤
│ • Id (PK)                │
│ • Name                   │
│ • Slug                   │
│ • Industry               │
│ • Tier                   │
│ • ContactEmail           │
│ • Settings (VO)          │
│ • Metadata (VO)          │
│ • Soft Delete Fields     │
└──────────────────────────┘
      │        │        │
      │        │        └──────────────────┐
      │        │                           │
      │1..N    │1..N                       │1..N
      ▼        ▼                           ▼
┌──────────────┐  ┌──────────────┐  ┌──────────────┐
│    USER      │  │   PROJECT    │  │   INVOICE    │
│  (Reference) │  │  (Reference) │  │  (Reference) │
├──────────────┤  ├──────────────┤  ├──────────────┤
│ • Id (PK)    │  │ • Id (PK)    │  │ • Id (PK)    │
│ • OrgId (FK) │  │ • OrgId (FK) │  │ • OrgId (FK) │
│ • Email      │  │ • Name       │  │ • ProjectId  │
│ • Name       │  │ • Status     │  │ • Amount     │
│ • Role       │  │ • Budget     │  │ • Status     │
│ • Active     │  │ • StartDate  │  │ • DueDate    │
│ • Bio        │  │ • EndDate    │  │ • IssuedDate │
└──────────────┘  └──────────────┘  └──────────────┘
      │                  │
      │1..N              │1..N
      ▼                  ▼
┌──────────────────────────────────┐
│       TIME ENTRY                 │
│      (Reference)                 │
├──────────────────────────────────┤
│ • Id (PK)                        │
│ • UserId (FK)                    │
│ • ProjectId (FK)                 │
│ • Duration                       │
│ • LogDate                        │
│ • BillingStatus                  │
└──────────────────────────────────┘

Legend:
  ◄── Aggregate Root (primary entity with business logic)
  ─── Reference Entities (managed through aggregate root)
  1..N ─── One-to-Many relationships
  FK ─── Foreign Key
  PK ─── Primary Key
  VO ─── Value Object (immutable)
```

---

## Key Architectural Principles

### 1. **Aggregate Root Pattern**
- **Organization** is the only aggregate root
- All other entities are reference entities managed through Organization
- Ensures data consistency and transactional boundaries
- External code should only reference entities through the Organization aggregate

### 2. **Encapsulation**
- All entities use private setters
- State changes go through explicit methods
- Properties initialized through constructors with validation
- Prevents invalid state transitions

### 3. **Soft Delete Pattern**
- All entities include `DeletedAt` and `DeletedBy` fields
- Logical deletion instead of physical removal
- Maintains data integrity and audit trail
- Queries automatically filter deleted records

### 4. **Value Objects (Immutable)**
- Settings and Metadata are implemented as immutable records
- Cannot be changed after creation
- Compared by value, not reference
- Changes require creating new instances

### 5. **Repository Pattern**
- Each aggregate root has a corresponding repository
- Repositories abstract data access
- CQRS queries use direct repository access
- Transactions managed at repository level

### 6. **Business Logic Encapsulation**
- Domain logic lives in entities, not application services
- Validators ensure rules at construction time
- Guard clauses prevent invalid states
- Status transitions follow defined workflows

---

## Repository Interfaces

All repositories inherit from the generic `IRepository<T>` base interface:

```csharp
public interface IRepository<T> where T : IEntity
{
    Task AddAsync(T entity);
    Task UpdateAsync(T entity);
    Task DeleteAsync(T entity);
    Task<T?> GetByIdAsync(string id);
    Task<IEnumerable<T>> GetAllAsync();
}
```

### Specialized Repositories

1. **IOrganizationRepository**
   - GetByIdAsync(string id)
   - GetAllAsync()
   - GetBySlugAsync(string slug)
   - GetDashboardAsync() - Aggregated statistics
   - GetAllFiltered(string? industry, string? tier)

2. **IUserRepository**
   - GetByIdAsync(string id)
   - GetByEmailAsync(string email)
   - GetAllAsync()
   - GetAllByOrgAsync(string orgId)
   - GetAllFiltered(string? orgId, string? role, bool? active)

3. **IProjectRepository**
   - GetByIdAsync(string id)
   - GetAllAsync()
   - GetAllByOrgAsync(string orgId)
   - GetAllFiltered(string? orgId, string? status)

4. **ITimeEntriesRepository**
   - GetByIdAsync(string id)
   - GetAllAsync()
   - GetByUserAsync(string userId)
   - GetByProjectAsync(string projectId)
   - GetAllFiltered(string? userId, string? projectId, DateTime? from, DateTime? to)

5. **IInvoiceRepository**
   - GetByIdAsync(string id)
   - GetAllAsync()
   - GetByOrgAsync(string orgId)
   - GetAllFiltered(string? orgId, string? status)

---

## Layer Architecture

```
┌─────────────────────────────────────┐
│   Presentation Layer (API)          │
│  ├─ Controllers (conventional API)  │
│  └─ Minimal API Endpoints           │
└────────────┬────────────────────────┘
             │
┌────────────▼────────────────────────┐
│   Application Layer (CQRS)          │
│  ├─ Queries                         │
│  ├─ Commands                        │
│  ├─ DTOs                            │
│  └─ Application Services            │
└────────────┬────────────────────────┘
             │
┌────────────▼────────────────────────┐
│   Domain Layer (Business Logic)     │
│  ├─ Entities & Aggregate Roots      │
│  ├─ Value Objects                   │
│  ├─ Repositories Interfaces         │
│  ├─ Domain Events                   │
│  └─ Guards & Validators             │
└────────────┬────────────────────────┘
             │
┌────────────▼────────────────────────┐
│   Infrastructure Layer              │
│  ├─ EF Core DbContext               │
│  ├─ Repository Implementations      │
│  ├─ Database Migrations             │
│  ├─ SQL Server / SQLite             │
│  └─ Data Seeding                    │
└─────────────────────────────────────┘
```

---

## Data Consistency & Transactions

### Transaction Boundaries
- Transactions are managed at the **Application Service** level
- Each command execution is a single transaction
- Related reference entities are included in the same transaction
- Cascading deletes handled through entity relationships

### Cascade Rules
```
Organization Deletion:
  └─► Delete all Users
  └─► Delete all Projects
  └─► Delete all Invoices
  └─► Delete all TimeEntries (through User/Project)

Project Deletion:
  └─► Delete all TimeEntries
  └─► Delete all Invoices (if project-specific)

User Deletion:
  └─► Delete all TimeEntries
```

---

## Design Patterns Used

| Pattern | Location | Purpose |
|---------|----------|---------|
| **Aggregate Root** | Domain/Entities/AggregateRoots | Encapsulates entity groups with transactional boundaries |
| **Repository** | Infrastructure/Repositories | Abstracts data access and persistence |
| **Value Object** | Domain/Entities/ValueObjects | Immutable objects defined by value, not identity |
| **DTO** | Application/DTOs | Transfer objects for API responses |
| **Guard Clause** | Domain/Guards | Validates preconditions and prevents invalid states |
| **CQRS** | Application/Queries, Commands | Separates read and write operations |
| **Dependency Injection** | Program.cs | Loosely coupled, testable components |
| **Soft Delete** | Domain/Entities (base class) | Logical deletion with audit trail |
| **Mediator** | Wolverine MessageBus | Command/Query routing and handling |

---

## Database Support

The application supports both **SQL Server** and **SQLite**:

- **SQL Server**: Recommended for production
- **SQLite**: Used for local development when SQL Server is unavailable

Configuration in `ServicesInjector.cs`:
```csharp
if (string.IsNullOrEmpty(connectionString))
{
    // Uses SQLite automatically
    AddSqliteDatabase(services);
}
else if (IsSqlServerAvailable(connectionString))
{
    // Uses SQL Server if available
    AddSqlServerDatabase(services, connectionString);
}
else
{
    // Falls back to SQLite if SQL Server is not available
    AddSqliteDatabase(services);
}
```

---

## API Endpoints Structure

### Conventional API (Controllers)
- Location: `src/Decryptcode.Assessment.Service.Api/Controllers/`
- Pattern: RESTful controllers with attribute routing
- Response Format: RequestResult wrapper with status codes

### Minimal API (Alternative)
- Location: `src/DecryptCode.Assessment.Service.ApiMinimal/Endpoints/`
- Pattern: Endpoint mapping with route groups
- Response Format: Same RequestResult wrapper format

Both implementations use the **same domain and application layer**, ensuring consistency.

---

## Testing Strategy

### Unit Tests
- Test domain entities and business logic
- Validate aggregate root invariants
- Located in: `tests/Decryptcode.Assessment.Service.UnitTests/`

### Integration Tests
- Test repository operations
- Test API endpoints (both conventional and minimal)
- Test data persistence
- Located in: `tests/Decryptcode.Assessment.Service.IntegrationTests/`

---

## Summary

```
┌─────────────────────────────────────────────────┐
│  Organization (AGGREGATE ROOT)                  │
│  ├─ Owns User entities (Reference)              │
│  ├─ Owns Project entities (Reference)           │
│  ├─ Owns Invoice entities (Reference)           │
│  └─ Indirectly owns TimeEntry (through User)    │
│                                                  │
│  Key Principles:                                │
│  • Single point of entry (Organization)         │
│  • Encapsulated state with validation           │
│  • Immutable value objects                      │
│  • Repository abstraction                       │
│  • CQRS for read/write separation               │
│  • Soft delete for data preservation            │
│  • Support for SQL Server & SQLite              │
└─────────────────────────────────────────────────┘
```

---

**Version**: 1.0  
**Last Updated**: 2024  
**Architecture Pattern**: Domain-Driven Design (DDD) + Clean Architecture + CQRS
