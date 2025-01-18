import {Component, Inject, OnInit, Optional} from '@angular/core';
import {MatButton} from "@angular/material/button";
import {MatIcon} from "@angular/material/icon";
import {FormControl, FormGroup, ReactiveFormsModule, Validators} from "@angular/forms";
import {ProductService} from "../../../../services/product.service";
import {Router} from "@angular/router";
import {MAT_DIALOG_DATA, MatDialog, MatDialogRef} from "@angular/material/dialog";
import {ToastrService} from "ngx-toastr";
import {MagicbagService} from "../../../../services/magicbag.service";

@Component({
  selector: 'app-magic-bag-delete',
  standalone: true,
    imports: [
        MatButton,
        MatIcon,
        ReactiveFormsModule
    ],
  templateUrl: './magic-bag-delete.component.html',
  styleUrl: './magic-bag-delete.component.scss'
})
export class MagicBagDeleteComponent implements OnInit {


  constructor(private productService: ProductService, private router: Router, private dialog: MatDialog,
              public dialogRef: MatDialogRef<MagicBagDeleteComponent>, @Optional() @Inject(MAT_DIALOG_DATA) public data: any,
              private toastr: ToastrService, private magicbag : MagicbagService) {


    console.log(data, 'data')
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





  deleteMagicBag() {
    console.log("date", this.dataDelete);
    this.magicbag.delete(this.dataDelete.id).subscribe(
      res => {
        console.log(res)

        this.toastr.success('Product Deleted Successfully');

        this.form.reset();
        this.closeModal();

        this.router.navigate(['/core/magic-bag']);

      }

    )
  }

  closeModal() {
    this.dialog.closeAll();
  }

}
