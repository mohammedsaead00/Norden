# 🚀 Railway Environment Variables

## Environment Variables المطلوبة على Railway:

### 1. Database Connection:
```
DATABASE_URL=postgresql://username:password@host:port/database
```

### 2. ASP.NET Core:
```
ASPNETCORE_ENVIRONMENT=Production
```

### 3. JWT Settings (اختياري):
```
JWT_SECRET_KEY=YourVerySecure32CharacterSecretKeyHere12345
JWT_ISSUER=https://api.norden.com
JWT_AUDIENCE=https://norden.com
```

---

## 📋 خطوات الإعداد:

### 1. إضافة PostgreSQL Database:
1. اذهب إلى Railway Dashboard
2. اضغط على مشروعك
3. اضغط "New" → "Database" → "PostgreSQL"
4. انتظر حتى يتم إنشاء Database

### 2. نسخ Connection String:
1. اضغط على PostgreSQL Database
2. اذهب إلى "Connect" tab
3. انسخ "DATABASE_URL"

### 3. إضافة Environment Variables:
1. اذهب إلى مشروعك الرئيسي
2. اضغط على "Variables" tab
3. أضف المتغيرات التالية:

```
DATABASE_URL=postgresql://postgres:password@host:port/database
ASPNETCORE_ENVIRONMENT=Production
```

### 4. إعادة النشر:
Railway سيعيد النشر تلقائياً عند إضافة Environment Variables

---

## ✅ التحقق من النجاح:
- ✅ API يعمل بدون crash
- ✅ Database متصل
- ✅ Migration تم تطبيقه
- ✅ Seed data متاح

---

## 🔗 Railway URLs:
- **API**: `https://your-app.railway.app`
- **Swagger**: `https://your-app.railway.app/swagger`
- **Health**: `https://your-app.railway.app/health`