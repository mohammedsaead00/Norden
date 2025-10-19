# NordenAPI - Fashion E-commerce API

A modern ASP.NET Core Web API for fashion e-commerce platform.

## Features
- 🔐 JWT Authentication
- 👥 User Management
- 🛍️ Product Catalog
- 🛒 Shopping Cart
- 📦 Order Management
- 💳 Payment Processing
- 📍 Address Management
- ❤️ Wishlist

## Technologies
- ASP.NET Core 9.0
- Entity Framework Core
- SQL Server
- JWT Authentication
- Swagger/OpenAPI

## API Endpoints

### Authentication
- `POST /api/auth/register` - User Registration
- `POST /api/auth/login` - User Login
- `POST /api/auth/guest-login` - Guest Login
- `POST /api/auth/refresh-token` - Refresh Token

### Products
- `GET /api/products` - Get All Products
- `GET /api/products/{id}` - Get Product by ID
- `GET /api/products/search?q={query}` - Search Products

### Cart
- `GET /api/cart` - Get Cart Items
- `POST /api/cart` - Add Item to Cart
- `PUT /api/cart/{id}` - Update Cart Item
- `DELETE /api/cart/{id}` - Remove Cart Item

### Orders
- `GET /api/orders` - Get User Orders
- `POST /api/orders` - Create Order

## Deployment

This API is deployed on Railway and can be accessed at:
- **API Base URL**: https://norden-api.railway.app
- **Swagger UI**: https://norden-api.railway.app/swagger

## Environment Variables

```
ASPNETCORE_ENVIRONMENT=Production
ConnectionStrings__DefaultConnection=your_connection_string
Jwt__SecretKey=your_secret_key
Jwt__Issuer=https://norden-api.railway.app
Jwt__Audience=https://norden.com
```