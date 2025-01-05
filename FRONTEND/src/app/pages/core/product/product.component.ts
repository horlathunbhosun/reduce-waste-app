import { Component, OnInit } from '@angular/core';
import {MatTableModule} from "@angular/material/table";
import {Router, Routes} from "@angular/router";
import {ProductCreateComponent} from "./product-create/product-create.component";
import {MatDialog} from "@angular/material/dialog";
import {MatButton, MatFabButton} from "@angular/material/button";
import {CdkAccordion} from "@angular/cdk/accordion";
import {ProductService} from "../../../services/product.service";
import { MatMenuModule } from '@angular/material/menu';
import {MatIconModule} from '@angular/material/icon';
import {MatButtonModule} from '@angular/material/button';
import {ProductEditComponent} from "./product-edit/product-edit.component";
import {ToastrService} from "ngx-toastr";

// @ts-ignore
@Component({
  selector: 'app-product',
  standalone: true,
  imports: [MatTableModule, MatFabButton, MatButton, CdkAccordion, MatMenuModule,MatIconModule,MatButtonModule],
  templateUrl: './product.component.html',
  styleUrl: './product.component.scss'
})




export class ProductComponent implements OnInit {

  displayedColumns: string[] = ['name', 'description', 'action'];
  dataSource = [];

    constructor(private  route: Router , private dialog : MatDialog, private product :ProductService,
                private toastr: ToastrService
                ) {
    }

  ngOnInit(): void {
      this.getAllProducts();
  }



  openDialog(): void {
    const dialogRef = this.dialog.open(ProductCreateComponent, {

    });

    dialogRef.afterClosed().subscribe(result => {
      console.log('The dialog was closed');
      // if (result !== undefined) {
      //   this.animal.set(result);
      // }
    });
  }
  getAllProducts() {
    this.product.getAll().subscribe((res : any)  => {
      console.log(res.success.data)
      this.dataSource = res.success.data;
    })
  }

  editProduct(elements: any) {

    const dialogRef = this.dialog.open(ProductEditComponent, {
      data: elements
    });

    dialogRef.afterClosed().subscribe(result => {
      console.log('The dialog was closed');


      this.getAllProducts();
      // if (result !== undefined) {
      //   this.animal.set(result);
      // }
    });


  }

  deleteProduct(elements: any) {
    console.log("delete")
  }


}
