import { Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, ActivatedRoute, Router } from '@angular/router';
import { ReactiveFormsModule, FormBuilder, FormGroup } from '@angular/forms';
import { ProductService, CartService, AuthService } from '../../../services';
import { Product, Category, ProductFilter, ProductListResponse } from '../../../models';

@Component({
  selector: 'app-product-list',
  standalone: true,
  imports: [CommonModule, RouterModule, ReactiveFormsModule],
  templateUrl: './product-list.component.html',
  styleUrls: ['./product-list.component.css']
})
export class ProductListComponent implements OnInit {
  private productService = inject(ProductService);
  private route = inject(ActivatedRoute);
  private router = inject(Router);
  private fb = inject(FormBuilder);
  
  cartService = inject(CartService);
  authService = inject(AuthService);

  filterForm: FormGroup;
  productResponse: ProductListResponse | null = null;
  categories: Category[] = [];
  searchTerm = '';
  pageTitle = 'Products';
  currentSort = 'name-asc';
  isLoading = false;
  isUpdating = false;

  constructor() {
    this.filterForm = this.fb.group({
      searchTerm: [''],
      categoryId: [''],
      minPrice: [''],
      maxPrice: [''],
      minRating: [''],
      brand: [''],
      color: [''],
      isOnSale: [false],
      isFeatured: [false],
      inStock: [false]
    });
  }

  ngOnInit(): void {
    this.loadCategories();
    this.setupRouteSubscription();
  }

  private setupRouteSubscription(): void {
    this.route.queryParams.subscribe(params => {
      this.filterForm.patchValue({
        searchTerm: params['searchTerm'] || '',
        categoryId: params['categoryId'] || '',
        minPrice: params['minPrice'] || '',
        maxPrice: params['maxPrice'] || '',
        minRating: params['minRating'] || '',
        brand: params['brand'] || '',
        color: params['color'] || '',
        isOnSale: params['isOnSale'] === 'true',
        isFeatured: params['isFeatured'] === 'true',
        inStock: params['inStock'] === 'true'
      });

      this.searchTerm = params['searchTerm'] || '';
      this.currentSort = params['sortBy'] && params['sortOrder'] ? 
        `${params['sortBy']}-${params['sortOrder']}` : 'name-asc';

      this.loadProducts();
    });
  }

  private loadCategories(): void {
    this.productService.getCategories().subscribe({
      next: (categories) => this.categories = categories,
      error: (error) => console.error('Error loading categories:', error)
    });
  }

  private loadProducts(page: number = 1): void {
    this.isLoading = true;
    
    const formValue = this.filterForm.value;
    const [sortBy, sortOrder] = this.currentSort.split('-');
    
    const filter: ProductFilter = {
      ...formValue,
      sortBy,
      sortOrder,
      page,
      pageSize: 20
    };

    // Remove empty values
    Object.keys(filter).forEach(key => {
      const value = (filter as any)[key];
      if (value === '' || value === null || value === undefined || value === false) {
        delete (filter as any)[key];
      }
    });

    this.productService.getProducts(filter).subscribe({
      next: (response) => {
        this.productResponse = response;
        this.isLoading = false;
      },
      error: (error) => {
        console.error('Error loading products:', error);
        this.isLoading = false;
      }
    });
  }

  applyFilters(): void {
    const formValue = this.filterForm.value;
    const [sortBy, sortOrder] = this.currentSort.split('-');
    
    const queryParams: any = { ...formValue, sortBy, sortOrder };

    // Remove empty/false values
    Object.keys(queryParams).forEach(key => {
      if (queryParams[key] === '' || queryParams[key] === null || queryParams[key] === false) {
        delete queryParams[key];
      }
    });

    this.router.navigate([], {
      relativeTo: this.route,
      queryParams,
      queryParamsHandling: 'replace'
    });
  }

  clearFilters(): void {
    this.filterForm.reset();
    this.currentSort = 'name-asc';
    this.router.navigate([], {
      relativeTo: this.route,
      queryParams: {},
      queryParamsHandling: 'replace'
    });
  }

  onSortChange(event: any): void {
    this.currentSort = event.target.value;
    this.applyFilters();
  }

  goToPage(page: number): void {
    this.loadProducts(page);
  }

  addToCart(product: Product): void {
    if (!this.authService.isAuthenticated()) return;
    
    this.isUpdating = true;
    this.cartService.quickAddToCart(product.id, 1).subscribe({
      next: () => {
        this.isUpdating = false;
        // Show success message
      },
      error: (error) => {
        this.isUpdating = false;
        console.error('Error adding to cart:', error);
      }
    });
  }

  toggleWishlist(product: Product): void {
    if (!this.authService.isAuthenticated()) return;
    
    this.cartService.toggleWishlist(product.id).subscribe({
      next: () => {
        // Success handled by service
      },
      error: (error) => {
        console.error('Error toggling wishlist:', error);
      }
    });
  }

  getStars(rating: number): string {
    const fullStars = Math.floor(rating);
    const hasHalfStar = rating % 1 !== 0;
    let stars = '★'.repeat(fullStars);
    if (hasHalfStar) stars += '☆';
    const emptyStars = 5 - Math.ceil(rating);
    stars += '☆'.repeat(emptyStars);
    return stars;
  }

  getPageNumbers(): number[] {
    if (!this.productResponse) return [];
    
    const current = this.productResponse.page;
    const total = this.productResponse.totalPages;
    const pages: number[] = [];
    
    const start = Math.max(1, current - 2);
    const end = Math.min(total, current + 2);
    
    for (let i = start; i <= end; i++) {
      pages.push(i);
    }
    
    return pages;
  }
}