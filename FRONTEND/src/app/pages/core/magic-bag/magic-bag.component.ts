import {Component, OnInit} from '@angular/core';
import {Router} from "@angular/router";
import {MatDialog} from "@angular/material/dialog";
import {ToastrService} from "ngx-toastr";
import {MagicBagCreateComponent} from "./magic-bag-create/magic-bag-create.component";
import {
  MatCell,
  MatCellDef,
  MatColumnDef,
  MatHeaderCell,
  MatHeaderRow,
  MatHeaderRowDef,
  MatRow, MatRowDef, MatTable, MatTableModule
} from "@angular/material/table";
import {MatButton, MatButtonModule, MatFabButton, MatIconButton} from "@angular/material/button";
import {MatIcon, MatIconModule} from "@angular/material/icon";
import {MatMenu, MatMenuItem, MatMenuModule, MatMenuTrigger} from "@angular/material/menu";
import {MagicbagService} from "../../../services/magicbag.service";
import {DatePipe, NgIf} from "@angular/common";
import {CdkAccordion} from "@angular/cdk/accordion";
import {MagicBagEditComponent} from "./magic-bag-edit/magic-bag-edit.component";
import {MagicBagDeleteComponent} from "./magic-bag-delete/magic-bag-delete.component";

@Component({
  selector: 'app-magic-bag',
  standalone: true,
  imports: [
    MatCell,
    MatCellDef,
    MatColumnDef,
    MatFabButton,
    MatHeaderCell,
    MatHeaderRow,
    MatHeaderRowDef,
    MatIcon,
    MatIconButton,
    MatMenu,
    MatMenuItem,
    MatRow,
    MatRowDef,
    MatTable,
    MatMenuTrigger,
    NgIf,
    MatTableModule, MatFabButton, MatButton, CdkAccordion, MatMenuModule, MatIconModule, MatButtonModule, DatePipe
  ],
  templateUrl: './magic-bag.component.html',
  styleUrl: './magic-bag.component.scss'
})
export class MagicBagComponent  implements OnInit {

  displayedColumns: string[] = ['name', 'description','price', 'status', 'created','action'];
  dataSourcer: any = [];

  constructor(private  router: Router , private dialog : MatDialog,
              private toastr: ToastrService, private magicbag : MagicbagService
  ) {
  }



  ngOnInit(): void {
    this.getAllMagicBags();
  }


  openDialog(): void {
    const dialogRef = this.dialog.open(MagicBagCreateComponent, {

    });

    dialogRef.afterClosed().subscribe(result => {
      console.log('The dialog was closed');
      // if (result !== undefined) {
      //   this.animal.set(result);
      // }
    });
  }


  getAllMagicBags() {
    let usertype = JSON.parse(localStorage.getItem('user') || '{}');
    if (usertype?.userType === 'Partner') {
      this.magicbag.getPartnerMagicBag().subscribe((res : any)  => {
        console.log("{ partner}", res.success.data)
        this.dataSourcer = res.success.data;

        console.log("source",  this.dataSourcer)

      })
    }else {
      this.magicbag.getPartnerMagicBagAll().subscribe((res : any)  => {
        console.log(res)
        this.dataSourcer = res.success.data;
        // console.log("source", this.dataSourcer)

      })

    }

  }

  editMagicBag(elements: any) {

    const dialogRef = this.dialog.open(MagicBagEditComponent, {
      data: elements
    });

    dialogRef.afterClosed().subscribe(result => {
      console.log('The dialog was closed');
      this.getAllMagicBags();
    });
  }

  deleteMagicBag(elements: any) {
    const dialogRef = this.dialog.open(MagicBagDeleteComponent, {
      data: elements
    });

    dialogRef.afterClosed().subscribe(result => {
      console.log('The dialog was closed');

      this.getAllMagicBags();

    });

  }

  openMagicBagProductItem(elements: any) {
    this.router.navigate([`/core/magic-bag/${elements.id}/item`]);
  }

}
