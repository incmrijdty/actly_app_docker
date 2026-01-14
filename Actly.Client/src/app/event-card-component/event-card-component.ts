import { ChangeDetectorRef, Component, Input, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Event } from '../models/event';
import { ParticipationService } from '../services/participation';
import { AuthService } from '../services/auth';
import { jwtDecode } from 'jwt-decode';

interface JwtPayload {
  'http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier': string;
  'http://schemas.microsoft.com/ws/2008/06/identity/claims/role': string;
  'http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name': string;
}

@Component({
  selector: 'app-event-card',
  imports: [CommonModule],
  templateUrl: './event-card-component.html',
  styleUrls: ['./event-card-component.css']
})
export class EventCardComponent implements OnInit {
  @Input() event!: Event;
  userId!: number;
  error = '';
  joined = false;
  canJoin = false;
  isLoggedIn = false;
  username = '';
  partId!: number;

  constructor(
    private participationService: ParticipationService,
    private auth: AuthService,
    private cdr: ChangeDetectorRef
  ) {}

  ngOnInit() {
    const token = this.auth.getToken();
    console.log('[EventCardComponent] token:', token);

    if (token) {
      try {
        const payload = jwtDecode<JwtPayload>(token);
        console.log('[EventCardComponent] decoded token:', payload);
        this.userId = Number(payload['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier']);
        this.canJoin = payload['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'] === 'Volunteer';
        this.username = payload['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name'];
        console.log(this.canJoin);

        if (this.canJoin) {
          this.participationService.checkParticipation(this.userId, this.event.id).subscribe({
            next: (hasJoined) => {
              this.joined = hasJoined;
              this.cdr.markForCheck();
            },
            error: () => this.error = 'Failed to check participation'
          });
        }
        this.auth.isLoggedIn.subscribe(status => {
          this.isLoggedIn = status;
        });


      } catch (err) {
        console.error('[EventCardComponent] Invalid token:', err);
      }
    } else {
      console.warn('[EventCardComponent] No token found');
    }
  }

  joinEvent() {
    if (!this.isLoggedIn) {
      alert('You must be logged in to join an event.');
      return;
    }

    if (!this.userId || !this.canJoin) return;

    const participation = {
      id: this.partId,
      userId: this.userId,
      username: this.username,
      eventId: this.event.id,
      attended: false,
      feedback: null
    };

    this.participationService.joinEvent(participation).subscribe({
      next: () => {
        this.joined = true;
        this.cdr.markForCheck();
      },
      error: (err) => {
        this.error = err.error || 'Failed to join';
      }
    });
  }

  
  leaveEvent() {
    if (!this.userId || !this.canJoin) return;

    this.participationService.leaveEvent(this.userId, this.event.id).subscribe({
      next: () => {
        this.joined = false;
        this.cdr.markForCheck();
      },
      error: () => {
        this.error = 'Failed to leave event';
      }
    });
  }
}
