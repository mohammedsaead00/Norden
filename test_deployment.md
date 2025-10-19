# 🧪 اختبار النشر - NordenAPI

## بعد النشر على Railway، اختبر:

### 1️⃣ اختبار Swagger UI:
```
افتح: https://your-app-name.railway.app/swagger
```

### 2️⃣ اختبار API:
```bash
curl https://your-app-name.railway.app/api/products
```

### 3️⃣ اختبار من Flutter:
```dart
final baseUrl = 'https://your-app-name.railway.app';
final response = await http.get(Uri.parse('$baseUrl/api/products'));
```

### 4️⃣ اختبار Authentication:
```bash
# Register User
curl -X POST https://your-app-name.railway.app/api/auth/register \
  -H "Content-Type: application/json" \
  -d '{"email":"test@example.com","password":"password123","displayName":"Test User"}'

# Login
curl -X POST https://your-app-name.railway.app/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{"email":"test@example.com","password":"password123"}'
```

## ✅ النجاح يعني:
- Swagger UI يفتح
- API يستجيب
- قاعدة البيانات تعمل
- Authentication يعمل

## 🎉 النتيجة النهائية:
API متكامل يعمل 24/7 على السحابة!
