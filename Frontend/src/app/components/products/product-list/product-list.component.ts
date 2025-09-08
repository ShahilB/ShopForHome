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
  template: `
    <div class="products-container">
      <div class="container">
        <!-- Page Header -->
        <div class="page-header">
          <h1>{{ pageTitle }}</h1>
          @if (searchTerm) {
            <p>Search results for "{{ searchTerm }}"</p>
          }
        </div>

        <!-- Filters Section -->
        <div class="filters-section">
          <form [formGroup]="filterForm" class="filters-form">
            <!-- Search Input -->
            <div class="search-group">
              <input
                type="text"
                formControlName="searchTerm"
                placeholder="Search products..."
                class="search-input"
                (keyup.enter)="applyFilters()"
              />
              <button type="button" class="search-btn" (click)="applyFilters()">🔍</button>
            </div>

            <!-- Category Filter -->
            <div class="filter-group">
              <label>Category:</label>
              <select formControlName="categoryId" (change)="applyFilters()">
                <option value="">All Categories</option>
                @for (category of categories; track category.id) {
                  <option [value]="category.id">{{ category.name }}</option>
                }
              </select>
            </div>

            <!-- Price Range -->
            <div class="filter-group">
              <label>Price Range:</label>
              <div class="price-inputs">
                <input type="number" formControlName="minPrice" placeholder="Min" class="price-input" (change)="applyFilters()" />
                <span>-</span>
                <input type="number" formControlName="maxPrice" placeholder="Max" class="price-input" (change)="applyFilters()" />
              </div>
            </div>

            <!-- Rating Filter -->
            <div class="filter-group">
              <label>Rating:</label>
              <select formControlName="minRating" (change)="applyFilters()">
                <option value="">Any Rating</option>
                <option value="4">4+ Stars</option>
                <option value="3">3+ Stars</option>
                <option value="2">2+ Stars</option>
                <option value="1">1+ Stars</option>
              </select>
            </div>

            <!-- Additional Filters -->
            <div class="additional-filters">
              <input type="text" formControlName="brand" placeholder="Brand" class="filter-input" (keyup.enter)="applyFilters()" />
              <input type="text" formControlName="color" placeholder="Color" class="filter-input" (keyup.enter)="applyFilters()" />
            </div>

            <!-- Checkboxes -->
            <div class="checkbox-filters">
              <label class="checkbox-label">
                <input type="checkbox" formControlName="isOnSale" (change)="applyFilters()" />
                On Sale
              </label>
              <label class="checkbox-label">
                <input type="checkbox" formControlName="isFeatured" (change)="applyFilters()" />
                Featured
              </label>
              <label class="checkbox-label">
                <input type="checkbox" formControlName="inStock" (change)="applyFilters()" />
                In Stock
              </label>
            </div>

            <!-- Filter Actions -->
            <div class="filter-actions">
              <button type="button" class="btn btn-secondary" (click)="clearFilters()">Clear</button>
              <button type="button" class="btn btn-primary" (click)="applyFilters()">Apply</button>
            </div>
          </form>
        </div>

        <!-- Toolbar -->
        <div class="toolbar">
          <div class="results-info">
            @if (productResponse) {
              <span>{{ productResponse.totalCount }} products found</span>
            }
          </div>
          <div class="sort-options">
            <label>Sort:</label>
            <select [value]="currentSort" (change)="onSortChange($event)">
              <option value="name-asc">Name A-Z</option>
              <option value="name-desc">Name Z-A</option>
              <option value="price-asc">Price Low-High</option>
              <option value="price-desc">Price High-Low</option>
              <option value="rating-desc">Rating High-Low</option>
              <option value="created-desc">Newest</option>
            </select>
          </div>
        </div>

        <!-- Loading -->
        @if (isLoading) {
          <div class="loading">
            <div class="spinner"></div>
            <p>Loading products...</p>
          </div>
        }

        <!-- Products Grid -->
        @if (productResponse && productResponse.products.length > 0) {
          <div class="products-grid">
            @for (product of productResponse.products; track product.id) {
              <div class="product-card">
                <div class="product-image">
                  <img [src]="product.imageUrl || '/assets/images/placeholder.jpg'" [alt]="product.name" />
                  <!-- @if (product.isOnSale) {
                    <div class="sale-badge">{{ product.discountPercentage }}% OFF</div>
                  } -->
                  @if (product.isOutOfStock) {
                    <div class="stock-badge">Out of Stock</div>
                  }
                </div>

                <div class="product-info">
                  <h3>{{ product.name }}</h3>
                  <p class="category">{{ product.categoryName }}</p>
                  <div class="rating">
                    <span class="stars">{{ getStars(product.rating) }}</span>
                    <span>({{ product.reviewCount }})</span>
                  </div>
                  <div class="price">
                    <!-- @if (product.isOnSale) {
                      <span class="original">₹₹{{ product.price }}</span>
                      <span class="sale">₹{{ product.effectivePrice }}</span>
                    } @else { -->
                      <span class="current">₹{{ product.price }}</span>
                    <!-- } -->
                  </div>
                  <div class="actions">
                    @if (authService.isAuthenticated()) {
                      <button class="btn btn-primary" (click)="addToCart(product)" [disabled]="product.isOutOfStock">
                        {{ product.isOutOfStock ? 'Out of Stock' : 'Add to Cart' }}
                      </button>
                      <button class="btn-icon" (click)="toggleWishlist(product)" [class.active]="cartService.isInWishlist(product.id)">
                        ❤️
                      </button>
                    } @else {
                      <a routerLink="/login" class="btn btn-primary">Login to Buy</a>
                    }
                  </div>
                </div>
              </div>
            }
          </div>

          <!-- Pagination -->
          @if (productResponse.totalPages > 1) {
            <div class="pagination">
              <button [disabled]="!productResponse.hasPreviousPage" (click)="goToPage(productResponse.page - 1)">← Prev</button>
              @for (page of getPageNumbers(); track page) {
                <button [class.active]="page === productResponse.page" (click)="goToPage(page)">{{ page }}</button>
              }
              <button [disabled]="!productResponse.hasNextPage" (click)="goToPage(productResponse.page + 1)">Next →</button>
            </div>
          }
        } @else if (!isLoading) {
          <div class="no-results">
            <h2>No products found</h2>
            <p>Try adjusting your filters</p>
            <button class="btn btn-primary" (click)="clearFilters()">Clear Filters</button>
          </div>
        }
      </div>
    </div>
  `,
  styles: [`
    .products-container { min-height: 100vh; background: #f8f9fa; padding: 2rem 0; }
    .container { max-width: 1200px; margin: 0 auto; padding: 0 1rem; }
    .page-header { text-align: center; margin-bottom: 2rem; }
    .page-header h1 { margin: 0 0 0.5rem 0; color: #2c3e50; font-size: 2.5rem; }
    .filters-section { background: white; border-radius: 10px; padding: 2rem; margin-bottom: 2rem; box-shadow: 0 2px 4px rgba(0,0,0,0.1); }
    .filters-form { display: grid; grid-template-columns: 2fr 1fr 1fr 1fr; gap: 1rem; align-items: end; }
    .search-group { display: flex; border: 2px solid #ddd; border-radius: 5px; overflow: hidden; }
    .search-input { flex: 1; padding: 0.75rem; border: none; outline: none; }
    .search-btn { padding: 0.75rem 1rem; background: #e74c3c; color: white; border: none; cursor: pointer; }
    .filter-group { display: flex; flex-direction: column; }
    .filter-group label { margin-bottom: 0.5rem; color: #2c3e50; font-weight: 500; font-size: 0.9rem; }
    .filter-group select, .filter-input { padding: 0.75rem; border: 2px solid #ddd; border-radius: 5px; outline: none; }
    .price-inputs { display: flex; align-items: center; gap: 0.5rem; }
    .price-input { flex: 1; padding: 0.75rem; border: 2px solid #ddd; border-radius: 5px; outline: none; }
    .additional-filters { grid-column: 1 / -1; display: flex; gap: 1rem; margin-top: 1rem; }
    .checkbox-filters { grid-column: 1 / -1; display: flex; gap: 2rem; margin-top: 1rem; }
    .checkbox-label { display: flex; align-items: center; cursor: pointer; }
    .checkbox-label input { margin-right: 0.5rem; }
    .filter-actions { grid-column: 1 / -1; display: flex; justify-content: center; gap: 1rem; margin-top: 1rem; }
    .toolbar { display: flex; justify-content: space-between; align-items: center; margin-bottom: 2rem; padding: 1rem; background: white; border-radius: 5px; }
    .sort-options { display: flex; align-items: center; gap: 0.5rem; }
    .products-grid { display: grid; grid-template-columns: repeat(auto-fill, minmax(280px, 1fr)); gap: 2rem; }
    .product-card { background: white; border-radius: 10px; overflow: hidden; box-shadow: 0 2px 4px rgba(0,0,0,0.1); transition: transform 0.3s; }
    .product-card:hover { transform: translateY(-2px); }
    .product-image { position: relative; height: 250px; overflow: hidden; }
    .product-image img { width: 100%; height: 100%; object-fit: cover; }
    .sale-badge, .stock-badge { position: absolute; top: 10px; right: 10px; background: #e74c3c; color: white; padding: 0.5rem; border-radius: 5px; font-size: 0.8rem; font-weight: bold; }
    .product-info { padding: 1.5rem; }
    .product-info h3 { margin: 0 0 0.5rem 0; color: #2c3e50; }
    .category { color: #7f8c8d; font-size: 0.9rem; margin-bottom: 0.5rem; }
    .rating { display: flex; align-items: center; gap: 0.5rem; margin-bottom: 1rem; }
    .stars { color: #f39c12; }
    .price { margin-bottom: 1rem; }
    .original { text-decoration: line-through; color: #7f8c8d; margin-right: 0.5rem; }
    .sale { color: #e74c3c; font-weight: bold; }
    .current { color: #2c3e50; font-weight: bold; }
    .actions { display: flex; gap: 0.5rem; align-items: center; }
    .btn { padding: 0.75rem 1rem; border: none; border-radius: 5px; cursor: pointer; font-weight: 500; transition: all 0.3s; }
    .btn-primary { background: #e74c3c; color: white; flex: 1; }
    .btn-primary:hover:not(:disabled) { background: #c0392b; }
    .btn-secondary { background: #6c757d; color: white; }
    .btn:disabled { opacity: 0.6; cursor: not-allowed; }
    .btn-icon { background: none; border: 1px solid #ddd; border-radius: 50%; width: 40px; height: 40px; display: flex; align-items: center; justify-content: center; }
    .btn-icon.active { background: #e74c3c; color: white; border-color: #e74c3c; }
    .loading, .no-results { text-align: center; padding: 4rem 0; }
    .spinner { width: 40px; height: 40px; border: 4px solid #f3f3f3; border-top: 4px solid #e74c3c; border-radius: 50%; animation: spin 1s linear infinite; margin: 0 auto 1rem; }
    @keyframes spin { 0% { transform: rotate(0deg); } 100% { transform: rotate(360deg); } }
    .pagination { display: flex; justify-content: center; gap: 0.5rem; margin-top: 2rem; }
    .pagination button { padding: 0.75rem 1rem; border: 1px solid #ddd; background: white; cursor: pointer; border-radius: 4px; }
    .pagination button.active { background: #e74c3c; color: white; }
    .pagination button:disabled { opacity: 0.5; cursor: not-allowed; }
    @media (max-width: 768px) {
      .filters-form { grid-template-columns: 1fr; }
      .additional-filters { flex-direction: column; }
      .checkbox-filters { flex-direction: column; gap: 1rem; }
      .toolbar { flex-direction: column; gap: 1rem; }
      .products-grid { grid-template-columns: 1fr; }
    }
  `]
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