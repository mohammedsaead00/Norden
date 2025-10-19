# نشر NordenAPI على Azure مجاناً

## 1. إنشاء حساب Azure
- اذهب إلى https://azure.microsoft.com/free/
- سجل حساب مجاني (يحتاج بطاقة ائتمان للتأكيد)

## 2. إنشاء App Service
```bash
# تثبيت Azure CLI
winget install Microsoft.AzureCLI

# تسجيل الدخول
az login

# إنشاء Resource Group
az group create --name NordenRG --location "East US"

# إنشاء App Service Plan
az appservice plan create --name NordenPlan --resource-group NordenRG --sku FREE

# إنشاء Web App
az webapp create --resource-group NordenRG --plan NordenPlan --name NordenAPI2024 --runtime "DOTNET|9.0"
```

## 3. رفع الكود
```bash
# Build المشروع
dotnet publish -c Release -o ./publish

# رفع الملفات
az webapp deployment source config-zip --resource-group NordenRG --name NordenAPI2024 --src ./publish.zip
```

## 4. إعداد قاعدة البيانات
- استخدم Azure SQL Database (Free Tier: 32MB)
- أو استخدم SQL Server Express محلي

## 5. النتيجة
- URL: https://nordenapi2024.azurewebsites.net
- Swagger: https://nordenapi2024.azurewebsites.net/swagger
