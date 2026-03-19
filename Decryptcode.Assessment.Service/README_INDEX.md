# 📖 Architecture Documentation Index

## Welcome to Decryptcode Assessment Service Architecture Documentation

This project uses **Domain-Driven Design (DDD)**, **Clean Architecture**, and **CQRS** patterns. All documentation has been organized to help you understand, navigate, and work with the codebase.

---

## 🎯 Quick Navigation

### For First-Time Readers
1. **START HERE** → [GETTING_STARTED.md](GETTING_STARTED.md)
   - Overview of the architecture
   - Key concepts explained
   - File organization guide

### For Understanding Concepts
2. **ARCHITECTURE.md** → [Main Architecture Guide](ARCHITECTURE.md)
   - Detailed entity relationships
   - Aggregate roots vs reference entities
   - Repository pattern
   - Design patterns used
   - Database support

### For Visual Learners
3. **ARCHITECTURE_VISUAL_GUIDE.md** → [Visual Diagrams](ARCHITECTURE_VISUAL_GUIDE.md)
   - Entity relationship diagrams
   - Data flow charts
   - State transition diagrams
   - Request/response flows

### For Code Examples
4. **ARCHITECTURE_CODE_EXAMPLES.md** → [Real Code Samples](ARCHITECTURE_CODE_EXAMPLES.md)
   - Domain entities (aggregate roots, reference entities)
   - Repository implementation
   - Query handlers
   - API endpoints (conventional and minimal)
   - Dependency injection setup

### For Quick Reference
5. **ARCHITECTURE_QUICK_REFERENCE.md** → [Cheat Sheet](ARCHITECTURE_QUICK_REFERENCE.md)
   - Entity classification table
   - Decision trees
   - Common patterns
   - File structure lookup
   - CLI commands

---

## 🏗️ The Core Concept (30 Seconds)

```
Organization (Aggregate Root)
    ├─ Users (Reference Entities - managed through Organization)
    ├─ Projects (Reference Entities - managed through Organization)
    ├─ Invoices (Reference Entities - managed through Organization)
    └─ TimeEntries (Reference Entities - managed through Organization)
```

**Key Idea**: Organization is the single entry point for all changes. Reference entities cannot exist independently.

---

## 📚 Documentation Structure

```
├─ GETTING_STARTED.md
│   └─ Entry point with overview and setup guide
│
├─ ARCHITECTURE.md
│   ├─ Aggregate Root Pattern (Organization)
│   ├─ Reference Entities (User, Project, Invoice, TimeEntry)
│   ├─ Value Objects (Settings, Metadata)
│   ├─ Entity Relationships
│   ├─ Repository Interfaces
│   ├─ Layer Architecture
│   ├─ Data Consistency & Transactions
│   └─ Design Patterns
│
├─ ARCHITECTURE_VISUAL_GUIDE.md
│   ├─ Entity Relationship Diagrams
│   ├─ Ownership Hierarchy
│   ├─ Data Flow Diagrams
│   ├─ State Transitions
│   ├─ Validation Flows
│   └─ API Endpoint to Database Flow
│
├─ ARCHITECTURE_CODE_EXAMPLES.md
│   ├─ Domain Model Examples
│   │   ├─ Aggregate Root: Organization
│   │   ├─ Reference Entity: User
│   │   └─ Value Objects: Settings & Metadata
│   ├─ Repository Pattern
│   │   ├─ Repository Interface
│   │   └─ Repository Implementation
│   ├─ CQRS Query Handler
│   ├─ API Endpoints
│   │   ├─ Minimal API
│   │   └─ Conventional Controller
│   └─ Dependency Injection Configuration
│
└─ ARCHITECTURE_QUICK_REFERENCE.md
    ├─ Cheat Sheet (Entity Classification)
    ├─ Decision Tree
    ├─ File Structure Lookup
    ├─ Common Patterns
    ├─ API Endpoint Patterns
    ├─ Error Handling
    ├─ Database Support
    ├─ Testing Examples
    ├─ Deployment Checklist
    └─ Common Commands
```

---

## 🎓 Learning Path

### Path 1: Visual Learner
1. GETTING_STARTED.md (overview)
2. ARCHITECTURE_VISUAL_GUIDE.md (diagrams)
3. ARCHITECTURE_CODE_EXAMPLES.md (real code)
4. ARCHITECTURE_QUICK_REFERENCE.md (reference)

### Path 2: Deep Learner
1. GETTING_STARTED.md (overview)
2. ARCHITECTURE.md (detailed concepts)
3. ARCHITECTURE_VISUAL_GUIDE.md (visualize)
4. ARCHITECTURE_CODE_EXAMPLES.md (implement)
5. ARCHITECTURE_QUICK_REFERENCE.md (reference)

### Path 3: Practical Developer
1. GETTING_STARTED.md (setup)
2. ARCHITECTURE_CODE_EXAMPLES.md (code)
3. ARCHITECTURE_QUICK_REFERENCE.md (patterns & commands)
4. ARCHITECTURE.md (concepts as needed)

---

## 🔑 Key Concepts

### Aggregate Root Pattern
- **Organization** is the only Aggregate Root
- Implements `IAggregateRoot` interface
- Single entry point for all changes
- Manages consistency of its aggregate

### Reference Entities
- **User, Project, Invoice, TimeEntry** are reference entities
- Do NOT implement `IAggregateRoot`
- Cannot exist without Organization
- Have repositories but are managed through Organization

### Repository Pattern
- Data access is abstracted through repository interfaces
- Each entity type has a repository
- Repositories implement IRepository<T>
- Infrastructure layer provides implementations

### CQRS Pattern
- Queries and commands separated
- Wolverine MessageBus routes requests
- Query handlers return DTOs
- Command handlers modify domain

### Value Objects
- **Settings** and **Metadata** are immutable records
- Defined by their values, not identity
- Cannot be changed after creation
- Create new instance to update

---

## 📊 Entity Quick Reference

| Entity | Type | Aggregate Root? | Repository? | Location |
|--------|------|-----------------|------------|----------|
| Organization | Root | ✅ YES | ✅ YES | AggregateRoots/ |
| User | Reference | ❌ NO | ✅ YES | ReferenceEntities/ |
| Project | Reference | ❌ NO | ✅ YES | ReferenceEntities/ |
| Invoice | Reference | ❌ NO | ✅ YES | ReferenceEntities/ |
| TimeEntry | Reference | ❌ NO | ✅ YES | ReferenceEntities/ |
| Settings | Value Object | N/A | N/A | ValueObjects/ |
| Metadata | Value Object | N/A | N/A | ValueObjects/ |

---

## 🗂️ Project Structure Overview

```
src/
├── Decryptcode.Assessment.Service.Domain/          (DDD Layer)
│   ├── Entities/
│   │   ├── AggregateRoots/Organization.cs
│   │   ├── ReferenceEntities/User.cs, Project.cs, etc.
│   │   └── ValueObjects/Settings.cs, Metadata.cs
│   └── Repositories/
│       └── [Repository Interfaces]
│
├── Decryptcode.Assessment.Service.Application/      (CQRS Layer)
│   ├── Organizations/
│   ├── Users/
│   ├── Projects/
│   ├── Invoices/
│   └── TimeEntries/
│
├── Decryptcode.Assessment.Service.Infrastructure.SqlServer/ (Data Layer)
│   ├── Contexts/ApiContext.cs
│   ├── Repositories/
│   │   └── [Repository Implementations]
│   ├── Migrations/
│   └── ServicesInjector.cs
│
├── Decryptcode.Assessment.Service.Api/             (API Layer - Conventional)
│   ├── Controllers/
│   └── Program.cs
│
└── DecryptCode.Assessment.Service.ApiMinimal/      (API Layer - Minimal)
    ├── Endpoints/
    └── Program.cs
```

---

## 💡 Common Questions

### Q: Why is Organization the only aggregate root?
**A:** Because all other entities depend on Organization for consistency. It maintains the transactional boundary and ensures invariants are enforced.

### Q: Can I query a User without going through Organization?
**A:** Yes! You can use the IUserRepository directly, but conceptually User belongs to Organization. The repository abstraction allows both patterns.

### Q: Why use Value Objects for Settings and Metadata?
**A:** They are immutable, reusable objects that don't change their identity. Creating new instances ensures thread-safety and clarity.

### Q: What's the difference between the two APIs?
**A:** Both use the same domain/application layers. The conventional API uses controllers, minimal API uses endpoint mapping. Pick based on preference.

### Q: Why SQL Server + SQLite?
**A:** Production uses SQL Server for reliability. SQLite is used locally when SQL Server isn't available, enabling offline development.

---

## 🚀 Getting Started

### 1. Read Documentation
- Start with GETTING_STARTED.md
- Review ARCHITECTURE.md for concepts
- Check ARCHITECTURE_CODE_EXAMPLES.md for patterns

### 2. Run the Application
```bash
dotnet run --project src/Decryptcode.Assessment.Service.Api
# or
dotnet run --project src/DecryptCode.Assessment.Service.ApiMinimal
```

### 3. Run Tests
```bash
dotnet test
# Expected: 41/41 tests passing ✅
```

### 4. Explore the Code
- Open Domain/Entities/AggregateRoots/Organization.cs
- Compare with Domain/Entities/ReferenceEntities/User.cs
- Notice the difference (IAggregateRoot vs regular class)

---

## 📖 How to Use This Documentation

### For Architecture Questions
→ **ARCHITECTURE.md** - Contains detailed explanations

### For Design Pattern Questions
→ **ARCHITECTURE_CODE_EXAMPLES.md** - Shows how patterns are implemented

### For Visual Understanding
→ **ARCHITECTURE_VISUAL_GUIDE.md** - Contains diagrams and flowcharts

### For Code Reference
→ **ARCHITECTURE_QUICK_REFERENCE.md** - Quick lookup tables and patterns

### For Getting Started
→ **GETTING_STARTED.md** - Overview and setup

---

## ✅ Quality Assurance

- ✅ All 41 integration tests passing
- ✅ Both API implementations working
- ✅ SQL Server and SQLite support
- ✅ Comprehensive error handling
- ✅ Full documentation coverage

---

## 🎯 Key Takeaways

| Concept | Key Point |
|---------|-----------|
| **Aggregate Root** | Organization is the boss; all changes go through it |
| **Reference Entities** | User, Project, Invoice, TimeEntry are employees of Organization |
| **Repository** | Data access abstraction; always query through repositories |
| **CQRS** | Separate read (queries) from write (commands) |
| **Value Objects** | Settings and Metadata are immutable |
| **Soft Delete** | Deleted records are filtered out automatically |
| **DTOs** | APIs return DTOs, not domain entities |
| **DDD** | Business logic belongs in the domain layer |

---

## 📞 Documentation Hierarchy

```
User Question/Need
    ↓
├─ "What's the overall structure?" 
│  → GETTING_STARTED.md
│
├─ "How do Aggregate Roots work?"
│  → ARCHITECTURE.md
│
├─ "Show me diagrams"
│  → ARCHITECTURE_VISUAL_GUIDE.md
│
├─ "How is User entity different from Organization?"
│  → ARCHITECTURE_CODE_EXAMPLES.md
│
├─ "Quick table lookup or cheat sheet"
│  → ARCHITECTURE_QUICK_REFERENCE.md
│
└─ "I'm confused" → START with GETTING_STARTED.md
```

---

## 📝 Documentation Versions

| Document | Version | Last Updated | Status |
|----------|---------|--------------|--------|
| GETTING_STARTED.md | 1.0 | 2024 | ✅ Complete |
| ARCHITECTURE.md | 1.0 | 2024 | ✅ Complete |
| ARCHITECTURE_VISUAL_GUIDE.md | 1.0 | 2024 | ✅ Complete |
| ARCHITECTURE_CODE_EXAMPLES.md | 1.0 | 2024 | ✅ Complete |
| ARCHITECTURE_QUICK_REFERENCE.md | 1.0 | 2024 | ✅ Complete |
| README_INDEX.md | 1.0 | 2024 | ✅ Complete |

---

## 🎉 You're All Set!

You now have comprehensive documentation covering:
- ✅ Architecture patterns and principles
- ✅ Entity relationships and hierarchy
- ✅ Code examples and implementation details
- ✅ Visual diagrams and flowcharts
- ✅ Quick reference guides
- ✅ Getting started guide

**Happy coding! 🚀**

---

**Framework**: .NET 10  
**Patterns**: DDD + Clean Architecture + CQRS  
**Tests**: 41/41 passing ✅  
**Documentation**: Complete & Comprehensive 📚
