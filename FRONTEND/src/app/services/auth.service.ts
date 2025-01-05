import { Injectable } from '@angular/core';
import {HttpClient} from "@angular/common/http";
import { environment } from 'src/environments/environment';
@Injectable({ providedIn: 'root' })
export class AuthService {

  private baseUrl = environment.apiUrl;
  constructor(private Http : HttpClient) {

  }

  register(payload : any)  {
    return this.Http.post(`${this.baseUrl}/user/create`, payload)
  }

  verify(token : any)  {
    return this.Http.get(`${this.baseUrl}/user/verify/${token}`)
  }

  login(payload : any)  {
    return this.Http.post(`${this.baseUrl}/user/login`, payload)
  }
}
