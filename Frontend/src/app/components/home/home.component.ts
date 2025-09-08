import { Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { ProductService } from '../../services';
import { Product, Category } from '../../models';

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
export class HomeComponent implements OnInit {
  private productService = inject(ProductService);

  categories: Category[] = [];
  featuredProducts: Product[] = [];
  isLoading = false;

  ngOnInit(): void {
    this.loadData();
  }

  private loadData(): void {
    this.isLoading = true;

    // Load categories
    this.productService.getCategories().subscribe({
      next: (categories) => {
        this.categories = categories.slice(0, 6); // Show first 6 categories
      },
      error: (error) => {
        console.error('Error loading categories:', error);
      }
    });

    // Load featured products
    this.productService.getFeaturedProducts(8).subscribe({
      next: (products) => {
        this.featuredProducts = products;
        this.isLoading = false;
      },
      error: (error) => {
        console.error('Error loading featured products:', error);
        this.isLoading = false;
      }
    });
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
}