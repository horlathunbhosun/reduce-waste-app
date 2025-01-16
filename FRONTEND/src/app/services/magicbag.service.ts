import { Injectable } from '@angular/core';
import {HttpClient} from "@angular/common/http";
import {environment} from "../../environments/environment";

@Injectable({ providedIn: 'root' })
export class MagicbagService {
  private baseUrl = environment.apiUrl;

  constructor(private Http : HttpClient) {

  }


  createBag(payload : any)  {

    const headers = {
      'Content-Type': 'application/json',
      'Authorization': `Bearer ${localStorage.getItem('token')}`
    }
    return this.Http.post(`${this.baseUrl}/magic-bag/create`, payload, {headers})
  }


  getPartnerMagicBag() {
    const headers = {
      'Content-Type': 'application/json',
      'Authorization': `Bearer ${localStorage.getItem('token')}`
    }
    return this.Http.get(`${this.baseUrl}/magic-bag/get-partners-magicbags`, {headers})
  }

  getPartnerMagicBagAll() {
    const headers = {
      'Content-Type': 'application/json',
      'Authorization': `Bearer ${localStorage.getItem('token')}`
    }
    return this.Http.get(`${this.baseUrl}/magic-bag/get-all`, {headers}).pipe()
  }


  edit(payload : any, id : string)  {
    const headers = {
      'Content-Type': 'application/json',
      'Authorization': `Bearer ${localStorage.getItem('token')}`
    }
    return this.Http.patch(`${this.baseUrl}/magic-bag/${id}/update`, payload, {headers})
  }

}
