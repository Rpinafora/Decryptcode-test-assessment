import { Component, inject } from '@angular/core';
import { RouterModule, ActivatedRoute } from '@angular/router';
import { toSignal } from '@angular/core/rxjs-interop';
import { catchError, map, of, switchMap } from 'rxjs';
import { ProjectService } from '../../services/project';
import { ProjectDetail } from '../../models';

@Component({
  selector: 'app-project-detail',
  standalone: true,
  imports: [RouterModule],
  templateUrl: './project-detail.component.html',
  styleUrl: './project-detail.component.scss'
})
export class ProjectDetailComponent {
  private projectService = inject(ProjectService);
  private route = inject(ActivatedRoute);

  state = toSignal(
    this.route.params.pipe(
      switchMap(params => {
        const id = params['id'];
        return this.projectService.getProject(id).pipe(
          map(data => ({ data, error: null, loading: false })),
          catchError(err => of({ data: null, error: err?.message || 'Failed to load project', loading: false }))
        );
      })
    ),
    { initialValue: { data: null, error: null, loading: true } }
  );
}
