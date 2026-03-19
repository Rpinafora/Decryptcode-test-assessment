import { ComponentFixture, TestBed } from '@angular/core/testing';
import { DashboardComponent } from './dashboard.component';
import { DashboardService } from '../../services/dashboard';
import { of, throwError } from 'rxjs';
import { DashboardData } from '../../models';
import { describe, it, expect, beforeEach, vi } from 'vitest';

describe('DashboardComponent', () => {
  let component: DashboardComponent;
  let fixture: ComponentFixture<DashboardComponent>;
  let mockDashboardService: any;

  beforeEach(async () => {
    mockDashboardService = {
      getDashboard: vi.fn()
    };

    await TestBed.configureTestingModule({
      imports: [DashboardComponent],
      providers: [
        { provide: DashboardService, useValue: mockDashboardService }
      ]
    }).compileComponents();
  });

  it('should create', () => {
    const mockData: DashboardData = {
      totalOrganizations: 5,
      totalUsers: 20,
      totalProjects: 10,
      activeProjects: 3,
      totalTimeEntries: 100,
      totalInvoiced: 50000
    };

    mockDashboardService.getDashboard.mockReturnValue(of(mockData));
    
    fixture = TestBed.createComponent(DashboardComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
    
    expect(component).toBeTruthy();
  });

  it('should load dashboard data successfully', () => {
    const mockData: DashboardData = {
      totalOrganizations: 5,
      totalUsers: 20,
      totalProjects: 10,
      activeProjects: 3,
      totalTimeEntries: 100,
      totalInvoiced: 50000
    };

    mockDashboardService.getDashboard.mockReturnValue(of(mockData));
    
    fixture = TestBed.createComponent(DashboardComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();

    expect(component.state().data).toEqual(mockData);
    expect(component.state().loading).toBe(false);
    expect(component.state().error).toBeNull();
  });

  it('should handle error when loading dashboard', () => {
    mockDashboardService.getDashboard.mockReturnValue(
      throwError(() => new Error('Network error'))
    );
    
    fixture = TestBed.createComponent(DashboardComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();

    expect(component.state().data).toBeNull();
    expect(component.state().error).toBeTruthy();
  });

  it('should render dashboard title in template', () => {
    const mockData: DashboardData = {
      totalOrganizations: 5,
      totalUsers: 20,
      totalProjects: 10,
      activeProjects: 3,
      totalTimeEntries: 100,
      totalInvoiced: 50000
    };

    mockDashboardService.getDashboard.mockReturnValue(of(mockData));
    
    fixture = TestBed.createComponent(DashboardComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
    fixture.detectChanges();

    const compiled = fixture.nativeElement;
    expect(compiled.querySelector('h1')?.textContent).toContain('Dashboard');
  });

  it('should display data when loaded', () => {
    const mockData: DashboardData = {
      totalOrganizations: 5,
      totalUsers: 20,
      totalProjects: 10,
      activeProjects: 3,
      totalTimeEntries: 100,
      totalInvoiced: 50000
    };

    mockDashboardService.getDashboard.mockReturnValue(of(mockData));
    
    fixture = TestBed.createComponent(DashboardComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
    fixture.detectChanges();

    expect(component.state().data?.totalOrganizations).toBe(5);
  });
});
