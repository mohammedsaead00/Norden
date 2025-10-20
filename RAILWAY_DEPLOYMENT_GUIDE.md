# 🚀 NordenAPI Railway Deployment Guide

## Quick Deploy to Railway

### Step 1: Prepare Your Repository
1. Ensure all files are committed to your Git repository
2. Push your code to GitHub/GitLab/Bitbucket

### Step 2: Deploy to Railway
1. Go to [Railway.app](https://railway.app)
2. Sign up/Login with your GitHub account
3. Click "New Project" → "Deploy from GitHub repo"
4. Select your repository
5. Railway will automatically detect it's a .NET application
6. Click "Deploy"

### Step 3: Configure Environment Variables (Optional)
- Railway will automatically set the `PORT` environment variable
- No additional configuration needed for basic deployment

### Step 4: Access Your API
- Railway will provide you with a URL like: `https://your-app-name.railway.app`
- Your API will be available at this URL

---

## 📋 API Documentation

### 🌐 API Base URL
```
https://your-app-name.railway.app
```

### 🔐 Authentication Details

#### How to get JWT tokens:
```bash
# Register a new user
POST /api/auth/register
Content-Type: application/json

{
  "email": "user@example.com",
  "password": "password123",
  "name": "John Doe"
}

# Login
POST /api/auth/login
Content-Type: application/json

{
  "email": "user@example.com",
  "password": "password123"
}
```

#### Token Usage:
- Include token in query parameter: `?token=your_token_here`
- Example: `GET /api/cart?token=token_abc123`

#### Token Refresh:
- Current implementation uses simple tokens
- For production, implement proper JWT with refresh tokens

#### Special Headers:
- `Content-Type: application/json` for POST/PUT requests
- No special authentication headers required (uses query parameter)

### 📚 API Documentation

#### Swagger/OpenAPI Documentation:
```
https://your-app-name.railway.app/swagger
```

#### Available Endpoints:

**Public Endpoints:**
- `GET /` - Health check
- `GET /health` - Health status
- `GET /api/test` - API test endpoint
- `GET /api/products` - List all products
- `GET /api/products/{id}` - Get product by ID
- `GET /api/products/search?q={query}` - Search products
- `GET /api/categories` - List all categories
- `GET /api/products/{id}/reviews` - Get product reviews

**Authentication Endpoints:**
- `POST /api/auth/register` - Register new user
- `POST /api/auth/login` - Login user
- `GET /api/auth/profile?token={token}` - Get user profile

**Protected Endpoints (require token):**
- `GET /api/cart?token={token}` - Get user cart
- `POST /api/cart/add` - Add item to cart
- `GET /api/orders?token={token}` - Get user orders
- `POST /api/orders` - Create new order
- `GET /api/wishlist?token={token}` - Get user wishlist
- `POST /api/wishlist/add` - Add item to wishlist
- `POST /api/products/{id}/reviews` - Add product review

### 🗄️ Database Schema

**Note:** Current implementation uses in-memory data. For production, you'll need to add a database.

#### Recommended MySQL Tables:

```sql
-- Users table
CREATE TABLE Users (
    Id CHAR(36) PRIMARY KEY,
    Email VARCHAR(255) UNIQUE NOT NULL,
    PasswordHash VARCHAR(255) NOT NULL,
    Name VARCHAR(255) NOT NULL,
    CreatedAt DATETIME DEFAULT CURRENT_TIMESTAMP,
    UpdatedAt DATETIME DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP
);

-- Products table
CREATE TABLE Products (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    Name VARCHAR(255) NOT NULL,
    Price DECIMAL(10,2) NOT NULL,
    Category VARCHAR(100) NOT NULL,
    Description TEXT,
    Image VARCHAR(500),
    Stock INT DEFAULT 0,
    IsNew BOOLEAN DEFAULT FALSE,
    IsFeatured BOOLEAN DEFAULT FALSE,
    CreatedAt DATETIME DEFAULT CURRENT_TIMESTAMP,
    UpdatedAt DATETIME DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    INDEX idx_category (Category),
    INDEX idx_featured (IsFeatured),
    INDEX idx_new (IsNew)
);

-- Categories table
CREATE TABLE Categories (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    Name VARCHAR(100) UNIQUE NOT NULL,
    Image VARCHAR(500),
    ProductCount INT DEFAULT 0,
    CreatedAt DATETIME DEFAULT CURRENT_TIMESTAMP
);

-- Cart Items table
CREATE TABLE CartItems (
    Id CHAR(36) PRIMARY KEY,
    UserId CHAR(36) NOT NULL,
    ProductId INT NOT NULL,
    Quantity INT NOT NULL,
    CreatedAt DATETIME DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (UserId) REFERENCES Users(Id) ON DELETE CASCADE,
    FOREIGN KEY (ProductId) REFERENCES Products(Id) ON DELETE CASCADE,
    UNIQUE KEY unique_user_product (UserId, ProductId)
);

-- Orders table
CREATE TABLE Orders (
    Id CHAR(36) PRIMARY KEY,
    OrderNumber VARCHAR(50) UNIQUE NOT NULL,
    UserId CHAR(36) NOT NULL,
    Status ENUM('Pending', 'Processing', 'Shipped', 'Delivered', 'Cancelled') DEFAULT 'Pending',
    Total DECIMAL(10,2) NOT NULL,
    CreatedAt DATETIME DEFAULT CURRENT_TIMESTAMP,
    UpdatedAt DATETIME DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    FOREIGN KEY (UserId) REFERENCES Users(Id) ON DELETE CASCADE,
    INDEX idx_user (UserId),
    INDEX idx_status (Status)
);

-- Order Items table
CREATE TABLE OrderItems (
    Id CHAR(36) PRIMARY KEY,
    OrderId CHAR(36) NOT NULL,
    ProductId INT NOT NULL,
    ProductName VARCHAR(255) NOT NULL,
    Price DECIMAL(10,2) NOT NULL,
    Quantity INT NOT NULL,
    Image VARCHAR(500),
    FOREIGN KEY (OrderId) REFERENCES Orders(Id) ON DELETE CASCADE,
    FOREIGN KEY (ProductId) REFERENCES Products(Id) ON DELETE CASCADE
);

-- Wishlist table
CREATE TABLE Wishlist (
    Id CHAR(36) PRIMARY KEY,
    UserId CHAR(36) NOT NULL,
    ProductId INT NOT NULL,
    AddedAt DATETIME DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (UserId) REFERENCES Users(Id) ON DELETE CASCADE,
    FOREIGN KEY (ProductId) REFERENCES Products(Id) ON DELETE CASCADE,
    UNIQUE KEY unique_user_product (UserId, ProductId)
);

-- Reviews table
CREATE TABLE Reviews (
    Id CHAR(36) PRIMARY KEY,
    ProductId INT NOT NULL,
    UserId CHAR(36) NOT NULL,
    UserName VARCHAR(255) NOT NULL,
    Rating INT CHECK (Rating >= 1 AND Rating <= 5),
    Comment TEXT,
    CreatedAt DATETIME DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (ProductId) REFERENCES Products(Id) ON DELETE CASCADE,
    FOREIGN KEY (UserId) REFERENCES Users(Id) ON DELETE CASCADE,
    INDEX idx_product (ProductId),
    INDEX idx_rating (Rating)
);
```

### 📁 File Upload Details

**Current Status:** File uploads are not implemented in the current version.

**Recommended Implementation:**
1. Use Railway's file storage or integrate with cloud storage (AWS S3, Cloudinary)
2. Add file upload endpoints for product images
3. Implement image processing and resizing
4. Add file validation and security measures

**Example Implementation:**
```csharp
// Add to Program.cs
app.MapPost("/api/upload", async (IFormFile file) =>
{
    if (file == null || file.Length == 0)
        return Results.BadRequest("No file uploaded");

    var fileName = $"{Guid.NewGuid()}_{file.FileName}";
    var filePath = Path.Combine("uploads", fileName);
    
    using (var stream = new FileStream(filePath, FileMode.Create))
    {
        await file.CopyToAsync(stream);
    }
    
    return Results.Ok(new { fileName, filePath });
});
```

---

## 🚀 Deployment Commands

### Manual Deployment (if needed):
```bash
# Install Railway CLI
npm install -g @railway/cli

# Login to Railway
railway login

# Link to your project
railway link

# Deploy
railway up
```

### Environment Variables:
```bash
# Railway automatically sets:
PORT=8080

# Optional custom variables:
ASPNETCORE_ENVIRONMENT=Production
```

---

## 🔧 Troubleshooting

### Common Issues:

1. **Port Issues:**
   - Ensure your app uses `$PORT` environment variable
   - Check that start.sh is executable

2. **Build Failures:**
   - Verify .NET 8.0 is available
   - Check for missing dependencies

3. **Runtime Errors:**
   - Check Railway logs in dashboard
   - Verify all environment variables are set

### Logs:
- View logs in Railway dashboard
- Use `railway logs` command if using CLI

---

## 📞 Support

- **Railway Documentation:** https://docs.railway.app
- **API Documentation:** `https://your-app-name.railway.app/swagger`
- **Health Check:** `https://your-app-name.railway.app/health`

---

## 🎯 Next Steps

1. **Add Database:** Integrate MySQL/PostgreSQL for persistent data
2. **Implement JWT:** Add proper JWT authentication
3. **File Uploads:** Add image upload functionality
4. **Email Service:** Add email notifications
5. **Payment Integration:** Add payment processing
6. **Monitoring:** Add application monitoring and logging
7. **Testing:** Add comprehensive test suite
8. **CI/CD:** Set up automated deployment pipeline

---

**Your API will be live at:** `https://your-app-name.railway.app` 🚀
