# 🚀 NordenAPI - E-commerce API

## 📋 نظرة عامة
NordenAPI هو API متكامل للتجارة الإلكترونية مبني بـ ASP.NET Core 9.0 مع قاعدة بيانات SQL Server.

## ✨ المميزات
- 🔐 **JWT Authentication** - تسجيل دخول آمن
- 🛍️ **Product Management** - إدارة المنتجات
- 🛒 **Shopping Cart** - عربة التسوق
- 📦 **Order Management** - إدارة الطلبات
- 👤 **User Management** - إدارة المستخدمين
- 📍 **Address Management** - إدارة العناوين
- 💳 **Payment Integration** - تكامل الدفع
- ❤️ **Wishlist** - قائمة الأمنيات

## 🛠️ التقنيات المستخدمة
- **ASP.NET Core 9.0**
- **Entity Framework Core 9.0**
- **SQL Server**
- **JWT Authentication**
- **BCrypt.Net** (Password Hashing)
- **Swagger/OpenAPI**

## 🚀 النشر السريع

### على Railway (مستحسن):
1. اذهب إلى: https://railway.app/
2. اضغط "Deploy from GitHub"
3. اختر repository: `mohammedsaead00/Norden`
4. اضغط "Deploy"

### النتيجة:
- 🌐 API: `https://your-app.railway.app`
- 📚 Swagger: `https://your-app.railway.app/swagger`

## 📱 استخدام API

### Base URL:
```
https://your-app.railway.app
```

### مثال Flutter:
```dart
final baseUrl = 'https://your-app.railway.app';
final response = await http.get(Uri.parse('$baseUrl/api/products'));
```

## 🔧 التطوير المحلي

### المتطلبات:
- .NET 9.0 SDK
- SQL Server Express

### التشغيل:
```bash
cd NordenAPI
dotnet run
```

### Swagger UI:
```
http://localhost:5130/swagger
```

## 📊 API Endpoints

### Authentication:
- `POST /api/auth/register` - تسجيل مستخدم جديد
- `POST /api/auth/login` - تسجيل دخول
- `POST /api/auth/refresh` - تحديث token

### Products:
- `GET /api/products` - جلب جميع المنتجات
- `GET /api/products/{id}` - جلب منتج محدد
- `GET /api/products/search?q={query}` - البحث في المنتجات

### Cart:
- `GET /api/cart` - جلب عربة التسوق
- `POST /api/cart/add` - إضافة منتج للعربة
- `DELETE /api/cart/remove/{id}` - حذف منتج من العربة

### Orders:
- `GET /api/orders` - جلب الطلبات
- `POST /api/orders` - إنشاء طلب جديد
- `GET /api/orders/{id}` - جلب طلب محدد

## 🔒 متغيرات البيئة

```
ASPNETCORE_ENVIRONMENT=Production
DATABASE_URL=postgresql://username:password@host:port/database
JWT_SECRET_KEY=YourVerySecure32CharacterSecretKeyHere12345
JWT_ISSUER=https://your-app.railway.app
JWT_AUDIENCE=https://your-app.railway.app
```

## 📈 المراقبة والـ Logs
- **Railway Dashboard** - مراقبة الأداء
- **Swagger UI** - اختبار API
- **Health Check** - `/health` endpoint

## 🔄 التحديثات
أي تغيير ترفعه على GitHub سيتم نشره تلقائياً على Railway!

```bash
git add .
git commit -m "Update API"
git push origin main
```

## 📞 الدعم
للمساعدة أو الاستفسارات، راجع ملف `DEPLOYMENT_GUIDE.md`

---
**تم النشر بنجاح! 🎉**
