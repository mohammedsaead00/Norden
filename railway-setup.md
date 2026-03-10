# 🗄️ إعداد PostgreSQL Database على Railway

## الخطوات المطلوبة:

### 1. إضافة PostgreSQL Database:
1. اذهب إلى Railway Dashboard
2. اضغط على مشروعك
3. اضغط "New" → "Database" → "PostgreSQL"
4. ستحصل على Connection String

### 2. إضافة Environment Variables:
- `DATABASE_URL` = Connection String من PostgreSQL
- `ASPNETCORE_ENVIRONMENT` = "Production"

### 3. تحديث Connection String:
سيتم تحديث `appsettings.json` تلقائياً لاستخدام `DATABASE_URL`

---

## 🔗 Railway PostgreSQL Setup:
1. **Add Database**: New → Database → PostgreSQL
2. **Copy Connection String**: من Database settings
3. **Add Environment Variable**: `DATABASE_URL`
4. **Deploy**: Railway سيعيد النشر تلقائياً

---

## 📋 Environment Variables المطلوبة:
```
DATABASE_URL=postgresql://username:password@host:port/database
ASPNETCORE_ENVIRONMENT=Production
```
