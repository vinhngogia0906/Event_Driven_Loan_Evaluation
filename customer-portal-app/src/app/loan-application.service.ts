import { Injectable } from '@angular/core';
import { ConfigService } from './config.service';
import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class LoanApplicationService {

  backendUrl : string;

  constructor(private configService: ConfigService, private http: HttpClient) {
    this.backendUrl = configService.backendUrl;
  }

  // Function to cancel a loan application
  cancelLoanApplication(applicationId: string): Observable<any> {
    const url = `${this.backendUrl}/cancelLoanApplication`;
    
    const params = { applicationId };
    
    const headers = new HttpHeaders({
      'accept': 'text/plain'
    });

    return this.http.put<any>(url, null, { params, headers });
  }

  getLoansByUserGuid(userGuid: string): Observable<any[]> {
    // Ensure the userGuid is trimmed of any surrounding quotes
    const cleanedUserGuid = userGuid.replace(/^"|"$/g, '').trim();
    const params = new HttpParams().set('customerId', cleanedUserGuid);
    return this.http.get<any[]>(`${this.backendUrl}/customerLoanApplications`, {
      params,
    });
  }

  // Function to apply new loan
  applyLoan(userGuid: string, name: string, limit: number, purpose: string): Observable<any> {
    const cleanedUserGuid = userGuid.replace(/^"|"$/g, '').trim();
    const url = "http://localhost:8080/loanApplicationService/submitLoanApplication";
    const params = new HttpParams()
      .set('name', name)
      .set('limit', limit.toString())
      .set('purpose', purpose)
      .set('customerId', cleanedUserGuid);
  
    const headers = new HttpHeaders({
      accept: 'text/plain',
    });
  
    return this.http.post(url, null, { params, headers, responseType: 'text' });
  }
}
