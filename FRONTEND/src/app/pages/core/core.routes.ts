import { Routes } from '@angular/router';
import {ProductComponent} from "./product/product.component";
import {MagicBagComponent} from "./magic-bag/magic-bag.component";
import {MagicBagProductItemComponent} from "./magic-bag-product-item/magic-bag-product-item.component";
import {TransactionComponent} from "./transaction/transaction.component";
import {ProductWasteComponent} from "./product-waste/product-waste.component";
import {SuccessComponent} from "./success/success.component";
import {FailedComponent} from "./failed/failed.component";

// ui

export const CoreRoutes: Routes = [
  {
    path: '',
    children: [
      // {
      //   path: 'badge',
      //   component: AppBadgeComponent,
      // },
      {
        path: 'product',
        component: ProductComponent,
      },

      {
        path: 'magic-bag',
        component: MagicBagComponent
      },
      {
        path: 'magic-bag/:id/item',
        component: MagicBagProductItemComponent
      },
      {
        path: 'transactions',
        component: TransactionComponent
      },
      {
        path: 'product-item',
        component: ProductWasteComponent
      },
      {

        path: 'success',
        component: SuccessComponent
      },
      {
        path: 'cancel',
        component: FailedComponent
      }


    ],
  },
];
