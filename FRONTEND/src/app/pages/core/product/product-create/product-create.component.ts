import { Component } from '@angular/core';
import {FormControl, FormGroup, FormsModule, ReactiveFormsModule, Validators} from "@angular/forms";
import {MatButton} from "@angular/material/button";
import {MatFormField, MatHint, MatLabel} from "@angular/material/form-field";
import {MatInput} from "@angular/material/input";
import {ProductService} from "../../../../services/product.service";
import {Router} from "@angular/router";
import {MatDialog} from "@angular/material/dialog";
import {ToastrService} from "ngx-toastr";

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
 constructor(private productService : ProductService, private router : Router, private dialog : MatDialog,
             private toastr: ToastrService) {
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
      res => {

        this.toastr.success('Product Created Successfully');

        console.log(res)
        this.form.reset();
        this.closeModal();
        //reload the page
        this.router.navigate(['/core/product']);
      }

    )
     // this.router.navigate(['/core/product']);
  }

  closeModal() {
    this.dialog.closeAll();
  }

}
