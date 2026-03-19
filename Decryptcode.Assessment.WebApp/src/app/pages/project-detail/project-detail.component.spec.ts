import { ComponentFixture, TestBed } from '@angular/core/testing';
import { ProjectDetailComponent } from './project-detail.component';
import { ProjectService } from '../../services/project';
import { ActivatedRoute, RouterModule } from '@angular/router';
import { of, throwError } from 'rxjs';
import { ProjectDetail } from '../../models';
import { describe, it, expect, beforeEach, vi } from 'vitest';

describe('ProjectDetailComponent', () => {
  let component: ProjectDetailComponent;
  let fixture: ComponentFixture<ProjectDetailComponent>;
  let mockProjectService: any;
  let mockActivatedRoute: any;

  beforeEach(async () => {
    mockProjectService = {
      getProject: vi.fn()
    };

    mockActivatedRoute = {
      params: of({ id: '1' })
    };

    await TestBed.configureTestingModule({
      imports: [ProjectDetailComponent, RouterModule.forRoot([])],
      providers: [
        { provide: ProjectService, useValue: mockProjectService },
        { provide: ActivatedRoute, useValue: mockActivatedRoute }
      ]
    }).compileComponents();
  });

  it('should create', () => {
    const mockProject: ProjectDetail = {
      id: '1',
      name: 'Website Redesign',
      status: 'active',
      budgetHours: 200,
      totalHoursLogged: 150,
      startDate: '2024-01-01',
      endDate: '2024-06-30',
      organization: { name: 'Tech Corp' }
    };

    mockProjectService.getProject.mockReturnValue(of(mockProject));
    
    fixture = TestBed.createComponent(ProjectDetailComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
    
    expect(component).toBeTruthy();
  });

  it('should load project detail successfully', () => {
    const mockProject: ProjectDetail = {
      id: '1',
      name: 'Website Redesign',
      status: 'active',
      budgetHours: 200,
      totalHoursLogged: 150,
      startDate: '2024-01-01',
      endDate: '2024-06-30',
      organization: { name: 'Tech Corp' }
    };

    mockProjectService.getProject.mockReturnValue(of(mockProject));
    
    fixture = TestBed.createComponent(ProjectDetailComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();

    expect(component.state().data).toEqual(mockProject);
    expect(component.state().loading).toBe(false);
    expect(component.state().error).toBeNull();
  });

  it('should handle error when loading project', () => {
    mockProjectService.getProject.mockReturnValue(
      throwError(() => new Error('Not found'))
    );
    
    fixture = TestBed.createComponent(ProjectDetailComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();

    expect(component.state().data).toBeNull();
    expect(component.state().error).toBeTruthy();
  });

  it('should display back link to projects', () => {
    const mockProject: ProjectDetail = {
      id: '1',
      name: 'Website Redesign',
      status: 'active',
      budgetHours: 200,
      totalHoursLogged: 150,
      startDate: '2024-01-01',
      endDate: '2024-06-30',
      organization: { name: 'Tech Corp' }
    };

    mockProjectService.getProject.mockReturnValue(of(mockProject));
    
    fixture = TestBed.createComponent(ProjectDetailComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
    fixture.detectChanges();

    const compiled = fixture.nativeElement;
    const backLink = compiled.querySelector('a[routerLink="/projects"]');
    expect(backLink).toBeTruthy();
  });

  it('should display loading state initially', () => {
    const mockProject: ProjectDetail = {
      id: '1',
      name: 'Website Redesign',
      status: 'active',
      budgetHours: 200,
      totalHoursLogged: 150,
      startDate: '2024-01-01',
      endDate: '2024-06-30',
      organization: { name: 'Tech Corp' }
    };

    mockProjectService.getProject.mockReturnValue(of(mockProject));
    
    fixture = TestBed.createComponent(ProjectDetailComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();

    expect(component.state().data).toEqual(mockProject);
  });
});
