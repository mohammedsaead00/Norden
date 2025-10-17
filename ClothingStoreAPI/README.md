# Clothing Store API

A comprehensive ASP.NET Core Web API for an online clothing store, built with Entity Framework Core and SQL Server.

## 📋 Table of Contents
- [Features](#features)
- [Technologies Used](#technologies-used)
- [Database Schema](#database-schema)
- [Project Structure](#project-structure)
- [Prerequisites](#prerequisites)
- [Installation & Setup](#installation--setup)
- [Running the Application](#running-the-application)
- [API Endpoints](#api-endpoints)
- [Configuration](#configuration)
- [API Documentation](#api-documentation)

## ✨ Features

- **User Management**: Registration, login, profile management
- **Product Catalog**: Full CRUD operations for products with categories
- **Shopping Cart**: Add, update, remove items from cart
- **Order Management**: Create and track orders
- **Payment Processing**: Handle payment information
- **Address Management**: Multiple shipping and billing addresses
- **Inventory Management**: Track product stock levels
- **Product Reviews**: Customer reviews and ratings
- **RESTful API**: Clean REST API architecture
- **Swagger Documentation**: Interactive API documentation

## 🛠 Technologies Used

- **ASP.NET Core 9.0** - Web API Framework
- **Entity Framework Core 9.0** - ORM
- **SQL Server** - Database
- **Swagger/OpenAPI** - API Documentation
- **BCrypt.Net** - Password Hashing
- **Repository Pattern** - Data Access Layer
- **Service Layer** - Business Logic Layer
- **DTOs** - Data Transfer Objects

## 🗄 Database Schema

The API connects to an existing SQL Server database with the following tables:

- **Roles** - User roles (admin, customer)
- **Users** - User accounts and authentication
- **Addresses** - Shipping and billing addresses
- **Categories** - Product categories (hierarchical)
- **Products** - Product information
- **Product_Images** - Product images
- **Inventory** - Product variants (size, color) and stock
- **Cart** - Shopping cart items
- **Orders** - Order information
- **Order_Items** - Order line items
- **Payments** - Payment transactions
- **Reviews** - Product reviews and ratings

## 📁 Project Structure

```
ClothingStoreAPI/
├── Controllers/          # API Controllers
│   ├── UsersController.cs
│   ├── ProductsController.cs
│   ├── CategoriesController.cs
│   ├── OrdersController.cs
│   ├── CartController.cs
│   ├── PaymentsController.cs
│   ├── AddressesController.cs
│   └── OrderItemsController.cs
├── Models/              # Entity Models
│   ├── User.cs
│   ├── Role.cs
│   ├── Product.cs
│   ├── Category.cs
│   ├── Order.cs
│   ├── OrderItem.cs
│   ├── Cart.cs
│   ├── Payment.cs
│   ├── Address.cs
│   ├── Inventory.cs
│   ├── ProductImage.cs
│   └── Review.cs
├── DTOs/               # Data Transfer Objects
│   ├── UserDtos.cs
│   ├── ProductDtos.cs
│   ├── CategoryDtos.cs
│   ├── OrderDtos.cs
│   ├── CartDtos.cs
│   ├── PaymentDtos.cs
│   ├── AddressDtos.cs
│   ├── InventoryDtos.cs
│   └── ReviewDtos.cs
├── Data/               # Database Context
│   └── ClothingStoreContext.cs
├── Repositories/       # Data Access Layer
│   ├── IRepository.cs
│   ├── Repository.cs
│   ├── IUserRepository.cs
│   ├── UserRepository.cs
│   ├── IProductRepository.cs
│   ├── ProductRepository.cs
│   ├── IOrderRepository.cs
│   ├── OrderRepository.cs
│   ├── ICartRepository.cs
│   └── CartRepository.cs
├── Services/          # Business Logic Layer
│   ├── IUserService.cs
│   ├── UserService.cs
│   ├── IProductService.cs
│   ├── ProductService.cs
│   ├── IOrderService.cs
│   ├── OrderService.cs
│   ├── ICartService.cs
│   └── CartService.cs
├── Program.cs         # Application Entry Point
├── appsettings.json   # Configuration
└── README.md         # This file
```

## 📋 Prerequisites

Before running this application, ensure you have the following installed:

- [.NET 9.0 SDK](https://dotnet.microsoft.com/download) or later
- [SQL Server](https://www.microsoft.com/sql-server) (Express or higher)
- [SQL Server Management Studio](https://aka.ms/ssmsfullsetup) (optional, for database management)
- A code editor (Visual Studio 2022, VS Code, or JetBrains Rider)

## 🚀 Installation & Setup

### 1. Clone or Download the Project

```bash
cd ClothingStoreAPI
```

### 2. Configure Database Connection

Update the connection string in `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=YOUR_SERVER_NAME;Database=ClothingStore;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true"
  }
}
```

**Common SQL Server connection strings:**

- **SQL Server Express (Local):**
  ```
  Server=localhost\\SQLEXPRESS;Database=ClothingStore;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true
  ```

- **SQL Server (Local):**
  ```
  Server=localhost;Database=ClothingStore;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true
  ```

- **SQL Server with Username/Password:**
  ```
  Server=YOUR_SERVER;Database=ClothingStore;User Id=YOUR_USERNAME;Password=YOUR_PASSWORD;TrustServerCertificate=True;MultipleActiveResultSets=true
  ```

### 3. Create the Database

Run the SQL script (`file.sql`) to create the database and tables:

1. Open SQL Server Management Studio (SSMS)
2. Connect to your SQL Server instance
3. Open the `file.sql` script
4. Execute the script to create the `ClothingStore` database

### 4. Restore NuGet Packages

```bash
dotnet restore
```

### 5. Build the Project

```bash
dotnet build
```

## ▶ Running the Application

### Development Mode

```bash
dotnet run
```

The API will start at:
- **HTTPS**: `https://localhost:7XXX` (port varies)
- **HTTP**: `http://localhost:5XXX` (port varies)

### Production Mode

```bash
dotnet run --configuration Release
```

### Using Visual Studio

1. Open `ClothingStoreAPI.csproj` in Visual Studio
2. Press `F5` or click "Start Debugging"

### Check the Application

Once running, navigate to:
- **Swagger UI**: `https://localhost:7XXX/` or `http://localhost:5XXX/`
- **API Base URL**: `https://localhost:7XXX/api` or `http://localhost:5XXX/api`

## 📡 API Endpoints

### Users Endpoints

| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | `/api/users/register` | Register a new user |
| POST | `/api/users/login` | Login user |
| GET | `/api/users` | Get all users |
| GET | `/api/users/{id}` | Get user by ID |
| PUT | `/api/users/{id}` | Update user |
| DELETE | `/api/users/{id}` | Delete user |

### Products Endpoints

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/products` | Get all products |
| GET | `/api/products/{id}` | Get product by ID |
| GET | `/api/products/category/{categoryId}` | Get products by category |
| GET | `/api/products/search?query={searchTerm}` | Search products |
| POST | `/api/products` | Create new product |
| PUT | `/api/products/{id}` | Update product |
| DELETE | `/api/products/{id}` | Delete product |

### Categories Endpoints

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/categories` | Get all categories |
| GET | `/api/categories/{id}` | Get category by ID |
| POST | `/api/categories` | Create new category |
| PUT | `/api/categories/{id}` | Update category |
| DELETE | `/api/categories/{id}` | Delete category |

### Cart Endpoints

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/cart/user/{userId}` | Get user's cart |
| POST | `/api/cart` | Add item to cart |
| PUT | `/api/cart/{cartId}` | Update cart item |
| DELETE | `/api/cart/{cartId}` | Remove item from cart |
| DELETE | `/api/cart/user/{userId}` | Clear user's cart |

### Orders Endpoints

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/orders` | Get all orders |
| GET | `/api/orders/{id}` | Get order by ID |
| GET | `/api/orders/user/{userId}` | Get orders by user |
| POST | `/api/orders` | Create new order |
| PUT | `/api/orders/{id}` | Update order status |
| DELETE | `/api/orders/{id}` | Delete order |

### Order Items Endpoints

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/orderitems/order/{orderId}` | Get order items |
| GET | `/api/orderitems/{id}` | Get order item by ID |

### Payments Endpoints

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/payments` | Get all payments |
| GET | `/api/payments/{id}` | Get payment by ID |
| GET | `/api/payments/order/{orderId}` | Get payment by order |
| POST | `/api/payments` | Create new payment |
| PUT | `/api/payments/{id}` | Update payment status |
| DELETE | `/api/payments/{id}` | Delete payment |

### Addresses Endpoints

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/addresses` | Get all addresses |
| GET | `/api/addresses/{id}` | Get address by ID |
| GET | `/api/addresses/user/{userId}` | Get user addresses |
| POST | `/api/addresses` | Create new address |
| PUT | `/api/addresses/{id}` | Update address |
| DELETE | `/api/addresses/{id}` | Delete address |

## 🔧 Configuration

### appsettings.json

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning",
      "Microsoft.EntityFrameworkCore": "Warning"
    }
  },
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost\\SQLEXPRESS;Database=ClothingStore;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true"
  },
  "AllowedHosts": "*"
}
```

### CORS Configuration

The API is configured with CORS to allow all origins (suitable for development). For production, update the CORS policy in `Program.cs`:

```csharp
builder.Services.AddCors(options =>
{
    options.AddPolicy("Production", policy =>
    {
        policy.WithOrigins("https://yourdomain.com")
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});
```

## 📚 API Documentation

### Swagger/OpenAPI

The API includes Swagger documentation. Access it at:
- **Development**: `https://localhost:7XXX/` or `http://localhost:5XXX/`

Swagger provides:
- Interactive API testing
- Request/response examples
- Schema definitions
- Authentication testing

### Example API Calls

#### Register a New User

```bash
POST /api/users/register
Content-Type: application/json

{
  "email": "john.doe@example.com",
  "password": "SecurePassword123",
  "firstName": "John",
  "lastName": "Doe",
  "phone": "1234567890"
}
```

#### Login

```bash
POST /api/users/login
Content-Type: application/json

{
  "email": "john.doe@example.com",
  "password": "SecurePassword123"
}
```

#### Get All Products

```bash
GET /api/products
```

#### Create an Order

```bash
POST /api/orders
Content-Type: application/json

{
  "userId": 1,
  "shippingAddressId": 1,
  "billingAddressId": 1,
  "orderItems": [
    {
      "productId": 1,
      "inventoryId": 1,
      "quantity": 2
    }
  ]
}
```

## 🔒 Security Notes

- Passwords are hashed using BCrypt before storing
- Connection string uses Windows Authentication (Trusted_Connection=True)
- For production, implement JWT authentication/authorization
- Update CORS policy for production environment
- Use HTTPS in production

## 🐛 Troubleshooting

### Database Connection Issues

1. Verify SQL Server is running
2. Check connection string in `appsettings.json`
3. Ensure the database exists (run the SQL script)
4. Check Windows Authentication or SQL Server Authentication settings

### Port Conflicts

If the default ports are in use, update `launchSettings.json`:

```json
{
  "profiles": {
    "https": {
      "applicationUrl": "https://localhost:7001;http://localhost:5001"
    }
  }
}
```

### Entity Framework Issues

If you encounter EF Core errors:

```bash
# Clear and rebuild
dotnet clean
dotnet build

# Update EF Core tools
dotnet tool update --global dotnet-ef
```

## 📝 Additional Commands

### Build for Production

```bash
dotnet publish -c Release -o ./publish
```

### Run Tests (if tests are added)

```bash
dotnet test
```

### Database Migrations (Alternative to SQL Script)

If you want to use EF Core migrations instead:

```bash
# Install EF Core tools
dotnet tool install --global dotnet-ef

# Create initial migration
dotnet ef migrations add InitialCreate

# Update database
dotnet ef database update
```

## 📄 License

This project is created for educational and commercial use.

## 👥 Support

For issues or questions:
- Check the Swagger documentation at the root URL
- Review the error messages in the API responses
- Check the console output for detailed error logs

---

**Happy Coding! 🚀**

