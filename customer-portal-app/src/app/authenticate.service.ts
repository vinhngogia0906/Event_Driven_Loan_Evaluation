import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { ConfigService } from './config.service';
import { BehaviorSubject } from 'rxjs';
import { HttpClient, HttpParams } from '@angular/common/http';

@Injectable({
  providedIn: 'root'
})
export class AuthenticateService {

  private currentUserGuid = new BehaviorSubject<string | null>(null);
  backendUrl: string;

  constructor(private http: HttpClient, private router: Router, private configService: ConfigService) {
    this.backendUrl = configService.backendUrl;
  }

  login(name: string, email: string) {
    const params = new HttpParams()
      .set('name', name)
      .set('email', email);

    this.http
      .post<string>(`${this.backendUrl}/signin`, null, {
        params,
        responseType: 'text' as 'json',
      })
      .subscribe({
        next: (response) => {
          console.log('Login response:', response); // Log the response
          if (response) {
            this.currentUserGuid.next(response); // Store the GUID
            this.router.navigate(['/dashboard']); // Redirect to dashboard
          } else {
            alert('Invalid credentials');
          }
        },
        error: (err) => {
          console.error('Login failed:', err);
          alert(err.error.message ||'Login failed. Please try again.');
        },
      });
  }

  signup(name: string, email: string) {
    const params = new HttpParams()
      .set('name', name)
      .set('email', email);

    this.http
      .post<string>(`${this.backendUrl}/register`, null, {
        params,
        responseType: 'text' as 'json',
      })
      .subscribe({
        next: (response) => {
          console.log('Signup response:', response);
          if (response) {
            alert('Signup successful!');
            this.router.navigate(['/signin']);
          } else {
            alert('Signup failed. Please try again.');
          }
        },
        error: (err) => {
          console.error('Signup failed:', err);
          alert('Signup failed. Please try again.');
        },
      });
  }

  getCurrentUserGuid() {
    return this.currentUserGuid.asObservable();
  }

  logout() {
    this.currentUserGuid.next(null);
    this.router.navigate(['/signin']);
  }
}
