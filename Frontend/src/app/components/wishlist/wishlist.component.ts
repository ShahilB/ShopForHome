import { Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { CartService, AuthService } from '../../services';
import { WishlistItem } from '../../models';

@Component({
  selector: 'app-wishlist',
  standalone: true,
  imports: [CommonModule, RouterModule],
  template: `
    <div class="wishlist-container">
      <div class="container">
        <div class="wishlist-header">
          <h1>My Wishlist</h1>
          @if (cartService.wishlistItemCount() > 0) {
            <p>{{ cartService.wishlistItemCount() }} item(s) saved for later</p>
          }
        </div>

        @if (cartService.isLoading()) {
          <div class="loading">
            <div class="spinner"></div>
            <p>Loading your wishlist...</p>
          </div>
        } @else if (cartService.wishlistItemCount() === 0) {
          <!-- Empty Wishlist -->
          <div class="empty-wishlist">
            <div class="empty-icon">❤️</div>
            <h2>Your wishlist is empty</h2>
            <p>Save items you love to your wishlist and shop them later.</p>
            <a routerLink="/products" class="btn btn-primary">Start Shopping</a>
          </div>
        } @else {
          <!-- Wishlist Items -->
          <div class="wishlist-grid">
            @for (item of cartService.wishlistItems(); track item.id) {
              <div class="wishlist-item">
                <div class="item-image">
                  <img 
                    [src]="item.product.imageUrl || '/assets/images/placeholder.jpg'" 
                    [alt]="item.product.name"
                  />
                  @if (item.product.isOnSale) {
                    <div class="sale-badge">{{ item.product.discountPercentage }}% OFF</div>
                  }
                  @if (item.product.isOutOfStock) {
                    <div class="stock-badge out-of-stock">Out of Stock</div>
                  } @else if (item.product.isLowStock) {
                    <div class="stock-badge low-stock">Low Stock</div>
                  }
                </div>
                
                <div class="item-content">
                  <div class="item-details">
                    <h3>{{ item.product.name }}</h3>
                    <p class="item-category">{{ item.product.categoryName }}</p>
                    <p class="item-sku">SKU: {{ item.product.sku }}</p>
                    
                    @if (item.product.description) {
                      <p class="item-description">{{ item.product.description | slice:0:100 }}...</p>
                    }
                    
                    <div class="item-rating">
                      <span class="stars">{{ getStars(item.product.rating) }}</span>
                      <span class="rating-text">({{ item.product.reviewCount }})</span>
                    </div>
                  </div>
                  
                  <div class="item-price">
                    @if (item.product.isOnSale) {
                      <span class="original-price">\${{ item.product.price }}</span>
                      <span class="sale-price">\${{ item.product.effectivePrice }}</span>
                    } @else {
                      <span class="price">\${{ item.product.price }}</span>
                    }
                  </div>
                  
                  <div class="item-actions">
                    <button 
                      class="btn btn-primary"
                      (click)="moveToCart(item)"
                      [disabled]="isUpdating || item.product.isOutOfStock"
                    >
                      @if (isUpdating) {
                        <span class="spinner-small"></span>
                      }
                      {{ item.product.isOutOfStock ? 'Out of Stock' : 'Add to Cart' }}
                    </button>
                    
                    <button 
                      class="btn btn-outline"
                      [routerLink]="['/products', item.product.id]"
                    >
                      View Details
                    </button>
                    
                    <button 
                      class="btn-icon remove-btn"
                      (click)="removeFromWishlist(item)"
                      [disabled]="isUpdating"
                      title="Remove from Wishlist"
                    >
                      🗑️
                    </button>
                  </div>
                  
                  <div class="item-meta">
                    <small>Added on {{ formatDate(item.createdAt) }}</small>
                  </div>
                </div>
              </div>
            }
          </div>

          <!-- Wishlist Actions -->
          <div class="wishlist-actions">
            <button 
              class="btn btn-secondary"
              (click)="clearWishlist()"
              [disabled]="isUpdating"
            >
              Clear Wishlist
            </button>
            
            <button 
              class="btn btn-primary"
              (click)="addAllToCart()"
              [disabled]="isUpdating || !hasAvailableItems()"
            >
              Add All Available to Cart
            </button>
          </div>
        }

        <!-- Continue Shopping -->
        <div class="continue-shopping">
          <a routerLink="/products" class="continue-link">
            ← Continue Shopping
          </a>
        </div>
      </div>
    </div>
  `,
  styles: [`
    .wishlist-container {
      min-height: 100vh;
      background: #f8f9fa;
      padding: 2rem 0;
    }

    .container {
      max-width: 1200px;
      margin: 0 auto;
      padding: 0 1rem;
    }

    .wishlist-header {
      text-align: center;
      margin-bottom: 3rem;
    }

    .wishlist-header h1 {
      margin: 0 0 0.5rem 0;
      color: #2c3e50;
      font-size: 2.5rem;
    }

    .wishlist-header p {
      margin: 0;
      color: #7f8c8d;
      font-size: 1.1rem;
    }

    .loading {
      text-align: center;
      padding: 4rem 0;
    }

    .spinner {
      width: 40px;
      height: 40px;
      border: 4px solid #f3f3f3;
      border-top: 4px solid #e74c3c;
      border-radius: 50%;
      animation: spin 1s linear infinite;
      margin: 0 auto 1rem;
    }

    .spinner-small {
      width: 16px;
      height: 16px;
      border: 2px solid transparent;
      border-top: 2px solid currentColor;
      border-radius: 50%;
      animation: spin 1s linear infinite;
      display: inline-block;
    }

    @keyframes spin {
      0% { transform: rotate(0deg); }
      100% { transform: rotate(360deg); }
    }

    .empty-wishlist {
      text-align: center;
      padding: 4rem 0;
    }

    .empty-icon {
      font-size: 4rem;
      margin-bottom: 1rem;
    }

    .empty-wishlist h2 {
      margin: 0 0 1rem 0;
      color: #2c3e50;
    }

    .empty-wishlist p {
      margin: 0 0 2rem 0;
      color: #7f8c8d;
      font-size: 1.1rem;
    }

    .wishlist-grid {
      display: grid;
      grid-template-columns: repeat(auto-fill, minmax(350px, 1fr));
      gap: 2rem;
      margin-bottom: 3rem;
    }

    .wishlist-item {
      background: white;
      border-radius: 10px;
      overflow: hidden;
      box-shadow: 0 2px 4px rgba(0,0,0,0.1);
      transition: transform 0.3s, box-shadow 0.3s;
    }

    .wishlist-item:hover {
      transform: translateY(-2px);
      box-shadow: 0 4px 8px rgba(0,0,0,0.15);
    }

    .item-image {
      position: relative;
      height: 250px;
      overflow: hidden;
    }

    .item-image img {
      width: 100%;
      height: 100%;
      object-fit: cover;
    }

    .sale-badge {
      position: absolute;
      top: 10px;
      right: 10px;
      background: #e74c3c;
      color: white;
      padding: 0.5rem;
      border-radius: 5px;
      font-size: 0.8rem;
      font-weight: bold;
    }

    .stock-badge {
      position: absolute;
      bottom: 10px;
      left: 10px;
      padding: 0.5rem;
      border-radius: 5px;
      font-size: 0.8rem;
      font-weight: bold;
    }

    .out-of-stock {
      background: #e74c3c;
      color: white;
    }

    .low-stock {
      background: #f39c12;
      color: white;
    }

    .item-content {
      padding: 1.5rem;
    }

    .item-details h3 {
      margin: 0 0 0.5rem 0;
      color: #2c3e50;
      font-size: 1.2rem;
      font-weight: 600;
    }

    .item-category,
    .item-sku {
      margin: 0 0 0.25rem 0;
      color: #7f8c8d;
      font-size: 0.9rem;
    }

    .item-description {
      margin: 0.5rem 0;
      color: #5a6c7d;
      font-size: 0.9rem;
      line-height: 1.4;
    }

    .item-rating {
      display: flex;
      align-items: center;
      gap: 0.5rem;
      margin: 0.5rem 0 1rem 0;
    }

    .stars {
      color: #f39c12;
    }

    .rating-text {
      color: #7f8c8d;
      font-size: 0.9rem;
    }

    .item-price {
      display: flex;
      align-items: center;
      gap: 0.5rem;
      margin-bottom: 1.5rem;
    }

    .price {
      font-size: 1.3rem;
      font-weight: bold;
      color: #2c3e50;
    }

    .original-price {
      text-decoration: line-through;
      color: #7f8c8d;
    }

    .sale-price {
      font-size: 1.3rem;
      font-weight: bold;
      color: #e74c3c;
    }

    .item-actions {
      display: flex;
      flex-direction: column;
      gap: 0.75rem;
    }

    .item-meta {
      margin-top: 1rem;
      padding-top: 1rem;
      border-top: 1px solid #eee;
      color: #7f8c8d;
      font-size: 0.8rem;
    }

    .btn {
      padding: 0.75rem 1.5rem;
      border: none;
      border-radius: 5px;
      cursor: pointer;
      font-size: 1rem;
      font-weight: 500;
      text-decoration: none;
      transition: all 0.3s;
      text-align: center;
      display: inline-flex;
      align-items: center;
      justify-content: center;
      gap: 0.5rem;
    }

    .btn-primary {
      background: #e74c3c;
      color: white;
    }

    .btn-primary:hover:not(:disabled) {
      background: #c0392b;
    }

    .btn-secondary {
      background: #6c757d;
      color: white;
    }

    .btn-secondary:hover:not(:disabled) {
      background: #5a6268;
    }

    .btn-outline {
      background: transparent;
      color: #e74c3c;
      border: 2px solid #e74c3c;
    }

    .btn-outline:hover:not(:disabled) {
      background: #e74c3c;
      color: white;
    }

    .btn:disabled {
      opacity: 0.6;
      cursor: not-allowed;
    }

    .btn-icon {
      background: none;
      border: 1px solid #ddd;
      border-radius: 4px;
      padding: 0.5rem;
      cursor: pointer;
      font-size: 1.2rem;
      transition: all 0.3s;
      width: 40px;
      height: 40px;
      display: flex;
      align-items: center;
      justify-content: center;
    }

    .btn-icon:hover:not(:disabled) {
      border-color: #e74c3c;
      background: #fdf2f2;
    }

    .btn-icon:disabled {
      opacity: 0.5;
      cursor: not-allowed;
    }

    .wishlist-actions {
      display: flex;
      justify-content: center;
      gap: 1rem;
      margin-bottom: 3rem;
    }

    .continue-shopping {
      margin-top: 3rem;
      text-align: center;
    }

    .continue-link {
      color: #e74c3c;
      text-decoration: none;
      font-size: 1.1rem;
      font-weight: 500;
    }

    .continue-link:hover {
      text-decoration: underline;
    }

    /* Responsive */
    @media (max-width: 768px) {
      .wishlist-grid {
        grid-template-columns: 1fr;
      }

      .wishlist-actions {
        flex-direction: column;
        align-items: center;
      }

      .wishlist-actions .btn {
        width: 100%;
        max-width: 300px;
      }
    }

    @media (max-width: 480px) {
      .item-content {
        padding: 1rem;
      }

      .wishlist-header h1 {
        font-size: 2rem;
      }
    }
  `]
})
export class WishlistComponent implements OnInit {
  cartService = inject(CartService);
  authService = inject(AuthService);

  isUpdating = false;
  errorMessage = '';

  ngOnInit(): void {
    // Cart service automatically loads wishlist items
  }

  moveToCart(item: WishlistItem): void {
    this.isUpdating = true;
    this.cartService.moveToCart(item.id, 1).subscribe({
      next: () => {
        this.isUpdating = false;
      },
      error: (error) => {
        this.isUpdating = false;
        this.errorMessage = 'Failed to move item to cart';
        console.error('Error moving to cart:', error);
      }
    });
  }

  removeFromWishlist(item: WishlistItem): void {
    if (confirm(`Remove "${item.product.name}" from your wishlist?`)) {
      this.isUpdating = true;
      this.cartService.removeFromWishlist(item.id).subscribe({
        next: () => {
          this.isUpdating = false;
        },
        error: (error) => {
          this.isUpdating = false;
          this.errorMessage = 'Failed to remove item';
          console.error('Error removing from wishlist:', error);
        }
      });
    }
  }

  clearWishlist(): void {
    if (confirm('Are you sure you want to clear your entire wishlist?')) {
      this.isUpdating = true;
      const items = this.cartService.wishlistItems();
      
      // Remove all items one by one
      let completed = 0;
      items.forEach(item => {
        this.cartService.removeFromWishlist(item.id).subscribe({
          next: () => {
            completed++;
            if (completed === items.length) {
              this.isUpdating = false;
            }
          },
          error: (error) => {
            completed++;
            if (completed === items.length) {
              this.isUpdating = false;
            }
            console.error('Error removing item:', error);
          }
        });
      });
    }
  }

  addAllToCart(): void {
    const availableItems = this.cartService.wishlistItems()
      .filter(item => !item.product.isOutOfStock);

    if (availableItems.length === 0) {
      alert('No available items to add to cart');
      return;
    }

    if (confirm(`Add ${availableItems.length} available item(s) to your cart?`)) {
      this.isUpdating = true;
      let completed = 0;
      let errors = 0;

      availableItems.forEach(item => {
        this.cartService.moveToCart(item.id, 1).subscribe({
          next: () => {
            completed++;
            if (completed + errors === availableItems.length) {
              this.isUpdating = false;
              if (errors === 0) {
                alert('All items added to cart successfully!');
              } else {
                alert(`${completed} items added to cart, ${errors} failed.`);
              }
            }
          },
          error: (error) => {
            errors++;
            if (completed + errors === availableItems.length) {
              this.isUpdating = false;
              alert(`${completed} items added to cart, ${errors} failed.`);
            }
            console.error('Error adding to cart:', error);
          }
        });
      });
    }
  }

  hasAvailableItems(): boolean {
    return this.cartService.wishlistItems()
      .some(item => !item.product.isOutOfStock);
  }

  getStars(rating: number): string {
    const fullStars = Math.floor(rating);
    const hasHalfStar = rating % 1 !== 0;
    let stars = '★'.repeat(fullStars);
    
    if (hasHalfStar) {
      stars += '☆';
    }
    
    const emptyStars = 5 - Math.ceil(rating);
    stars += '☆'.repeat(emptyStars);
    
    return stars;
  }

  formatDate(dateString: string): string {
    const date = new Date(dateString);
    return date.toLocaleDateString('en-US', { 
      year: 'numeric', 
      month: 'short', 
      day: 'numeric' 
    });
  }
}