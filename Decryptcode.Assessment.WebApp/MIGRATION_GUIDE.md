# React to Angular Migration - Decryptcode Assessment

This Angular application is a complete migration from the React version 

## Migration Overview

The React app has been completely converted to a modern Angular standalone component architecture with the following structure:

### Project Structure

```
src/app/
├── models/
│   └── index.ts              # TypeScript interfaces for all data types
├── services/
│   └── api.service.ts        # HttpClient-based API service
├── pages/
│   ├── dashboard/            # Dashboard page component
│   ├── organizations/        # Organizations list component
│   ├── organization-detail/  # Organization detail component
│   ├── projects/             # Projects list component
│   └── project-detail/       # Project detail component
├── app.ts                    # Root app component
├── app.routes.ts             # Route configuration
├── app.config.ts             # App configuration with providers
├── app.html                  # Root template with navigation
└── app.scss                  # Root styles
```

## Key Features

### 1. **API Service** (`src/app/services/api.service.ts`)
- Replaces the React `api.js` module
- Uses Angular's `HttpClient` for type-safe HTTP requests
- Provides methods for all API endpoints:
  - `getDashboard()`
  - `getOrganizations(params?)`
  - `getOrganization(id)`
  - `getOrganizationSummary(id)`
  - `getUsers(params?)`
  - `getUser(id)`
  - `getProjects(params?)`
  - `getProject(id)`
  - `getTimeEntries(params?)`
  - `getInvoices(params?)`

### 2. **Data Models** (`src/app/models/index.ts`)
- TypeScript interfaces for type safety:
  - `DashboardData`
  - `Organization` & `OrganizationSummary`
  - `Project` & `ProjectDetail`
  - `User`, `TimeEntry`, `Invoice`

### 3. **Routing** (`src/app/app.routes.ts`)
Routes configured match the React app:
- `/` - Dashboard
- `/organizations` - Organizations list
- `/organizations/:id` - Organization detail
- `/projects` - Projects list
- `/projects/:id` - Project detail

### 4. **Components**

#### DashboardComponent
- Displays dashboard statistics
- Shows total organizations, users, projects, time entries, and invoiced amount
- Loads data from `/api/dashboard` endpoint

#### OrganizationsComponent
- Lists all organizations in a 2-column grid
- Displays organization name, industry, tier badge, and contact email
- Links to organization detail page

#### OrganizationDetailComponent
- Shows detailed organization information
- Displays project count, user count, and total invoiced amount
- Loads data from `/api/organizations/:id/summary` endpoint

#### ProjectsComponent
- Lists all projects in a 2-column grid
- Shows project name, status badge, budget hours, and date range
- Links to project detail page

#### ProjectDetailComponent
- Shows detailed project information
- Displays status, budget, hours logged, organization, and dates
- Loads data from `/api/projects/:id` endpoint

### 5. **Styling**
- CSS styles migrated from React's `index.css`
- Global styles in `src/styles.scss`
- Component-scoped styles in each component's `.scss` file
- Responsive grid layout with media queries
- Semantic badge coloring for status/tier indicators

## Key Differences from React

| React | Angular |
|-------|---------|
| `useState` for state | RxJS Observables + OnInit |
| `useEffect` hooks | `subscribe()` in lifecycle hooks |
| `useParams` from React Router | `ActivatedRoute` injection |
| `Link` component | `routerLink` directive |
| Inline CSS classes | SCSS modules & class binding |
| Error handling in try-catch | Observable error handlers |

## Usage

### Development
```bash
npm start
```

### Building
```bash
npm run build
```

### Running Tests
```bash
npm test
```

## API Configuration

The app expects the API to be available at `/api`. Configure the base URL in `src/app/services/api.service.ts` if needed:

```typescript
private readonly BASE = '/api';
```

## Error Handling

Each component includes error state management:
- Loading state shows "Loading…" message
- Error state displays error message
- Success state renders component data

## Dependencies

- **@angular/core** - Core Angular framework
- **@angular/common** - Common Angular directives
- **@angular/router** - Routing module
- **@angular/common/http** - HTTP client module

## Environment Setup

The app uses Angular's standalone component architecture and is configured with:
- `provideBrowserGlobalErrorListeners()` - Error handling
- `provideRouter(routes)` - Routing configuration
- `provideHttpClient()` - HTTP client provider

## Future Enhancements

Potential improvements for the future:
- Add loading indicators with spinners
- Implement pagination for lists
- Add search/filter functionality
- Implement state management (NgRx)
- Add unit tests with Jasmine/Karma
- Add e2e tests with Cypress/Playwright
- Implement caching strategies in API service
- Add request interceptors for authentication
- Implement error interceptors for global error handling
