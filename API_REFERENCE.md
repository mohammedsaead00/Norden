# 📚 NordenAPI Reference

## 🌐 Base URL
```
https://your-app-name.railway.app
```

## 🔐 Authentication

### Token-based Authentication
- **Method:** Query parameter
- **Parameter:** `token`
- **Example:** `GET /api/cart?token=your_token_here`

### Getting Tokens

#### Register User
```http
POST /api/auth/register
Content-Type: application/json

{
  "email": "user@example.com",
  "password": "password123",
  "name": "John Doe"
}
```

**Response:**
```json
{
  "user": {
    "id": "uuid",
    "email": "user@example.com",
    "name": "John Doe",
    "createdAt": "2024-01-01T00:00:00Z"
  },
  "token": "token_abc123def456",
  "message": "Registration successful"
}
```

#### Login User
```http
POST /api/auth/login
Content-Type: application/json

{
  "email": "user@example.com",
  "password": "password123"
}
```

**Response:**
```json
{
  "user": {
    "id": "uuid",
    "email": "user@example.com",
    "name": "John Doe",
    "createdAt": "2024-01-01T00:00:00Z"
  },
  "token": "token_abc123def456",
  "message": "Login successful"
}
```

---

## 📦 Products API

### Get All Products
```http
GET /api/products
```

**Response:**
```json
{
  "products": [
    {
      "id": 1,
      "name": "Luxury Wool Coat",
      "price": 299.99,
      "category": "Coats",
      "description": "Premium wool coat for winter",
      "image": "https://images.unsplash.com/photo-1551028719-00167b16eac5?w=400",
      "stock": 50,
      "isNew": true,
      "isFeatured": true
    }
  ],
  "total": 5
}
```

### Get Product by ID
```http
GET /api/products/{id}
```

**Response:**
```json
{
  "id": 1,
  "name": "Luxury Wool Coat",
  "price": 299.99,
  "category": "Coats",
  "description": "Premium wool coat for winter",
  "image": "https://images.unsplash.com/photo-1551028719-00167b16eac5?w=400",
  "stock": 50,
  "isNew": true,
  "isFeatured": true,
  "colors": ["Black", "Navy", "Brown"],
  "sizes": ["S", "M", "L", "XL"]
}
```

### Search Products
```http
GET /api/products/search?q={query}
```

**Example:**
```http
GET /api/products/search?q=wool
```

**Response:**
```json
{
  "products": [
    {
      "id": 1,
      "name": "Luxury Wool Coat",
      "price": 299.99,
      "category": "Coats"
    }
  ],
  "query": "wool"
}
```

---

## 🛒 Cart API

### Get Cart
```http
GET /api/cart?token={token}
```

**Response:**
```json
{
  "items": [
    {
      "productId": 1,
      "productName": "Luxury Wool Coat",
      "price": 299.99,
      "quantity": 2,
      "image": "https://images.unsplash.com/photo-1551028719-00167b16eac5?w=100"
    }
  ],
  "total": 799.97,
  "itemCount": 3
}
```

### Add to Cart
```http
POST /api/cart/add
Content-Type: application/json

{
  "productId": 1,
  "quantity": 2
}
```

**Response:**
```json
{
  "message": "Product added to cart",
  "productId": 1,
  "quantity": 2
}
```

---

## 📋 Orders API

### Get Orders
```http
GET /api/orders?token={token}
```

**Response:**
```json
{
  "orders": [
    {
      "id": "uuid",
      "orderNumber": "ORD-2024-001",
      "status": "Delivered",
      "total": 799.97,
      "createdAt": "2024-01-01T00:00:00Z",
      "items": [
        {
          "productId": 1,
          "productName": "Luxury Wool Coat",
          "price": 299.99,
          "quantity": 2,
          "image": "https://images.unsplash.com/photo-1551028719-00167b16eac5?w=100"
        }
      ]
    }
  ]
}
```

### Create Order
```http
POST /api/orders
Content-Type: application/json

{
  "items": [
    {
      "productId": 1,
      "productName": "Luxury Wool Coat",
      "price": 299.99,
      "quantity": 2,
      "image": "https://images.unsplash.com/photo-1551028719-00167b16eac5?w=100"
    }
  ]
}
```

**Response:**
```json
{
  "order": {
    "id": "uuid",
    "orderNumber": "ORD-2024-001",
    "status": "Pending",
    "total": 599.98,
    "createdAt": "2024-01-01T00:00:00Z",
    "items": [...]
  },
  "message": "Order created successfully"
}
```

---

## ❤️ Wishlist API

### Get Wishlist
```http
GET /api/wishlist?token={token}
```

**Response:**
```json
{
  "wishlist": [
    {
      "id": "uuid",
      "productId": 1,
      "productName": "Luxury Wool Coat",
      "price": 299.99,
      "image": "https://images.unsplash.com/photo-1551028719-00167b16eac5?w=200",
      "addedAt": "2024-01-01T00:00:00Z"
    }
  ]
}
```

### Add to Wishlist
```http
POST /api/wishlist/add
Content-Type: application/json

{
  "productId": 1
}
```

**Response:**
```json
{
  "message": "Product added to wishlist",
  "productId": 1
}
```

---

## 🏷️ Categories API

### Get Categories
```http
GET /api/categories
```

**Response:**
```json
{
  "categories": [
    {
      "id": 1,
      "name": "Coats",
      "image": "https://images.unsplash.com/photo-1551028719-00167b16eac5?w=200",
      "productCount": 12
    }
  ]
}
```

---

## ⭐ Reviews API

### Get Product Reviews
```http
GET /api/products/{id}/reviews
```

**Response:**
```json
{
  "reviews": [
    {
      "id": "uuid",
      "productId": 1,
      "userId": "uuid",
      "userName": "Ahmed Hassan",
      "rating": 5,
      "comment": "Excellent quality and perfect fit!",
      "createdAt": "2024-01-01T00:00:00Z"
    }
  ]
}
```

### Add Product Review
```http
POST /api/products/{id}/reviews
Content-Type: application/json

{
  "productId": 1,
  "rating": 5,
  "comment": "Great product!"
}
```

**Response:**
```json
{
  "review": {
    "id": "uuid",
    "productId": 1,
    "userId": "uuid",
    "userName": "Current User",
    "rating": 5,
    "comment": "Great product!",
    "createdAt": "2024-01-01T00:00:00Z"
  },
  "message": "Review added successfully"
}
```

---

## 🔍 Utility Endpoints

### Health Check
```http
GET /health
```

**Response:**
```
Healthy
```

### API Test
```http
GET /api/test
```

**Response:**
```json
{
  "message": "API is working!",
  "timestamp": "2024-01-01T00:00:00Z"
}
```

---

## 📊 Response Codes

| Code | Description |
|------|-------------|
| 200  | Success |
| 400  | Bad Request |
| 401  | Unauthorized |
| 404  | Not Found |
| 500  | Internal Server Error |

---

## 🔧 Error Responses

### Bad Request (400)
```json
{
  "error": "Email and password are required"
}
```

### Unauthorized (401)
```json
{
  "error": "Unauthorized"
}
```

### Not Found (404)
```json
{
  "error": "Not Found"
}
```

---

## 📝 Data Models

### User
```json
{
  "id": "uuid",
  "email": "string",
  "name": "string",
  "createdAt": "datetime"
}
```

### Product
```json
{
  "id": "integer",
  "name": "string",
  "price": "decimal",
  "category": "string",
  "description": "string",
  "image": "string",
  "stock": "integer",
  "isNew": "boolean",
  "isFeatured": "boolean",
  "colors": "string[]",
  "sizes": "string[]"
}
```

### Order
```json
{
  "id": "uuid",
  "orderNumber": "string",
  "status": "string",
  "total": "decimal",
  "createdAt": "datetime",
  "items": "OrderItem[]"
}
```

---

## 🚀 Getting Started

1. **Deploy to Railway:** Follow the [Railway Deployment Guide](RAILWAY_DEPLOYMENT_GUIDE.md)
2. **Test the API:** Use the Swagger UI at `/swagger`
3. **Register a user:** `POST /api/auth/register`
4. **Get a token:** Use the token from registration/login
5. **Start building:** Use the token in query parameters for protected endpoints

---

**Live API Documentation:** `https://your-app-name.railway.app/swagger` 📚
