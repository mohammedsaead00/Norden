# Norden Luxury E-Commerce API 🎩

API كامل لمتجر Norden للأزياء الفاخرة، مبني بـ **ASP.NET Core 9** و **MySQL** و **JWT Authentication**.

## 📋 جدول المحتويات

- [نظرة عامة](#نظرة-عامة)
- [التقنيات المستخدمة](#التقنيات-المستخدمة)
- [المميزات المنفذة](#المميزات-المنفذة)
- [التثبيت والتشغيل](#التثبيت-والتشغيل)
- [قاعدة البيانات](#قاعدة-البيانات)
- [API Endpoints](#api-endpoints)
- [المميزات الإضافية المطلوبة](#المميزات-الإضافية-المطلوبة)
- [أمثلة على الاستخدام](#أمثلة-على-الاستخدام)
- [Flutter Integration](#flutter-integration)

---

## 🎯 نظرة عامة

هذا المشروع يوفر REST API كامل لتطبيق Norden Luxury للتجارة الإلكترونية. يدعم:

✅ **Authentication**: JWT مع Refresh Tokens، تسجيل، تسجيل دخول، Google Sign-In، Guest Login  
✅ **Products**: CRUD كامل، بحث، تصنيفات، featured/new  
✅ **Cart & Wishlist**: إدارة السلة والمفضلة  
✅ **Orders**: إنشاء طلبات، تتبع، تاريخ الطلبات  
✅ **User Profile**: إدارة الملف الشخصي، الصور  
✅ **Addresses**: عناوين متعددة للشحن والفواتير  
✅ **Response Format**: موحد `{success, data, error}`  
✅ **UUID**: جميع المعرفات بـ GUID  

---

## 🛠 التقنيات المستخدمة

| التقنية | الاستخدام |
|---------|----------|
| **ASP.NET Core 9** | Web API Framework |
| **SQL Server** | قاعدة البيانات |
| **Entity Framework Core 9** | ORM |
| **JWT Bearer Authentication** | المصادقة |
| **BCrypt.Net** | تشفير كلمات المرور |
| **Swagger/OpenAPI** | توثيق تفاعلي |
| **GUID/UUID** | المعرفات الفريدة |

---

## ✅ المميزات المنفذة

### 1. Models (كامل) ✅
- ✅ User - مع دعم Google Sign-In و Guest
- ✅ RefreshToken - JWT Refresh Tokens
- ✅ Product - مع JSON arrays للصور والألوان
- ✅ Cart & CartItem
- ✅ Wishlist
- ✅ Order & OrderItem
- ✅ Address
- ✅ PaymentMethod

### 2. DTOs (كامل) ✅
- ✅ ApiResponse<T> - Response Wrapper موحد
- ✅ AuthDtos - Register, Login, Google, Guest
- ✅ ProductDtos
- ✅ CartDtos
- ✅ OrderDtos
- ✅ UserDtos
- ✅ AddressDtos
- ✅ WishlistDtos

### 3. Services (جزئي) ⚡
- ✅ JwtService - توليد والتحقق من Tokens
- ✅ AuthService - كامل (Register, Login, Guest, Refresh)
- ⏳ ProductService
- ⏳ CartService
- ⏳ OrderService
- ⏳ FileUploadService

### 4. Controllers (معظمها جاهز) ✅
- ✅ AuthController - كامل
- ✅ ProductsController - كامل (Get, Search)
- ✅ CartController - كامل
- ✅ WishlistController - كامل
- ✅ OrdersController - كامل
- ✅ AddressesController - كامل
- ⏳ UserProfileController
- ⏳ PaymentMethodsController
- ⏳ AdminController
- ⏳ AnalyticsController

### 5. Configuration (كامل) ✅
- ✅ Program.cs - JWT, DbContext, Swagger
- ✅ appsettings.json
- ✅ DbContext configuration

---

## 🚀 التثبيت والتشغيل

### المتطلبات:
- .NET 9.0 SDK
- MySQL Server 8.0+
- Visual Studio 2022 أو VS Code

### 1. Clone/Download المشروع
```bash
cd NordenAPI
```

### 2. تعديل Connection String

في `appsettings.json`:
```json
"ConnectionStrings": {
  "DefaultConnection": "Server=localhost\\SQLEXPRESS;Database=NordenDB;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true"
}
```

أو للـ SQL Server بـ username/password:
```json
"ConnectionStrings": {
  "DefaultConnection": "Server=YOUR_SERVER;Database=NordenDB;User Id=sa;Password=YourPassword;TrustServerCertificate=True;MultipleActiveResultSets=true"
}
```

### 3. تعديل JWT Secret Key

في `appsettings.json`:
```json
"Jwt": {
  "SecretKey": "YourVerySecure32CharacterSecretKeyHere12345"
}
```

⚠️ **مهم**: غير الـ SecretKey في الإنتاج!

### 4. إنشاء قاعدة البيانات

```bash
dotnet ef migrations add InitialCreate
dotnet ef database update
```

أو استخدم SQL Script مباشرة (انظر قسم قاعدة البيانات).

### 5. تشغيل المشروع

```bash
dotnet restore
dotnet build
dotnet run
```

### 6. الوصول للـ API

- **Swagger UI**: `https://localhost:XXXX/`
- **API Base**: `https://localhost:XXXX/api`

---

## 🗄 قاعدة البيانات

### الطريقة الموصى بها: EF Core Migrations

```bash
# في مجلد المشروع
dotnet ef migrations add InitialCreate
dotnet ef database update
```

### إنشاء قاعدة البيانات يدوياً (SQL Server)

```sql
CREATE DATABASE NordenDB;
GO

USE NordenDB;
GO

-- Users table
CREATE TABLE users (
    id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    email NVARCHAR(255) NOT NULL UNIQUE,
    password_hash NVARCHAR(255),
    display_name NVARCHAR(255) NOT NULL,
    phone_number NVARCHAR(20),
    photo_url NVARCHAR(500),
    is_guest BIT DEFAULT 0,
    is_admin BIT DEFAULT 0,
    google_id NVARCHAR(100),
    created_at DATETIME2 DEFAULT GETUTCDATE(),
    updated_at DATETIME2 DEFAULT GETUTCDATE()
);
CREATE INDEX idx_email ON users(email);
CREATE INDEX idx_google_id ON users(google_id);
GO

-- Products table
CREATE TABLE products (
    id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    name NVARCHAR(255) NOT NULL,
    description NVARCHAR(MAX),
    price DECIMAL(10,2) NOT NULL,
    category NVARCHAR(50) NOT NULL,
    images NVARCHAR(MAX),
    colors NVARCHAR(MAX),
    sizes NVARCHAR(MAX),
    stock INT DEFAULT 0,
    is_new BIT DEFAULT 0,
    is_featured BIT DEFAULT 0,
    created_at DATETIME2 DEFAULT GETUTCDATE(),
    updated_at DATETIME2 DEFAULT GETUTCDATE()
);
CREATE INDEX idx_category ON products(category);
CREATE INDEX idx_is_new ON products(is_new);
CREATE INDEX idx_is_featured ON products(is_featured);
GO

-- Carts table
CREATE TABLE carts (
    id CHAR(36) PRIMARY KEY,
    user_id CHAR(36) NOT NULL,
    created_at DATETIME DEFAULT CURRENT_TIMESTAMP,
    updated_at DATETIME DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    FOREIGN KEY (user_id) REFERENCES users(id) ON DELETE CASCADE
);

-- Cart Items table
CREATE TABLE cart_items (
    id CHAR(36) PRIMARY KEY,
    cart_id CHAR(36) NOT NULL,
    product_id CHAR(36) NOT NULL,
    quantity INT NOT NULL DEFAULT 1,
    selected_color VARCHAR(50) NOT NULL,
    selected_size VARCHAR(10) NOT NULL,
    price DECIMAL(10,2) NOT NULL,
    added_at DATETIME DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (cart_id) REFERENCES carts(id) ON DELETE CASCADE,
    FOREIGN KEY (product_id) REFERENCES products(id)
);

-- Wishlist table
CREATE TABLE wishlists (
    id CHAR(36) PRIMARY KEY,
    user_id CHAR(36) NOT NULL,
    product_id CHAR(36) NOT NULL,
    added_at DATETIME DEFAULT CURRENT_TIMESTAMP,
    UNIQUE KEY unique_wishlist (user_id, product_id),
    FOREIGN KEY (user_id) REFERENCES users(id) ON DELETE CASCADE,
    FOREIGN KEY (product_id) REFERENCES products(id) ON DELETE CASCADE
);

-- Addresses table
CREATE TABLE addresses (
    id CHAR(36) PRIMARY KEY,
    user_id CHAR(36) NOT NULL,
    label VARCHAR(50) NOT NULL,
    name VARCHAR(255) NOT NULL,
    phone VARCHAR(20) NOT NULL,
    street VARCHAR(500) NOT NULL,
    city VARCHAR(100) NOT NULL,
    country VARCHAR(100) NOT NULL,
    is_default BOOLEAN DEFAULT FALSE,
    created_at DATETIME DEFAULT CURRENT_TIMESTAMP,
    updated_at DATETIME DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    FOREIGN KEY (user_id) REFERENCES users(id) ON DELETE CASCADE
);

-- Orders table
CREATE TABLE orders (
    id CHAR(36) PRIMARY KEY,
    order_number VARCHAR(50) NOT NULL UNIQUE,
    user_id CHAR(36) NOT NULL,
    status ENUM('Pending', 'Processing', 'Shipped', 'Delivered', 'Cancelled') DEFAULT 'Pending',
    subtotal DECIMAL(10,2) NOT NULL,
    tax DECIMAL(10,2) DEFAULT 0,
    shipping DECIMAL(10,2) DEFAULT 0,
    total DECIMAL(10,2) NOT NULL,
    payment_method ENUM('Card', 'CashOnDelivery') NOT NULL,
    shipping_address_id CHAR(36) NOT NULL,
    created_at DATETIME DEFAULT CURRENT_TIMESTAMP,
    updated_at DATETIME DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    INDEX idx_order_number (order_number),
    INDEX idx_user_id (user_id),
    INDEX idx_status (status),
    FOREIGN KEY (user_id) REFERENCES users(id),
    FOREIGN KEY (shipping_address_id) REFERENCES addresses(id)
);

-- Order Items table
CREATE TABLE order_items (
    id CHAR(36) PRIMARY KEY,
    order_id CHAR(36) NOT NULL,
    product_id CHAR(36) NOT NULL,
    product_name VARCHAR(255) NOT NULL,
    product_image VARCHAR(500),
    quantity INT NOT NULL,
    selected_color VARCHAR(50) NOT NULL,
    selected_size VARCHAR(10) NOT NULL,
    price DECIMAL(10,2) NOT NULL,
    subtotal DECIMAL(10,2) NOT NULL,
    FOREIGN KEY (order_id) REFERENCES orders(id) ON DELETE CASCADE,
    FOREIGN KEY (product_id) REFERENCES products(id)
);

-- Payment Methods table
CREATE TABLE payment_methods (
    id CHAR(36) PRIMARY KEY,
    user_id CHAR(36) NOT NULL,
    type VARCHAR(20) DEFAULT 'card',
    card_last4 VARCHAR(4) NOT NULL,
    card_brand VARCHAR(50) NOT NULL,
    expiry_month INT NOT NULL,
    expiry_year INT NOT NULL,
    is_default BOOLEAN DEFAULT FALSE,
    created_at DATETIME DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (user_id) REFERENCES users(id) ON DELETE CASCADE
);

-- Refresh Tokens table
CREATE TABLE refresh_tokens (
    id CHAR(36) PRIMARY KEY,
    user_id CHAR(36) NOT NULL,
    token VARCHAR(255) NOT NULL,
    expires_at DATETIME NOT NULL,
    created_at DATETIME DEFAULT CURRENT_TIMESTAMP,
    revoked_at DATETIME,
    INDEX idx_token (token),
    FOREIGN KEY (user_id) REFERENCES users(id) ON DELETE CASCADE
);
```

---

## 📡 API Endpoints

### 🔐 Authentication (`/api/auth`)

| Method | Endpoint | الوصف | تم التنفيذ |
|--------|----------|-------|-----------|
| POST | `/auth/register` | تسجيل مستخدم جديد | ✅ |
| POST | `/auth/login` | تسجيل الدخول | ✅ |
| POST | `/auth/guest` | تسجيل دخول زائر | ✅ |
| POST | `/auth/google` | Google Sign-In | ⚡ (Placeholder) |
| POST | `/auth/forgot-password` | نسيت كلمة المرور | ⚡ (Placeholder) |
| POST | `/auth/logout` | تسجيل الخروج | ✅ |
| POST | `/auth/refresh` | تحديث الـ Token | ✅ |

### 🛍 Products (`/api/products`)

| Method | Endpoint | الوصف | تم التنفيذ |
|--------|----------|-------|-----------|
| GET | `/products` | جلب جميع المنتجات | ✅ |
| GET | `/products/{id}` | جلب منتج معين | ✅ |
| GET | `/products/search?q={term}` | بحث المنتجات | ✅ |

### 🛒 Cart (`/api/cart`)

| Method | Endpoint | الوصف | تم التنفيذ |
|--------|----------|-------|-----------|
| GET | `/cart` | جلب سلة المستخدم | ⏳ |
| POST | `/cart/items` | إضافة للسلة | ⏳ |
| PUT | `/cart/items/{id}` | تحديث عنصر | ⏳ |
| DELETE | `/cart/items/{id}` | حذف عنصر | ⏳ |
| DELETE | `/cart` | إفراغ السلة | ⏳ |

### ❤️ Wishlist (`/api/wishlist`)

| Method | Endpoint | الوصف | تم التنفيذ |
|--------|----------|-------|-----------|
| GET | `/wishlist` | جلب المفضلة | ⏳ |
| POST | `/wishlist/items` | إضافة للمفضلة | ⏳ |
| DELETE | `/wishlist/items/{id}` | حذف من المفضلة | ⏳ |
| GET | `/wishlist/check/{id}` | التحقق من وجود منتج | ⏳ |

### 📦 Orders (`/api/orders`)

| Method | Endpoint | الوصف | تم التنفيذ |
|--------|----------|-------|-----------|
| GET | `/orders` | جلب طلبات المستخدم | ⏳ |
| GET | `/orders/{id}` | تفاصيل طلب | ⏳ |
| POST | `/orders` | إنشاء طلب | ⏳ |
| POST | `/orders/{id}/cancel` | إلغاء طلب | ⏳ |

### 👤 User Profile (`/api/users`)

| Method | Endpoint | الوصف | تم التنفيذ |
|--------|----------|-------|-----------|
| GET | `/users/profile` | جلب الملف الشخصي | ⏳ |
| PUT | `/users/profile` | تحديث الملف | ⏳ |
| POST | `/users/profile/photo` | رفع صورة | ⏳ |

### 📍 Addresses (`/api/addresses`)

| Method | Endpoint | الوصف | تم التنفيذ |
|--------|----------|-------|-----------|
| GET | `/addresses` | جلب العناوين | ⏳ |
| POST | `/addresses` | إضافة عنوان | ⏳ |
| PUT | `/addresses/{id}` | تحديث عنوان | ⏳ |
| DELETE | `/addresses/{id}` | حذف عنوان | ⏳ |
| POST | `/addresses/{id}/set-default` | تعيين كافتراضي | ⏳ |

### 💳 Payment Methods (`/api/payment-methods`)

| Method | Endpoint | الوصف | تم التنفيذ |
|--------|----------|-------|-----------|
| GET | `/payment-methods` | جلب طرق الدفع | ⏳ |
| POST | `/payment-methods` | إضافة طريقة دفع | ⏳ |
| DELETE | `/payment-methods/{id}` | حذف طريقة | ⏳ |

### 👨‍💼 Admin API (`/api/admin`)

| Method | Endpoint | الوصف | تم التنفيذ |
|--------|----------|-------|-----------|
| POST | `/admin/products` | إنشاء منتج | ⏳ |
| PUT | `/admin/products/{id}` | تحديث منتج | ⏳ |
| DELETE | `/admin/products/{id}` | حذف منتج | ⏳ |
| GET | `/admin/orders` | جميع الطلبات | ⏳ |
| PUT | `/admin/orders/{id}/status` | تحديث حالة طلب | ⏳ |

### 📊 Analytics (`/api/admin/analytics`)

| Method | Endpoint | الوصف | تم التنفيذ |
|--------|----------|-------|-----------|
| GET | `/admin/analytics/dashboard` | إحصائيات Dashboard | ⏳ |
| GET | `/admin/analytics/sales` | تقرير المبيعات | ⏳ |

---

## 🔨 المميزات الإضافية المطلوبة

### 1. إكمال Controllers المتبقية

انظر ملف `IMPLEMENTATION_GUIDE.md` للأمثلة الكاملة.

### 2. File Upload Service

```csharp
// Services/IFileUploadService.cs
public interface IFileUploadService
{
    Task<string> UploadProductImageAsync(IFormFile file, string productId);
    Task<List<string>> GenerateThumbnailsAsync(string imagePath);
    Task<bool> DeleteImageAsync(string imageUrl);
}
```

### 3. Google Sign-In Implementation

في `AuthService.cs`، أكمل:
```csharp
public async Task<AuthResponse?> GoogleSignInAsync(string idToken)
{
    // Install: Google.Apis.Auth
    var payload = await GoogleJsonWebSignature.ValidateAsync(idToken);
    
    // Find or create user
    var user = await _context.Users.FirstOrDefaultAsync(u => u.GoogleId == payload.Subject);
    
    if (user == null)
    {
        user = new User
        {
            Email = payload.Email,
            DisplayName = payload.Name,
            PhotoURL = payload.Picture,
            GoogleId = payload.Subject,
            IsGuest = false
        };
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
    }
    
    // Generate tokens...
}
```

### 4. Rate Limiting

```csharp
// في Program.cs
builder.Services.AddMemoryCache();
builder.Services.Configure<IpRateLimitOptions>(builder.Configuration.GetSection("IpRateLimiting"));
builder.Services.AddSingleton<IIpPolicyStore, MemoryCacheIpPolicyStore>();
builder.Services.AddSingleton<IRateLimitCounterStore, MemoryCacheRateLimitCounterStore>();
builder.Services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();
builder.Services.AddSingleton<IProcessingStrategy, AsyncKeyLockProcessingStrategy>();
builder.Services.AddInMemoryRateLimiting();

// في appsettings.json
"IpRateLimiting": {
  "EnableEndpointRateLimiting": true,
  "StackBlockedRequests": false,
  "RealIpHeader": "X-Real-IP",
  "ClientIdHeader": "X-ClientId",
  "HttpStatusCode": 429,
  "GeneralRules": [
    {
      "Endpoint": "*/auth/*",
      "Period": "1m",
      "Limit": 5
    },
    {
      "Endpoint": "*",
      "Period": "1m",
      "Limit": 100
    }
  ]
}
```

---

## 💡 أمثلة على الاستخدام

### 1. Register User

```http
POST /api/auth/register
Content-Type: application/json

{
  "email": "user@norden.com",
  "password": "SecurePass123",
  "displayName": "John Doe",
  "phoneNumber": "+201234567890"
}
```

**Response:**
```json
{
  "success": true,
  "data": {
    "userId": "123e4567-e89b-12d3-a456-426614174000",
    "email": "user@norden.com",
    "displayName": "John Doe",
    "isAdmin": false,
    "isGuest": false,
    "token": "eyJhbGciOiJIUzI1NiIs...",
    "refreshToken": "abc123..."
  }
}
```

### 2. Login

```http
POST /api/auth/login
Content-Type: application/json

{
  "email": "user@norden.com",
  "password": "SecurePass123"
}
```

### 3. Get Products

```http
GET /api/products?category=Blazers&limit=20&offset=0
```

### 4. Search Products

```http
GET /api/products/search?q=luxury&category=Blazers
```

---

## 📱 Flutter Integration

### استخدام الـ API من Flutter

```dart
import 'package:http/http.dart' as http;
import 'dart:convert';

class NordenApiService {
  static const String baseUrl = 'http://10.0.2.2:5000/api'; // Android Emulator
  // للجهاز الحقيقي: http://YOUR_IP:5000/api
  
  String? _accessToken;
  
  // Register
  Future<Map<String, dynamic>> register({
    required String email,
    required String password,
    required String displayName,
    String? phoneNumber,
  }) async {
    final url = Uri.parse('$baseUrl/auth/register');
    
    final response = await http.post(
      url,
      headers: {'Content-Type': 'application/json'},
      body: jsonEncode({
        'email': email,
        'password': password,
        'displayName': displayName,
        'phoneNumber': phoneNumber,
      }),
    );
    
    final result = jsonDecode(response.body);
    
    if (result['success']) {
      _accessToken = result['data']['token'];
      // Save refreshToken securely
    }
    
    return result;
  }
  
  // Get Products
  Future<List<dynamic>> getProducts({
    String? category,
    bool? isFeatured,
    bool? isNew,
    int limit = 50,
    int offset = 0,
  }) async {
    var url = '$baseUrl/products?limit=$limit&offset=$offset';
    if (category != null) url += '&category=$category';
    if (isFeatured != null) url += '&isFeatured=$isFeatured';
    if (isNew != null) url += '&isNew=$isNew';
    
    final response = await http.get(Uri.parse(url));
    final result = jsonDecode(response.body);
    
    if (result['success']) {
      return result['data']['products'];
    }
    
    throw Exception(result['error']['message']);
  }
  
  // Add to Cart (requires authentication)
  Future<Map<String, dynamic>> addToCart({
    required String productId,
    required int quantity,
    required String selectedColor,
    required String selectedSize,
  }) async {
    final url = Uri.parse('$baseUrl/cart/items');
    
    final response = await http.post(
      url,
      headers: {
        'Content-Type': 'application/json',
        'Authorization': 'Bearer $_accessToken',
      },
      body: jsonEncode({
        'productId': productId,
        'quantity': quantity,
        'selectedColor': selectedColor,
        'selectedSize': selectedSize,
      }),
    );
    
    return jsonDecode(response.body);
  }
}
```

---

## 🔒 الأمان

### JWT Configuration
- Access Token: 15 دقيقة
- Refresh Token: 7 أيام
- تشفير كلمات المرور: BCrypt

### Best Practices
1. ✅ استخدم HTTPS في Production
2. ✅ غير JWT SecretKey
3. ✅ فعّل Rate Limiting
4. ✅ Validate جميع Inputs
5. ✅ استخدم Parameterized Queries (EF Core يفعل هذا تلقائياً)

---

## 📞 الدعم

- **Developer**: Ali Abou Ali
- **Email**: aliabouali2005@gmail.com

---

## 📄 License

© 2025 Norden Luxury E-Commerce. All rights reserved.

---

## 🚀 الخطوات التالية

1. ✅ اختبر Auth Endpoints
2. ⏳ أكمل Cart & Wishlist Controllers
3. ⏳ أكمل Orders Controller
4. ⏳ نفذ File Upload Service
5. ⏳ أضف Admin & Analytics APIs
6. ⏳ فعّل Rate Limiting
7. ⏳ أكمل Google Sign-In
8. ⏳ اختبر مع Flutter App

**للتفاصيل الكاملة**: انظر `IMPLEMENTATION_GUIDE.md`

