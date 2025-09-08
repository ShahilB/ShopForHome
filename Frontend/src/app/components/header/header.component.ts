import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, Router } from '@angular/router';
import { AuthService, CartService } from '../../services';

@Component({
  selector: 'app-header',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './header.component.html',
  styleUrl: './header.component.css'
})
export class HeaderComponent {
  authService = inject(AuthService);
  cartService = inject(CartService);
  
  showMobileMenu = false;

  onSearch(searchTerm: string): void {
    if (searchTerm.trim()) {
      // Navigate to products page with search term
      this.router.navigate(['/products'], {
        queryParams: { searchTerm: searchTerm.trim() }
      });
      this.closeMobileMenu();
    }
  }

  private router = inject(Router);

  logout(): void {
    this.authService.logout();
    this.closeMobileMenu();
  }

  toggleMobileMenu(): void {
    this.showMobileMenu = !this.showMobileMenu;
  }

  closeMobileMenu(): void {
    this.showMobileMenu = false;
  }
}