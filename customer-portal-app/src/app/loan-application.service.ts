import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class LoanApplicationService {

  private loans = [
    { id: 1, userId: '550e8400-e29b-41d4-a716-446655440000', amount: 1000, status: 'Pending' },
    { id: 2, userId: '550e8400-e29b-41d4-a716-446655440000', amount: 2000, status: 'Approved' },
  ];

  constructor() {}

  // getLoansByUserGuid(userGuid: string) {
  //   return of(this.loans.filter((loan) => loan.userId === userGuid));
  // }
}
