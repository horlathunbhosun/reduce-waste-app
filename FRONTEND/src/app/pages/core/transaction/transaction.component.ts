import {Component, OnInit} from '@angular/core';
import {Router} from "@angular/router";
import {MatDialog} from "@angular/material/dialog";
import {ProductService} from "../../../services/product.service";
import {ToastrService} from "ngx-toastr";
import {MagicbagService} from "../../../services/magicbag.service";
import {
  MatCell,
  MatCellDef,
  MatColumnDef,
  MatHeaderCell,
  MatHeaderRow,
  MatHeaderRowDef,
  MatRow, MatRowDef, MatTable, MatTableModule
} from "@angular/material/table";
import {MatIcon, MatIconModule} from "@angular/material/icon";
import {MatButton, MatButtonModule, MatFabButton, MatIconButton} from "@angular/material/button";
import {MatMenu, MatMenuItem, MatMenuModule, MatMenuTrigger} from "@angular/material/menu";
import {DatePipe, NgIf} from "@angular/common";
import {CdkAccordion} from "@angular/cdk/accordion";
import {TransactionService} from "../../../services/transaction.service";

@Component({
  selector: 'app-transaction',
  standalone: true,
  imports: [
    MatCell,
    MatCellDef,
    MatColumnDef,
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
    NgIf,
    DatePipe,
    MatMenuTrigger,
    MatTableModule, MatFabButton, MatButton, CdkAccordion, MatMenuModule, MatIconModule, MatButtonModule

  ],
  templateUrl: './transaction.component.html',
  styleUrl: './transaction.component.scss'
})
export class TransactionComponent  implements OnInit  {

  displayedColumns: string[] = ['magicbag', 'transactionDate','amount', 'pickUpDate', 'status','action'];
  dataSource = [];

  constructor(private  route: Router , private dialog : MatDialog, private transaction :TransactionService,
              private toastr: ToastrService
    , private magicbag : MagicbagService
  ) {
  }

  ngOnInit(): void {
    this.getAllTransaction();
  }

  getAllTransaction() {

    let usertype = JSON.parse(localStorage.getItem('user') || '{}');
    if (usertype?.userType === 'Partner' || usertype?.userType === 'Admin') {
      this.transaction.getAll().subscribe((res : any)  => {
        console.log(res.success.data)
       this.dataSource = res.success.data;

       // console.log("source",  this.dataSourcer)

      })
    }else {
      this.transaction.getUserTransactions().subscribe((res : any)  => {
        console.log(res)
       this.dataSource = res.success.data;
        // console.log("source", this.dataSourcer)

      })

    }
  }
}
