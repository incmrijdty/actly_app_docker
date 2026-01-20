import { Component, Input, OnInit, ChangeDetectorRef } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { CommonModule } from '@angular/common';
import { environment } from '../environments/environment';
@Component({
  selector: 'app-event-attendance',
  imports: [CommonModule],
  templateUrl: './event-attendance.html',
  styleUrl: './event-attendance.css'
})
export class EventAttendance implements OnInit {
  @Input() eventId!: number;

  event: any;
  participants: any[] = [];

  constructor(private http: HttpClient, private cdr: ChangeDetectorRef) {}

  ngOnInit(): void {
    console.log('EventAttendance initialized with eventId', this.eventId);
    this.fetchParticipants();
    console.log ("fetched")
  }

  fetchParticipants() {
    this.http.get<any[]>(`${environment.apiUrl}/api/Events/${this.eventId}/participants`)
      .subscribe(data => {
        console.log('Fetched participants:', data);
        this.participants = data;
        console.log('Fetched participants2:', this.participants);
        console.log ('Length:', this.participants.length);
        this.cdr.detectChanges();
      });
  }

  toggleAttendance(p: any) {
    const newStatus = !p.attended;
    this.http.put(`${environment.apiUrl}/api/Participation/${p.id}/attendance`, newStatus).subscribe(() => {
      p.attended = newStatus;
      this.cdr.detectChanges();
    });
  }
}
