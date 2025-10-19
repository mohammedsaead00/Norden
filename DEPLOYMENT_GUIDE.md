# 🚀 دليل النشر السريع - NordenAPI

## 📋 المتطلبات
- ✅ GitHub Repository: `mohammedsaead00/Norden`
- ✅ ملفات النشر جاهزة (Procfile, runtime.txt, railway.json)
- ✅ إعدادات الإنتاج جاهزة

## 🎯 خطوات النشر على Railway

### 1️⃣ إنشاء حساب Railway
1. اذهب إلى: https://railway.app/
2. اضغط "Login" → "Continue with GitHub"
3. سجل دخول بحساب GitHub

### 2️⃣ نشر التطبيق
1. اضغط "New Project"
2. اختر "Deploy from GitHub repo"
3. ابحث عن: `mohammedsaead00/Norden`
4. اضغط "Deploy"

### 3️⃣ إضافة قاعدة البيانات
1. في Railway Dashboard
2. اضغط "+ New" → "Database" → "PostgreSQL"
3. انتظر إنشاء قاعدة البيانات

### 4️⃣ إعداد متغيرات البيئة
في Railway Dashboard → Settings → Variables:

```
ASPNETCORE_ENVIRONMENT=Production
DATABASE_URL=postgresql://username:password@host:port/database
JWT_SECRET_KEY=YourVerySecure32CharacterSecretKeyHere12345
JWT_ISSUER=https://your-app-name.railway.app
JWT_AUDIENCE=https://your-app-name.railway.app
GOOGLE_CLIENT_ID=your_google_client_id
```

### 5️⃣ النتيجة النهائية
- 🌐 API URL: `https://your-app-name.railway.app`
- 📚 Swagger UI: `https://your-app-name.railway.app/swagger`
- 🗄️ Database: متصلة تلقائياً
- 🔒 SSL: مجاني ومفعل

## 🔧 اختبار النشر

### اختبار API:
```bash
curl https://your-app-name.railway.app/api/products
```

### اختبار Swagger:
افتح في المتصفح:
```
https://your-app-name.railway.app/swagger
```

## 📱 استخدام API من التطبيقات

### Base URL:
```
https://your-app-name.railway.app
```

### مثال Flutter:
```dart
final baseUrl = 'https://your-app-name.railway.app';
final response = await http.get(Uri.parse('$baseUrl/api/products'));
```

## 🎉 مميزات النشر السحابي

✅ **متاح 24/7** - لا يحتاج جهازك أن يكون مفتوحاً  
✅ **SSL مجاني** - HTTPS آمن  
✅ **Custom Domain** - يمكن ربط دومين خاص  
✅ **Auto-deploy** - تحديث تلقائي عند push للـ GitHub  
✅ **Database** - قاعدة بيانات سحابية  
✅ **Monitoring** - مراقبة الأداء  
✅ **Logs** - سجلات مفصلة  
✅ **Scaling** - توسع تلقائي حسب الحاجة  

## 🚨 نصائح مهمة

1. **لا تشارك JWT_SECRET_KEY** مع أحد
2. **استخدم HTTPS** دائماً في الإنتاج
3. **راقب Logs** في Railway Dashboard
4. **احفظ Database URL** في مكان آمن
5. **اختبر API** بعد كل تحديث

## 🔄 التحديثات المستقبلية

بعد النشر، أي تغيير ترفعه على GitHub سيتم نشره تلقائياً على Railway!

```bash
git add .
git commit -m "Update API"
git push origin main
# سيتم النشر تلقائياً خلال 2-3 دقائق
```
