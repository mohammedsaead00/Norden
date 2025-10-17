# Clothing Store API - Quick Start Guide

## 🎉 Your API is Ready!

Congratulations! Your complete ASP.NET Core Web API for the clothing store has been successfully created.

## 📂 What Was Created

### Project Structure
```
ClothingStoreAPI/
├── Models/          ✅ 12 Entity Models (User, Product, Order, etc.)
├── DTOs/            ✅ 8 DTO Files for clean API contracts
├── Data/            ✅ DbContext with full entity configuration
├── Repositories/    ✅ Repository pattern implementation
├── Services/        ✅ Business logic layer
├── Controllers/     ✅ 8 REST API Controllers
└── README.md        ✅ Complete documentation
```

### Key Features Implemented

✅ **User Management**
- Register/Login endpoints with BCrypt password hashing
- User profile management
- Role-based access (Admin/Customer)

✅ **Product Catalog**
- Full CRUD operations
- Category filtering
- Product search
- Image support
- Inventory tracking with size/color variants

✅ **Shopping Cart**
- Add/Update/Remove items
- User-specific cart management
- Stock validation

✅ **Order Processing**
- Order creation with automatic inventory updates
- Order tracking
- Multiple shipping/billing addresses
- Order status management

✅ **Payment Integration**
- Payment record management
- Multiple payment methods
- Transaction tracking

✅ **Additional Features**
- Address management
- Product reviews (models ready)
- Swagger/OpenAPI documentation
- CORS enabled
- Clean architecture with Repository and Service patterns

## 🚀 How to Run

### Step 1: Update Connection String

Edit `appsettings.json` and update the connection string to match your SQL Server:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=YOUR_SERVER;Database=ClothingStore;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true"
}
```

### Step 2: Ensure Database Exists

Run your SQL script (`file.sql`) to create the `ClothingStore` database if you haven't already.

### Step 3: Restore Packages & Run

```bash
cd ClothingStoreAPI
dotnet restore
dotnet build
dotnet run
```

### Step 4: Access the API

Once running, open your browser and go to:
- **Swagger UI**: `https://localhost:XXXX/` (port will be displayed in console)

## 📡 API Endpoints Overview

### Users API (`/api/users`)
- `POST /api/users/register` - Register new user
- `POST /api/users/login` - Login user
- `GET /api/users` - Get all users
- `GET /api/users/{id}` - Get user by ID
- `PUT /api/users/{id}` - Update user
- `DELETE /api/users/{id}` - Delete user

### Products API (`/api/products`)
- `GET /api/products` - Get all products
- `GET /api/products/{id}` - Get product details
- `GET /api/products/category/{categoryId}` - Get by category
- `GET /api/products/search?query={term}` - Search products
- `POST /api/products` - Create product
- `PUT /api/products/{id}` - Update product
- `DELETE /api/products/{id}` - Delete product

### Categories API (`/api/categories`)
- `GET /api/categories` - Get all categories
- `GET /api/categories/{id}` - Get category by ID
- `POST /api/categories` - Create category
- `PUT /api/categories/{id}` - Update category
- `DELETE /api/categories/{id}` - Delete category

### Cart API (`/api/cart`)
- `GET /api/cart/user/{userId}` - Get user cart
- `POST /api/cart` - Add to cart
- `PUT /api/cart/{cartId}` - Update cart item
- `DELETE /api/cart/{cartId}` - Remove from cart
- `DELETE /api/cart/user/{userId}` - Clear cart

### Orders API (`/api/orders`)
- `GET /api/orders` - Get all orders
- `GET /api/orders/{id}` - Get order details
- `GET /api/orders/user/{userId}` - Get user orders
- `POST /api/orders` - Create order
- `PUT /api/orders/{id}` - Update order status
- `DELETE /api/orders/{id}` - Delete order

### Payments API (`/api/payments`)
- `GET /api/payments` - Get all payments
- `GET /api/payments/{id}` - Get payment by ID
- `GET /api/payments/order/{orderId}` - Get payment by order
- `POST /api/payments` - Create payment
- `PUT /api/payments/{id}` - Update payment
- `DELETE /api/payments/{id}` - Delete payment

### Addresses API (`/api/addresses`)
- `GET /api/addresses` - Get all addresses
- `GET /api/addresses/{id}` - Get address by ID
- `GET /api/addresses/user/{userId}` - Get user addresses
- `POST /api/addresses` - Create address
- `PUT /api/addresses/{id}` - Update address
- `DELETE /api/addresses/{id}` - Delete address

### Order Items API (`/api/orderitems`)
- `GET /api/orderitems/order/{orderId}` - Get order items
- `GET /api/orderitems/{id}` - Get order item by ID

## 🧪 Testing with Swagger

1. **Start the application**: `dotnet run`
2. **Open Swagger UI** in your browser (URL will be shown in console)
3. **Expand any endpoint** to see details
4. **Click "Try it out"** to test the endpoint
5. **Fill in parameters** and click "Execute"
6. **View the response** below

### Example: Register a User

1. Open Swagger UI
2. Navigate to `POST /api/users/register`
3. Click "Try it out"
4. Enter request body:
```json
{
  "email": "test@example.com",
  "password": "Test123!",
  "firstName": "John",
  "lastName": "Doe",
  "phone": "1234567890"
}
```
5. Click "Execute"
6. You should get a 201 Created response with the user details

## 🔧 Architecture Highlights

### Dependency Injection
All services and repositories are registered in `Program.cs`:
- DbContext for database access
- Repository pattern for data access
- Service layer for business logic
- Controllers for API endpoints

### Database Connection
- Uses Entity Framework Core 9.0
- SQL Server provider
- Configured relationships and constraints
- Automatic timestamp updates via triggers

### Security
- BCrypt password hashing
- Input validation using Data Annotations
- DTO pattern for data transfer
- SQL injection protection via EF Core parameterized queries

### Best Practices
- Clean separation of concerns
- Repository pattern
- Service layer pattern
- DTO pattern
- Async/await throughout
- RESTful API design
- Comprehensive error handling

## 📊 Database Schema

The API connects to these database tables:
- **Roles** - User roles
- **Users** - User accounts
- **Addresses** - Shipping/billing addresses
- **Categories** - Product categories (hierarchical)
- **Products** - Product information
- **Product_Images** - Product images
- **Inventory** - Product variants (size/color/stock)
- **Cart** - Shopping cart items
- **Orders** - Customer orders
- **Order_Items** - Order line items
- **Payments** - Payment records
- **Reviews** - Product reviews

## 🛠 Troubleshooting

### Database Connection Error
- Verify SQL Server is running
- Check connection string in `appsettings.json`
- Ensure database exists (run the SQL script)

### Port Already in Use
- Check `Properties/launchSettings.json` to change port
- Or let .NET choose a random available port

### Build Errors
```bash
dotnet clean
dotnet restore
dotnet build
```

## 📝 Next Steps

### Recommended Enhancements
1. **Authentication/Authorization**
   - Implement JWT tokens
   - Add [Authorize] attributes to controllers
   - Role-based access control

2. **Additional Features**
   - Product reviews endpoints
   - Wishlist functionality
   - Order history with filters
   - Password reset functionality
   - Email notifications

3. **Production Readiness**
   - Add logging (Serilog)
   - Implement global error handling
   - Add API rate limiting
   - Configure production CORS policy
   - Add API versioning
   - Implement caching
   - Add unit tests

4. **Database**
   - Consider using migrations instead of SQL script
   - Add database seeding
   - Optimize indexes

## 📚 Additional Resources

- Full documentation: See `README.md`
- Entity Framework Core: https://docs.microsoft.com/ef/core/
- ASP.NET Core: https://docs.microsoft.com/aspnet/core/
- Swagger: https://swagger.io/

---

**🎊 Your API is complete and ready to use! Happy coding!**

