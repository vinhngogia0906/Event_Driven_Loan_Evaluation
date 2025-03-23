import { Component } from '@angular/core';
import { AuthenticateService } from '../authenticate.service';
import { LoanApplicationService } from '../loan-application.service';

@Component({
  selector: 'app-dashboard',
  standalone: false,
  templateUrl: './dashboard.component.html',
  styleUrl: './dashboard.component.css'
})
export class DashboardComponent {
  loans: any[] = [];

  constructor(private authService: AuthenticateService, private loanService: LoanApplicationService) {}

  // ngOnInit(): void {
  //   const userGuid = this.authService.getCurrentUserGuid();
  //   if (userGuid) {
  //     this.loanService.getLoansByUserGuid(userGuid).subscribe((loans) => {
  //       this.loans = loans;
  //     });
  //   }
  // }

  logout(): void {
    this.authService.logout();
  }
}
