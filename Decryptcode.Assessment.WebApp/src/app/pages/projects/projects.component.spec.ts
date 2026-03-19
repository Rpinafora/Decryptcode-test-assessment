import { ComponentFixture, TestBed } from '@angular/core/testing';
import { ProjectsComponent } from './projects.component';
import { ProjectService } from '../../services/project';
import { RouterModule } from '@angular/router';
import { of, throwError } from 'rxjs';
import { Project } from '../../models';
import { describe, it, expect, beforeEach, vi } from 'vitest';

describe('ProjectsComponent', () => {
  let component: ProjectsComponent;
  let fixture: ComponentFixture<ProjectsComponent>;
  let mockProjectService: any;

  beforeEach(async () => {
    mockProjectService = {
      getProjects: vi.fn()
    };

    await TestBed.configureTestingModule({
      imports: [ProjectsComponent, RouterModule.forRoot([])],
      providers: [
        { provide: ProjectService, useValue: mockProjectService }
      ]
    }).compileComponents();
  });

  it('should create', () => {
    mockProjectService.getProjects.mockReturnValue(of([]));
    
    fixture = TestBed.createComponent(ProjectsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
    
    expect(component).toBeTruthy();
  });

  it('should load projects successfully', () => {
    const mockProjects: Project[] = [
      {
        id: '1',
        name: 'Website Redesign',
        status: 'active',
        budgetHours: 200,
        startDate: '2024-01-01',
        endDate: '2024-06-30'
      }
    ];

    mockProjectService.getProjects.mockReturnValue(of(mockProjects));
    
    fixture = TestBed.createComponent(ProjectsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();

    expect(component.state().data).toEqual(mockProjects);
    expect(component.state().loading).toBe(false);
    expect(component.state().error).toBeNull();
  });

  it('should handle error when loading projects', () => {
    mockProjectService.getProjects.mockReturnValue(
      throwError(() => new Error('Server error'))
    );
    
    fixture = TestBed.createComponent(ProjectsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();

    expect(component.state().data).toBeNull();
    expect(component.state().error).toBeTruthy();
  });

  it('should return correct status badge class', () => {
    mockProjectService.getProjects.mockReturnValue(of([]));
    
    fixture = TestBed.createComponent(ProjectsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();

    const getStatusBadgeClass = component.getStatusBadgeClass();
    
    expect(getStatusBadgeClass('active')).toBe('badge--active');
    expect(getStatusBadgeClass('completed')).toBe('badge--completed');
    expect(getStatusBadgeClass('draft')).toBe('badge--draft');
  });

  it('should display projects title in template', () => {
    mockProjectService.getProjects.mockReturnValue(of([]));
    
    fixture = TestBed.createComponent(ProjectsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
    fixture.detectChanges();

    const compiled = fixture.nativeElement;
    expect(compiled.querySelector('h1')?.textContent).toContain('Projects');
  });
});
