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


  createMagicItem(payload : any)  {

    const headers = {
      'Content-Type': 'application/json',
      'Authorization': `Bearer ${localStorage.getItem('token')}`
    }
    return this.Http.post(`${this.baseUrl}/magic-bag/add-magic-bag-item`, payload, {headers})
  }

  getProductMagicItem(id: string | null) {
    const headers = {
      'Content-Type': 'application/json',
      'Authorization': `Bearer ${localStorage.getItem('token')}`
    }
    return this.Http.get(`${this.baseUrl}/magic-bag/get-all-magicbag-items/${id}`, {headers}).pipe()
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

  delete (id : string) {
    const headers = {
      'Content-Type': 'application/json',
      'Authorization': `Bearer ${localStorage.getItem('token')}`
    }
    return this.Http.delete(`${this.baseUrl}/magic-bag/${id}/delete`, {headers})
  }


}
