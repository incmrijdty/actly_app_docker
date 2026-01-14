import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { ParticipationDto } from '../models/participation';
import { Observable, of } from 'rxjs';
import { map, catchError } from 'rxjs/operators';

@Injectable({
  providedIn: 'root'
})
export class ParticipationService {
  private apiUrl = 'http://localhost:5126/api/Participation';

  constructor(private http: HttpClient) {}

  checkParticipation(userId: number, eventId: number): Observable<boolean> {
    return this.http.get<any>(`${this.apiUrl}/user/${userId}/event/${eventId}`).pipe(
      map(response => !!response),
      catchError(() => of(false)) 
    );
  }

  joinEvent(participation: ParticipationDto): Observable<any> {
    return this.http.post(this.apiUrl, participation);
  }

  leaveEvent(userId: number, eventId: number): Observable<any> {
    return this.http.delete(`${this.apiUrl}/${userId}/${eventId}`);
  }
}
