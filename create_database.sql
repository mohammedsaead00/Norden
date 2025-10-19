-- Create NordenDB Database
IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'NordenDB')
BEGIN
    CREATE DATABASE NordenDB;
    PRINT 'Database NordenDB created successfully.';
END
ELSE
BEGIN
    PRINT 'Database NordenDB already exists.';
END
GO

USE NordenDB;
GO

-- Create Users table
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'users')
BEGIN
    CREATE TABLE users (
        id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
        email NVARCHAR(255) NOT NULL UNIQUE,
        password_hash NVARCHAR(255),
        display_name NVARCHAR(255) NOT NULL,
        phone_number NVARCHAR(20),
        photo_url NVARCHAR(500),
        is_guest BIT DEFAULT 0,
        is_admin BIT DEFAULT 0,
        google_id NVARCHAR(100),
        created_at DATETIME2 DEFAULT GETUTCDATE(),
        updated_at DATETIME2 DEFAULT GETUTCDATE()
    );
    CREATE INDEX idx_email ON users(email);
    CREATE INDEX idx_google_id ON users(google_id);
    PRINT 'Users table created successfully.';
END
ELSE
BEGIN
    PRINT 'Users table already exists.';
END
GO

-- Create Products table
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'products')
BEGIN
    CREATE TABLE products (
        id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
        name NVARCHAR(255) NOT NULL,
        description NVARCHAR(MAX),
        price DECIMAL(10,2) NOT NULL,
        category NVARCHAR(50) NOT NULL,
        images NVARCHAR(MAX),
        colors NVARCHAR(MAX),
        sizes NVARCHAR(MAX),
        stock INT DEFAULT 0,
        is_new BIT DEFAULT 0,
        is_featured BIT DEFAULT 0,
        created_at DATETIME2 DEFAULT GETUTCDATE(),
        updated_at DATETIME2 DEFAULT GETUTCDATE()
    );
    CREATE INDEX idx_category ON products(category);
    CREATE INDEX idx_is_new ON products(is_new);
    CREATE INDEX idx_is_featured ON products(is_featured);
    PRINT 'Products table created successfully.';
END
ELSE
BEGIN
    PRINT 'Products table already exists.';
END
GO

-- Insert sample data
IF NOT EXISTS (SELECT * FROM products)
BEGIN
    INSERT INTO products (name, description, price, category, images, colors, sizes, stock, is_new, is_featured) VALUES
    ('Luxury Wool Coat', 'Premium wool coat for winter', 299.99, 'Coats', '["https://example.com/coat1.jpg"]', '["Black", "Navy", "Brown"]', '["S", "M", "L", "XL"]', 50, 1, 1),
    ('Classic Blazer', 'Professional blazer for business', 199.99, 'Blazers', '["https://example.com/blazer1.jpg"]', '["Black", "Navy", "Grey"]', '["S", "M", "L", "XL"]', 30, 0, 1),
    ('Silk Dress Shirt', 'Premium silk dress shirt', 89.99, 'DressShirts', '["https://example.com/shirt1.jpg"]', '["White", "Blue", "Pink"]', '["S", "M", "L", "XL"]', 75, 1, 0);
    PRINT 'Sample products inserted successfully.';
END
ELSE
BEGIN
    PRINT 'Products table already has data.';
END
GO

-- Show database info
SELECT 'Database: ' + DB_NAME() as Info;
SELECT COUNT(*) as UserCount FROM users;
SELECT COUNT(*) as ProductCount FROM products;
SELECT 'Tables in database:' as Info;
SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'BASE TABLE';
