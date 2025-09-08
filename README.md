# ShopForHome

ShopForHome is a modern e-commerce platform for home décor and furniture, featuring a robust frontend built with Angular and a secure backend powered by ASP.NET Core.

## Features

- Browse products by category, brand, color, and price
- Advanced filtering, sorting, and search
- Product details, reviews, and ratings
- Shopping cart and wishlist management
- User authentication and registration
- Secure payment and order processing
- Admin features for product and category management

## Technology Stack

**Frontend:**  
- Angular (TypeScript)
- RxJS, Angular Router, Reactive Forms
- CSS

**Backend:**  
- ASP.NET Core Web API (C#)
- Entity Framework Core (SQL Server)
- JWT Authentication

## Getting Started

### Prerequisites

- Node.js & npm (for frontend)
- .NET 9 SDK (for backend)
- SQL Server (local or cloud)

### Setup

#### Frontend

```bash
cd Frontend
npm install
ng serve
```

#### Backend

```bash
cd Backend
dotnet restore
dotnet ef database update
dotnet run
```

## Folder Structure

```
ShopForHome/
  Frontend/   # Angular app
  Backend/    # ASP.NET Core API
```

## Contributing

Pull requests are welcome! For major changes, please open an issue first to discuss what you would like to change.

## License

This project is licensed under
