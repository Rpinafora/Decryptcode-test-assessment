import { Component, inject, computed } from '@angular/core';
import { RouterModule } from '@angular/router';
import { toSignal } from '@angular/core/rxjs-interop';
import { catchError, map, of } from 'rxjs';
import { ProjectService } from '../../services/project';
import { Project } from '../../models';

@Component({
  selector: 'app-projects',
  standalone: true,
  imports: [RouterModule],
  templateUrl: './projects.component.html',
  styleUrl: './projects.component.scss'
})
export class ProjectsComponent {
  private projectService = inject(ProjectService);

  state = toSignal(
    this.projectService.getProjects().pipe(
      map(data => ({ data, error: null, loading: false })),
      catchError(err => of({ data: null, error: err?.message || 'Failed to load projects', loading: false }))
    ),
    { initialValue: { data: null, error: null, loading: true } }
  );

  getStatusBadgeClass = computed(() => {
    return (status: string) => {
      if (status === 'active') return 'badge--active';
      if (status === 'completed') return 'badge--completed';
      return 'badge--draft';
    };
  });
}
