import {Component, Inject, OnInit, Optional} from '@angular/core';
import {ProductService} from "../../../../services/product.service";
import {Router} from "@angular/router";
import {MAT_DIALOG_DATA, MatDialog, MatDialogRef} from "@angular/material/dialog";
import {ToastrService} from "ngx-toastr";
import {MatButton} from "@angular/material/button";
import {MatFormField, MatHint, MatLabel} from "@angular/material/form-field";
import {MatInput} from "@angular/material/input";
import {FormControl, FormGroup, ReactiveFormsModule, Validators} from "@angular/forms";
import {MatIconModule} from '@angular/material/icon';

@Component({
  selector: 'app-product-delete',
  standalone: true,
  imports: [
    MatButton,
    MatFormField,
    MatHint,
    MatInput,
    MatLabel,
    MatIconModule,
    ReactiveFormsModule
  ],
  templateUrl: './product-delete.component.html',
  styleUrl: './product-delete.component.scss'
})
export class ProductDeleteComponent implements OnInit {

  constructor(private productService : ProductService, private router : Router, private dialog : MatDialog,
              public dialogRef: MatDialogRef<ProductDeleteComponent>, @Optional() @Inject(MAT_DIALOG_DATA) public data: any,
              private toastr: ToastrService
              ) {


    console.log(data)
  }

  form = new FormGroup({
    id: new FormControl('', [Validators.required]),
  });

  ngOnInit(): void {
    this.form.patchValue(this.dataDelete);
  }

   dataDelete = {
    id: this.data.id,
  }


  deleteProduct() {
    console.log("date", this.dataDelete);
      this.productService.delete(this.dataDelete.id).subscribe(
        res => {
          console.log(res)

          this.toastr.success('Product Deleted Successfully');

          this.form.reset();
          this.closeModal();

          this.router.navigate(['/core/product']);

        }

      )

  }
  closeModal()
  {
      this.dialog.closeAll();
  }



}
