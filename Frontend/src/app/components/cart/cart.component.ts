import { Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { CartService, AuthService } from '../../services';
import { CartItem, CartSummary } from '../../models';

@Component({
  selector: 'app-cart',
  standalone: true,
  imports: [CommonModule, RouterModule],
  template: `
    <div class="cart-container">
      <div class="container">
        <div class="cart-header">
          <h1>Shopping Cart</h1>
          @if (cartService.cartItemCount() > 0) {
            <p>{{ cartService.cartItemCount() }} item(s) in your cart</p>
          }
        </div>

        @if (cartService.isLoading()) {
          <div class="loading">
            <div class="spinner"></div>
            <p>Loading your cart...</p>
          </div>
        } @else if (cartService.cartItemCount() === 0) {
          <!-- Empty Cart -->
          <div class="empty-cart">
            <div class="empty-icon">🛒</div>
            <h2>Your cart is empty</h2>
            <p>Looks like you haven't added any items to your cart yet.</p>
            <a routerLink="/products" class="btn btn-primary">Start Shopping</a>
          </div>
        } @else {
          <!-- Cart Items -->
          <div class="cart-content">
            <div class="cart-items">
              @for (item of cartService.cartItems(); track item.id) {
                <div class="cart-item">
                  <div class="item-image">
                    <img 
                      [src]="item.product.imageUrl || '/assets/images/placeholder.jpg'" 
                      [alt]="item.product.name"
                    />
                  </div>
                  
                  <div class="item-details">
                    <h3>{{ item.product.name }}</h3>
                    <p class="item-category">{{ item.product.categoryName }}</p>
                    <p class="item-sku">SKU: {{ item.product.sku }}</p>
                    
                    @if (item.product.isOnSale) {
                      <div class="item-price">
                        <span class="original-price">\${{ item.product.price }}</span>
                        <span class="sale-price">\${{ item.product.effectivePrice }}</span>
                        <span class="discount-badge">{{ item.product.discountPercentage }}% OFF</span>
                      </div>
                    } @else {
                      <div class="item-price">
                        <span class="price">\${{ item.product.price }}</span>
                      </div>
                    }
                  </div>
                  
                  <div class="item-quantity">
                    <label>Quantity:</label>
                    <div class="quantity-controls">
                      <button 
                        class="qty-btn"
                        (click)="updateQuantity(item, item.quantity - 1)"
                        [disabled]="item.quantity <= 1 || isUpdating"
                      >
                        -
                      </button>
                      <span class="quantity">{{ item.quantity }}</span>
                      <button 
                        class="qty-btn"
                        (click)="updateQuantity(item, item.quantity + 1)"
                        [disabled]="isUpdating"
                      >
                        +
                      </button>
                    </div>
                  </div>
                  
                  <div class="item-total">
                    <p class="total-price">\${{ item.totalPrice.toFixed(2) }}</p>
                  </div>
                  
                  <div class="item-actions">
                    <button 
                      class="btn-icon wishlist-btn"
                      (click)="moveToWishlist(item)"
                      [disabled]="isUpdating"
                      title="Move to Wishlist"
                    >
                      ❤️
                    </button>
                    <button 
                      class="btn-icon remove-btn"
                      (click)="removeItem(item)"
                      [disabled]="isUpdating"
                      title="Remove from Cart"
                    >
                      🗑️
                    </button>
                  </div>
                </div>
              }
            </div>

            <!-- Cart Summary -->
            <div class="cart-summary">
              <div class="summary-card">
                <h3>Order Summary</h3>
                
                <div class="summary-row">
                  <span>Subtotal ({{ cartService.cartItemCount() }} items):</span>
                  <span>\${{ cartService.cartSubtotal().toFixed(2) }}</span>
                </div>
                
                @if (cartSummary) {
                  <div class="summary-row">
                    <span>Tax:</span>
                    <span>\${{ cartSummary.tax.toFixed(2) }}</span>
                  </div>
                  
                  <div class="summary-row">
                    <span>Shipping:</span>
                    <span>
                      @if (cartSummary.shipping === 0) {
                        <span class="free-shipping">FREE</span>
                      } @else {
                        \${{ cartSummary.shipping.toFixed(2) }}
                      }
                    </span>
                  </div>
                  
                  @if (cartSummary.shipping > 0) {
                    <div class="shipping-notice">
                      <small>💡 Free shipping on orders over $50</small>
                    </div>
                  }
                  
                  <div class="summary-divider"></div>
                  
                  <div class="summary-row total-row">
                    <span>Total:</span>
                    <span>\${{ cartSummary.total.toFixed(2) }}</span>
                  </div>
                }
                
                <div class="summary-actions">
                  <button 
                    class="btn btn-secondary btn-full"
                    (click)="clearCart()"
                    [disabled]="isUpdating"
                  >
                    Clear Cart
                  </button>
                  
                  <button 
                    class="btn btn-primary btn-full"
                    (click)="proceedToCheckout()"
                    [disabled]="isUpdating"
                  >
                    Proceed to Checkout
                  </button>
                </div>
              </div>
            </div>
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
    .cart-container {
      min-height: 100vh;
      background: #f8f9fa;
      padding: 2rem 0;
    }

    .container {
      max-width: 1200px;
      margin: 0 auto;
      padding: 0 1rem;
    }

    .cart-header {
      text-align: center;
      margin-bottom: 3rem;
    }

    .cart-header h1 {
      margin: 0 0 0.5rem 0;
      color: #2c3e50;
      font-size: 2.5rem;
    }

    .cart-header p {
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

    @keyframes spin {
      0% { transform: rotate(0deg); }
      100% { transform: rotate(360deg); }
    }

    .empty-cart {
      text-align: center;
      padding: 4rem 0;
    }

    .empty-icon {
      font-size: 4rem;
      margin-bottom: 1rem;
    }

    .empty-cart h2 {
      margin: 0 0 1rem 0;
      color: #2c3e50;
    }

    .empty-cart p {
      margin: 0 0 2rem 0;
      color: #7f8c8d;
      font-size: 1.1rem;
    }

    .cart-content {
      display: grid;
      grid-template-columns: 1fr 350px;
      gap: 3rem;
    }

    .cart-items {
      display: flex;
      flex-direction: column;
      gap: 1.5rem;
    }

    .cart-item {
      background: white;
      border-radius: 10px;
      padding: 1.5rem;
      box-shadow: 0 2px 4px rgba(0,0,0,0.1);
      display: grid;
      grid-template-columns: 120px 1fr auto auto auto;
      gap: 1.5rem;
      align-items: center;
    }

    .item-image img {
      width: 120px;
      height: 120px;
      object-fit: cover;
      border-radius: 8px;
    }

    .item-details h3 {
      margin: 0 0 0.5rem 0;
      color: #2c3e50;
      font-size: 1.2rem;
    }

    .item-category,
    .item-sku {
      margin: 0 0 0.25rem 0;
      color: #7f8c8d;
      font-size: 0.9rem;
    }

    .item-price {
      display: flex;
      align-items: center;
      gap: 0.5rem;
      flex-wrap: wrap;
    }

    .price {
      font-size: 1.2rem;
      font-weight: bold;
      color: #2c3e50;
    }

    .original-price {
      text-decoration: line-through;
      color: #7f8c8d;
    }

    .sale-price {
      font-size: 1.2rem;
      font-weight: bold;
      color: #e74c3c;
    }

    .discount-badge {
      background: #e74c3c;
      color: white;
      padding: 0.25rem 0.5rem;
      border-radius: 4px;
      font-size: 0.8rem;
      font-weight: bold;
    }

    .item-quantity label {
      display: block;
      margin-bottom: 0.5rem;
      color: #2c3e50;
      font-weight: 500;
      font-size: 0.9rem;
    }

    .quantity-controls {
      display: flex;
      align-items: center;
      border: 1px solid #ddd;
      border-radius: 4px;
      overflow: hidden;
    }

    .qty-btn {
      background: #f8f9fa;
      border: none;
      padding: 0.5rem 0.75rem;
      cursor: pointer;
      font-size: 1.2rem;
      font-weight: bold;
      color: #2c3e50;
    }

    .qty-btn:hover:not(:disabled) {
      background: #e9ecef;
    }

    .qty-btn:disabled {
      opacity: 0.5;
      cursor: not-allowed;
    }

    .quantity {
      padding: 0.5rem 1rem;
      background: white;
      border-left: 1px solid #ddd;
      border-right: 1px solid #ddd;
      font-weight: bold;
      min-width: 50px;
      text-align: center;
    }

    .item-total {
      text-align: right;
    }

    .total-price {
      margin: 0;
      font-size: 1.3rem;
      font-weight: bold;
      color: #2c3e50;
    }

    .item-actions {
      display: flex;
      flex-direction: column;
      gap: 0.5rem;
    }

    .btn-icon {
      background: none;
      border: 1px solid #ddd;
      border-radius: 4px;
      padding: 0.5rem;
      cursor: pointer;
      font-size: 1.2rem;
      transition: all 0.3s;
    }

    .btn-icon:hover:not(:disabled) {
      border-color: #e74c3c;
      background: #fdf2f2;
    }

    .btn-icon:disabled {
      opacity: 0.5;
      cursor: not-allowed;
    }

    .cart-summary {
      position: sticky;
      top: 2rem;
    }

    .summary-card {
      background: white;
      border-radius: 10px;
      padding: 2rem;
      box-shadow: 0 2px 4px rgba(0,0,0,0.1);
    }

    .summary-card h3 {
      margin: 0 0 1.5rem 0;
      color: #2c3e50;
      font-size: 1.5rem;
    }

    .summary-row {
      display: flex;
      justify-content: space-between;
      align-items: center;
      margin-bottom: 1rem;
      color: #2c3e50;
    }

    .summary-row span:last-child {
      font-weight: 500;
    }

    .total-row {
      font-size: 1.2rem;
      font-weight: bold;
      padding-top: 1rem;
      border-top: 2px solid #e74c3c;
    }

    .free-shipping {
      color: #27ae60;
      font-weight: bold;
    }

    .shipping-notice {
      margin: 0.5rem 0;
      padding: 0.5rem;
      background: #e8f5e8;
      border-radius: 4px;
      color: #27ae60;
      font-size: 0.9rem;
    }

    .summary-divider {
      height: 1px;
      background: #eee;
      margin: 1rem 0;
    }

    .summary-actions {
      display: flex;
      flex-direction: column;
      gap: 1rem;
      margin-top: 2rem;
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

    .btn:disabled {
      opacity: 0.6;
      cursor: not-allowed;
    }

    .btn-full {
      width: 100%;
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
      .cart-content {
        grid-template-columns: 1fr;
        gap: 2rem;
      }

      .cart-item {
        grid-template-columns: 80px 1fr;
        gap: 1rem;
      }

      .item-image img {
        width: 80px;
        height: 80px;
      }

      .item-quantity,
      .item-total,
      .item-actions {
        grid-column: 1 / -1;
        margin-top: 1rem;
      }

      .item-actions {
        flex-direction: row;
        justify-content: center;
      }

      .cart-summary {
        position: static;
      }
    }
  `]
})
export class CartComponent implements OnInit {
  cartService = inject(CartService);
  authService = inject(AuthService);

  cartSummary: CartSummary | null = null;
  isUpdating = false;
  errorMessage = '';

  ngOnInit(): void {
    this.loadCartSummary();
  }

  updateQuantity(item: CartItem, newQuantity: number): void {
    if (newQuantity < 1) return;

    this.isUpdating = true;
    this.cartService.updateCartItem(item.id, { quantity: newQuantity }).subscribe({
      next: () => {
        this.isUpdating = false;
        this.loadCartSummary();
      },
      error: (error) => {
        this.isUpdating = false;
        this.errorMessage = 'Failed to update quantity';
        console.error('Error updating quantity:', error);
      }
    });
  }

  removeItem(item: CartItem): void {
    if (confirm(`Remove "${item.product.name}" from your cart?`)) {
      this.isUpdating = true;
      this.cartService.removeFromCart(item.id).subscribe({
        next: () => {
          this.isUpdating = false;
          this.loadCartSummary();
        },
        error: (error) => {
          this.isUpdating = false;
          this.errorMessage = 'Failed to remove item';
          console.error('Error removing item:', error);
        }
      });
    }
  }

  moveToWishlist(item: CartItem): void {
    this.isUpdating = true;
    
    // First add to wishlist, then remove from cart
    this.cartService.addToWishlist(item.productId).subscribe({
      next: () => {
        this.cartService.removeFromCart(item.id).subscribe({
          next: () => {
            this.isUpdating = false;
            this.loadCartSummary();
          },
          error: (error) => {
            this.isUpdating = false;
            console.error('Error removing from cart:', error);
          }
        });
      },
      error: (error) => {
        this.isUpdating = false;
        console.error('Error adding to wishlist:', error);
      }
    });
  }

  clearCart(): void {
    if (confirm('Are you sure you want to clear your entire cart?')) {
      this.isUpdating = true;
      this.cartService.clearCart().subscribe({
        next: () => {
          this.isUpdating = false;
          this.cartSummary = null;
        },
        error: (error) => {
          this.isUpdating = false;
          this.errorMessage = 'Failed to clear cart';
          console.error('Error clearing cart:', error);
        }
      });
    }
  }

  proceedToCheckout(): void {
    // This will be implemented when we create the checkout component
    alert('');
  }

  private loadCartSummary(): void {
    this.cartService.getCartSummary().subscribe({
      next: (summary) => {
        this.cartSummary = summary;
      },
      error: (error) => {
        console.error('Error loading cart summary:', error);
      }
    });
  }
}