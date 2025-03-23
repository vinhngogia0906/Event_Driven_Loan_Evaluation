import { Component } from '@angular/core';
import { AuthenticateService } from '../authenticate.service';
import { ConfigService } from '../config.service';

@Component({
  selector: 'app-signin',
  standalone: false,
  templateUrl: './signin.component.html',
  styleUrl: './signin.component.css'
})
export class SigninComponent {
  fullName: string = '';
  email: string = '';
  backendUrl: string;

  constructor(private authService: AuthenticateService, private configService: ConfigService) {
    this.backendUrl = this.configService.backendUrl;
  }

  onSubmit(): void {
    this.authService.login(this.fullName, this.email);
  }
}
