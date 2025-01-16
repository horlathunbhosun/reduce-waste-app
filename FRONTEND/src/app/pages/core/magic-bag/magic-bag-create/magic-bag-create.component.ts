import { Component } from '@angular/core';
import {MatButton} from "@angular/material/button";
import {MatFormField, MatHint, MatLabel} from "@angular/material/form-field";
import {MatInput} from "@angular/material/input";
import {FormControl, FormGroup, ReactiveFormsModule, Validators} from "@angular/forms";
import {Router} from "@angular/router";
import {MatDialog} from "@angular/material/dialog";
import {ToastrService} from "ngx-toastr";
import {MagicbagService} from "../../../../services/magicbag.service";

@Component({
  selector: 'app-magic-bag-create',
  standalone: true,
  imports: [
    MatButton,
    MatFormField,
    MatHint,
    MatInput,
    MatLabel,
    ReactiveFormsModule
  ],
  templateUrl: './magic-bag-create.component.html',
  styleUrl: './magic-bag-create.component.scss'
})
export class MagicBagCreateComponent {

  constructor(private  router: Router , private dialog : MatDialog,
              private toastr: ToastrService, private magicbag : MagicbagService
  ) {
  }

  form = new FormGroup({
    name: new FormControl('', [Validators.required, Validators.minLength(6)]),
    description: new FormControl('', [Validators.required]),
    price: new FormControl('', [Validators.required]),
  });


  get f(){
    return this.form.controls;
  }

  submit() {
    console.log(this.form.value);
    let usertype = JSON.parse(localStorage.getItem('user') || '{}');
    console.log(usertype)
    const payload = {
      Name : this.form.controls['name'].value,
      Description : this.form.controls['description'].value,
      Price : this.form.controls['price'].value,
      PartnerId :usertype?.partner.partnerId
    }



    this.magicbag.createBag(payload).subscribe(
      res => {

        this.toastr.success('Magic Bag Created Successfully');

        console.log(res)
        this.form.reset();
        this.closeModal();
        //reload the page
        this.router.navigate(['/core/magic-bag']);
      }

    )
    // this.router.navigate(['/core/product']);
  }

  closeModal() {
    this.dialog.closeAll();
  }
}
