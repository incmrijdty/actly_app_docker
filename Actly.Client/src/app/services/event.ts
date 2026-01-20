import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Event } from '../models/event'; 
import { environment } from '../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class EventService {
  private apiUrl = `${environment.apiUrl}/api/Users`;
  private apiUrlOrganizer = `${environment.apiUrl}/api/Events`; 

  constructor(private http: HttpClient) {}

  getUserEvents(userId: number): Observable<Event[]> {
    return this.http.get<Event[]>(`${this.apiUrl}/${userId}/events`);
  }

  getEventsByOrganizerId(organizerId: number): Observable<Event[]> {
    return this.http.get<Event[]>(`${this.apiUrlOrganizer}/organizer/${organizerId}`);
  }

  createEvent(event: Event): Observable<Event> {
    return this.http.post<Event>(this.apiUrlOrganizer, event);
  }

  updateEvent(id: number, event: Event): Observable<void> {
    return this.http.put<void>(`${this.apiUrlOrganizer}/${id}`, event);
  }

  deleteEvent(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrlOrganizer}/${id}`);
  }
}
