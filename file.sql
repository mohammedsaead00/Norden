-- =============================================
-- Online Clothing Store Database Schema
-- SQL Server (T-SQL) Version
-- =============================================

-- Create Database
IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'ClothingStore')
BEGIN
    CREATE DATABASE ClothingStore;
END
GO

USE ClothingStore;
GO

-- =============================================
-- 1. Roles Table
-- =============================================
IF OBJECT_ID('Roles', 'U') IS NOT NULL DROP TABLE Roles;
GO

CREATE TABLE Roles (
    role_id INT PRIMARY KEY IDENTITY(1,1),
    role_name NVARCHAR(50) NOT NULL UNIQUE,
    description NVARCHAR(MAX),
    created_at DATETIME2 DEFAULT GETDATE()
);
GO

-- =============================================
-- 2. Users Table
-- =============================================
IF OBJECT_ID('Users', 'U') IS NOT NULL DROP TABLE Users;
GO

CREATE TABLE Users (
    user_id INT PRIMARY KEY IDENTITY(1,1),
    email NVARCHAR(255) NOT NULL UNIQUE,
    password_hash NVARCHAR(255) NOT NULL,
    first_name NVARCHAR(100) NOT NULL,
    last_name NVARCHAR(100) NOT NULL,
    phone NVARCHAR(20),
    role_id INT NOT NULL DEFAULT 2,
    is_active BIT DEFAULT 1,
    created_at DATETIME2 DEFAULT GETDATE(),
    updated_at DATETIME2 DEFAULT GETDATE(),
    CONSTRAINT FK_Users_Roles FOREIGN KEY (role_id) 
        REFERENCES Roles(role_id)
);
GO

-- Trigger for auto-updating updated_at
CREATE TRIGGER trg_Users_UpdatedAt
ON Users
AFTER UPDATE
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE Users
    SET updated_at = GETDATE()
    FROM Users u
    INNER JOIN inserted i ON u.user_id = i.user_id;
END;
GO

-- =============================================
-- 3. Addresses Table
-- =============================================
IF OBJECT_ID('Addresses', 'U') IS NOT NULL DROP TABLE Addresses;
GO

CREATE TABLE Addresses (
    address_id INT PRIMARY KEY IDENTITY(1,1),
    user_id INT NOT NULL,
    address_type NVARCHAR(20) NOT NULL CHECK (address_type IN ('shipping', 'billing', 'both')),
    street_address NVARCHAR(255) NOT NULL,
    apartment NVARCHAR(100),
    city NVARCHAR(100) NOT NULL,
    state NVARCHAR(100) NOT NULL,
    postal_code NVARCHAR(20) NOT NULL,
    country NVARCHAR(100) NOT NULL DEFAULT 'USA',
    is_default BIT DEFAULT 0,
    created_at DATETIME2 DEFAULT GETDATE(),
    CONSTRAINT FK_Addresses_Users FOREIGN KEY (user_id) 
        REFERENCES Users(user_id) ON DELETE CASCADE
);
GO

-- =============================================
-- 4. Categories Table
-- =============================================
IF OBJECT_ID('Categories', 'U') IS NOT NULL DROP TABLE Categories;
GO

CREATE TABLE Categories (
    category_id INT PRIMARY KEY IDENTITY(1,1),
    category_name NVARCHAR(100) NOT NULL,
    description NVARCHAR(MAX),
    parent_category_id INT NULL,
    gender NVARCHAR(20) NOT NULL CHECK (gender IN ('Men', 'Women', 'Unisex')),
    is_active BIT DEFAULT 1,
    created_at DATETIME2 DEFAULT GETDATE(),
    CONSTRAINT FK_Categories_Parent FOREIGN KEY (parent_category_id) 
        REFERENCES Categories(category_id)
);
GO

-- =============================================
-- 5. Products Table
-- =============================================
IF OBJECT_ID('Products', 'U') IS NOT NULL DROP TABLE Products;
GO

CREATE TABLE Products (
    product_id INT PRIMARY KEY IDENTITY(1,1),
    product_name NVARCHAR(255) NOT NULL,
    description NVARCHAR(MAX),
    category_id INT NOT NULL,
    base_price DECIMAL(10, 2) NOT NULL,
    brand NVARCHAR(100),
    material NVARCHAR(255),
    care_instructions NVARCHAR(MAX),
    is_active BIT DEFAULT 1,
    created_at DATETIME2 DEFAULT GETDATE(),
    updated_at DATETIME2 DEFAULT GETDATE(),
    CONSTRAINT FK_Products_Categories FOREIGN KEY (category_id) 
        REFERENCES Categories(category_id)
);
GO

-- Create indexes
CREATE INDEX idx_Products_Category ON Products(category_id);
CREATE INDEX idx_Products_Price ON Products(base_price);
CREATE INDEX idx_Products_Active ON Products(is_active);
GO

-- Trigger for auto-updating updated_at
CREATE TRIGGER trg_Products_UpdatedAt
ON Products
AFTER UPDATE
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE Products
    SET updated_at = GETDATE()
    FROM Products p
    INNER JOIN inserted i ON p.product_id = i.product_id;
END;
GO

-- =============================================
-- 6. Product_Images Table
-- =============================================
IF OBJECT_ID('Product_Images', 'U') IS NOT NULL DROP TABLE Product_Images;
GO

CREATE TABLE Product_Images (
    image_id INT PRIMARY KEY IDENTITY(1,1),
    product_id INT NOT NULL,
    image_url NVARCHAR(500) NOT NULL,
    is_primary BIT DEFAULT 0,
    display_order INT DEFAULT 0,
    created_at DATETIME2 DEFAULT GETDATE(),
    CONSTRAINT FK_ProductImages_Products FOREIGN KEY (product_id) 
        REFERENCES Products(product_id) ON DELETE CASCADE
);
GO

CREATE INDEX idx_ProductImages_Product ON Product_Images(product_id);
GO

-- =============================================
-- 7. Inventory Table
-- =============================================
IF OBJECT_ID('Inventory', 'U') IS NOT NULL DROP TABLE Inventory;
GO

CREATE TABLE Inventory (
    inventory_id INT PRIMARY KEY IDENTITY(1,1),
    product_id INT NOT NULL,
    size NVARCHAR(10) NOT NULL,
    color NVARCHAR(50) NOT NULL,
    quantity_in_stock INT NOT NULL DEFAULT 0,
    reorder_level INT DEFAULT 10,
    last_restocked DATETIME2 NULL,
    created_at DATETIME2 DEFAULT GETDATE(),
    updated_at DATETIME2 DEFAULT GETDATE(),
    CONSTRAINT FK_Inventory_Products FOREIGN KEY (product_id) 
        REFERENCES Products(product_id) ON DELETE CASCADE,
    CONSTRAINT UQ_Inventory_Variant UNIQUE (product_id, size, color)
);
GO

CREATE INDEX idx_Inventory_Stock ON Inventory(quantity_in_stock);
CREATE INDEX idx_Inventory_Product ON Inventory(product_id);
GO

-- Trigger for auto-updating updated_at
CREATE TRIGGER trg_Inventory_UpdatedAt
ON Inventory
AFTER UPDATE
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE Inventory
    SET updated_at = GETDATE()
    FROM Inventory inv
    INNER JOIN inserted i ON inv.inventory_id = i.inventory_id;
END;
GO

-- =============================================
-- 8. Cart Table
-- =============================================
IF OBJECT_ID('Cart', 'U') IS NOT NULL DROP TABLE Cart;
GO

CREATE TABLE Cart (
    cart_id INT PRIMARY KEY IDENTITY(1,1),
    user_id INT NOT NULL,
    product_id INT NOT NULL,
    inventory_id INT NOT NULL,
    quantity INT NOT NULL DEFAULT 1,
    added_at DATETIME2 DEFAULT GETDATE(),
    CONSTRAINT FK_Cart_Users FOREIGN KEY (user_id) 
        REFERENCES Users(user_id) ON DELETE CASCADE,
    CONSTRAINT FK_Cart_Products FOREIGN KEY (product_id) 
        REFERENCES Products(product_id),
    CONSTRAINT FK_Cart_Inventory FOREIGN KEY (inventory_id) 
        REFERENCES Inventory(inventory_id)
);
GO

CREATE INDEX idx_Cart_User ON Cart(user_id);
GO

-- =============================================
-- 9. Orders Table
-- =============================================
IF OBJECT_ID('Orders', 'U') IS NOT NULL DROP TABLE Orders;
GO

CREATE TABLE Orders (
    order_id INT PRIMARY KEY IDENTITY(1,1),
    user_id INT NOT NULL,
    order_date DATETIME2 DEFAULT GETDATE(),
    total_amount DECIMAL(10, 2) NOT NULL,
    order_status NVARCHAR(20) NOT NULL DEFAULT 'pending' 
        CHECK (order_status IN ('pending', 'processing', 'shipped', 'delivered', 'cancelled')),
    shipping_address_id INT NOT NULL,
    billing_address_id INT NOT NULL,
    tracking_number NVARCHAR(100),
    created_at DATETIME2 DEFAULT GETDATE(),
    updated_at DATETIME2 DEFAULT GETDATE(),
    CONSTRAINT FK_Orders_Users FOREIGN KEY (user_id) 
        REFERENCES Users(user_id),
    CONSTRAINT FK_Orders_ShippingAddress FOREIGN KEY (shipping_address_id) 
        REFERENCES Addresses(address_id),
    CONSTRAINT FK_Orders_BillingAddress FOREIGN KEY (billing_address_id) 
        REFERENCES Addresses(address_id)
);
GO

CREATE INDEX idx_Orders_User ON Orders(user_id);
CREATE INDEX idx_Orders_Status ON Orders(order_status);
CREATE INDEX idx_Orders_Date ON Orders(order_date);
GO

-- Trigger for auto-updating updated_at
CREATE TRIGGER trg_Orders_UpdatedAt
ON Orders
AFTER UPDATE
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE Orders
    SET updated_at = GETDATE()
    FROM Orders o
    INNER JOIN inserted i ON o.order_id = i.order_id;
END;
GO

-- =============================================
-- 10. Order_Items Table
-- =============================================
IF OBJECT_ID('Order_Items', 'U') IS NOT NULL DROP TABLE Order_Items;
GO

CREATE TABLE Order_Items (
    order_item_id INT PRIMARY KEY IDENTITY(1,1),
    order_id INT NOT NULL,
    product_id INT NOT NULL,
    inventory_id INT NOT NULL,
    quantity INT NOT NULL,
    unit_price DECIMAL(10, 2) NOT NULL,
    subtotal DECIMAL(10, 2) NOT NULL,
    created_at DATETIME2 DEFAULT GETDATE(),
    CONSTRAINT FK_OrderItems_Orders FOREIGN KEY (order_id) 
        REFERENCES Orders(order_id) ON DELETE CASCADE,
    CONSTRAINT FK_OrderItems_Products FOREIGN KEY (product_id) 
        REFERENCES Products(product_id),
    CONSTRAINT FK_OrderItems_Inventory FOREIGN KEY (inventory_id) 
        REFERENCES Inventory(inventory_id)
);
GO

CREATE INDEX idx_OrderItems_Order ON Order_Items(order_id);
GO

-- =============================================
-- 11. Payments Table
-- =============================================
IF OBJECT_ID('Payments', 'U') IS NOT NULL DROP TABLE Payments;
GO

CREATE TABLE Payments (
    payment_id INT PRIMARY KEY IDENTITY(1,1),
    order_id INT NOT NULL UNIQUE,
    payment_method NVARCHAR(30) NOT NULL 
        CHECK (payment_method IN ('credit_card', 'debit_card', 'paypal', 'stripe', 'cash_on_delivery')),
    payment_status NVARCHAR(20) NOT NULL DEFAULT 'pending' 
        CHECK (payment_status IN ('pending', 'completed', 'failed', 'refunded')),
    transaction_id NVARCHAR(255),
    amount DECIMAL(10, 2) NOT NULL,
    payment_date DATETIME2 DEFAULT GETDATE(),
    CONSTRAINT FK_Payments_Orders FOREIGN KEY (order_id) 
        REFERENCES Orders(order_id)
);
GO

CREATE INDEX idx_Payments_Status ON Payments(payment_status);
CREATE INDEX idx_Payments_Transaction ON Payments(transaction_id);
GO

-- =============================================
-- 12. Reviews Table
-- =============================================
IF OBJECT_ID('Reviews', 'U') IS NOT NULL DROP TABLE Reviews;
GO

CREATE TABLE Reviews (
    review_id INT PRIMARY KEY IDENTITY(1,1),
    product_id INT NOT NULL,
    user_id INT NOT NULL,
    rating INT NOT NULL CHECK (rating >= 1 AND rating <= 5),
    review_text NVARCHAR(MAX),
    created_at DATETIME2 DEFAULT GETDATE(),
    CONSTRAINT FK_Reviews_Products FOREIGN KEY (product_id) 
        REFERENCES Products(product_id) ON DELETE CASCADE,
    CONSTRAINT FK_Reviews_Users FOREIGN KEY (user_id) 
        REFERENCES Users(user_id) ON DELETE CASCADE
);
GO

CREATE INDEX idx_Reviews_Product ON Reviews(product_id);
CREATE INDEX idx_Reviews_Rating ON Reviews(rating);
GO

-- =============================================
-- Insert Default Data
-- =============================================

-- Insert Roles
INSERT INTO Roles (role_name, description) VALUES
('admin', 'Administrator with full access'),
('customer', 'Regular customer user');
GO

-- Insert Sample Categories
INSERT INTO Categories (category_name, description, gender, parent_category_id) VALUES
('Tops', 'Upper body clothing', 'Men', NULL),
('Bottoms', 'Lower body clothing', 'Men', NULL),
('Tops', 'Upper body clothing', 'Women', NULL),
('Bottoms', 'Lower body clothing', 'Women', NULL);
GO

INSERT INTO Categories (category_name, description, gender, parent_category_id) VALUES
('T-Shirts', 'Casual t-shirts', 'Men', 1),
('Shirts', 'Formal and casual shirts', 'Men', 1),
('Jeans', 'Denim jeans', 'Men', 2),
('Trousers', 'Formal trousers', 'Men', 2),
('Blouses', 'Women blouses', 'Women', 3),
('Dresses', 'Women dresses', 'Women', 3),
('Jeans', 'Denim jeans', 'Women', 4),
('Skirts', 'Women skirts', 'Women', 4);
GO

-- =============================================
-- Useful Views
-- =============================================

-- View: Product Catalog with Stock Info
IF OBJECT_ID('vw_product_catalog', 'V') IS NOT NULL DROP VIEW vw_product_catalog;
GO

CREATE VIEW vw_product_catalog AS
SELECT 
    p.product_id,
    p.product_name,
    p.description,
    c.category_name,
    c.gender,
    p.base_price,
    p.brand,
    SUM(i.quantity_in_stock) as total_stock,
    COUNT(DISTINCT i.inventory_id) as variants_count,
    AVG(CAST(r.rating AS FLOAT)) as avg_rating,
    COUNT(r.review_id) as review_count
FROM Products p
LEFT JOIN Categories c ON p.category_id = c.category_id
LEFT JOIN Inventory i ON p.product_id = i.product_id
LEFT JOIN Reviews r ON p.product_id = r.product_id
WHERE p.is_active = 1
GROUP BY p.product_id, p.product_name, p.description, c.category_name, 
         c.gender, p.base_price, p.brand;
GO

-- View: Order Summary
IF OBJECT_ID('vw_order_summary', 'V') IS NOT NULL DROP VIEW vw_order_summary;
GO

CREATE VIEW vw_order_summary AS
SELECT 
    o.order_id,
    o.order_date,
    u.email,
    CONCAT(u.first_name, ' ', u.last_name) as customer_name,
    o.total_amount,
    o.order_status,
    p.payment_status,
    COUNT(oi.order_item_id) as items_count
FROM Orders o
JOIN Users u ON o.user_id = u.user_id
LEFT JOIN Payments p ON o.order_id = p.order_id
LEFT JOIN Order_Items oi ON o.order_id = oi.order_id
GROUP BY o.order_id, o.order_date, u.email, u.first_name, u.last_name,
         o.total_amount, o.order_status, p.payment_status;
GO

-- View: Low Stock Alert
IF OBJECT_ID('vw_low_stock_alert', 'V') IS NOT NULL DROP VIEW vw_low_stock_alert;
GO

CREATE VIEW vw_low_stock_alert AS
SELECT 
    p.product_id,
    p.product_name,
    i.size,
    i.color,
    i.quantity_in_stock,
    i.reorder_level
FROM Inventory i
JOIN Products p ON i.product_id = p.product_id
WHERE i.quantity_in_stock <= i.reorder_level;
GO

-- =============================================
-- Additional Indexes for Performance
-- =============================================

CREATE INDEX idx_Users_Email ON Users(email);
CREATE INDEX idx_Products_Name ON Products(product_name);
CREATE INDEX idx_Orders_UserDate ON Orders(user_id, order_date);
CREATE INDEX idx_Inventory_ProductStock ON Inventory(product_id, quantity_in_stock);
GO

PRINT 'Database schema created successfully for SQL Server!';
GO
SELECT * FROM INFORMATION_SCHEMA.TABLES;
