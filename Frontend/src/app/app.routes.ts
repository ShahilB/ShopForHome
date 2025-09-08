import { Routes } from '@angular/router';
import { HomeComponent } from './components/home/home.component';

export const routes: Routes = [
  { path: '', component: HomeComponent },
  { path: 'home', redirectTo: '', pathMatch: 'full' },
  
  // Auth routes
  { path: 'login', loadComponent: () => import('./components/auth/login/login.component').then(m => m.LoginComponent) },
  { path: 'register', loadComponent: () => import('./components/auth/register/register.component').then(m => m.RegisterComponent) },
  
  // Cart and Wishlist routes
  { path: 'cart', loadComponent: () => import('./components/cart/cart.component').then(m => m.CartComponent) },
  { path: 'wishlist', loadComponent: () => import('./components/wishlist/wishlist.component').then(m => m.WishlistComponent) },
  
  // Product routes
  { path: 'products', loadComponent: () => import('./components/products/product-list/product-list.component').then(m => m.ProductListComponent) },
  // { path: 'products/:id', loadComponent: () => import('./components/products/product-detail/product-detail.component').then(m => m.ProductDetailComponent) },
  // { path: 'categories', loadComponent: () => import('./components/categories/category-list/category-list.component').then(m => m.CategoryListComponent) },
  // { path: 'profile', loadComponent: () => import('./components/user/profile/profile.component').then(m => m.ProfileComponent) },
  // { path: 'orders', loadComponent: () => import('./components/orders/order-list/order-list.component').then(m => m.OrderListComponent) },
  // { path: 'admin', loadComponent: () => import('./components/admin/dashboard/admin-dashboard.component').then(m => m.AdminDashboardComponent) },
  
  // Fallback route
  { path: '**', redirectTo: '', pathMatch: 'full' }
];
