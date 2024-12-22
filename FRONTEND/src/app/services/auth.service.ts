import { Injectable } from '@angular/core';
import {HttpClient} from "@angular/common/http";

@Injectable({ providedIn: 'root' })
export class AuthService {


  constructor(private Http : HttpClient) {

  }


  register(payload : any)  {
    return this.Http.post(`http://localhost:5104/api/user/create`, payload)
  }

  verify(token : any)  {
    return this.Http.get(`http://localhost:5104/api/user/verify/${token}`)
  }
}
