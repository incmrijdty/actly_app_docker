import { ChangeDetectorRef, Component, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router'; 
import { Event } from '../models/event';
import { EventCardComponent } from '../event-card-component/event-card-component';
import { AuthService } from '../services/auth';
import { Router } from '@angular/router';
import { Subscription } from 'rxjs';
import { FormsModule } from '@angular/forms';
import { environment } from '../environments/environment';

@Component({
  selector: 'app-home-page',
  standalone: true,
  imports: [CommonModule, RouterModule, EventCardComponent, FormsModule],
  templateUrl: './home-page.html',
  styleUrl: './home-page.css'
})

export class HomePage implements OnInit {
  events: Event[] = [];
  filteredEvents: Event[] = [];
  loading = true;
  isLoggedIn = false;
  userRole: string | null = null;
  private authSub?: Subscription;

  categories: string[] = []; 
  selectedCategory = '';
  minParticipants?: number;
  maxParticipants?: number;
  selectedDate?: string; 
  locationFilter = '';

  constructor(private http: HttpClient, private cd: ChangeDetectorRef, private authService: AuthService, private router: Router) {}

  ngOnInit(): void {
    this.fetchEvents();

    this.authSub = this.authService.isLoggedIn.subscribe(loggedIn => {
      this.isLoggedIn = loggedIn;
      this.userRole = this.authService.getUserRole();
      this.cd.detectChanges();
    });


    this.isLoggedIn = this.authService.isLoggedIn.getValue();
    this.userRole = this.authService.getUserRole();
  }

  ngOnDestroy(): void {
    this.authSub?.unsubscribe();
  }

  fetchEvents(): void {
    this.http.get<Event[]>(`${environment.apiUrl}/api/Events`) 
      .subscribe({
        next: (data) => {
          this.events = data;
          this.categories = [...new Set(data.map(e => e.category))]; 
          this.applyFilters();
          this.loading = false;
          this.cd.detectChanges();
        },
        error: (err) => {
          console.error('Failed to fetch events', err);
          this.loading = false;
          this.cd.detectChanges();
        }
      });
  }

  applyFilters(): void {
    this.filteredEvents = this.events.filter(event => {
      if (this.selectedCategory && event.category !== this.selectedCategory) {
        return false;
      }
      if (this.minParticipants != null && event.maxParticipants < this.minParticipants) {
        return false;
      }
      if (this.maxParticipants != null && event.maxParticipants > this.maxParticipants) {
        return false;
      }
      if (this.selectedDate) {
        const eventDate = new Date(event.date);
        const selectedDateObj = new Date(this.selectedDate);
        if (eventDate < selectedDateObj) {
          return false;
        }
      }
      if (this.locationFilter) {
        const loc = this.locationFilter.toLowerCase();
        if (!event.location.toLowerCase().includes(loc)) {
          return false;
        }
      }
      return true;
    });
  }

}
