import { Component, Input, Output, EventEmitter, ChangeDetectorRef } from '@angular/core';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { EventService } from '../services/event';
import { Event } from '../models/event';
import { debounceTime, distinctUntilChanged, switchMap } from 'rxjs/operators';
import { HttpClient } from '@angular/common/http';
import { of } from 'rxjs';

@Component({
  selector: 'app-event-form',
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './event-form-component.html',
  styleUrls: ['./event-form-component.css']
})
export class EventFormComponent {
  @Input() eventData: Event | null = null;
  @Output() saved = new EventEmitter<void>();
  eventForm: FormGroup;
  suggestions: any[] = [];

  constructor(private fb: FormBuilder, private eventService: EventService, private cdr: ChangeDetectorRef, private http: HttpClient) {
    this.eventForm = this.fb.group({
      title: ['', Validators.required],
      description: ['', Validators.required],
      location: this.fb.control('', {
        validators: Validators.required,
        updateOn: 'change' 
      }),
      date: ['', Validators.required],
      maxParticipants: ['', [Validators.required, Validators.min(1)]],
      category: ['', Validators.required]
    });
  }

  ngOnInit() {
    this.eventForm.get('location')?.valueChanges.pipe(
      debounceTime(300),
      distinctUntilChanged(),
      switchMap(value => this.searchLocation(value))
    ).subscribe(results => {
      this.suggestions = results;
    });
  }

  ngOnChanges() {
    if (this.eventData) {
      this.eventForm.patchValue(this.eventData);
    }
  }

  searchLocation(query: string) {
    if (!query || query.length < 3) {
      return of([]);
    }

    const url = `https://nominatim.openstreetmap.org/search?format=json&q=${encodeURIComponent(query)}`;
    return this.http.get<any[]>(url);
  }

  selectSuggestion(suggestion: any) {
    this.eventForm.get('location')?.setValue(suggestion.display_name);
    this.suggestions = [];
  }

  submit() {
    console.log(this.eventForm.value);
    const formValue = this.eventForm.value;

    const eventPayload = {
      id: this.eventData?.id,
      title: this.eventForm.value.title,
      description: this.eventForm.value.description,
      location: this.eventForm.value.location,
      date: this.eventForm.value.date,
      maxParticipants: this.eventForm.value.maxParticipants,
      category: this.eventForm.value.category
    };
    console.log('Payload:', eventPayload);

    if (this.eventData) {
      this.eventService.updateEvent(this.eventData.id, formValue).subscribe(() => {
        this.saved.emit();
        this.cdr.detectChanges(); 
      });
    } else {
      this.eventService.createEvent(formValue).subscribe(() => {
        this.saved.emit();
        this.cdr.detectChanges(); 
      });
    }
  }
}
