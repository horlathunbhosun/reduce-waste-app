import {Component, Inject, OnInit, Optional} from '@angular/core';
import {Router} from "@angular/router";
import {MAT_DIALOG_DATA, MatDialog, MatDialogRef} from "@angular/material/dialog";
import {ToastrService} from "ngx-toastr";
import {MagicbagService} from "../../../services/magicbag.service";
import {MatTableModule} from "@angular/material/table";
import {MatButton, MatButtonModule, MatFabButton} from "@angular/material/button";
import {CdkAccordion} from "@angular/cdk/accordion";
import {MatMenuModule} from "@angular/material/menu";
import {MatIconModule} from "@angular/material/icon";
import {DatePipe, NgIf} from "@angular/common";
import {MagicBagCreateComponent} from "../magic-bag/magic-bag-create/magic-bag-create.component";
import {
  MagicBagProductItemCreateComponent
} from "./magic-bag-product-item-create/magic-bag-product-item-create.component";

@Component({
  selector: 'app-magic-bag-product-item',
  standalone: true,
  imports: [
    MatTableModule, MatFabButton, MatButton, CdkAccordion, MatMenuModule, MatIconModule, MatButtonModule, DatePipe, NgIf

  ],
  templateUrl: './magic-bag-product-item.component.html',
  styleUrl: './magic-bag-product-item.component.scss'
})
export class MagicBagProductItemComponent implements OnInit {

  displayedColumns: string[] = ['name', 'description','price', 'status', 'created','action'];
  dataSourcer: any = [];

  constructor(private  router: Router , private dialog : MatDialog,
              private toastr: ToastrService, private magicbag : MagicbagService,

  ) {

  }


  ngOnInit(): void {
   // this.getAllMagicBags();
  }

  openDialog(): void {
    const dialogRef = this.dialog.open(MagicBagProductItemCreateComponent, {

    });

    dialogRef.afterClosed().subscribe(result => {
      console.log('The dialog was closed');
      // if (result !== undefined) {
      //   this.animal.set(result);
      // }
    });
  }


  getAllMagicBagsItem(element: any) {


  }
}
