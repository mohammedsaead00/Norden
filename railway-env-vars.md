# Railway Environment Variables

أضف هذه المتغيرات في Railway Dashboard → Settings → Variables:

```
ASPNETCORE_ENVIRONMENT=Production
DATABASE_URL=postgresql://username:password@host:port/database
JWT_SECRET_KEY=YourVerySecure32CharacterSecretKeyHere12345
JWT_ISSUER=https://your-app-name.railway.app
JWT_AUDIENCE=https://your-app-name.railway.app
```

## كيفية إضافة متغيرات البيئة:

1. اذهب إلى Railway Dashboard
2. اضغط على مشروع Norden
3. اضغط على "Settings"
4. اضغط على "Variables"
5. أضف كل متغير واحد تلو الآخر
6. اضغط "Deploy" لإعادة النشر
