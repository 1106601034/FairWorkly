# CLAUDE.md - AI Navigation Index

> This file guides Claude Code to the right documents in this repository.

## Session Start (Read in Order)

| Step | Document | Purpose |
|------|----------|---------|
| 1 | `.raw_materials/AI_README_FIRST.md` | Understand boundaries (READ-ONLY) |
| 2 | `.doc/AI_GUIDE.md` | Project status & current tasks |
| 3 | `.doc/issues/ISSUE_*.md` | Active task details (if any) |

---

## Document Directory

### Working Documents (AI can READ/WRITE)

| Path | Purpose |
|------|---------|
| `.doc/AI_GUIDE.md` | Project navigation & status |
| `.doc/CODING_RULES.md` | Coding standards |
| `.doc/SPEC_*.md` | Technical specifications |
| `.doc/DEVLOG.md` | Development decisions log |
| `.doc/issues/` | Task tracking |
| `**/AI_GUIDE.md` | Code directory navigation |

### Constitutional Documents (READ-ONLY)

| Path | Purpose |
|------|---------|
| `.raw_materials/BUSINESS_RULES/` | Rate tables, API contracts (absolute red line) |
| `.raw_materials/TECH_CONSTRAINTS/` | Architecture, coding standards |
| `.raw_materials/REFERENCE/` | Reference materials |
| `**/README.md` | Human documentation |

**If you find issues in constitutional documents**: DO NOT modify. Report to human and wait for confirmation.

---

## Common Commands

```bash
# Run tests
dotnet test
dotnet test --filter "FullyQualifiedName~TestClassName"

# Run API
dotnet run --project src/FairWorkly.API

# Database operations
dotnet ef database drop --force --project src/FairWorkly.Infrastructure --startup-project src/FairWorkly.API
dotnet ef database update --project src/FairWorkly.Infrastructure --startup-project src/FairWorkly.API
dotnet ef migrations add MigrationName --project src/FairWorkly.Infrastructure --startup-project src/FairWorkly.API
```

---

## Red Line Files (DO NOT MODIFY)

| Path | Reason |
|------|--------|
| `FairWorkly.Domain/*/Entities/*.cs` | Entities finalized |
| `FairWorkly.Infrastructure/Persistence/FairWorklyDbContext.cs` | Audit logic configured |
| `.raw_materials/*` | Constitutional documents |
| `**/README.md` | Human documentation |

---

## Code Standards (Quick Reference)

| Type | Use | Avoid |
|------|-----|-------|
| Money | `decimal` | `float`, `double` |
| Timestamps | `DateTimeOffset` | `DateTime` |
| Date only | `DateOnly` | - |
| Current time | `IDateTimeProvider` | `DateTime.Now` |
| DI registration | Layer's `DependencyInjection.cs` | `Program.cs` |
| Comments/naming | English only | - |

**Tolerance values**: $0.01 for rates, $0.05 for amounts

---

## Architecture (Quick Reference)

```
src/
├── FairWorkly.API/           # Controllers (thin, forwards to MediatR)
├── FairWorkly.Application/   # Use cases, services, handlers, DTOs
├── FairWorkly.Domain/        # Entities, enums (DO NOT MODIFY)
└── FairWorkly.Infrastructure/ # DbContext, repositories
```

**Pattern**: CQRS with MediatR (Command/Query → Validator → Handler)

---

*For detailed information, see `.doc/AI_GUIDE.md`*
