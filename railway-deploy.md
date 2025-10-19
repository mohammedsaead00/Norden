# نشر NordenAPI على Railway مجاناً

## 1. إنشاء حساب
- اذهب إلى https://railway.app/
- سجل بحساب GitHub

## 2. ربط المشروع
- اضغط "New Project"
- اختر "Deploy from GitHub repo"
- اختر repository الخاص بك

## 3. إعدادات النشر
```json
// railway.json
{
  "build": {
    "builder": "NIXPACKS"
  },
  "deploy": {
    "startCommand": "dotnet NordenAPI.dll",
    "healthcheckPath": "/health"
  }
}
```

## 4. متغيرات البيئة
```
ASPNETCORE_ENVIRONMENT=Production
ConnectionStrings__DefaultConnection=your_connection_string
Jwt__SecretKey=your_secret_key
```

## 5. النتيجة
- URL: https://your-project.railway.app
- Swagger: https://your-project.railway.app/swagger
