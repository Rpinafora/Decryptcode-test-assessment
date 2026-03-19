import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { BaseService } from '../base/base.service';
import { Project, ProjectDetail } from '../../models';

@Injectable({
  providedIn: 'root'
})
export class ProjectService extends BaseService {
  constructor(http: HttpClient) {
    super(http);
  }

  getProjects(params?: Record<string, any>): Observable<Project[]> {
    return this.get<Project[]>('/projects', params);
  }

  getProject(id: string): Observable<ProjectDetail> {
    return this.get<ProjectDetail>(`/projects/${id}`);
  }
}
