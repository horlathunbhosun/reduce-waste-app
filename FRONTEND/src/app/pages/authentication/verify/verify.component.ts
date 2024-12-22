import { Component } from '@angular/core';
import {FormBuilder, FormControl, FormGroup, FormsModule, ReactiveFormsModule, Validators} from "@angular/forms";
import {MatButton} from "@angular/material/button";
import {MatCheckbox} from "@angular/material/checkbox";
import {MatFormField, MatHint, MatLabel} from "@angular/material/form-field";
import {MatInput} from "@angular/material/input";
import {Router, RouterLink} from "@angular/router";
import {AuthService} from "../../../services/auth.service";

@Component({
  selector: 'app-verify',
  standalone: true,
  imports: [
    FormsModule,
    MatButton,
    MatCheckbox,
    MatFormField,
    MatHint,
    MatInput,
    MatLabel,
    ReactiveFormsModule,
    RouterLink
  ],
  templateUrl: './verify.component.html',
  styleUrl: './verify.component.scss'
})
export class VerifyComponent {

  constructor(private router: Router, private authService : AuthService) { }

  form = new FormGroup({
    token: new FormControl('', [Validators.required, Validators.minLength(6)]),
  });


  get f(){
    return this.form.controls;
  }

  submit() {
    console.log(this.form.value);

    this.authService.verify(this.form.controls['token'].value).subscribe(res => {
      console.log(res);
      }
    )
    this.router.navigate(['/authentication/login']);
  }
}
