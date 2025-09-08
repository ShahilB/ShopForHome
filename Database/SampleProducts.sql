-- Insert Sample Products for ShopForHome
USE ShopForHomeDB;
GO

-- Insert sample products
INSERT INTO Products (Name, Description, Price, DiscountPrice, SKU, Stock, MinStockLevel, CategoryId, Brand, Color, Material, Dimensions, Weight, Rating, ReviewCount, ImageUrl, IsFeatured, IsActive, CreatedAt, UpdatedAt)
VALUES
-- Furniture Category (ID: 1)
('Modern Sectional Sofa', 'Comfortable 3-piece sectional sofa with premium fabric upholstery', 1299.99, 999.99, 'SOFA-001', 15, 10, 1, 'ComfortLiving', 'Gray', 'Fabric', '120x80x35 inches', 85.5, 4.5, 23, '/assets/images/products/sofa-sectional.jpg', 1, 1, GETUTCDATE(), GETUTCDATE()),
('Oak Dining Table', 'Solid oak dining table seats 6 people comfortably', 899.99, NULL, 'TABLE-001', 8, 10, 1, 'WoodCraft', 'Natural Oak', 'Solid Oak', '72x36x30 inches', 65.0, 4.8, 15, '/assets/images/products/dining-table.jpg', 1, 1, GETUTCDATE(), GETUTCDATE()),
('Ergonomic Office Chair', 'High-back ergonomic office chair with lumbar support', 349.99, 279.99, 'CHAIR-001', 25, 10, 1, 'ErgoMax', 'Black', 'Mesh', '26x26x45 inches', 22.0, 4.3, 41, '/assets/images/products/office-chair.jpg', 0, 1, GETUTCDATE(), GETUTCDATE()),

-- Home Décor Category (ID: 2)
('Abstract Canvas Wall Art', 'Modern abstract canvas painting perfect for living room', 129.99, NULL, 'ART-001', 30, 10, 2, 'ArtisticVisions', 'Multicolor', 'Canvas', '24x36 inches', 2.5, 4.2, 18, '/assets/images/products/wall-art.jpg', 1, 1, GETUTCDATE(), GETUTCDATE()),
('Ceramic Decorative Vase', 'Handcrafted ceramic vase with elegant design', 79.99, 59.99, 'VASE-001', 20, 10, 2, 'CeramicCraft', 'White', 'Ceramic', '8x8x12 inches', 3.2, 4.6, 12, '/assets/images/products/ceramic-vase.jpg', 0, 1, GETUTCDATE(), GETUTCDATE()),
('Throw Pillow Set', 'Set of 4 decorative throw pillows with removable covers', 49.99, NULL, 'PILLOW-001', 50, 10, 2, 'CozyHome', 'Blue', 'Cotton', '18x18 inches', 1.8, 4.4, 35, '/assets/images/products/throw-pillows.jpg', 1, 1, GETUTCDATE(), GETUTCDATE()),

-- Lighting Category (ID: 3)
('Crystal Chandelier', 'Elegant crystal chandelier with LED bulbs', 599.99, 449.99, 'LIGHT-001', 5, 10, 3, 'LuxuryLights', 'Crystal', 'Crystal & Metal', '24x24x30 inches', 15.5, 4.9, 8, '/assets/images/products/chandelier.jpg', 1, 1, GETUTCDATE(), GETUTCDATE()),
('Modern Table Lamp', 'Minimalist table lamp with adjustable brightness', 89.99, NULL, 'LAMP-001', 35, 10, 3, 'ModernLights', 'Black', 'Metal', '8x8x18 inches', 4.2, 4.1, 27, '/assets/images/products/table-lamp.jpg', 0, 1, GETUTCDATE(), GETUTCDATE()),
('LED Floor Lamp', 'Smart LED floor lamp with remote control', 199.99, 159.99, 'LAMP-002', 18, 10, 3, 'SmartHome', 'White', 'Metal & Plastic', '12x12x60 inches', 8.5, 4.7, 22, '/assets/images/products/floor-lamp.jpg', 0, 1, GETUTCDATE(), GETUTCDATE()),

-- Kitchen & Dining Category (ID: 4)
('Stainless Steel Cookware Set', 'Professional 12-piece stainless steel cookware set', 299.99, 229.99, 'COOK-001', 12, 10, 4, 'ChefMaster', 'Stainless Steel', 'Stainless Steel', 'Various sizes', 18.0, 4.6, 31, '/assets/images/products/cookware-set.jpg', 1, 1, GETUTCDATE(), GETUTCDATE()),
('Wooden Cutting Board', 'Large bamboo cutting board with juice groove', 39.99, NULL, 'BOARD-001', 45, 10, 4, 'BambooKitchen', 'Natural', 'Bamboo', '18x12x1 inches', 2.8, 4.3, 19, '/assets/images/products/cutting-board.jpg', 0, 1, GETUTCDATE(), GETUTCDATE()),

-- Bedroom Category (ID: 5)
('Memory Foam Mattress', 'Queen size memory foam mattress with cooling gel', 799.99, 599.99, 'MATT-001', 10, 10, 5, 'SleepWell', 'White', 'Memory Foam', '60x80x12 inches', 55.0, 4.7, 45, '/assets/images/products/mattress.jpg', 1, 1, GETUTCDATE(), GETUTCDATE()),
('Bedside Nightstand', 'Modern nightstand with 2 drawers and USB charging', 149.99, NULL, 'NIGHT-001', 22, 10, 5, 'ModernBedroom', 'Walnut', 'Wood', '18x16x24 inches', 25.0, 4.4, 16, '/assets/images/products/nightstand.jpg', 0, 1, GETUTCDATE(), GETUTCDATE()),

-- Bathroom Category (ID: 6)
('Rainfall Shower Head', 'Large rainfall shower head with multiple spray settings', 159.99, 119.99, 'SHOWER-001', 28, 10, 6, 'AquaLux', 'Chrome', 'Stainless Steel', '8x8x4 inches', 3.5, 4.5, 33, '/assets/images/products/shower-head.jpg', 0, 1, GETUTCDATE(), GETUTCDATE()),
('Bamboo Bath Towel Set', 'Luxury bamboo bath towel set - ultra soft and absorbent', 89.99, NULL, 'TOWEL-001', 40, 10, 6, 'BambooLux', 'Beige', 'Bamboo Fiber', 'Various sizes', 2.2, 4.8, 28, '/assets/images/products/towel-set.jpg', 1, 1, GETUTCDATE(), GETUTCDATE());

PRINT 'Sample products inserted successfully!';

-- Check total products count
SELECT COUNT(*) AS TotalProducts FROM Products;