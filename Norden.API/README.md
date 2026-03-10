# Norden Backend API

A comprehensive .NET 9 Web API for the Norden e-commerce application, built with ASP.NET Core, Entity Framework Core, and JWT authentication.

## 🚀 Features

- **Authentication & Authorization**: JWT-based authentication with role-based access control
- **Product Management**: Full CRUD operations for products with categories, brands, and inventory
- **Shopping Cart**: Add, update, and remove items from cart
- **Wishlist**: Save products for later purchase
- **Order Management**: Complete order processing with tracking
- **Reviews & Ratings**: Product reviews with helpful voting
- **Address Management**: Multiple shipping addresses per user
- **Search & Filtering**: Advanced product search with filters
- **Admin Dashboard**: Analytics and management tools
- **API Documentation**: Swagger/OpenAPI documentation

## 🛠 Technology Stack

- **.NET 9**: Latest .NET framework
- **ASP.NET Core Web API**: RESTful API framework
- **Entity Framework Core**: ORM for database operations
- **SQL Server**: Database (LocalDB for development)
- **JWT Authentication**: Secure token-based authentication
- **BCrypt**: Password hashing
- **Serilog**: Structured logging
- **Swagger/OpenAPI**: API documentation
- **AutoMapper**: Object mapping

## 📋 Prerequisites

- .NET 9 SDK
- SQL Server (LocalDB included with Visual Studio)
- Visual Studio 2022 or VS Code

## 🚀 Getting Started

### 1. Clone the Repository
```bash
git clone <repository-url>
cd Norden.API
```

### 2. Restore Dependencies
```bash
dotnet restore
```

### 3. Update Connection String
Update the connection string in `appsettings.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=NordenDB;Trusted_Connection=true;MultipleActiveResultSets=true"
  }
}
```

### 4. Run Database Migrations
```bash
dotnet ef database update
```

### 5. Run the Application
```bash
dotnet run
```

The API will be available at:
- **HTTPS**: `https://localhost:7000`
- **HTTP**: `http://localhost:5000`
- **Swagger UI**: `https://localhost:7000` (root URL)

## 📚 API Documentation

### Authentication Endpoints
- `POST /api/auth/register` - Register new user
- `POST /api/auth/login` - User login
- `POST /api/auth/refresh-token` - Refresh JWT token
- `POST /api/auth/forgot-password` - Password reset

### Product Endpoints
- `GET /api/products` - Get products with filtering and pagination
- `GET /api/products/{id}` - Get product by ID

### Cart Endpoints (Authenticated)
- `GET /api/cart` - Get user's cart
- `POST /api/cart/items` - Add item to cart
- `PUT /api/cart/items/{id}` - Update cart item
- `DELETE /api/cart/items/{id}` - Remove item from cart

### Wishlist Endpoints (Authenticated)
- `GET /api/wishlist` - Get user's wishlist
- `POST /api/wishlist/items` - Add item to wishlist
- `DELETE /api/wishlist/items/{productId}` - Remove item from wishlist

### Order Endpoints (Authenticated)
- `GET /api/orders` - Get user's orders
- `POST /api/orders` - Create new order
- `GET /api/orders/{id}` - Get order details
- `GET /api/orders/{id}/tracking` - Get order tracking

### Review Endpoints
- `GET /api/reviews/products/{productId}` - Get product reviews
- `POST /api/reviews/products/{productId}` - Create review (Authenticated)
- `PUT /api/reviews/{id}` - Update review (Authenticated)
- `DELETE /api/reviews/{id}` - Delete review (Authenticated)
- `POST /api/reviews/{id}/helpful` - Mark review as helpful (Authenticated)

### Address Endpoints (Authenticated)
- `GET /api/addresses` - Get user's addresses
- `POST /api/addresses` - Create address
- `PUT /api/addresses/{id}` - Update address
- `DELETE /api/addresses/{id}` - Delete address

### Search Endpoints
- `GET /api/search` - Search products with filters

### Admin Endpoints (Admin Only)
- `GET /api/admin/dashboard` - Get dashboard analytics
- `GET /api/admin/products` - Get all products
- `GET /api/admin/orders` - Get all orders
- `PUT /api/admin/orders/{id}/status` - Update order status

## 🔐 Authentication

The API uses JWT (JSON Web Tokens) for authentication. Include the token in the Authorization header:

```
Authorization: Bearer <your-jwt-token>
```

### Default Test Users
- **Admin**: `admin@norden.com` / `Admin123!`
- **User**: `user@norden.com` / `User123!`

## 📊 Database Schema

The API includes the following main entities:
- **Users**: User accounts with authentication
- **Products**: Product catalog with categories and inventory
- **Reviews**: Product reviews and ratings
- **CartItems**: Shopping cart items
- **WishlistItems**: User wishlist items
- **Addresses**: User shipping addresses
- **Orders**: Order information
- **OrderItems**: Order line items
- **OrderTracking**: Order status tracking

## 🧪 Testing

Use the provided `TestEndpoints.http` file to test the API endpoints. The file includes examples for all major endpoints.

### Quick Test Commands
```bash
# Health check
curl https://localhost:7000/health

# Get products
curl https://localhost:7000/api/products

# Login (replace with actual credentials)
curl -X POST https://localhost:7000/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{"email":"user@norden.com","password":"User123!"}'
```

## 🔧 Configuration

### JWT Settings
Update JWT settings in `appsettings.json`:
```json
{
  "Jwt": {
    "SecretKey": "YourSuperSecretKeyThatIsAtLeast32CharactersLong!",
    "Issuer": "NordenAPI",
    "Audience": "NordenApp"
  }
}
```

### CORS Settings
Configure CORS for your Flutter app in `Program.cs`:
```csharp
policy.WithOrigins("http://localhost:3000", "https://localhost:3000")
```

## 📝 Logging

The API uses Serilog for structured logging. Logs are written to the console and can be configured to write to files or external services.

## 🚀 Deployment

### Production Considerations
1. Update connection string for production database
2. Set secure JWT secret key
3. Configure CORS for production domains
4. Set up proper logging
5. Configure HTTPS certificates
6. Set up database backups

### Docker Support
```dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["Norden.API.csproj", "."]
RUN dotnet restore
COPY . .
RUN dotnet build -c Release -o /app/build

FROM build AS publish
RUN dotnet publish -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Norden.API.dll"]
```

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Add tests if applicable
5. Submit a pull request

## 📄 License

This project is licensed under the MIT License.

## 🆘 Support

For support and questions:
- Create an issue in the repository
- Contact the development team
- Check the API documentation at `/swagger`

---

**Norden API** - Built with ❤️ using .NET 9
