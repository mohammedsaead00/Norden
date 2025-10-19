# نشر NordenAPI على Render مجاناً

## 1. إنشاء حساب
- اذهب إلى https://render.com/
- سجل بحساب GitHub

## 2. إنشاء Web Service
- اضغط "New +" → "Web Service"
- اختر GitHub repository
- اختر "Build Command": `dotnet publish -c Release -o ./publish`
- اختر "Start Command": `dotnet ./publish/NordenAPI.dll`

## 3. إعدادات البيئة
```
ASPNETCORE_ENVIRONMENT=Production
ASPNETCORE_URLS=http://0.0.0.0:10000
```

## 4. قاعدة البيانات
- استخدم PostgreSQL المجاني
- أو استخدم MongoDB المجاني

## 5. النتيجة
- URL: https://norden-api.onrender.com
- Swagger: https://norden-api.onrender.com/swagger
