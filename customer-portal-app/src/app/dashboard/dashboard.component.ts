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
  currentUserGuid: string | null = null;

  showForm = false;
  loanType = '';
  loanLimit: number | null = null;
  loanPurpose = '';

  toggleForm(): void {
    this.showForm = !this.showForm;
  }

  constructor(private authService: AuthenticateService, private loanService: LoanApplicationService) {}

  ngOnInit(): void {
    // Subscribe to get the current user's GUID
    this.authService.getCurrentUserGuid().subscribe((userGuid) => {
      this.currentUserGuid = userGuid;
      if (this.currentUserGuid) {
        this.loadLoans(this.currentUserGuid); // Load loans if GUID is available
      } else {
        console.error('No user GUID found.');
      }
    });
  }

  loadLoans(userGuid: string): void {
    this.loanService.getLoansByUserGuid(userGuid).subscribe({
      next: (loans) => {
        this.loans = loans;
      },
      error: (err) => {
        console.error('Failed to load loans:', err);
      },
    });
  }

   // Method to cancel loan application
   cancelLoan(applicationId: string): void {
    if (confirm('Are you sure you want to cancel this loan application?')) {
      this.loanService.cancelLoanApplication(applicationId).subscribe({
        next: () => {
          console.log('Loan application cancelled successfully');
          // Reload loans after cancellation
          this.loadLoans(this.currentUserGuid!);
        },
        error: (err) => {
          console.error('Error canceling loan application', err);
        }
      });
    }
  }

  submitLoan(): void {
    if (!this.currentUserGuid || !this.loanType || !this.loanLimit || !this.loanPurpose) {
      alert('Please fill out all fields.');
      return;
    }
  
    this.loanService
      .applyLoan(this.currentUserGuid, this.loanType, this.loanLimit, this.loanPurpose)
      .subscribe({
        next: () => {
          alert('Loan application submitted!');
          this.showForm = false;
          this.loanType = '';
          this.loanLimit = null;
          this.loanPurpose = '';
          this.loadLoans(this.currentUserGuid!); // Refresh loans
        },
        error: (err) => {
          console.error(err);
          alert('Failed to submit loan application.');
        },
      });
  }

  logout(): void {
    this.authService.logout();
  }
}
