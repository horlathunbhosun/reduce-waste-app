import {Component, Inject, OnInit, Optional} from '@angular/core';
import {MatButton} from "@angular/material/button";
import {MatFormField, MatHint, MatLabel} from "@angular/material/form-field";
import {MatInput} from "@angular/material/input";
import {FormControl, FormGroup, ReactiveFormsModule, Validators} from "@angular/forms";
import {ProductService} from "../../../../services/product.service";
import {Router} from "@angular/router";
import {MAT_DIALOG_DATA, MatDialog, MatDialogRef} from "@angular/material/dialog";
import {ToastrService} from "ngx-toastr";
import {MagicbagService} from "../../../../services/magicbag.service";

@Component({
  selector: 'app-magic-bag-edit',
  standalone: true,
    imports: [
        MatButton,
        MatFormField,
        MatHint,
        MatInput,
        MatLabel,
        ReactiveFormsModule
    ],
  templateUrl: './magic-bag-edit.component.html',
  styleUrl: './magic-bag-edit.component.scss'
})
export class MagicBagEditComponent  implements OnInit {


  constructor(private productService: ProductService, private router: Router, private dialog: MatDialog,
              public dialogRef: MatDialogRef<MagicBagEditComponent>, @Optional() @Inject(MAT_DIALOG_DATA) public data: any,
              private toastr: ToastrService, private magicbag : MagicbagService) {


    console.log(data, 'data')
  }

  ngOnInit(): void {
    this.form.patchValue(this.editData);
  }

  form = new FormGroup({
    name: new FormControl('', [Validators.required]),
    description: new FormControl('', [Validators.required]),
    price: new FormControl('', [Validators.required]),
  });

  editData = {
    name: this.data.name,
    description: this.data.description,
    price: this.data.bagPrice,
  }


  get f() {
    return this.form.controls;
  }


  update() {
    let usertype = JSON.parse(localStorage.getItem('user') || '{}');
    const payload = {
      Name: this.form.controls['name'].value,
      Description: this.form.controls['description'].value,
      Price : this.form.controls['price'].value,
      PartnerId :usertype?.partner.partnerId
    }

    this.magicbag.edit(payload, this.data.id).subscribe(
      res => {

        console.log(res)
        this.toastr.success('Magic Bag Updated Successfully');

        this.form.reset();
        this.closeModal();
        this.router.navigate(['/core/product']);

      }
    )
    // this.router.navigate(['/core/product']);
  }

  closeModal() {
    this.dialog.closeAll();
  }


}
