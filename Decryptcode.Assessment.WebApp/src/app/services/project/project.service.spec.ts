import { TestBed } from '@angular/core/testing';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { provideHttpClient } from '@angular/common/http';
import { ProjectService } from './project.service';
import { Project, ProjectDetail } from '../../models';
import { describe, it, expect, beforeEach, afterEach } from 'vitest';

describe('ProjectService', () => {
  let service: ProjectService;
  let httpMock: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [ProjectService, provideHttpClient(), provideHttpClientTesting()]
    });
    service = TestBed.inject(ProjectService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpMock.verify();
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('should fetch all projects', () => {
    const mockProjects: Project[] = [
      {
        id: '1',
        name: 'Project 1',
        status: 'active',
        budgetHours: 100,
        startDate: '2024-01-01',
        endDate: '2024-12-31'
      }
    ];

    const mockResponse = { content: mockProjects, error: null, statusCode: 200 };

    service.getProjects().subscribe(result => {
      expect(result).toEqual(mockProjects);
      expect(result[0].status).toBe('active');
    });

    const req = httpMock.expectOne('https://localhost:7193/api/projects');
    expect(req.request.method).toBe('GET');
    req.flush(mockResponse);
  });

  it('should fetch single project detail', () => {
    const mockProject: ProjectDetail = {
      id: '1',
      name: 'Project 1',
      status: 'active',
      budgetHours: 100,
      totalHoursLogged: 50,
      startDate: '2024-01-01',
      endDate: '2024-12-31',
      organization: { name: 'Org 1' }
    };

    const mockResponse = { content: mockProject, error: null, statusCode: 200 };

    service.getProject('1').subscribe(result => {
      expect(result).toEqual(mockProject);
      expect(result.totalHoursLogged).toBe(50);
    });

    const req = httpMock.expectOne('https://localhost:7193/api/projects/1');
    expect(req.request.method).toBe('GET');
    req.flush(mockResponse);
  });
});
