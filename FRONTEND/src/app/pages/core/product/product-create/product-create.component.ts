import { Component } from '@angular/core';
import {FormControl, FormGroup, FormsModule, ReactiveFormsModule, Validators} from "@angular/forms";
import {MatButton} from "@angular/material/button";
import {MatFormField, MatHint, MatLabel} from "@angular/material/form-field";
import {MatInput} from "@angular/material/input";
import {ProductService} from "../../../../services/product.service";
import {Router} from "@angular/router";

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
 constructor(private productService : ProductService, private router : Router ) {
 }

  form = new FormGroup({
    name: new FormControl('', [Validators.required, Validators.minLength(6)]),
    description: new FormControl('', [Validators.required]),
  });


  get f(){
    return this.form.controls;
  }

  submit() {
    console.log(this.form.value);

    const payload = {
      Name : this.form.controls['name'].value,
      Description : this.form.controls['description'].value,
    }

    this.productService.create(payload).subscribe(
      res => { console.log(res)}

    )
     this.router.navigate(['/core/product']);
  }

}
