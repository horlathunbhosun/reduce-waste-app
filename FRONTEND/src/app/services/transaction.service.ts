import { Injectable } from '@angular/core';
import {HttpClient} from "@angular/common/http";
import {environment} from "../../environments/environment";

@Injectable({ providedIn: 'root' })
export class TransactionService {

  private baseUrl = environment.apiUrl;

  constructor(private Http: HttpClient) {

  }

  getAll() {
    const headers = {
      'Content-Type': 'application/json',
      'Authorization': `Bearer ${localStorage.getItem('token')}`
    }
    return this.Http.get(`${this.baseUrl}/transaction/all`, {headers})
  }

  getUserTransactions() {
    const headers = {
      'Content-Type': 'application/json',
      'Authorization': `Bearer ${localStorage.getItem('token')}`
    }
    return this.Http.get(`${this.baseUrl}/transaction/users`, {headers})
  }

  create(payload: any) {
    const headers = {
      'Content-Type': 'application/json',
      'Authorization': `Bearer ${localStorage.getItem('token')}`
    }
    return this.Http.post(`${this.baseUrl}/transaction/create`, payload, {headers})
  }

}
