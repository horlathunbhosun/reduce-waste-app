import { Injectable } from '@angular/core';
import {HttpClient} from "@angular/common/http";
import {environment} from "../../environments/environment";

@Injectable({ providedIn: 'root' })
export class ProductService {

  private baseUrl = environment.apiUrl;

  constructor(private Http : HttpClient) {

  }


  create(payload : any)  {

    const headers = {
      'Content-Type': 'application/json',
      'Authorization': `Bearer ${localStorage.getItem('token')}`
    }
    return this.Http.post(`${this.baseUrl}/product/create`, payload, {headers})
  }


  edit(payload : any, id : string)  {
    const headers = {
      'Content-Type': 'application/json',
      'Authorization': `Bearer ${localStorage.getItem('token')}`
    }
    return this.Http.patch(`${this.baseUrl}/product/${id}/update`, payload, {headers})
  }


  getAll() {
    const headers = {
      'Content-Type': 'application/json',
      'Authorization': `Bearer ${localStorage.getItem('token')}`
    }
    return this.Http.get(`${this.baseUrl}/product/all`, {headers})
  }


  delete (id : string) {
    const headers = {
      'Content-Type': 'application/json',
      'Authorization': `Bearer ${localStorage.getItem('token')}`
    }
    return this.Http.delete(`${this.baseUrl}/product/${id}/delete`, {headers})
  }



}
