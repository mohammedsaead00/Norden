# نشر NordenAPI على Heroku مجاناً

## 1. تثبيت Heroku CLI
```bash
# تحميل من https://devcenter.heroku.com/articles/heroku-cli
# أو
winget install Heroku.HerokuCLI
```

## 2. إنشاء المشروع
```bash
# تسجيل الدخول
heroku login

# إنشاء تطبيق
heroku create norden-api-2024

# إضافة Buildpack
heroku buildpacks:set https://github.com/jincod/dotnetcore-buildpack
```

## 3. ملفات مطلوبة
```bash
# Procfile
echo "web: dotnet NordenAPI.dll --urls http://0.0.0.0:\$PORT" > Procfile

# runtime.txt
echo "dotnet-9.0.0" > runtime.txt
```

## 4. رفع الكود
```bash
git add .
git commit -m "Deploy to Heroku"
git push heroku main
```

## 5. النتيجة
- URL: https://norden-api-2024.herokuapp.com
- Swagger: https://norden-api-2024.herokuapp.com/swagger
