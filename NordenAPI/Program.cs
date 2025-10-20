using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using System;
using System.Text.Json;
using Microsoft.AspNetCore.HttpOverrides;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo 
    { 
        Title = "NordenAPI", 
        Version = "v1",
        Description = "A comprehensive e-commerce API for luxury fashion items",
        Contact = new OpenApiContact
        {
            Name = "NordenAPI Support",
            Email = "support@nordenapi.com"
        }
    });
    
    // Include XML comments if available
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath);
    }
});

// Register CORS services so middleware can resolve ICorsService
builder.Services.AddCors();

// Respect proxy headers from Railway (X-Forwarded-For, X-Forwarded-Proto)
builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
    options.RequireHeaderSymmetry = false;
    options.KnownNetworks.Clear();
    options.KnownProxies.Clear();
});

var app = builder.Build();

// Configure the HTTP request pipeline.
// Enable Swagger in all environments for Railway deployment
app.UseForwardedHeaders();
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "NordenAPI v1");
    c.RoutePrefix = string.Empty; // Serve Swagger UI at /
    c.DocumentTitle = "NordenAPI Documentation";
    c.DefaultModelsExpandDepth(-1);
});

// Only redirect to HTTPS in production
if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

// Simple CORS policy for development
app.UseCors(policy => policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());

// Simple endpoints (moved root to Swagger UI). Keep a secondary health root if needed
app.MapGet("/health", () => "Healthy");
app.MapGet("/api/test", () => new { message = "API is working!", timestamp = DateTime.UtcNow });

// Products API
app.MapGet("/api/products", () => new {
    products = new[] {
        new { 
            id = 1, 
            name = "Luxury Wool Coat", 
            price = 299.99,
            category = "Coats",
            description = "Premium wool coat for winter",
            image = "https://images.unsplash.com/photo-1551028719-00167b16eac5?w=400",
            stock = 50,
            isNew = true,
            isFeatured = true
        },
        new { 
            id = 2, 
            name = "Classic Blazer", 
            price = 199.99,
            category = "Blazers",
            description = "Professional blazer for business",
            image = "https://images.unsplash.com/photo-1507003211169-0a1dd7228f2d?w=400",
            stock = 30,
            isNew = false,
            isFeatured = true
        },
        new { 
            id = 3, 
            name = "Silk Dress Shirt", 
            price = 89.99,
            category = "Shirts",
            description = "Premium silk dress shirt",
            image = "https://images.unsplash.com/photo-1596755094514-f87e34085b2c?w=400",
            stock = 75,
            isNew = true,
            isFeatured = false
        },
        new { 
            id = 4, 
            name = "Designer Jeans", 
            price = 149.99,
            category = "Jeans",
            description = "High-quality designer jeans",
            image = "https://images.unsplash.com/photo-1542272604-787c3835535d?w=400",
            stock = 40,
            isNew = false,
            isFeatured = true
        },
        new { 
            id = 5, 
            name = "Cashmere Sweater", 
            price = 179.99,
            category = "Sweaters",
            description = "Luxury cashmere sweater",
            image = "https://images.unsplash.com/photo-1434389677669-e08b4cac3105?w=400",
            stock = 25,
            isNew = true,
            isFeatured = false
        }
    },
    total = 5
});

// Get single product
app.MapGet("/api/products/{id}", (int id) => {
    var products = new[] {
        new { 
            id = 1, 
            name = "Luxury Wool Coat", 
            price = 299.99,
            category = "Coats",
            description = "Premium wool coat for winter",
            image = "https://images.unsplash.com/photo-1551028719-00167b16eac5?w=400",
            stock = 50,
            isNew = true,
            isFeatured = true,
            colors = new[] { "Black", "Navy", "Brown" },
            sizes = new[] { "S", "M", "L", "XL" }
        },
        new { 
            id = 2, 
            name = "Classic Blazer", 
            price = 199.99,
            category = "Blazers",
            description = "Professional blazer for business",
            image = "https://images.unsplash.com/photo-1507003211169-0a1dd7228f2d?w=400",
            stock = 30,
            isNew = false,
            isFeatured = true,
            colors = new[] { "Black", "Navy", "Grey" },
            sizes = new[] { "S", "M", "L", "XL" }
        }
    };
    
    var product = products.FirstOrDefault(p => p.id == id);
    return product != null ? Results.Ok(product) : Results.NotFound();
});

// Search products
app.MapGet("/api/products/search", (string q) => {
    var products = new[] {
        new { 
            id = 1, 
            name = "Luxury Wool Coat", 
            price = 299.99,
            category = "Coats"
        },
        new { 
            id = 2, 
            name = "Classic Blazer", 
            price = 199.99,
            category = "Blazers"
        }
    };
    
    var filteredProducts = products.Where(p => 
        p.name.ToLower().Contains(q.ToLower()) ||
        p.category.ToLower().Contains(q.ToLower())
    ).ToArray();
    
    return new { products = filteredProducts, query = q };
});

// Authentication API
app.MapPost("/api/auth/register", (RegisterRequest request) => {
    // Simple validation
    if (string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.Password))
        return Results.BadRequest("Email and password are required");
    
    // Simulate user creation
    var user = new {
        id = Guid.NewGuid(),
        email = request.Email,
        name = request.Name,
        createdAt = DateTime.UtcNow
    };
    
    // Generate simple token (in real app, use JWT)
    var token = $"token_{Guid.NewGuid().ToString("N")[..16]}";
    
    return Results.Ok(new {
        user,
        token,
        message = "Registration successful"
    });
});

app.MapPost("/api/auth/login", (LoginRequest request) => {
    // Simple validation
    if (string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.Password))
        return Results.BadRequest("Email and password are required");
    
    // Simulate login (in real app, check database)
    var user = new {
        id = Guid.NewGuid(),
        email = request.Email,
        name = "John Doe",
        createdAt = DateTime.UtcNow
    };
    
    // Generate simple token
    var token = $"token_{Guid.NewGuid().ToString("N")[..16]}";
    
    return Results.Ok(new {
        user,
        token,
        message = "Login successful"
    });
});

app.MapGet("/api/auth/profile", (string token) => {
    // Simple token validation
    if (string.IsNullOrEmpty(token) || !token.StartsWith("token_"))
        return Results.Unauthorized();
    
    var user = new {
        id = Guid.NewGuid(),
        email = "user@example.com",
        name = "John Doe",
        createdAt = DateTime.UtcNow,
        preferences = new {
            theme = "light",
            notifications = true
        }
    };
    
    return Results.Ok(user);
});

// Cart API
app.MapGet("/api/cart", (string token) => {
    // Simple token validation
    if (string.IsNullOrEmpty(token) || !token.StartsWith("token_"))
        return Results.Unauthorized();
    
    var cart = new {
        items = new[] {
            new {
                productId = 1,
                productName = "Luxury Wool Coat",
                price = 299.99,
                quantity = 2,
                image = "https://images.unsplash.com/photo-1551028719-00167b16eac5?w=100"
            },
            new {
                productId = 2,
                productName = "Classic Blazer",
                price = 199.99,
                quantity = 1,
                image = "https://images.unsplash.com/photo-1507003211169-0a1dd7228f2d?w=100"
            }
        },
        total = 799.97,
        itemCount = 3
    };
    
    return Results.Ok(cart);
});

app.MapPost("/api/cart/add", (AddToCartRequest request) => {
    // Simple validation
    if (request.ProductId <= 0 || request.Quantity <= 0)
        return Results.BadRequest("Invalid product or quantity");
    
    return Results.Ok(new {
        message = "Product added to cart",
        productId = request.ProductId,
        quantity = request.Quantity
    });
});

// Categories API
app.MapGet("/api/categories", () => {
    var categories = new[] {
        new { id = 1, name = "Coats", image = "https://images.unsplash.com/photo-1551028719-00167b16eac5?w=200", productCount = 12 },
        new { id = 2, name = "Blazers", image = "https://images.unsplash.com/photo-1507003211169-0a1dd7228f2d?w=200", productCount = 8 },
        new { id = 3, name = "Shirts", image = "https://images.unsplash.com/photo-1596755094514-f87e34085b2c?w=200", productCount = 25 },
        new { id = 4, name = "Jeans", image = "https://images.unsplash.com/photo-1542272604-787c3835535d?w=200", productCount = 15 },
        new { id = 5, name = "Sweaters", image = "https://images.unsplash.com/photo-1434389677669-e08b4cac3105?w=200", productCount = 20 }
    };
    
    return Results.Ok(new { categories });
});

// Orders API
app.MapGet("/api/orders", (string token) => {
    // Simple token validation
    if (string.IsNullOrEmpty(token) || !token.StartsWith("token_"))
        return Results.Unauthorized();
    
    var orders = new[] {
        new {
            id = Guid.NewGuid(),
            orderNumber = "ORD-2024-001",
            status = "Delivered",
            total = 799.97,
            createdAt = DateTime.UtcNow.AddDays(-5),
            items = new[] {
                new {
                    productId = 1,
                    productName = "Luxury Wool Coat",
                    price = 299.99,
                    quantity = 2,
                    image = "https://images.unsplash.com/photo-1551028719-00167b16eac5?w=100"
                },
                new {
                    productId = 2,
                    productName = "Classic Blazer",
                    price = 199.99,
                    quantity = 1,
                    image = "https://images.unsplash.com/photo-1507003211169-0a1dd7228f2d?w=100"
                }
            }
        },
        new {
            id = Guid.NewGuid(),
            orderNumber = "ORD-2024-002",
            status = "Processing",
            total = 449.98,
            createdAt = DateTime.UtcNow.AddDays(-2),
            items = new[] {
                new {
                    productId = 3,
                    productName = "Silk Dress Shirt",
                    price = 89.99,
                    quantity = 5,
                    image = "https://images.unsplash.com/photo-1596755094514-f87e34085b2c?w=100"
                }
            }
        }
    };
    
    return Results.Ok(new { orders });
});

app.MapPost("/api/orders", (CreateOrderRequest request) => {
    // Simple validation
    if (request.Items == null || !request.Items.Any())
        return Results.BadRequest("Order must contain items");
    
    // Simulate order creation
    var order = new {
        id = Guid.NewGuid(),
        orderNumber = $"ORD-{DateTime.UtcNow:yyyy-MM}-{Random.Shared.Next(1000, 9999)}",
        status = "Pending",
        total = request.Items.Sum(item => item.Price * item.Quantity),
        createdAt = DateTime.UtcNow,
        items = request.Items
    };
    
    return Results.Ok(new {
        order,
        message = "Order created successfully"
    });
});

// Wishlist API
app.MapGet("/api/wishlist", (string token) => {
    // Simple token validation
    if (string.IsNullOrEmpty(token) || !token.StartsWith("token_"))
        return Results.Unauthorized();
    
    var wishlist = new[] {
        new {
            id = Guid.NewGuid(),
            productId = 1,
            productName = "Luxury Wool Coat",
            price = 299.99,
            image = "https://images.unsplash.com/photo-1551028719-00167b16eac5?w=200",
            addedAt = DateTime.UtcNow.AddDays(-3)
        },
        new {
            id = Guid.NewGuid(),
            productId = 3,
            productName = "Silk Dress Shirt",
            price = 89.99,
            image = "https://images.unsplash.com/photo-1596755094514-f87e34085b2c?w=200",
            addedAt = DateTime.UtcNow.AddDays(-1)
        }
    };
    
    return Results.Ok(new { wishlist });
});

app.MapPost("/api/wishlist/add", (AddToWishlistRequest request) => {
    // Simple validation
    if (request.ProductId <= 0)
        return Results.BadRequest("Invalid product ID");
    
    return Results.Ok(new {
        message = "Product added to wishlist",
        productId = request.ProductId
    });
});

// Reviews API
app.MapGet("/api/products/{id}/reviews", (int id) => {
    var reviews = new[] {
        new {
            id = Guid.NewGuid(),
            productId = id,
            userId = Guid.NewGuid(),
            userName = "Ahmed Hassan",
            rating = 5,
            comment = "Excellent quality and perfect fit!",
            createdAt = DateTime.UtcNow.AddDays(-2)
        },
        new {
            id = Guid.NewGuid(),
            productId = id,
            userId = Guid.NewGuid(),
            userName = "Sarah Mohamed",
            rating = 4,
            comment = "Great product, fast shipping",
            createdAt = DateTime.UtcNow.AddDays(-5)
        }
    };
    
    return Results.Ok(new { reviews });
});

app.MapPost("/api/products/{id}/reviews", (CreateReviewRequest request) => {
    // Simple validation
    if (request.Rating < 1 || request.Rating > 5)
        return Results.BadRequest("Rating must be between 1 and 5");
    
    var review = new {
        id = Guid.NewGuid(),
        productId = request.ProductId,
        userId = Guid.NewGuid(),
        userName = "Current User",
        rating = request.Rating,
        comment = request.Comment,
        createdAt = DateTime.UtcNow
    };
    
    return Results.Ok(new {
        review,
        message = "Review added successfully"
    });
});

app.Run();

// DTOs
public record RegisterRequest(string Email, string Password, string Name);
public record LoginRequest(string Email, string Password);
public record AddToCartRequest(int ProductId, int Quantity);
public record CreateOrderRequest(OrderItem[] Items);
public record OrderItem(int ProductId, string ProductName, decimal Price, int Quantity, string Image);
public record AddToWishlistRequest(int ProductId);
public record CreateReviewRequest(int ProductId, int Rating, string Comment);