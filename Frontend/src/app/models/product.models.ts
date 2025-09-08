export interface Product {
  id: number;
  name: string;
  description?: string;
  price: number;
  discountPrice?: number;
  sku: string;
  stock: number;
  minStockLevel: number;
  categoryId: number;
  categoryName?: string;
  brand?: string;
  color?: string;
  material?: string;
  dimensions?: string;
  weight?: number;
  rating: number;
  reviewCount: number;
  imageUrl?: string;
  isActive: boolean;
  isFeatured: boolean;
  createdAt: string;
  updatedAt: string;
  productImages: ProductImage[];
  
  // Computed properties
  effectivePrice: number;
  isOnSale: boolean;
  discountPercentage: number;
  isLowStock: boolean;
  isOutOfStock: boolean;
}

export interface ProductImage {
  id: number;
  productId: number;
  imageUrl: string;
  altText?: string;
  isPrimary: boolean;
  displayOrder: number;
  createdAt: string;
}

export interface Category {
  id: number;
  name: string;
  description?: string;
  imageUrl?: string;
  isActive: boolean;
  createdAt: string;
  updatedAt: string;
  productCount: number;
}

export interface ProductFilter {
  categoryId?: number;
  minPrice?: number;
  maxPrice?: number;
  minRating?: number;
  brand?: string;
  color?: string;
  material?: string;
  isOnSale?: boolean;
  isFeatured?: boolean;
  inStock?: boolean;
  searchTerm?: string;
  sortBy?: string;
  sortOrder?: string;
  page?: number;
  pageSize?: number;
}

export interface ProductListResponse {
  products: Product[];
  totalCount: number;
  page: number;
  pageSize: number;
  totalPages: number;
  hasNextPage: boolean;
  hasPreviousPage: boolean;
}

export interface CreateProductRequest {
  name: string;
  description?: string;
  price: number;
  discountPrice?: number;
  sku: string;
  stock: number;
  minStockLevel: number;
  categoryId: number;
  brand?: string;
  color?: string;
  material?: string;
  dimensions?: string;
  weight?: number;
  imageUrl?: string;
  isFeatured: boolean;
  productImages: CreateProductImageRequest[];
}

export interface CreateProductImageRequest {
  imageUrl: string;
  altText?: string;
  isPrimary: boolean;
  displayOrder: number;
}

export interface UpdateProductRequest {
  name: string;
  description?: string;
  price: number;
  discountPrice?: number;
  stock: number;
  minStockLevel: number;
  categoryId: number;
  brand?: string;
  color?: string;
  material?: string;
  dimensions?: string;
  weight?: number;
  imageUrl?: string;
  isActive: boolean;
  isFeatured: boolean;
}

export interface CreateCategoryRequest {
  name: string;
  description?: string;
  imageUrl?: string;
}

export interface UpdateCategoryRequest {
  name: string;
  description?: string;
  imageUrl?: string;
  isActive: boolean;
}