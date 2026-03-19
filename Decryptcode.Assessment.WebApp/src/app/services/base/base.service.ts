import { HttpClient } from '@angular/common/http';
import { map, Observable } from 'rxjs';

export interface ApiResponse<T> {
  content: T;
  error: string | null;
  statusCode: number;
}

export abstract class BaseService {
  protected readonly BASE = 'https://localhost:7193/api';

  constructor(protected http: HttpClient) {}

  protected handleResponse<T>(response: ApiResponse<T>): T {
    if (response.error) {
      throw new Error(response.error);
    }
    if (!response.content) {
      throw new Error('No data in response');
    }
    return response.content;
  }

  protected get<T>(endpoint: string, params?: Record<string, any>): Observable<T> {
    return this.http.get<ApiResponse<T>>(`${this.BASE}${endpoint}`, { params }).pipe(
      map(response => this.handleResponse(response))
    );
  }

  protected post<T>(endpoint: string, body: any, params?: Record<string, any>): Observable<T> {
    return this.http.post<ApiResponse<T>>(`${this.BASE}${endpoint}`, body, { params }).pipe(
      map(response => this.handleResponse(response))
    );
  }

  protected put<T>(endpoint: string, body: any, params?: Record<string, any>): Observable<T> {
    return this.http.put<ApiResponse<T>>(`${this.BASE}${endpoint}`, body, { params }).pipe(
      map(response => this.handleResponse(response))
    );
  }

  protected delete<T>(endpoint: string, params?: Record<string, any>): Observable<T> {
    return this.http.delete<ApiResponse<T>>(`${this.BASE}${endpoint}`, { params }).pipe(
      map(response => this.handleResponse(response))
    );
  }
}
