import { Component } from '@angular/core';
import {FormControl, FormGroup, FormsModule, ReactiveFormsModule, Validators} from "@angular/forms";
import {MatButton} from "@angular/material/button";
import {MatFormField, MatHint, MatLabel} from "@angular/material/form-field";
import {MatInput} from "@angular/material/input";

@Component({
  selector: 'app-product-create',
  standalone: true,
  imports: [
    FormsModule,
    MatButton,
    MatFormField,
    MatHint,
    MatInput,
    MatLabel,
    ReactiveFormsModule
  ],
  templateUrl: './product-create.component.html',
  styleUrl: './product-create.component.scss'
})
export class ProductCreateComponent {
 constructor() {
 }

  form = new FormGroup({
    token: new FormControl('', [Validators.required, Validators.minLength(6)]),
  });


  get f(){
    return this.form.controls;
  }

  submit() {
    console.log(this.form.value);

    // this.authService.verify(this.form.controls['token'].value).subscribe(res => {
    //     console.log(res);
    //   }
    // )
    // this.router.navigate(['/authentication/login']);
  }

}
