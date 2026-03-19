import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { BaseService } from '../base/base.service';
import { User } from '../../models';

@Injectable({
  providedIn: 'root'
})
export class UserService extends BaseService {
  constructor(http: HttpClient) {
    super(http);
  }

  getUsers(params?: Record<string, any>): Observable<User[]> {
    return this.get<User[]>('/users', params);
  }

  getUser(id: string): Observable<User> {
    return this.get<User>(`/users/${id}`);
  }
}
