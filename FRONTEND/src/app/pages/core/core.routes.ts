import { Routes } from '@angular/router';
import {ProductComponent} from "./product/product.component";
import {MagicBagComponent} from "./magic-bag/magic-bag.component";

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
      }
      //
      // {
      //   path: 'magic-bag/:id',
      //   component: MagicBagComponent
      // }

    ],
  },
];
