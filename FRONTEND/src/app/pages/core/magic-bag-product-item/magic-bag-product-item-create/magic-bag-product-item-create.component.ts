import {Component, OnInit} from '@angular/core';
import {MatButton} from "@angular/material/button";
import {MatFormField, MatHint, MatLabel} from "@angular/material/form-field";
import {MatInput} from "@angular/material/input";
import {FormControl, FormGroup, ReactiveFormsModule, Validators} from "@angular/forms";
import {MagicbagService} from "../../../../services/magicbag.service";
import {ToastrService} from "ngx-toastr";
import {MatDialog} from "@angular/material/dialog";
import {Router, ActivatedRoute} from "@angular/router";
import {ProductService} from "../../../../services/product.service";
import {MatOption} from "@angular/material/autocomplete";
import {MatSelect} from "@angular/material/select";
import {CommonModule} from "@angular/common";

@Component({
  selector: 'app-magic-bag-product-item-create',
  standalone: true,
  imports: [
    MatButton,
    MatFormField,
    MatHint,
    MatInput,
    MatLabel,
    ReactiveFormsModule,
    MatOption,
    MatSelect,
    CommonModule
  ],
  templateUrl: './magic-bag-product-item-create.component.html',
  styleUrl: './magic-bag-product-item-create.component.scss'
})
export class MagicBagProductItemCreateComponent  implements OnInit {
  magicBagId: string | null = null;

  toppingList: any[] = [];

  constructor(private  router: Router ,
              private route: ActivatedRoute,

              private dialog : MatDialog,
              private toastr: ToastrService, private magicbag : MagicbagService, private productService : ProductService
  ) {

    console.log(this.toppingList)
  }

  form = new FormGroup({
    quanity: new FormControl('', [Validators.required]),
    productID: new FormControl('', [Validators.required]),
  });


  ngOnInit(): void {
    this.getAllProducts();
    this.route.paramMap.subscribe(params => {
      this.magicBagId = params.get('id');
      console.log('Magic Bag ID:', this.magicBagId);
      // You can now use this.magicBagId to fetch data or perform other actions
    });
  }

  get f(){
    return this.form.controls;
  }


  getAllProducts() {
    this.productService.getAll().subscribe((res : any)  => {
      console.log(res.success.data)
      //this.dataSource = res.success.data;
      this.toppingList = res.success.data;
    })
  }

  submit() {
    console.log(this.form.value);
    let usertype = JSON.parse(localStorage.getItem('user') || '{}');
    console.log(usertype)
    const payload = {
      Name : this.form.controls['quanity'].value,
      PartnerId :usertype?.partner.partnerId,
      ProductID: this.form.controls['productID'].value

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
