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
import { MatButtonModule } from '@angular/material/button';
import {AuthService} from "../../../services/auth.service";

@Component({
  selector: 'app-side-login',
  standalone: true,
  imports: [
    RouterModule,
    MaterialModule,
    FormsModule,
    ReactiveFormsModule,
    MatButtonModule,
  ],
  templateUrl: './side-login.component.html',
})
export class AppSideLoginComponent {
  constructor(private router: Router, private authService: AuthService) { }

  form = new FormGroup({
    email: new FormControl('', [Validators.required, Validators.email]),
    password: new FormControl('', [Validators.required]),
  });

  get f() {
    return this.form.controls;
  }

  submit() {
    // console.log(this.form.value);
    const payload = {
      Email : this.form.controls['email'].value,
      Password: this.form.controls['password'].value
    }


    this.authService.login(payload).subscribe((res : any) => {
      console.log(res.success.data)
      localStorage.setItem('token', res.success.data.token)
      localStorage.setItem('user', JSON.stringify(res.success.data.userResponseDto))
      this.router.navigate(['/dashboard']);
    }, (error) => {
      console.log(error)

    });

  }
}
