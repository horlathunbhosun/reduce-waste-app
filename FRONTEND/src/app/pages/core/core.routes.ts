import { Routes } from '@angular/router';
import {ProductComponent} from "./product/product.component";

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

    ],
  },
];
