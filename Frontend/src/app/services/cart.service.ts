import { Injectable, signal, computed } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, BehaviorSubject, tap } from 'rxjs';
import { CartItem, WishlistItem, AddToCartRequest, UpdateCartItemRequest, CartSummary } from '../models';

@Injectable({
  providedIn: 'root'
})
export class CartService {
  private readonly API_URL = 'http://localhost:5118/api';

  // Signals for reactive state management
  private _cartItems = signal<CartItem[]>([]);
  private _wishlistItems = signal<WishlistItem[]>([]);
  private _isLoading = signal<boolean>(false);

  // Public readonly signals
  public cartItems = this._cartItems.asReadonly();
  public wishlistItems = this._wishlistItems.asReadonly();
  public isLoading = this._isLoading.asReadonly();

  // Computed signals
  public cartItemCount = computed(() => 
    this._cartItems().reduce((total, item) => total + item.quantity, 0)
  );
  
  public cartSubtotal = computed(() => 
    this._cartItems().reduce((total, item) => total + item.totalPrice, 0)
  );

  public wishlistItemCount = computed(() => this._wishlistItems().length);

  // Subjects for data streams
  private cartItemsSubject = new BehaviorSubject<CartItem[]>([]);
  private wishlistItemsSubject = new BehaviorSubject<WishlistItem[]>([]);

  public cartItems$ = this.cartItemsSubject.asObservable();
  public wishlistItems$ = this.wishlistItemsSubject.asObservable();

  constructor(private http: HttpClient) {
    this.loadCartItems();
    this.loadWishlistItems();
  }

  // Cart operations
  getCartItems(): Observable<CartItem[]> {
    this._isLoading.set(true);
    return this.http.get<CartItem[]>(`${this.API_URL}/Cart`)
      .pipe(
        tap(items => {
          this._cartItems.set(items);
          this.cartItemsSubject.next(items);
          this._isLoading.set(false);
        })
      );
  }

  addToCart(request: AddToCartRequest): Observable<CartItem> {
    return this.http.post<CartItem>(`${this.API_URL}/Cart`, request)
      .pipe(
        tap(() => this.loadCartItems())
      );
  }

  updateCartItem(itemId: number, request: UpdateCartItemRequest): Observable<CartItem> {
    return this.http.put<CartItem>(`${this.API_URL}/Cart/${itemId}`, request)
      .pipe(
        tap(() => this.loadCartItems())
      );
  }

  removeFromCart(itemId: number): Observable<any> {
    return this.http.delete(`${this.API_URL}/Cart/${itemId}`)
      .pipe(
        tap(() => this.loadCartItems())
      );
  }

  clearCart(): Observable<any> {
    return this.http.delete(`${this.API_URL}/Cart/clear`)
      .pipe(
        tap(() => {
          this._cartItems.set([]);
          this.cartItemsSubject.next([]);
        })
      );
  }

  getCartSummary(): Observable<CartSummary> {
    return this.http.get<CartSummary>(`${this.API_URL}/Cart/summary`);
  }

  // Wishlist operations
  getWishlistItems(): Observable<WishlistItem[]> {
    this._isLoading.set(true);
    return this.http.get<WishlistItem[]>(`${this.API_URL}/Wishlist`)
      .pipe(
        tap(items => {
          this._wishlistItems.set(items);
          this.wishlistItemsSubject.next(items);
          this._isLoading.set(false);
        })
      );
  }

  addToWishlist(productId: number): Observable<WishlistItem> {
    return this.http.post<WishlistItem>(`${this.API_URL}/Wishlist`, { productId })
      .pipe(
        tap(() => this.loadWishlistItems())
      );
  }

  removeFromWishlist(itemId: number): Observable<any> {
    return this.http.delete(`${this.API_URL}/Wishlist/${itemId}`)
      .pipe(
        tap(() => this.loadWishlistItems())
      );
  }

  moveToCart(wishlistItemId: number, quantity: number = 1): Observable<any> {
    return this.http.post(`${this.API_URL}/Wishlist/${wishlistItemId}/move-to-cart`, { quantity })
      .pipe(
        tap(() => {
          this.loadWishlistItems();
          this.loadCartItems();
        })
      );
  }

  // Helper methods
  private loadCartItems(): void {
    this.getCartItems().subscribe();
  }

  private loadWishlistItems(): void {
    this.getWishlistItems().subscribe();
  }

  // Check if product is in cart
  isInCart(productId: number): boolean {
    return this._cartItems().some(item => item.productId === productId);
  }

  // Check if product is in wishlist
  isInWishlist(productId: number): boolean {
    return this._wishlistItems().some(item => item.productId === productId);
  }

  // Get cart item by product ID
  getCartItemByProductId(productId: number): CartItem | undefined {
    return this._cartItems().find(item => item.productId === productId);
  }

  // Get wishlist item by product ID
  getWishlistItemByProductId(productId: number): WishlistItem | undefined {
    return this._wishlistItems().find(item => item.productId === productId);
  }

  // Quick add to cart with quantity check
  quickAddToCart(productId: number, quantity: number = 1): Observable<CartItem> {
    const existingItem = this.getCartItemByProductId(productId);
    
    if (existingItem) {
      // Update existing item
      return this.updateCartItem(existingItem.id, { 
        quantity: existingItem.quantity + quantity 
      });
    } else {
      // Add new item
      return this.addToCart({ productId, quantity });
    }
  }

  // Toggle wishlist item
  toggleWishlist(productId: number): Observable<any> {
    const existingItem = this.getWishlistItemByProductId(productId);
    
    if (existingItem) {
      return this.removeFromWishlist(existingItem.id);
    } else {
      return this.addToWishlist(productId);
    }
  }

  // Refresh data
  refreshCart(): void {
    this.loadCartItems();
  }

  refreshWishlist(): void {
    this.loadWishlistItems();
  }
}