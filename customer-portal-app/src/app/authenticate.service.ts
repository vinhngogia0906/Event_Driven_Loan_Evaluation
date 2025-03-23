import { Injectable } from '@angular/core';
import { Router } from '@angular/router';

@Injectable({
  providedIn: 'root'
})
export class AuthenticateService {

  private currentUserGuid: string | null = null;

  constructor(private router: Router) {}

  login(fullName: string, email: string): void {
    // Simulate a login API call
    this.currentUserGuid = '550e8400-e29b-41d4-a716-446655440000'; // Hardcoded GUID for demo
    this.router.navigate(['/dashboard']);
  }

  signup(fullName: string, email: string): void {
    // Simulate a signup API call
    this.currentUserGuid = '550e8400-e29b-41d4-a716-446655440000'; // Hardcoded GUID for demo
    this.router.navigate(['/dashboard']);
  }

  getCurrentUserGuid(): string | null {
    return this.currentUserGuid;
  }

  logout(): void {
    this.currentUserGuid = null;
    this.router.navigate(['/signin']);
  }
}
