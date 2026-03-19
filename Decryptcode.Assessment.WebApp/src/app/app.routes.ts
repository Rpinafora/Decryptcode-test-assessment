import { Routes } from '@angular/router';
import { DashboardComponent } from './pages/dashboard/dashboard.component';
import { OrganizationsComponent } from './pages/organizations/organizations.component';
import { OrganizationDetailComponent } from './pages/organization-detail/organization-detail.component';
import { ProjectsComponent } from './pages/projects/projects.component';
import { ProjectDetailComponent } from './pages/project-detail/project-detail.component';

export const routes: Routes = [
  { path: '', component: DashboardComponent },
  { path: 'organizations', component: OrganizationsComponent },
  { path: 'organizations/:id', component: OrganizationDetailComponent },
  { path: 'projects', component: ProjectsComponent },
  { path: 'projects/:id', component: ProjectDetailComponent },
  { path: '**', redirectTo: '' }
];
