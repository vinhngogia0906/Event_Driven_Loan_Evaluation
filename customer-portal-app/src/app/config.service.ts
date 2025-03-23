import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class ConfigService {
  private _backendUrl = 'http://localhost:8081/customerService'; // Default backend URL of the Customer Service

  get backendUrl(): string {
    return this._backendUrl;
  }

  set backendUrl(url: string) {
    this._backendUrl = url;
  }
}
