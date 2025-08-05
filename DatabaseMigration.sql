-- Create database
CREATE DATABASE IF NOT EXISTS villagesmartpos;
USE villagesmartpos;

-- Create products table with unit type system
CREATE TABLE IF NOT EXISTS products (
    id INT AUTO_INCREMENT PRIMARY KEY,
    name VARCHAR(255) NOT NULL,
    barcode VARCHAR(50) UNIQUE,
    price DECIMAL(10,2) NOT NULL,
    marked_price DECIMAL(10,2) NOT NULL,
    quantity INT NOT NULL DEFAULT 0,
    unit_type ENUM('mass', 'unit') NOT NULL DEFAULT 'unit',
    unit_measure VARCHAR(20) NOT NULL DEFAULT 'pieces',
    category VARCHAR(100),
    supplier VARCHAR(100),
    reorder_level INT DEFAULT 10,
    description TEXT,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP
);

-- Create bill_items table
CREATE TABLE IF NOT EXISTS bill_items (
    id INT AUTO_INCREMENT PRIMARY KEY,
    bill_id VARCHAR(50) NOT NULL,
    product_id INT NOT NULL,
    product_name VARCHAR(255) NOT NULL,
    quantity INT NOT NULL,
    unit_price DECIMAL(10,2) NOT NULL,
    total_price DECIMAL(10,2) NOT NULL,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (product_id) REFERENCES products(id) ON DELETE CASCADE
);

-- Create loan_customers table
CREATE TABLE IF NOT EXISTS loan_customers (
    id INT AUTO_INCREMENT PRIMARY KEY,
    name VARCHAR(255) NOT NULL,
    phone VARCHAR(20),
    address TEXT,
    balance DECIMAL(10,2) DEFAULT 0.00,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP
);

-- Create loan_transactions table
CREATE TABLE IF NOT EXISTS loan_transactions (
    id INT AUTO_INCREMENT PRIMARY KEY,
    customer_id INT NOT NULL,
    transaction_type ENUM('payment', 'loan') NOT NULL,
    amount DECIMAL(10,2) NOT NULL,
    old_balance DECIMAL(10,2) NOT NULL,
    new_balance DECIMAL(10,2) NOT NULL,
    description TEXT,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (customer_id) REFERENCES loan_customers(id) ON DELETE CASCADE
);

-- Create bill_settings table for bill customization
CREATE TABLE IF NOT EXISTS bill_settings (
    id INT AUTO_INCREMENT PRIMARY KEY,
    grocery_name VARCHAR(255) NOT NULL DEFAULT 'රසිංහ වෙළඳසැල',
    store_address TEXT NOT NULL,
    phone_number_1 VARCHAR(20) NOT NULL DEFAULT '0352263213',
    phone_number_2 VARCHAR(20) NOT NULL DEFAULT '0763082845',
    cashier_name VARCHAR(255) NOT NULL DEFAULT 'Avindra Ranasinghe',
    logo_text VARCHAR(50) NOT NULL DEFAULT 'RS',
    receipt_header TEXT NOT NULL,
    receipt_footer TEXT NOT NULL,
    currency_symbol VARCHAR(10) NOT NULL DEFAULT 'Rs.',
    tax_rate DECIMAL(5,2) NOT NULL DEFAULT 0.00,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP
);

-- Insert default bill settings (only if table is empty)
INSERT INTO bill_settings (grocery_name, store_address, phone_number_1, phone_number_2, cashier_name, logo_text, receipt_header, receipt_footer, currency_symbol, tax_rate)
SELECT * FROM (
    SELECT 'රසිංහ වෙළඳසැල' as grocery_name,
           'රන්දෙණිය, පිරිබැද්දර, කාගල්ල' as store_address,
           '0352263213' as phone_number_1,
           '0763082845' as phone_number_2,
           'Avindra Ranasinghe' as cashier_name,
           'RS' as logo_text,
           'Thank you for shopping with us!' as receipt_header,
           'Please visit again!' as receipt_footer,
           'Rs.' as currency_symbol,
           0.00 as tax_rate
) AS temp
WHERE NOT EXISTS (SELECT 1 FROM bill_settings LIMIT 1);

-- Insert sample products with unit type information
INSERT INTO products (name, barcode, price, marked_price, quantity, unit_type, unit_measure, category, supplier, description) VALUES
('samba hal', '1', 200.00, 240.00, 459, 'mass', 'kg', 'General', 'Local Supplier', 'Premium quality rice'),
('nadu hal', '2', 240.00, 288.00, 49, 'mass', 'kg', 'General', 'Local Supplier', 'Medium grain rice'),
('kiri samba', '3', 100.00, 120.00, 472, 'mass', 'kg', 'hal', 'Local Supplier', 'Milk rice variety'),
('hal massa', '23', 400.00, 480.00, 339, 'mass', 'kg', 'karawala', 'Local Supplier', 'Special rice variety'),
('ala', '6', 300.00, 360.00, 79, 'unit', 'pieces', 'ala', 'Local Supplier', 'Potatoes'),
('banis', '8', 100.00, 120.00, 79, 'unit', 'pieces', 'pan', 'Local Supplier', 'Bread'),
('biscut', '4', 60.00, 72.00, 72, 'unit', 'pieces', 'General', 'Local Supplier', 'Biscuits'),
('lunu', '7', 100.00, 120.00, 214, 'unit', 'pieces', 'sdf', 'Local Supplier', 'Salt'),
('pan', '9', 250.00, 300.00, 185, 'unit', 'pieces', 'zsdf', 'Local Supplier', 'Bread variety'),
('samba sahal', '10', 250.00, 300.00, 285, 'mass', 'kg', 'sahal', 'Local Supplier', 'Rice variety'),
('sini', '5', 234.00, 280.80, 79, 'mass', 'kg', 'sillara', 'Local Supplier', 'Sugar');

-- Insert sample loan customers
INSERT INTO loan_customers (name, phone, address, balance) VALUES
('John Doe', '0771234567', '123 Main Street, Colombo', 1500.00),
('Jane Smith', '0769876543', '456 Oak Avenue, Kandy', 2300.00),
('Bob Johnson', '0755555555', '789 Pine Road, Galle', 800.00);

-- Insert sample loan transactions
INSERT INTO loan_transactions (customer_id, transaction_type, amount, old_balance, new_balance, description) VALUES
(1, 'loan', 1000.00, 0.00, 1000.00, 'Initial loan'),
(1, 'payment', 500.00, 1000.00, 500.00, 'Partial payment'),
(1, 'loan', 1000.00, 500.00, 1500.00, 'Additional loan'),
(2, 'loan', 2000.00, 0.00, 2000.00, 'Initial loan'),
(2, 'loan', 300.00, 2000.00, 2300.00, 'Additional loan'),
(3, 'loan', 800.00, 0.00, 800.00, 'Initial loan'); 