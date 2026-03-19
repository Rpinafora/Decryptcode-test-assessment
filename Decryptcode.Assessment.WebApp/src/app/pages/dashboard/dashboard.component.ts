import { Component, inject } from '@angular/core';
import { toSignal } from '@angular/core/rxjs-interop';
import { catchError, map, of } from 'rxjs';
import { DashboardService } from '../../services/dashboard';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [],
  templateUrl: './dashboard.component.html',
  styleUrl: './dashboard.component.scss'
})
export class DashboardComponent {
  private dashboardService = inject(DashboardService);
  
  state = toSignal(
    this.dashboardService.getDashboard().pipe(
      map(data => ({ data, error: null, loading: false })),
      catchError(err => of({ 
        data: null, 
        error: err?.message || 'Failed to load dashboard', 
        loading: false 
      }))
    ),
    { initialValue: { data: null, error: null, loading: true } }
  );
}
