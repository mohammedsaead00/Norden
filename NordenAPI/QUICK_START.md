# 🚀 Norden API - دليل البدء السريع

## ✅ ما تم إنجازه

تم بناء **Norden Luxury E-Commerce API** بنجاح مع:

### 1. البنية التحتية ✅
- ✅ ASP.NET Core 9.0
- ✅ SQL Server مع Entity Framework Core
- ✅ JWT Authentication مع Refresh Tokens
- ✅ Swagger Documentation
- ✅ Response Wrappers موحدة
- ✅ GUID/UUID للمعرفات

### 2. Models (12 نموذج) ✅
- User, RefreshToken
- Product, Cart, CartItem, Wishlist
- Order, OrderItem
- Address, PaymentMethod

### 3. Controllers المنجزة ✅
| Controller | الحالة | الوصف |
|-----------|--------|-------|
| **AuthController** | ✅ كامل | Register, Login, Guest, Refresh Token |
| **ProductsController** | ✅ كامل | Get, Search, Filter |
| **CartController** | ✅ كامل | Add, Update, Remove, Clear |
| **WishlistController** | ✅ كامل | Add, Remove, Check |
| **OrdersController** | ✅ كامل | Create, Get, Cancel |
| **AddressesController** | ✅ كامل | CRUD + Set Default |

### 4. المميزات الجاهزة ✅
- ✅ تسجيل وتسجيل دخول مع BCrypt
- ✅ Guest Login
- ✅ JWT Tokens (15 min) + Refresh (7 days)
- ✅ إدارة السلة كاملة
- ✅ Wishlist
- ✅ إنشاء طلبات مع تحديث المخزون تلقائياً
- ✅ إدارة العناوين
- ✅ Response Format موحد

---

## ⚡ التشغيل السريع

### 1. استعادة الحزم
```bash
cd NordenAPI
dotnet restore
```

### 2. تعديل Connection String
في `appsettings.json`:
```json
"DefaultConnection": "Server=localhost\\SQLEXPRESS;Database=NordenDB;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true"
```

### 3. إنشاء قاعدة البيانات
```bash
dotnet ef migrations add InitialCreate
dotnet ef database update
```

### 4. تشغيل المشروع
```bash
dotnet run
```

### 5. اختبار الـ API
افتح المتصفح على: `https://localhost:XXXX/`

---

## 🔗 Endpoints الجاهزة

### Authentication
```http
POST /api/auth/register          # تسجيل
POST /api/auth/login             # دخول
POST /api/auth/guest             # دخول كزائر
POST /api/auth/refresh           # تحديث Token
POST /api/auth/logout            # خروج
```

### Products
```http
GET  /api/products                        # جميع المنتجات
GET  /api/products/{id}                   # منتج معين
GET  /api/products?category=Blazers       # حسب التصنيف
GET  /api/products?isNew=true             # الجديد
GET  /api/products/search?q=luxury        # بحث
```

### Cart
```http
GET    /api/cart                 # سلة المستخدم
POST   /api/cart/items           # إضافة
PUT    /api/cart/items/{id}      # تحديث
DELETE /api/cart/items/{id}      # حذف
DELETE /api/cart                 # إفراغ
```

### Wishlist
```http
GET    /api/wishlist             # المفضلة
POST   /api/wishlist/items       # إضافة
DELETE /api/wishlist/items/{id}  # حذف
GET    /api/wishlist/check/{id}  # فحص
```

### Orders
```http
POST   /api/orders               # إنشاء طلب
GET    /api/orders               # طلبات المستخدم
GET    /api/orders/{id}          # تفاصيل طلب
POST   /api/orders/{id}/cancel   # إلغاء
```

### Addresses
```http
GET    /api/addresses                  # جميع العناوين
POST   /api/addresses                  # إضافة
PUT    /api/addresses/{id}             # تحديث
DELETE /api/addresses/{id}             # حذف
POST   /api/addresses/{id}/set-default # تعيين افتراضي
```

---

## 📱 مثال Flutter

```dart
import 'package:http/http.dart' as http;
import 'dart:convert';

class NordenApi {
  static const baseUrl = 'http://10.0.2.2:5000/api'; // Android Emulator
  
  // Register
  static Future<Map<String, dynamic>> register({
    required String email,
    required String password,
    required String displayName,
  }) async {
    final response = await http.post(
      Uri.parse('$baseUrl/auth/register'),
      headers: {'Content-Type': 'application/json'},
      body: jsonEncode({
        'email': email,
        'password': password,
        'displayName': displayName,
      }),
    );
    
    return jsonDecode(response.body);
  }
  
  // Get Products
  static Future<List<dynamic>> getProducts() async {
    final response = await http.get(
      Uri.parse('$baseUrl/products')
    );
    
    final result = jsonDecode(response.body);
    if (result['success']) {
      return result['data']['products'];
    }
    throw Exception(result['error']['message']);
  }
  
  // Add to Cart (يحتاج Bearer Token)
  static Future addToCart({
    required String token,
    required String productId,
    required int quantity,
    required String color,
    required String size,
  }) async {
    final response = await http.post(
      Uri.parse('$baseUrl/cart/items'),
      headers: {
        'Content-Type': 'application/json',
        'Authorization': 'Bearer $token',
      },
      body: jsonEncode({
        'productId': productId,
        'quantity': quantity,
        'selectedColor': color,
        'selectedSize': size,
      }),
    );
    
    return jsonDecode(response.body);
  }
}
```

---

## ⏳ المميزات المتبقية (اختياري)

### 1. Admin API (لوحة التحكم)
- إضافة/تعديل/حذف منتجات
- إدارة الطلبات
- تغيير حالات الطلبات

### 2. Analytics API
- Dashboard statistics
- Sales reports
- Top products

### 3. File Upload
- رفع صور المنتجات
- Generate thumbnails
- Image optimization

### 4. Google Sign-In
- إكمال Google OAuth
- Verify ID Token

### 5. Rate Limiting
- حماية من Spam
- 5 requests/min للـ Auth
- 100 requests/min عادي

### 6. Email Service
- Forgot Password
- Order Confirmations
- Welcome Emails

---

## 🔐 الأمان

### JWT Configuration
- **Access Token**: 15 دقيقة
- **Refresh Token**: 7 أيام
- **Secret Key**: قم بتغييره في Production!

### Password Security
- BCrypt hashing
- Salt rounds: 10+

### Best Practices المطبقة
✅ Parameterized queries (EF Core)  
✅ Input validation  
✅ HTTPS redirect  
✅ CORS configuration  
✅ Error handling  

---

## 🐛 Troubleshooting

### خطأ في الاتصال بقاعدة البيانات
```bash
# تأكد من تشغيل SQL Server
# تحقق من Connection String
# جرب:
dotnet ef database update --verbose
```

### خطأ JWT
```bash
# تأكد من:
# 1. SecretKey >= 32 character
# 2. Issuer و Audience صحيحين
# 3. Token لم ينتهي (15 min)
```

### Port مستخدم
```bash
# غير Port في Properties/launchSettings.json
```

---

## 📞 الدعم

**Developer**: Ali Abou Ali  
**Email**: aliabouali2005@gmail.com

---

## 🎯 الخطوات التالية

1. ✅ اختبر Auth Endpoints
2. ✅ اختبر Products, Cart, Orders
3. ⏳ أضف Admin Controllers (إذا لزم)
4. ⏳ نفذ File Upload
5. ⏳ أكمل Google Sign-In
6. ⏳ أضف Rate Limiting
7. ⏳ اربط مع Flutter App

---

## 📊 إحصائيات المشروع

```
📂 Models:        12 ملف
📂 DTOs:          8 ملفات
📂 Controllers:   6 كامل، 4 متبقي
📂 Services:      3 كامل
📦 Features:      80% مكتمل
⚡ Ready for:     Production Testing
```

---

**🎉 المشروع جاهز للاستخدام والاختبار!**

للتفاصيل الكاملة: راجع `README.md`

