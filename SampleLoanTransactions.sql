-- Sample Loan Transactions for Testing
-- Run this script in MySQL Workbench to add sample transactions

-- First, let's add some sample transactions for existing customers

-- Add a purchase transaction for LC001 (Kamal Perera)
INSERT INTO loan_transactions (customer_id, transaction_type, amount, old_balance, new_balance, bill_number, notes) VALUES
('LC001', 'PURCHASE', 5000.00, 0.00, 5000.00, 'BILL001', 'Initial purchase - groceries');

-- Add a repayment transaction for LC001
INSERT INTO loan_transactions (customer_id, transaction_type, amount, old_balance, new_balance, bill_number, notes) VALUES
('LC001', 'REPAYMENT', 2000.00, 5000.00, 3000.00, '', 'Partial repayment');

-- Add another purchase for LC001
INSERT INTO loan_transactions (customer_id, transaction_type, amount, old_balance, new_balance, bill_number, notes) VALUES
('LC001', 'PURCHASE', 1500.00, 3000.00, 4500.00, 'BILL002', 'Additional purchase');

-- Add transactions for LC003 (Nimal Fernando)
INSERT INTO loan_transactions (customer_id, transaction_type, amount, old_balance, new_balance, bill_number, notes) VALUES
('LC003', 'PURCHASE', 7534.00, 0.00, 7534.00, 'BILL003', 'Large purchase - electronics');

-- Add a repayment for LC003
INSERT INTO loan_transactions (customer_id, transaction_type, amount, old_balance, new_balance, bill_number, notes) VALUES
('LC003', 'REPAYMENT', 3534.00, 7534.00, 4000.00, '', 'Major repayment');

-- Add transactions for P001 (pubudu)
INSERT INTO loan_transactions (customer_id, transaction_type, amount, old_balance, new_balance, bill_number, notes) VALUES
('P001', 'PURCHASE', 400.00, 0.00, 400.00, 'BILL004', 'Small purchase');

-- Update the current balances in loan_customers table
UPDATE loan_customers SET current_balance = 4500.00 WHERE customer_id = 'LC001';
UPDATE loan_customers SET current_balance = 4000.00 WHERE customer_id = 'LC003';
UPDATE loan_customers SET current_balance = 400.00 WHERE customer_id = 'P001'; 