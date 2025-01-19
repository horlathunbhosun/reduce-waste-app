import {Component, OnInit} from '@angular/core';
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
import {Router} from "@angular/router";
import {MatDialog} from "@angular/material/dialog";
import {ProductService} from "../../../services/product.service";
import {ToastrService} from "ngx-toastr";
import {ProductEditComponent} from "../product/product-edit/product-edit.component";
import {ProductDeleteComponent} from "../product/product-delete/product-delete.component";
import {MagicbagService} from "../../../services/magicbag.service";
import {DatePipe, NgForOf, NgIf} from "@angular/common";
import {CdkAccordion} from "@angular/cdk/accordion";
import {
  MatCard,
  MatCardActions,
  MatCardContent,
  MatCardSubtitle,
  MatCardTitle,
  MatCardTitleGroup
} from "@angular/material/card";
import {TransactionService} from "../../../services/transaction.service";

interface Product {
  name: string;
  description: string;
  bagPrice: number;
  status?: string;
}

@Component({
  selector: 'app-product-waste',
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
    DatePipe,
    NgIf,
    MatMenuTrigger,
    MatTableModule, MatFabButton, MatButton, CdkAccordion, MatMenuModule, MatIconModule, MatButtonModule, MatCardActions, MatCardContent, MatCardSubtitle, MatCardTitle, MatCardTitleGroup, MatCard, NgForOf
  ],
  templateUrl: './product-waste.component.html',
  styleUrl: './product-waste.component.scss'
})

export class ProductWasteComponent  implements OnInit {


  displayedColumns: string[] = ['name', 'description','price', 'status','action'];
  dataSource = [];
  dataSourcer: Product[] = [];

  constructor(private  route: Router , private dialog : MatDialog, private transactionService :TransactionService,
              private toastr: ToastrService
              , private magicbag : MagicbagService
  ) {
  }

  ngOnInit(): void {
    this.getAllProducts();
  }



  getAllProducts() {
    this.magicbag.getPartnerMagicBagAll().subscribe((res : any)  => {
      console.log(res.success.data)
      this.dataSource = res.success.data;
      this.dataSourcer = res.success.data;
    })
  }

  buyProduct(elements: any) {
    const payload = {
      magicBagId: elements.id
    }


    this.transactionService.create(payload).subscribe((res : any)  => {
      console.log(res.success.data.url)
      window.open(res.success.data.url, "_blank");
      this.toastr.success('Product bought successfully');
      this.getAllProducts();
    })

    console.log(elements);
  }


}
