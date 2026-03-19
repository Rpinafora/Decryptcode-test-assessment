# Visual Architecture Guide

## Quick Reference: Aggregate Roots vs Reference Entities

### Aggregate Roots (Decision Points)

```
✅ AGGREGATE ROOT: Organization
┌────────────────────────────────────────────┐
│  • Is an independent entity                │
│  • Has a repository interface              │
│  • Can be queried independently            │
│  • Represents a bounded context            │
│  • Manages lifecycle of related entities   │
│  • Enforces business rules for aggregate   │
│  • Is a transaction boundary               │
└────────────────────────────────────────────┘
```

### Reference Entities (Non-Aggregate Roots)

```
❌ NOT AGGREGATE ROOTS: User, Project, Invoice, TimeEntry

User (Reference Entity)
┌────────────────────────────────────────────┐
│  • Belongs to an Organization              │
│  • Cannot exist without Organization       │
│  • Has OrgId foreign key                   │
│  • Managed through Organization            │
│  • Accessed via IUserRepository            │
│  • But logically owned by Organization     │
└────────────────────────────────────────────┘

Project (Reference Entity)
┌────────────────────────────────────────────┐
│  • Belongs to an Organization              │
│  • Cannot exist without Organization       │
│  • Has OrgId foreign key                   │
│  • Managed through Organization            │
│  • Accessed via IProjectRepository         │
│  • But logically owned by Organization     │
└────────────────────────────────────────────┘

Invoice (Reference Entity)
┌────────────────────────────────────────────┐
│  • Belongs to an Organization              │
│  • Cannot exist without Organization       │
│  • Has OrgId foreign key                   │
│  • May reference a Project                 │
│  • Accessed via IInvoiceRepository         │
│  • But logically owned by Organization     │
└────────────────────────────────────────────┘

TimeEntry (Reference Entity)
┌────────────────────────────────────────────┐
│  • Created by a User                       │
│  • Associated with a Project               │
│  • Both User & Project belong same Org     │
│  • Has UserId and ProjectId foreign keys   │
│  • Accessed via ITimeEntriesRepository     │
│  • But logically owned by Organization     │
└────────────────────────────────────────────┘
```

---

## Entity Relationships Flowchart

```
User Creates TimeEntry on Project
└─ All must belong to same Organization

                    ┌─────────────────────────┐
                    │   ORGANIZATION          │
                    │   (Aggregate Root)      │
                    └───────┬─────┬───────┬───┘
                            │     │       │
                    ┌───────▼──┐  │  ┌───▼────────┐
                    │  User 1  │  │  │ Project 1  │
                    └────┬─────┘  │  └───┬────────┘
                         │        │      │
                    ┌────▼────────▼──────▼────┐
                    │   TimeEntry (1-1-1)     │
                    │   Must have:             │
                    │   • UserId = User 1      │
                    │   • ProjectId = Project 1│
                    │   • Both same Org        │
                    └─────────────────────────┘
```

---

## Data Flow: Creating TimeEntry

```
User Request
    ↓
Application Layer (Command Handler)
    ├─ Validate User exists (in Org)
    ├─ Validate Project exists (in Org)
    ├─ Create TimeEntry entity
    └─ Persist via Repository
        ↓
    Repository Layer
        ├─ Add to DbContext
        ├─ SaveChangesAsync
        └─ Return created entity
            ↓
    Response to User
```

---

## Ownership Hierarchy

```
        Organization (Owner/Aggregate Root)
        ├─ Users (Owned - Reference Entities)
        │   └─ Cannot be deleted independently
        │   └─ Must belong to Organization
        │
        ├─ Projects (Owned - Reference Entities)
        │   └─ Cannot be deleted independently
        │   └─ Must belong to Organization
        │
        ├─ Invoices (Owned - Reference Entities)
        │   └─ Cannot be deleted independently
        │   └─ Must belong to Organization
        │
        └─ TimeEntries (Indirectly Owned)
            └─ Created by Users (Reference)
            └─ Associated with Projects (Reference)
            └─ Both belong to Organization
            └─ Cascading delete through User/Project
```

---

## Repository Access Pattern

### CORRECT ✅

```csharp
// Access through repositories
var org = await _organizationRepository.GetByIdAsync(orgId);
var users = await _userRepository.GetAllByOrgAsync(orgId);
var projects = await _projectRepository.GetAllByOrgAsync(orgId);

// Reference entities have their own repositories
var timeEntry = await _timeEntriesRepository.GetByIdAsync(timeEntryId);
```

### INCORRECT ❌

```csharp
// DON'T: Try to directly access User without repository
var user = await _dbContext.Users.FindAsync(userId);

// DON'T: Create TimeEntry without validating User & Project
var timeEntry = new TimeEntry { UserId = "X", ProjectId = "Y" };
```

---

## Query Patterns

### Get Organization with Related Data

```csharp
// Get organization (aggregate root)
var organization = await _organizationRepository.GetByIdAsync(orgId);

// Get users within organization
var users = await _userRepository.GetAllByOrgAsync(orgId);

// Get projects within organization
var projects = await _projectRepository.GetAllByOrgAsync(orgId);

// Get time entries for specific user/project
var timeEntries = await _timeEntriesRepository.GetAllFiltered(
    userId: userId,
    projectId: projectId
);
```

### Business Query Example

```csharp
// Get organization dashboard (aggregated statistics)
var dashboard = await _organizationRepository.GetDashboardAsync(orgId);

// Result contains:
// - Total organizations
// - Total users
// - Total projects
// - Total time entries
// - Total invoices
```

---

## State Transitions

### Organization States
```
┌─────────────┐
│   Created   │
└──────┬──────┘
       │
       ▼
┌──────────────┐
│   Active     │◄──┐
└──────┬───────┘   │
       │           │
       ▼           │
┌──────────────┐   │
│   Deleted    │   │
│ (SoftDelete) │   │
└──────────────┘   │
       ▲           │
       └───────────┘
    (Can Restore)
```

### Project Status Transitions
```
┌─────────┐
│ Active  │
└────┬────┘
     │
     ▼
┌──────────┐
│ OnHold   │
└────┬─────┘
     │
     ▼
┌───────────┐
│ Completed │
└─────┬─────┘
      │
      ▼
┌──────────┐
│ Archived │
└──────────┘
```

### Invoice Status Transitions
```
┌────────┐
│ Draft  │
└───┬────┘
    │
    ▼
┌──────────┐
│  Sent    │
└───┬──────┘
    │
    ├─►┌──────────┐
    │  │   Paid   │
    │  └──────────┘
    │
    └─►┌──────────┐
       │ Overdue  │
       └──────────┘

    ┌──────────┐
    │ Cancelled│ (From any state)
    └──────────┘
```

---

## Validation Flow

```
API Request (Create TimeEntry)
    ↓
Application Handler
    ├─ Validate TimeEntry DTO
    ├─ Check User exists & is active
    ├─ Check Project exists & is active
    ├─ Check User & Project same Organization
    ├─ Create TimeEntry entity
    │   ↓
    │   Entity Constructor Validation
    │   ├─ Validate Duration > 0
    │   ├─ Validate LogDate valid
    │   ├─ Validate BillingStatus valid
    │   └─ Guard clauses prevent invalid state
    │
    └─► Persist to Database
        ↓
    Success/Failure Response
```

---

## Error Handling: Invariant Violations

```
Scenario: Trying to create TimeEntry with non-existent User

1. API receives request
2. Application handler fetches User from repository
3. User is null (doesn't exist)
4. Handler throws ArgumentNullException or Domain Exception
5. API returns 404 or 400 error response

✅ Invalid state is PREVENTED (not persisted)
✅ Domain invariants maintained
✅ Data consistency guaranteed
```

---

## Soft Delete Behavior

```
Organization Organization exists (Active)
    ├─ CreatedAt: 2024-01-01
    ├─ DeletedAt: null
    ├─ DeletedBy: null

Organization Soft Deleted
    ├─ CreatedAt: 2024-01-01
    ├─ DeletedAt: 2024-06-15 15:30:00
    ├─ DeletedBy: "admin@org.com"

Query Behavior:
    ├─ GetByIdAsync(id) → Returns null (if deleted)
    ├─ GetAllAsync() → Excludes deleted records
    └─ Database still has the record for audit trail

Restore:
    └─ Set DeletedAt = null, DeletedBy = null
```

---

## API Endpoint to Database Flow

```
HTTP Request
    ↓
Endpoint Handler (Minimal API/Controller)
    ├─ Parse route/query parameters
    ├─ Inject dependencies (MessageBus, DbContext, etc.)
    ├─ Call command/query handler
    ↓
CQRS Handler (Application Layer)
    ├─ Validate request DTO
    ├─ Get entities from repositories
    ├─ Apply business logic
    ├─ Create/update/delete entities
    ├─ Persist via repository
    ↓
Repository (Infrastructure Layer)
    ├─ Create DbContext operations
    ├─ Add/Update/Remove from DbSet
    ├─ SaveChangesAsync
    ├─ Transaction management
    ↓
Entity Framework Core
    ├─ Translate to SQL
    ├─ Execute against database
    ├─ Handle relationships
    ├─ Apply migrations
    ↓
Database (SQL Server or SQLite)
    ├─ Store data
    ├─ Enforce foreign keys
    ├─ Manage transactions
    ↓
Response (Application → API)
    ├─ Return RequestResult object
    ├─ Status code (200, 400, 404, 500, etc.)
    ├─ Data payload
    └─ Error messages (if any)
```

---

## Summary: Why This Architecture?

| Benefit | How Achieved |
|---------|-------------|
| **Data Consistency** | Single aggregate root (Organization) controls all changes |
| **Transaction Boundaries** | Repositories manage transactions per operation |
| **Encapsulation** | Private setters, constructors with validation |
| **Testability** | Repository interfaces allow mocking |
| **Scalability** | Separate application and infrastructure layers |
| **Maintainability** | Clear separation of concerns (DDD) |
| **Auditability** | Soft delete tracks who deleted and when |
| **Flexibility** | Supports SQL Server and SQLite |

---

**Remember:**
- 🟢 **Organization** = Aggregate Root (boss)
- 🔵 **User, Project, Invoice, TimeEntry** = Reference Entities (employees of Organization)
- 📊 Reference entities serve the aggregate root
- 🔄 All changes go through repositories
- ⚡ Consistency and business rules enforced at domain level
