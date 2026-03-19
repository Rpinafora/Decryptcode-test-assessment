import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { BaseService } from '../base/base.service';
import { TimeEntry } from '../../models';

@Injectable({
  providedIn: 'root'
})
export class TimeEntryService extends BaseService {
  constructor(http: HttpClient) {
    super(http);
  }

  getTimeEntries(params?: Record<string, any>): Observable<TimeEntry[]> {
    return this.get<TimeEntry[]>('/time-entries', params);
  }
}
