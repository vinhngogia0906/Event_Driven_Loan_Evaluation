import { Component } from '@angular/core';
import { AuthenticateService } from '../authenticate.service';

@Component({
  selector: 'app-signin',
  standalone: false,
  templateUrl: './signin.component.html',
  styleUrl: './signin.component.css'
})
export class SigninComponent {
  fullName: string = '';
  email: string = '';

  constructor(private authService: AuthenticateService) {}

  onSubmit(): void {
    this.authService.login(this.fullName, this.email);
  }
}
