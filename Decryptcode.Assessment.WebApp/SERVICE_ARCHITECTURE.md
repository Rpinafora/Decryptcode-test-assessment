# Service Architecture - Modular Design

## Overview
Services have been reorganized into a modular architecture with a base service that handles common API operations and response handling.

## Directory Structure

```
src/app/services/
├── base/
│   ├── base.service.ts          # Abstract base service with HTTP methods
│   └── index.ts                 # Export
├── dashboard/
│   ├── dashboard.service.ts     # Dashboard service
│   └── index.ts
├── organization/
│   ├── organization.service.ts  # Organization service
│   └── index.ts
├── project/
│   ├── project.service.ts       # Project service
│   └── index.ts
├── user/
│   ├── user.service.ts          # User service
│   └── index.ts
├── time-entry/
│   ├── time-entry.service.ts    # Time Entry service
│   └── index.ts
└── invoice/
    ├── invoice.service.ts       # Invoice service
    └── index.ts
```

## BaseService

The `BaseService` is an abstract base class that provides:

### Features:
- **API Base URL**: Centralized configuration at `https://localhost:7193/api`
- **Response Handling**: `handleResponse()` method that:
  - Extracts data from the `content` field
  - Throws errors if `error` field is not null
  - Validates data exists
  
### HTTP Methods:
All methods are protected and use RxJS `map()` operator to unwrap responses:

```typescript
protected get<T>(endpoint: string, params?: Record<string, any>): Observable<T>
protected post<T>(endpoint: string, body: any, params?: Record<string, any>): Observable<T>
protected put<T>(endpoint: string, body: any, params?: Record<string, any>): Observable<T>
protected delete<T>(endpoint: string, params?: Record<string, any>): Observable<T>
```

### ApiResponse Interface:
```typescript
interface ApiResponse<T> {
  content: T;           // Actual data
  error: string | null; // Error message if any
  statusCode: number;   // HTTP status code
}
```

## Individual Services

### DashboardService
```typescript
extends BaseService
- getDashboard(): Observable<DashboardData>
```

### OrganizationService
```typescript
extends BaseService
- getOrganizations(params?): Observable<Organization[]>
- getOrganization(id): Observable<Organization>
- getOrganizationSummary(id): Observable<OrganizationSummary>
```

### ProjectService
```typescript
extends BaseService
- getProjects(params?): Observable<Project[]>
- getProject(id): Observable<ProjectDetail>
```

### UserService
```typescript
extends BaseService
- getUsers(params?): Observable<User[]>
- getUser(id): Observable<User>
```

### TimeEntryService
```typescript
extends BaseService
- getTimeEntries(params?): Observable<TimeEntry[]>
```

### InvoiceService
```typescript
extends BaseService
- getInvoices(params?): Observable<Invoice[]>
```

## Usage in Components

### Before (Monolithic):
```typescript
constructor(private apiService: ApiService) {}
this.apiService.getDashboard().subscribe(...)
```

### After (Modular):
```typescript
constructor(private dashboardService: DashboardService) {}
this.dashboardService.getDashboard().subscribe(...)
```

## Benefits

1. **Separation of Concerns**: Each service handles one domain
2. **Reusability**: Base service methods are shared across all services
3. **Maintainability**: Changes to HTTP handling in one place
4. **Testability**: Easier to mock individual services
5. **Scalability**: Simple to add new services following the pattern
6. **Type Safety**: Full TypeScript support with generic types

## Adding a New Service

Simply create a new folder under `services/` and extend `BaseService`:

```typescript
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { BaseService } from '../base/base.service';
import { MyModel } from '../../models';

@Injectable({
  providedIn: 'root'
})
export class MyService extends BaseService {
  constructor(http: HttpClient) {
    super(http);
  }

  getItems(): Observable<MyModel[]> {
    return this.get<MyModel[]>('/my-endpoint');
  }
}
```

## Error Handling

Errors are automatically handled by the `handleResponse()` method and propagated through the observable chain, where components handle them in the error callback:

```typescript
this.service.getItems().subscribe({
  next: (data) => { /* handle data */ },
  error: (err) => { /* handle error */ }
});
```
