import { Routes } from '@angular/router';

import { AppSideLoginComponent } from './side-login/side-login.component';
import { AppSideRegisterComponent } from './side-register/side-register.component';

export const AuthenticationRoutes: Routes = [

    {
      path: '',
      component: AppSideLoginComponent,
    },

    {
      path: 'login',
      component: AppSideLoginComponent,
    },
    {
      path: 'register',
      component: AppSideRegisterComponent,
    },
  // {
  //   path: '',
  //   redirectTo: 'login',
  //   pathMatch: 'full',
  //
  //   // children: [
  //   //   {
  //   //     path: 'login',
  //   //     component: AppSideLoginComponent,
  //   //   },
  //   //   {
  //   //     path: 'register',
  //   //     component: AppSideRegisterComponent,
  //   //   },
  //   // ],
  // },
];
