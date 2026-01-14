import { Component, OnInit } from '@angular/core';
import { AuthService } from '../services/auth';
import { EventService } from '../services/event';
import { jwtDecode } from 'jwt-decode';
import { CommonModule } from '@angular/common';
import { EventCardComponent } from '../event-card-component/event-card-component';
import { ChangeDetectorRef } from '@angular/core';
import { Router } from '@angular/router';

interface JwtPayload {
  'http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier': string;
  'http://schemas.microsoft.com/ws/2008/06/identity/claims/role': string;
  'http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress': string;
  'http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name': string;
}

@Component({
  selector: 'app-user-profile',
  imports: [CommonModule, EventCardComponent],
  templateUrl: './user-profile.html',
  styleUrls: ['./user-profile.css']
})
export class UserProfileComponent implements OnInit {

  user: { id: number; username: string; email: string; role: string } | null = null;
  attendedEvents: any[] = [];
  upcomingEvents: any[] = [];

  constructor(private auth: AuthService, private eventService: EventService, private cdRef: ChangeDetectorRef, private router: Router) {}

  ngOnInit() {
    const token = this.auth.getToken();
    if (!token) return;

    const payload = jwtDecode<JwtPayload>(token);
    this.user = {
      id: Number(payload['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier']),
      username: payload['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name'],
      email: payload['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress'],
      role: payload['http://schemas.microsoft.com/ws/2008/06/identity/claims/role']
    };

    this.eventService.getUserEvents(this.user.id).subscribe(events => {
      console.log('Received events:', events);
      this.attendedEvents = events.filter(e => e.attended);
      this.upcomingEvents = events.filter(e => !e.attended);
      console.log('Attended:', events.filter(e => e.attended));
      console.log('Upcoming:', events.filter(e => !e.attended));
      this.cdRef.detectChanges();
    });
  }

  logout() {
    this.auth.logout();
    alert('You have been logged out successfully.');
    this.router.navigate(['/']);  
  }
}
