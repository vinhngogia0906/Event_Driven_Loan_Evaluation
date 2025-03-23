import { Component } from '@angular/core';
import { AuthenticateService } from '../authenticate.service';

@Component({
  selector: 'app-signup',
  standalone: false,
  templateUrl: './signup.component.html',
  styleUrl: './signup.component.css'
})
export class SignupComponent {
  fullName: string = '';
  email: string = '';

  constructor(private authService: AuthenticateService) {}

  onSubmit(): void {
    this.authService.signup(this.fullName, this.email);
  }
}
