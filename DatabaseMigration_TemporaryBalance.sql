-- Temporary Balance Table for VillageSmart POS
-- This table stores temporary balances for customers who couldn't pay full amount

-- Drop table if exists (for clean recreation)
DROP TABLE IF EXISTS temporary_balances;

-- Create temporary_balances table
CREATE TABLE temporary_balances (
    id INT AUTO_INCREMENT PRIMARY KEY,
    customer_name VARCHAR(255) NOT NULL,
    balance DECIMAL(10,2) NOT NULL DEFAULT 0.00,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    notes TEXT,
    INDEX idx_customer_name (customer_name),
    INDEX idx_balance (balance)
);

-- Insert some sample data for testing
INSERT INTO temporary_balances (customer_name, balance, notes) VALUES
('කමල් පෙරේරා', 150.00, 'Rice and vegetables - 2025-08-05'),
('සුනිල් සිල්වා', 75.50, 'Milk and bread - 2025-08-04'),
('නිර්මලා ප්‍රනාන්දු', 200.00, 'Groceries - 2025-08-03'),
('රංජිත් මෙන්ඩිස්', 45.75, 'Snacks and drinks - 2025-08-02'),
('අනුෂා විජේසේකර', 120.25, 'Household items - 2025-08-01');

-- Display the created table structure
DESCRIBE temporary_balances;

-- Display sample data
SELECT * FROM temporary_balances ORDER BY created_at DESC; 