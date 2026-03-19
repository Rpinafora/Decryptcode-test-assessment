# Architecture Visual Guide

## System Architecture Diagram

```
┌─────────────────────────────────────────────────────────────────────────────┐
│                          CLIENT APPLICATIONS                                │
│                  (Angular Frontend, Mobile Apps, etc.)                      │
└────────────────────────────────────────────────────┬────────────────────────┘
                                                      │ HTTP/REST
                                                      ▼
┌─────────────────────────────────────────────────────────────────────────────┐
│                        PRESENTATION LAYER (API)                             │
│                                                                             │
│  ┌──────────────────────┬──────────────────────┬─────────────────────┐      │
│  │ OrganizationsCtrl    │ ProjectsCtrl         │ DashboardCtrl       │      │
│  │ - GetAll             │ - GetAll             │ - GetDashboard      │      │
│  │ - GetById            │ - GetById            │                     │      │
│  │ - Create (Planned)   │ - Create (Planned)   │ [+ 10 more routes]  │      │
│  │ - Update (Planned)   │ - Update (Planned)   │                     │      │
│  │ - Delete (Planned)   │ - Delete  (Planned)  │                     │      │
│  └──────────────────────┴──────────────────────┴─────────────────────┘      │
│                                                                             │
│  ┌─────────────────────────────────────────────────────────────────────┐    │
│  │  Middleware Stack                                                   │    │
│  │  • Exception Handling [PLANNED]                                     │    │
│  │  • Logging & Correlation [PLANNED]                                  │    │
│  │  • Authentication & Authorization [PLANNED]                         │    │
│  │  • Rate Limiting [PLANNED]                                          │    │
│  │  • CORS Configuration                                               │    │
│  └─────────────────────────────────────────────────────────────────────┘    │
└────────────────────────────────────────────────────┬────────────────────────┘
                                                      │ Dependency Injection
                                                      ▼
┌─────────────────────────────────────────────────────────────────────────────┐
│                    APPLICATION LAYER (Business Logic)                       │
│                         CQRS Pattern                                         │
│                                                                              │
│  ┌──────────────────────────┐    ┌──────────────────────────────────────┐ │
│  │      COMMANDS [PLANNED]  │    │       QUERIES                        │ │
│  │  (Write Operations)      │    │   (Read Operations)                  │ │
│  │                          │    │                                      │ │
│  │ • CreateOrganization     │    │ • GetOrganizations                   │ │
│  │ • UpdateOrganization     │    │ • GetOrganizationById                │ │
│  │ • DeleteOrganization     │    │ • GetDashboard                       │ │
│  │ • CreateProject          │    │ • GetAllProjects                     │ │
│  │ • [+ more commands]      │    │ • [+ more queries]                   │ │
│  │                          │    │                                      │ │
│  │ ↓                        │    │ ↓                                    │ │
│  │ CommandHandlers          │    │ QueryHandlers                        │ │
│  │ (Orchestrate domain)     │    │ (Return projections)                 │ │
│  └──────────────────────────┘    └──────────────────────────────────────┘ │
│                                                                           │
│  ┌─────────────────────────────────────────────────────────────────────┐  │
│  │  Mappings & DTOs                                                    │  │
│  │  • OrganizationMappings → OrganizationDto                          │   │
│  │  • ProjectMappings → ProjectDto                                    │   │
│  │  • DashboardMappings → DashboardDto                                │   │
│  │  • [+ more mappings]                                               │   │
│  └─────────────────────────────────────────────────────────────────────┘  │
└────────────────────────────────────────────────────┬────────────────────────┘
                                                      │
                                                      ▼
┌─────────────────────────────────────────────────────────────────────────────┐
│                      DOMAIN LAYER (Business Rules)                          │
│                         Domain-Driven Design                                │
│                                                                              │
│  ┌──────────────────────┐  ┌──────────────────────┐  ┌──────────────────┐ │
│  │  Aggregate Roots     │  │  Value Objects       │  │  Enums           │ │
│  │                      │  │                      │  │                  │ │
│  │ • Organization       │  │ • Settings           │  │ • ProjectStatus  │ │
│  │ • User               │  │ • Metadata           │  │ • InvoiceStatus  │ │
│  │ • Project (ref'd)    │  │                      │  │ • Others...      │ │
│  │ • TimeEntry (ref'd)  │  │  (Immutable Records) │  │                  │ │
│  │ • Invoice (ref'd)    │  │                      │  │                  │ │
│  │                      │  │                      │  │                  │ │
│  │ (Enforce rules)      │  │ (Encapsulate values) │  │ (Type safety)    │ │
│  └──────────────────────┘  └──────────────────────┘  └──────────────────┘ │
│                                                                              │
│  ┌─────────────────────────────────────────────────────────────────────┐  │
│  │  Repository Interfaces (Abstraction)                                │  │
│  │                                                                     │  │
│  │  • IRepository<T> (Generic)                                         │  │
│  │  • IOrganizationRepository                                          │  │
│  │  • IProjectRepository                                               │  │
│  │  • [+ more repository interfaces]                                   │  │
│  │                                                                     │  │
│  │  Business Rules:                                                    │  │
│  │  • Soft delete support                                              │  │
│  │  • Audit tracking (CreatedAt, UpdatedAt, DeletedAt)                │  │
│  │  • Immutable IDs and timestamps                                     │  │
│  └─────────────────────────────────────────────────────────────────────┘  │
│                                                                              │
│  ┌─────────────────────────────────────────────────────────────────────┐  │
│  │  Domain Exceptions & Guards                                         │  │
│  │  • Guard.NullOrEmpty()                                              │  │
│  │  • Guard.OutOfRange()                                               │  │
│  │  • DomainException [PLANNED]                                        │  │
│  │  • ValidationException [PLANNED]                                    │  │
│  └─────────────────────────────────────────────────────────────────────┘  │
└────────────────────────────────────────────────────┬────────────────────────┘
                                                      │
                                                      ▼
┌─────────────────────────────────────────────────────────────────────────────┐
│               INFRASTRUCTURE LAYER (Data Access & External Services)        │
│                                                                              │
│  ┌──────────────────────────┐         ┌────────────────────────────────┐  │
│  │  Entity Framework Core   │         │  Repository Implementations   │  │
│  │                          │         │                              │  │
│  │ • DbContext (ApiContext) │  ────→  │ • OrganizationRepository     │  │
│  │ • DbSet<T> Properties    │         │ • ProjectRepository          │  │
│  │ • Migrations             │         │ • UserRepository             │  │
│  │ • Configurations         │         │ • InvoiceRepository          │  │
│  │ • Seed Data              │         │ • TimeEntryRepository        │  │
│  └──────────────────────────┘         │ • [+ more implementations]   │  │
│           │                           └────────────────────────────────┘  │
│           │                                                                 │
│           └─────────┬──────────────────────────────────────────────┬──────┘
│                     │                                              │
│                     ▼                                              ▼
│          ┌────────────────────┐                        ┌─────────────────┐
│          │   SQL Server       │                        │  Configuration  │
│          │                    │                        │                 │
│          │ • Organizations    │                        │ • Entity Config │
│          │ • Projects         │                        │ • Relationships │
│          │ • Users            │                        │ • Constraints   │
│          │ • TimeEntries      │                        │ • Indexes       │
│          │ • Invoices         │                        │                 │
│          │ • Migrations       │                        │                 │
│          └────────────────────┘                        │                 │
│                                                        │                 │
│                                                        └─────────────────┘
└─────────────────────────────────────────────────────────────────────────────┘
```

---

## CQRS Pattern - Detailed Flow

```
CLIENT REQUEST
      │
      ▼
┌─────────────────────────────────────────────┐
│     Controller / Endpoint Handler           │
│  (Gets request from HTTP)                   │
└────────────┬────────────────────────────────┘
             │
             ├─── Is it a WRITE? ──→ ┌──────────────────────────────────┐
             │                       │  COMMAND Path                    │
             │                       │  (Modifies State)                │
             │                       │                                  │
             │                       │ 1. Parse request into Command    │
             │                       │ 2. Send to CommandHandler        │
             │                       │ 3. Handler:                      │
             │                       │    - Validate input              │
             │                       │    - Create/Update domain object │
             │                       │    - Save to repository          │
             │                       │    - Return result               │
             │                       │ 4. Return to controller          │
             │                       │ 5. Map to response DTO           │
             │                       │ 6. Return 201/200 to client      │
             │                       └──────────────────────────────────┘
             │
             └─── Is it a READ? ──→ ┌──────────────────────────────────┐
                                    │  QUERY Path                      │
                                    │  (Reads State)                   │
                                    │                                  │
                                    │ 1. Parse request into Query      │
                                    │ 2. Send to QueryHandler          │
                                    │ 3. Handler:                      │
                                    │    - Execute query/projection    │
                                    │    - No side effects             │
                                    │    - Return projection/DTO       │
                                    │ 4. Return to controller          │
                                    │ 5. Return 200 to client          │
                                    └──────────────────────────────────┘
```

---

## Data Flow Example: Create Organization (Planned)

```
1. CLIENT REQUEST
   ┌────────────────────────────────┐
   │ POST /api/organizations        │
   │ {                              │
   │   "name": "Acme Corp",        │
   │   "slug": "acme-corp",        │
   │   "industry": "Technology",   │
   │   ...                          │
   │ }                              │
   └────────────────────────────────┘
              │
              ▼
2. CONTROLLER
   ┌────────────────────────────────┐
   │ OrganizationsController        │
   │ .Create(createOrgDto)          │
   └────────────────────────────────┘
              │
              ▼
3. SEND COMMAND
   ┌────────────────────────────────┐
   │ CreateOrganizationCommand      │
   │ (Contains request data)        │
   │ ↓ via MediatR                  │
   └────────────────────────────────┘
              │
              ▼
4. COMMAND HANDLER
   ┌────────────────────────────────┐
   │ CreateOrganizationCommandHandler│
   │                                │
   │ Validate input                 │
   │ Create Organization entity     │
   │ (Domain Logic)                 │
   │ ↓                              │
   │ _repository.AddAsync()         │
   │ ↓                              │
   │ await SaveChangesAsync()       │
   └────────────────────────────────┘
              │
              ▼
5. REPOSITORY LAYER
   ┌────────────────────────────────┐
   │ OrganizationRepository         │
   │                                │
   │ _context.Organizations.Add()   │
   │ ↓                              │
   │ Entity Framework Core          │
   │ ↓                              │
   └────────────────────────────────┘
              │
              ▼
6. DATABASE
   ┌────────────────────────────────┐
   │ SQL Server                     │
   │ INSERT INTO Organizations ...  │
   │ (Transaction)                  │
   └────────────────────────────────┘
              │
              ▼
7. RESPONSE
   ┌────────────────────────────────┐
   │ {                              │
   │   "id": "org-001",            │
   │   "name": "Acme Corp",        │
   │   "slug": "acme-corp",        │
   │   ...                          │
   │   "createdAt": "2024-03-15"   │
   │ }                              │
   │                                │
   │ Status: 201 Created            │
   └────────────────────────────────┘
```

---

## Query Example: Get Dashboard

```
1. CLIENT REQUEST
   ┌────────────────────────────────┐
   │ GET /api/dashboard             │
   └────────────────────────────────┘
              │
              ▼
2. CONTROLLER
   ┌────────────────────────────────┐
   │ DashboardController            │
   │ .GetDashboard()                │
   └────────────────────────────────┘
              │
              ▼
3. SEND QUERY
   ┌────────────────────────────────┐
   │ GetDashboardQuery              │
   │ (Lightweight)                  │
   │ ↓ via Wolverine                │
   └────────────────────────────────┘
              │
              ▼
4. QUERY HANDLER
   ┌────────────────────────────────┐
   │ GetDashboardQueryHandler       │
   │                                │
   │ Get organizations              │
   │ Apply projection:              │
   │ - Count organizations          │
   │ - Sum users per org            │
   │ - Sum projects                 │
   │ - Count active projects        │
   │ - Sum time entries             │
   │ - Sum invoiced amounts         │
   │ ↓                              │
   │ Return DashboardDto            │
   │ (No modification)              │
   └────────────────────────────────┘
              │
              ▼
5. DATABASE QUERY
   ┌────────────────────────────────┐
   │ SELECT                         │
   │   COUNT(*) as TotalOrgs,       │
   │   SUM(...) as TotalUsers, ... │
   │ FROM Organizations...         │
   │ (Server-side aggregation)     │
   └────────────────────────────────┘
              │
              ▼
6. RESPONSE
   ┌────────────────────────────────┐
   │ {                              │
   │   "totalOrganizations": 4,     │
   │   "totalUsers": 8,             │
   │   "totalProjects": 6,          │
   │   "totalActiveProjects": 4,    │
   │   "totalTimeEntries": 12,      │
   │   "totalInvoiced": 137000      │
   │ }                              │
   │                                │
   │ Status: 200 OK                 │
   └────────────────────────────────┘
```

---

## Dependency Injection Container

```
Startup (Program.cs)
      │
      ├─→ AddScoped<IOrganizationRepository, OrganizationRepository>
      ├─→ AddScoped<IProjectRepository, ProjectRepository>
      ├─→ AddScoped<IUserRepository, UserRepository>
      ├─→ AddScoped<IInvoiceRepository, InvoiceRepository>
      ├─→ AddScoped<ITimeEntryRepository, TimeEntryRepository>
      ├─→ AddScoped<DbContext>
      └─→ AddMediatR(handlers from application assembly)
                    │
                    ├─→ RegisterHandler<GetDashboardQuery, GetDashboardQueryHandler>
                    ├─→ RegisterHandler<CreateOrganizationCommand, CreateOrganizationCommandHandler>
                    ├─→ RegisterHandler<...>
                    │
                    └─→ All handlers automatically resolved at runtime
                        based on constructor parameters
```

---

## Layered Architecture Dependencies

```
Presentation (API Controllers)
       ↓ depends on
Application (CQRS Handlers)
       ↓ depends on
Domain (Business Logic, Interfaces)
       ↓ depends on
Infrastructure (Implementations, Database)

       ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

Key Rule:
Inner layers (Domain) should NOT depend on outer layers (Infrastructure)

This is achieved through:
• Repository Interfaces in Domain
• Repository Implementations in Infrastructure
• Dependency Injection at runtime
```

---

## Testing Strategy Pyramid

```
                    ╭───╮
                   ╱  E2E  ╲
                 ╱  Tests   ╲
               ╱    (Few)     ╲     🐢 Slow
             ╱─────────────────╲    - Full API tests
           ╱                     ╲   - Real database
         ╱  Integration Tests    ╲  - Realistic flows
       ╱      (Medium)            ╲
     ╱────────────────────────────╲
   ╱                                ╲
 ╱      Unit Tests (Many)            ╲  🚀 Fast
╱────────────────────────────────────╲ - Isolated tests
                                      ╲ - Mocked deps
                                      ╲ - Sub-1s execution
```

---

## Clean Architecture Principles

```
┌──────────────────────────────────────────────────────┐
│         Independent of Frameworks                    │
│         Independent of UI                           │
│         Independent of Database                     │
│         Independent of any External Agency          │
│                                                      │
│  → Business Logic Pure & Reusable                    │
│  → Easy to Test                                      │
│  → Easy to Maintain                                  │
│  → Easy to Scale                                     │
└──────────────────────────────────────────────────────┘

         Outer layers depend on inner layers
         Never the opposite
         Dependencies always point inward
```

---

**Architecture Version**: 1.0  
**Last Updated**: March 2025  
**Status**: Clean Architecture with CQRS & DDD
