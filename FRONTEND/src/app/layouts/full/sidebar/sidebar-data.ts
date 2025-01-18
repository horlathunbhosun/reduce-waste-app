import { NavItem } from './nav-item/nav-item';






export const navItems: NavItem[] = [
  {
    navCap: 'Home',
    roles: ['Admin','Partner', "User"],

  },
  {
    displayName: 'Dashboard',
    iconName: 'layout-dashboard',
    bgcolor: 'primary',
    route: '/dashboard',
    roles: ['Admin','Partner', "User"],

  },
  {
    navCap: 'Core ',
    roles: ['Admin','Partner', "User"],

  },

  {
    displayName: 'Products Waste',
    iconName: 'brand-producthunt',
    bgcolor: 'accent',
    route: '/core/product',
    roles: ['User'],
  },


  {
    displayName: 'Products',
    iconName: 'brand-producthunt',
    bgcolor: 'accent',
    route: '/core/product',
    roles: ['Admin','Partner'],
  },

  {
    displayName: 'Magic Bags',
    iconName: 'brand-producthunt',
    bgcolor: 'accent',
    route: '/core/magic-bag',
    roles: ['Admin','Partner'],
  },

  {
    displayName: 'Transactions',
    iconName: 'brand-producthunt',
    bgcolor: 'accent',
    route: '/core/transactions',
    roles: ['Admin','Partner', 'User'],
  },

  {
    displayName: 'Badge',
    iconName: 'rosette',
    bgcolor: 'accent',
    route: '/ui-components/badge',
  },
  {
    displayName: 'Chips',
    iconName: 'poker-chip',
    bgcolor: 'warning',
    route: '/ui-components/chips',
  },
  {
    displayName: 'Lists',
    iconName: 'list',
    bgcolor: 'success',
    route: '/ui-components/lists',
  },
  {
    displayName: 'Menu',
    iconName: 'layout-navbar-expand',
    bgcolor: 'error',
    route: '/ui-components/menu',
  },
  {
    displayName: 'Tooltips',
    iconName: 'tooltip',
    bgcolor: 'primary',
    route: '/ui-components/tooltips',
  },
  {
    navCap: 'Auth',
  },
  {
    displayName: 'Login',
    iconName: 'lock',
    bgcolor: 'accent',
    route: '/authentication/login',
  },
  {
    displayName: 'Register',
    iconName: 'user-plus',
    bgcolor: 'warning',
    route: '/authentication/register',
  },
  {
    navCap: 'Extra',
  },
  {
    displayName: 'Icons',
    iconName: 'mood-smile',
    bgcolor: 'success',
    route: '/extra/icons',
  },
  {
    displayName: 'Sample Page',
    iconName: 'aperture',
    bgcolor: 'error',
    route: '/extra/sample-page',
  },
];
