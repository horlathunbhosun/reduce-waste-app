import {Component, Inject, OnInit, Optional} from '@angular/core';
import {FormControl, FormGroup, FormsModule, ReactiveFormsModule, Validators} from "@angular/forms";
import {MatButton} from "@angular/material/button";
import {MatFormField, MatHint, MatLabel} from "@angular/material/form-field";
import {MatInput} from "@angular/material/input";
import {ProductService} from "../../../../services/product.service";
import {Router} from "@angular/router";
import {MAT_DIALOG_DATA, MatDialog, MatDialogRef} from "@angular/material/dialog";
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-product-edit',
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
  templateUrl: './product-edit.component.html',
  styleUrl: './product-edit.component.scss'
})
export class ProductEditComponent implements OnInit {


  constructor(private productService: ProductService, private router: Router, private dialog: MatDialog,
              public dialogRef: MatDialogRef<ProductEditComponent>, @Optional() @Inject(MAT_DIALOG_DATA) public data: any,
              private toastr: ToastrService) {


    console.log(data)
  }

  ngOnInit(): void {
    this.form.patchValue(this.editData);
  }

  form = new FormGroup({
    name: new FormControl('', [Validators.required]),
    description: new FormControl('', [Validators.required]),
  });

  editData = {
    name: this.data.name,
    description: this.data.description
  }


  get f() {
    return this.form.controls;
  }

  update() {

    const payload = {
      Name: this.form.controls['name'].value,
      Description: this.form.controls['description'].value,
    }

    this.productService.edit(payload, this.data.id).subscribe(
      res => {

        console.log(res)
        this.toastr.success('Product Updated Successfully');

        this.form.reset();
        this.closeModal();
      }
    )
    // this.router.navigate(['/core/product']);
  }

  closeModal() {
    this.dialog.closeAll();
  }
}
