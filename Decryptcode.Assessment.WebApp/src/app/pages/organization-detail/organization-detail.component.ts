import { Component, inject, computed } from '@angular/core';
import { RouterModule, ActivatedRoute } from '@angular/router';
import { toSignal } from '@angular/core/rxjs-interop';
import { catchError, map, of, switchMap } from 'rxjs';
import { OrganizationService } from '../../services/organization';
import { OrganizationSummary } from '../../models';

@Component({
  selector: 'app-organization-detail',
  standalone: true,
  imports: [RouterModule],
  templateUrl: './organization-detail.component.html',
  styleUrl: './organization-detail.component.scss'
})
export class OrganizationDetailComponent {
  private organizationService = inject(OrganizationService);
  private route = inject(ActivatedRoute);

  private params = toSignal(this.route.params);

  state = toSignal(
    this.route.params.pipe(
      switchMap(params => {
        const id = params['id'];
        return this.organizationService.getOrganizationSummary(id).pipe(
          map(data => ({ data, error: null, loading: false })),
          catchError(err => of({ data: null, error: err?.message || 'Failed to load organization', loading: false }))
        );
      })
    ),
    { initialValue: { data: null, error: null, loading: true } }
  );
}
