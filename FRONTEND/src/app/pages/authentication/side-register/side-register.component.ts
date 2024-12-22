import { Component } from '@angular/core';
import {
  FormGroup,
  FormControl,
  Validators,
  FormsModule,
  ReactiveFormsModule,
} from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { MaterialModule } from '../../../material.module';
import {AuthService} from "../../../services/auth.service";

@Component({
  selector: 'app-side-register',
  standalone: true,
  imports: [RouterModule, MaterialModule, FormsModule, ReactiveFormsModule],
  templateUrl: './side-register.component.html',
})
export class AppSideRegisterComponent {
  constructor(private router: Router, private authService: AuthService) { }

  form = new FormGroup({
    uname: new FormControl('', [Validators.required, Validators.minLength(6)]),
    email: new FormControl('', [Validators.required]),
    password: new FormControl('', [Validators.required]),
    username: new FormControl('', [Validators.required, Validators.minLength(6)]),
    phone_number: new FormControl('', [Validators.required, Validators.minLength(10)]),
    usertype: new FormControl('', [Validators.required]),

  });

  get f() {
    return this.form.controls;
  }

  submit() {
   // console.log(this.form.value);
    const payload = {
      fullName: this.form.controls['uname'].value,
      email: this.form.controls['email'].value,
      userName : this.form.controls['username'].value,
      phoneNumber : this.form.controls['phone_number'].value,
      userType: this.form.controls['usertype'].value,
      password: this.form.controls['password'].value,
      // "partner": {
      //   "businessNumber": 0,
      //   "logo": "string",
      //   "address": "string"
      // }
    }

    this.authService.register(payload).subscribe(
      res => { console.log(res)}
    )
    console.log(payload);
    this.router.navigate(['/']);
  }
}
