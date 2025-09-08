import { Injectable, signal } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable, BehaviorSubject, tap } from 'rxjs';
import { 
  Product, 
  Category, 
  ProductFilter, 
  ProductListResponse,
  CreateProductRequest,
  UpdateProductRequest,
  CreateCategoryRequest,
  UpdateCategoryRequest
} from '../models';

@Injectable({
  providedIn: 'root'
})
export class ProductService {
  private readonly API_URL = 'http://localhost:5118/api';

  // Signals for reactive state management
  private _isLoading = signal<boolean>(false);
  private _categories = signal<Category[]>([]);
  private _featuredProducts = signal<Product[]>([]);

  // Public readonly signals
  public isLoading = this._isLoading.asReadonly();
  public categories = this._categories.asReadonly();
  public featuredProducts = this._featuredProducts.asReadonly();

  // Subjects for data streams
  private categoriesSubject = new BehaviorSubject<Category[]>([]);
  public categories$ = this.categoriesSubject.asObservable();

  constructor(private http: HttpClient) {
    this.loadCategories();
    this.loadFeaturedProducts();
  }

  private loadCategories(): void {
    this.getCategories().subscribe(categories => {
      this._categories.set(categories);
      this.categoriesSubject.next(categories);
    });
  }

  private loadFeaturedProducts(): void {
    this.getFeaturedProducts().subscribe(products => {
      this._featuredProducts.set(products);
    });
  }

  // Product operations
  getProducts(filter?: ProductFilter): Observable<ProductListResponse> {
    this._isLoading.set(true);
    
    let params = new HttpParams();
    if (filter) {
      Object.keys(filter).forEach(key => {
        const value = (filter as any)[key];
        if (value !== undefined && value !== null && value !== '') {
          params = params.set(key, value.toString());
        }
      });
    }

    return this.http.get<ProductListResponse>(`${this.API_URL}/Products`, { params })
      .pipe(
        tap(() => this._isLoading.set(false))
      );
  }

  getProduct(id: number): Observable<Product> {
    return this.http.get<Product>(`${this.API_URL}/Products/${id}`);
  }

  getProductBySku(sku: string): Observable<Product> {
    return this.http.get<Product>(`${this.API_URL}/Products/sku/${sku}`);
  }

  createProduct(product: CreateProductRequest): Observable<Product> {
    return this.http.post<Product>(`${this.API_URL}/Products`, product);
  }

  updateProduct(id: number, product: UpdateProductRequest): Observable<Product> {
    return this.http.put<Product>(`${this.API_URL}/Products/${id}`, product);
  }

  deleteProduct(id: number): Observable<any> {
    return this.http.delete(`${this.API_URL}/Products/${id}`);
  }

  updateProductStock(id: number, stock: number): Observable<any> {
    return this.http.patch(`${this.API_URL}/Products/${id}/stock`, stock);
  }

  // Search and filtering
  searchProducts(searchTerm: string, filter?: ProductFilter): Observable<ProductListResponse> {
    this._isLoading.set(true);
    
    let params = new HttpParams().set('searchTerm', searchTerm);
    if (filter) {
      Object.keys(filter).forEach(key => {
        const value = (filter as any)[key];
        if (value !== undefined && value !== null && value !== '') {
          params = params.set(key, value.toString());
        }
      });
    }

    return this.http.get<ProductListResponse>(`${this.API_URL}/Products/search`, { params })
      .pipe(
        tap(() => this._isLoading.set(false))
      );
  }

  getFeaturedProducts(count: number = 10): Observable<Product[]> {
    return this.http.get<Product[]>(`${this.API_URL}/Products/featured?count=${count}`)
      .pipe(
        tap(products => this._featuredProducts.set(products))
      );
  }

  getProductsByCategory(categoryId: number, count: number = 20): Observable<Product[]> {
    return this.http.get<Product[]>(`${this.API_URL}/Products/category/${categoryId}?count=${count}`);
  }

  getRelatedProducts(id: number, count: number = 5): Observable<Product[]> {
    return this.http.get<Product[]>(`${this.API_URL}/Products/${id}/related?count=${count}`);
  }

  getTopRatedProducts(count: number = 10): Observable<Product[]> {
    return this.http.get<Product[]>(`${this.API_URL}/Products/top-rated?count=${count}`);
  }

  getBestSellingProducts(count: number = 10): Observable<Product[]> {
    return this.http.get<Product[]>(`${this.API_URL}/Products/best-selling?count=${count}`);
  }

  // Category operations
  getCategories(): Observable<Category[]> {
    return this.http.get<Category[]>(`${this.API_URL}/Categories`)
      .pipe(
        tap(categories => {
          this._categories.set(categories);
          this.categoriesSubject.next(categories);
        })
      );
  }

  getCategory(id: number): Observable<Category> {
    return this.http.get<Category>(`${this.API_URL}/Categories/${id}`);
  }

  createCategory(category: CreateCategoryRequest): Observable<Category> {
    return this.http.post<Category>(`${this.API_URL}/Categories`, category)
      .pipe(
        tap(() => this.loadCategories()) // Refresh categories after creation
      );
  }

  updateCategory(id: number, category: UpdateCategoryRequest): Observable<Category> {
    return this.http.put<Category>(`${this.API_URL}/Categories/${id}`, category)
      .pipe(
        tap(() => this.loadCategories()) // Refresh categories after update
      );
  }

  deleteCategory(id: number): Observable<any> {
    return this.http.delete(`${this.API_URL}/Categories/${id}`)
      .pipe(
        tap(() => this.loadCategories()) // Refresh categories after deletion
      );
  }

  getCategoryProducts(id: number, count: number = 20): Observable<Product[]> {
    return this.http.get<Product[]>(`${this.API_URL}/Categories/${id}/products?count=${count}`);
  }

  // Admin operations
  getLowStockProducts(): Observable<Product[]> {
    return this.http.get<Product[]>(`${this.API_URL}/Products/low-stock`);
  }

  getProductCountByCategory(): Observable<{ [key: string]: number }> {
    return this.http.get<{ [key: string]: number }>(`${this.API_URL}/Products/analytics/category-count`);
  }

  // Bulk operations
  bulkUpdateProductStatus(productIds: number[], isActive: boolean): Observable<any> {
    return this.http.patch(`${this.API_URL}/Products/bulk/status`, { productIds, isActive });
  }

  bulkDeleteProducts(productIds: number[]): Observable<any> {
    return this.http.delete(`${this.API_URL}/Products/bulk`, { body: { productIds } });
  }

  // Product images
  addProductImage(productId: number, imageData: any): Observable<any> {
    return this.http.post(`${this.API_URL}/Products/${productId}/images`, imageData);
  }

  deleteProductImage(productId: number, imageId: number): Observable<any> {
    return this.http.delete(`${this.API_URL}/Products/${productId}/images/${imageId}`);
  }

  setPrimaryImage(productId: number, imageId: number): Observable<any> {
    return this.http.patch(`${this.API_URL}/Products/${productId}/images/${imageId}/primary`, {});
  }

  // Helper methods
  refreshCategories(): void {
    this.loadCategories();
  }

  refreshFeaturedProducts(): void {
    this.loadFeaturedProducts();
  }
}