import { Component, inject } from '@angular/core';
import { RouterModule } from '@angular/router';
import { toSignal } from '@angular/core/rxjs-interop';
import { catchError, map, of } from 'rxjs';
import { OrganizationService } from '../../services/organization';
import { Organization } from '../../models';

@Component({
  selector: 'app-organizations',
  standalone: true,
  imports: [RouterModule],
  templateUrl: './organizations.component.html',
  styleUrl: './organizations.component.scss'
})
export class OrganizationsComponent {
  private organizationService = inject(OrganizationService);

  state = toSignal(
    this.organizationService.getOrganizations().pipe(
      map(data => ({ data, error: null, loading: false })),
      catchError(err => of({ data: null, error: err?.message || 'Failed to load organizations', loading: false }))
    ),
    { initialValue: { data: null, error: null, loading: true } }
  );
}
