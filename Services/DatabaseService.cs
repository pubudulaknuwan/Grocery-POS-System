using System;
using System.Data;
using MySql.Data.MySqlClient;
using VillageSmartPOS.Models;

namespace VillageSmartPOS.Services
{
    public class DatabaseService
    {
        private readonly string connectionString = "server=localhost;user=root;password=pubudu1234;database=posdb;";

        public void AddProduct(string name, string barcode, decimal price, decimal markedPrice, int quantity, string unitType = "unit", string unitMeasure = "pieces", string category = "", string supplier = "", string description = "")
        {
            try
            {
                // Check if barcode already exists
                if (BarcodeExists(barcode))
                {
                    throw new InvalidOperationException($"Barcode '{barcode}' already exists. Please use a unique barcode.");
                }

                using MySqlConnection conn = new MySqlConnection(connectionString);
                conn.Open();

                string query = @"INSERT INTO products (name, barcode, price, marked_price, quantity, unit_type, unit_measure, category, supplier, description, reorder_level) 
                               VALUES (@name, @barcode, @price, @markedPrice, @quantity, @unitType, @unitMeasure, @category, @supplier, @description, @reorderLevel)";
                using MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@name", name);
                cmd.Parameters.AddWithValue("@barcode", barcode);
                cmd.Parameters.AddWithValue("@price", price);
                cmd.Parameters.AddWithValue("@markedPrice", markedPrice);
                cmd.Parameters.AddWithValue("@quantity", quantity);
                cmd.Parameters.AddWithValue("@unitType", unitType);
                cmd.Parameters.AddWithValue("@unitMeasure", unitMeasure);
                cmd.Parameters.AddWithValue("@category", category);
                cmd.Parameters.AddWithValue("@supplier", supplier);
                cmd.Parameters.AddWithValue("@description", description);
                cmd.Parameters.AddWithValue("@reorderLevel", 10); // Default reorder level

                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                // Log error or show message
                System.Diagnostics.Debug.WriteLine($"Error adding product: {ex.Message}");
                throw; // Re-throw to handle in UI
            }
        }

        /// <summary>
        /// Checks if a barcode already exists in the database
        /// </summary>
        public bool BarcodeExists(string barcode)
        {
            try
            {
                using MySqlConnection conn = new MySqlConnection(connectionString);
                conn.Open();

                string query = "SELECT COUNT(*) FROM products WHERE barcode = @barcode";
                using MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@barcode", barcode);

                int count = Convert.ToInt32(cmd.ExecuteScalar());
                return count > 0;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error checking barcode existence: {ex.Message}");
                return false;
            }
        }

        public void UpdateProductQuantity(int productId, int newQuantity)
        {
            try
            {
                using MySqlConnection conn = new MySqlConnection(connectionString);
                conn.Open();

                string query = "UPDATE products SET quantity = @quantity, last_updated = CURRENT_TIMESTAMP WHERE id = @id";
                using MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@quantity", newQuantity);
                cmd.Parameters.AddWithValue("@id", productId);

                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error updating product quantity: {ex.Message}");
            }
        }

        public void UpdateProduct(int productId, string name, string barcode, decimal price, decimal markedPrice, int quantity, string category, string supplier, string description)
        {
            try
            {
                using MySqlConnection conn = new MySqlConnection(connectionString);
                conn.Open();

                string query = @"UPDATE products SET name = @name, barcode = @barcode, price = @price, 
                               marked_price = @markedPrice, quantity = @quantity, category = @category, 
                               supplier = @supplier, description = @description, last_updated = CURRENT_TIMESTAMP 
                               WHERE id = @id";
                using MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@name", name);
                cmd.Parameters.AddWithValue("@barcode", barcode);
                cmd.Parameters.AddWithValue("@price", price);
                cmd.Parameters.AddWithValue("@markedPrice", markedPrice);
                cmd.Parameters.AddWithValue("@quantity", quantity);
                cmd.Parameters.AddWithValue("@category", category);
                cmd.Parameters.AddWithValue("@supplier", supplier);
                cmd.Parameters.AddWithValue("@description", description);
                cmd.Parameters.AddWithValue("@id", productId);

                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error updating product: {ex.Message}");
            }
        }

        public void DeleteProduct(int productId)
        {
            try
            {
                using MySqlConnection conn = new MySqlConnection(connectionString);
                conn.Open();

                string query = "DELETE FROM products WHERE id = @id";
                using MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@id", productId);

                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error deleting product: {ex.Message}");
            }
        }

        public List<Product> GetAllProducts()
        {
            List<Product> products = new();
            try
            {
                using MySqlConnection conn = new MySqlConnection(connectionString);
                conn.Open();

                string query = "SELECT * FROM products ORDER BY name";
                using MySqlCommand cmd = new MySqlCommand(query, conn);
                using MySqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    var product = new Product
                    {
                        Id = Convert.ToInt32(reader["id"]),
                        Name = reader["name"].ToString() ?? string.Empty,
                        Barcode = reader["barcode"].ToString() ?? string.Empty,
                        Price = Convert.ToDecimal(reader["price"]),
                        MarkedPrice = reader["marked_price"] != DBNull.Value ? Convert.ToDecimal(reader["marked_price"]) : 0,
                        Quantity = Convert.ToInt32(reader["quantity"]),
                        UnitType = reader["unit_type"]?.ToString() ?? "unit",
                        UnitMeasure = reader["unit_measure"]?.ToString() ?? "pieces",
                        Category = reader["category"]?.ToString() ?? string.Empty,
                        Supplier = reader["supplier"]?.ToString() ?? string.Empty,
                        Description = reader["description"]?.ToString() ?? string.Empty,
                        ReorderLevel = reader["reorder_level"] != DBNull.Value ? Convert.ToInt32(reader["reorder_level"]) : 10,
                        LastUpdated = reader["updated_at"] != DBNull.Value ? Convert.ToDateTime(reader["updated_at"]) : DateTime.Now
                    };
                    products.Add(product);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error getting products: {ex.Message}");
            }

            return products;
        }

        public List<Product> GetLowStockProducts()
        {
            List<Product> products = new();
            try
            {
                using MySqlConnection conn = new MySqlConnection(connectionString);
                conn.Open();

                string query = "SELECT * FROM products WHERE quantity <= reorder_level ORDER BY quantity ASC";
                using MySqlCommand cmd = new MySqlCommand(query, conn);
                using MySqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    products.Add(new Product
                    {
                        Id = Convert.ToInt32(reader["id"]),
                        Name = reader["name"].ToString() ?? string.Empty,
                        Barcode = reader["barcode"].ToString() ?? string.Empty,
                        Price = Convert.ToDecimal(reader["price"]),
                        Quantity = Convert.ToInt32(reader["quantity"]),
                        Category = reader["category"]?.ToString() ?? string.Empty,
                        Supplier = reader["supplier"]?.ToString() ?? string.Empty,
                        Description = reader["description"]?.ToString() ?? string.Empty,
                        ReorderLevel = reader["reorder_level"] != DBNull.Value ? Convert.ToInt32(reader["reorder_level"]) : 10,
                        LastUpdated = reader["updated_at"] != DBNull.Value ? Convert.ToDateTime(reader["updated_at"]) : DateTime.Now
                    });
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error getting low stock products: {ex.Message}");
            }

            return products;
        }

        public List<Product> SearchProducts(string searchTerm)
        {
            List<Product> products = new();
            try
            {
                using MySqlConnection conn = new MySqlConnection(connectionString);
                conn.Open();

                string query = @"SELECT * FROM products 
                               WHERE name LIKE @search OR barcode LIKE @search OR category LIKE @search 
                               ORDER BY name";
                using MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@search", $"%{searchTerm}%");
                using MySqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    products.Add(new Product
                    {
                        Id = Convert.ToInt32(reader["id"]),
                        Name = reader["name"].ToString() ?? string.Empty,
                        Barcode = reader["barcode"].ToString() ?? string.Empty,
                        Price = Convert.ToDecimal(reader["price"]),
                        MarkedPrice = reader["marked_price"] != DBNull.Value ? Convert.ToDecimal(reader["marked_price"]) : 0,
                        Quantity = Convert.ToInt32(reader["quantity"]),
                        Category = reader["category"]?.ToString() ?? string.Empty,
                        Supplier = reader["supplier"]?.ToString() ?? string.Empty,
                        Description = reader["description"]?.ToString() ?? string.Empty,
                        ReorderLevel = reader["reorder_level"] != DBNull.Value ? Convert.ToInt32(reader["reorder_level"]) : 10,
                        LastUpdated = reader["updated_at"] != DBNull.Value ? Convert.ToDateTime(reader["updated_at"]) : DateTime.Now
                    });
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error searching products: {ex.Message}");
            }

            return products;
        }

        public Product? GetProductByNameOrBarcode(string searchTerm)
        {
            Product? product = null;

            try
            {
                using (var conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    // Search by barcode (exact match) OR product name (exact match first, then partial match)
                    string query = "SELECT * FROM products WHERE Barcode = @search OR Name = @search OR Name LIKE @searchPattern ORDER BY CASE WHEN Barcode = @search THEN 1 WHEN Name = @search THEN 2 ELSE 3 END LIMIT 1";
                    using (var cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@search", searchTerm);
                        cmd.Parameters.AddWithValue("@searchPattern", $"%{searchTerm}%");
                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                product = new Product
                                {
                                    Id = Convert.ToInt32(reader["id"]),
                                    Name = reader["Name"].ToString() ?? string.Empty,
                                    Barcode = reader["Barcode"].ToString() ?? string.Empty,
                                    Price = reader.GetDecimal("Price"),
                                    MarkedPrice = reader["marked_price"] != DBNull.Value ? Convert.ToDecimal(reader["marked_price"]) : 0,
                                    Quantity = reader.GetInt32("Quantity"),
                                    UnitType = reader["unit_type"]?.ToString() ?? "unit",
                                    UnitMeasure = reader["unit_measure"]?.ToString() ?? "pieces",
                                    Category = reader["category"]?.ToString() ?? string.Empty,
                                    Supplier = reader["supplier"]?.ToString() ?? string.Empty,
                                    Description = reader["description"]?.ToString() ?? string.Empty,
                                    ReorderLevel = reader["reorder_level"] != DBNull.Value ? Convert.ToInt32(reader["reorder_level"]) : 10,
                                    LastUpdated = reader["updated_at"] != DBNull.Value ? Convert.ToDateTime(reader["updated_at"]) : DateTime.Now
                                };
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error getting product by name/barcode: {ex.Message}");
            }

            return product;
        }

        public List<Product> GetProductSuggestions(string searchTerm, int limit = 10)
        {
            List<Product> suggestions = new();
            
            try
            {
                using (var conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    // Search only by product name with partial matching
                    string query = @"SELECT * FROM products 
                                   WHERE Name LIKE @searchPattern 
                                   ORDER BY CASE 
                                       WHEN Name = @search THEN 1 
                                       WHEN Name LIKE @searchStartPattern THEN 2
                                       WHEN Name LIKE @searchPattern THEN 3
                                       ELSE 4 
                                   END, Name 
                                   LIMIT @limit";
                    
                    using (var cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@search", searchTerm);
                        cmd.Parameters.AddWithValue("@searchPattern", $"%{searchTerm}%");
                        cmd.Parameters.AddWithValue("@searchStartPattern", $"{searchTerm}%");
                        cmd.Parameters.AddWithValue("@limit", limit);
                        
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                suggestions.Add(new Product
                                {
                                    Id = Convert.ToInt32(reader["id"]),
                                    Name = reader["Name"].ToString() ?? string.Empty,
                                    Barcode = reader["Barcode"].ToString() ?? string.Empty,
                                    Price = reader.GetDecimal("Price"),
                                    MarkedPrice = reader["marked_price"] != DBNull.Value ? Convert.ToDecimal(reader["marked_price"]) : 0,
                                    Quantity = reader.GetInt32("Quantity"),
                                    UnitType = reader["unit_type"]?.ToString() ?? "unit",
                                    UnitMeasure = reader["unit_measure"]?.ToString() ?? "pieces",
                                    Category = reader["category"]?.ToString() ?? string.Empty,
                                    Supplier = reader["supplier"]?.ToString() ?? string.Empty,
                                    Description = reader["description"]?.ToString() ?? string.Empty,
                                    ReorderLevel = reader["reorder_level"] != DBNull.Value ? Convert.ToInt32(reader["reorder_level"]) : 10,
                                    LastUpdated = reader["updated_at"] != DBNull.Value ? Convert.ToDateTime(reader["updated_at"]) : DateTime.Now
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error getting product suggestions: {ex.Message}");
            }

            return suggestions;
        }

        public void UpdateStockAfterSale(string barcode, decimal soldQuantity)
        {
            try
            {
                using MySqlConnection conn = new MySqlConnection(connectionString);
                conn.Open();

                string query = "UPDATE products SET quantity = quantity - @soldQuantity WHERE barcode = @barcode";
                using MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@soldQuantity", soldQuantity);
                cmd.Parameters.AddWithValue("@barcode", barcode);

                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error updating stock after sale: {ex.Message}");
            }
        }

        // ===== LOAN CUSTOMER MANAGEMENT METHODS =====

        public void AddLoanCustomer(string customerId, string name, string phone, string address)
        {
            try
            {
                using MySqlConnection conn = new MySqlConnection(connectionString);
                conn.Open();

                string query = @"INSERT INTO loan_customers (customer_id, name, phone, address) 
                               VALUES (@customerId, @name, @phone, @address)";
                using MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@customerId", customerId);
                cmd.Parameters.AddWithValue("@name", name);
                cmd.Parameters.AddWithValue("@phone", phone);
                cmd.Parameters.AddWithValue("@address", address);

                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error adding loan customer: {ex.Message}");
                throw;
            }
        }

        public void UpdateLoanCustomer(int id, string customerId, string name, string phone, string address)
        {
            try
            {
                using MySqlConnection conn = new MySqlConnection(connectionString);
                conn.Open();

                string query = @"UPDATE loan_customers SET customer_id = @customerId, name = @name, 
                               phone = @phone, address = @address WHERE id = @id";
                using MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@customerId", customerId);
                cmd.Parameters.AddWithValue("@name", name);
                cmd.Parameters.AddWithValue("@phone", phone);
                cmd.Parameters.AddWithValue("@address", address);
                cmd.Parameters.AddWithValue("@id", id);

                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error updating loan customer: {ex.Message}");
                throw;
            }
        }

        public void DeleteLoanCustomer(int id)
        {
            try
            {
                using MySqlConnection conn = new MySqlConnection(connectionString);
                conn.Open();

                string query = "DELETE FROM loan_customers WHERE id = @id";
                using MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@id", id);

                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error deleting loan customer: {ex.Message}");
                throw;
            }
        }

        public List<LoanCustomer> GetAllLoanCustomers()
        {
            var customers = new List<LoanCustomer>();
            try
            {
                using MySqlConnection conn = new MySqlConnection(connectionString);
                conn.Open();

                string query = "SELECT * FROM loan_customers ORDER BY name";
                using MySqlCommand cmd = new MySqlCommand(query, conn);
                using MySqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    customers.Add(new LoanCustomer
                    {
                        Id = reader.GetInt32("id"),
                        CustomerId = reader.GetString("customer_id"),
                        Name = reader.GetString("name"),
                        Phone = reader.IsDBNull("phone") ? "" : reader.GetString("phone"),
                        Address = reader.IsDBNull("address") ? "" : reader.GetString("address"),
                        CurrentBalance = reader.GetDecimal("current_balance"),
                        CreatedDate = reader.GetDateTime("created_date"),
                        UpdatedDate = reader.GetDateTime("updated_date")
                    });
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error getting loan customers: {ex.Message}");
            }
            return customers;
        }

        public LoanCustomer? GetLoanCustomerById(string customerId)
        {
            try
            {
                using MySqlConnection conn = new MySqlConnection(connectionString);
                conn.Open();

                string query = "SELECT * FROM loan_customers WHERE customer_id = @customerId";
                using MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@customerId", customerId);
                using MySqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    return new LoanCustomer
                    {
                        Id = reader.GetInt32("id"),
                        CustomerId = reader.GetString("customer_id"),
                        Name = reader.GetString("name"),
                        Phone = reader.IsDBNull("phone") ? "" : reader.GetString("phone"),
                        Address = reader.IsDBNull("address") ? "" : reader.GetString("address"),
                        CurrentBalance = reader.GetDecimal("current_balance"),
                        CreatedDate = reader.GetDateTime("created_date"),
                        UpdatedDate = reader.GetDateTime("updated_date")
                    };
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error getting loan customer: {ex.Message}");
            }
            return null;
        }

        public List<LoanCustomer> SearchLoanCustomers(string searchTerm)
        {
            var customers = new List<LoanCustomer>();
            try
            {
                using MySqlConnection conn = new MySqlConnection(connectionString);
                conn.Open();

                string query = @"SELECT * FROM loan_customers 
                               WHERE customer_id LIKE @searchTerm OR name LIKE @searchTerm OR phone LIKE @searchTerm 
                               ORDER BY name";
                using MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@searchTerm", $"%{searchTerm}%");
                using MySqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    customers.Add(new LoanCustomer
                    {
                        Id = reader.GetInt32("id"),
                        CustomerId = reader.GetString("customer_id"),
                        Name = reader.GetString("name"),
                        Phone = reader.IsDBNull("phone") ? "" : reader.GetString("phone"),
                        Address = reader.IsDBNull("address") ? "" : reader.GetString("address"),
                        CurrentBalance = reader.GetDecimal("current_balance"),
                        CreatedDate = reader.GetDateTime("created_date"),
                        UpdatedDate = reader.GetDateTime("updated_date")
                    });
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error searching loan customers: {ex.Message}");
            }
            return customers;
        }

        public void AddLoanTransaction(string customerId, TransactionType transactionType, decimal amount, 
                                     decimal oldBalance, decimal newBalance, string billNumber = "", string notes = "")
        {
            try
            {
                using MySqlConnection conn = new MySqlConnection(connectionString);
                conn.Open();

                // Add transaction record
                string transactionQuery = @"INSERT INTO loan_transactions 
                                         (customer_id, transaction_type, amount, old_balance, new_balance, bill_number, notes) 
                                         VALUES (@customerId, @transactionType, @amount, @oldBalance, @newBalance, @billNumber, @notes)";
                using MySqlCommand cmd = new MySqlCommand(transactionQuery, conn);
                cmd.Parameters.AddWithValue("@customerId", customerId);
                cmd.Parameters.AddWithValue("@transactionType", transactionType.ToString());
                cmd.Parameters.AddWithValue("@amount", amount);
                cmd.Parameters.AddWithValue("@oldBalance", oldBalance);
                cmd.Parameters.AddWithValue("@newBalance", newBalance);
                cmd.Parameters.AddWithValue("@billNumber", billNumber);
                cmd.Parameters.AddWithValue("@notes", notes);

                cmd.ExecuteNonQuery();

                // Update customer balance
                string updateQuery = "UPDATE loan_customers SET current_balance = @newBalance WHERE customer_id = @customerId";
                using MySqlCommand updateCmd = new MySqlCommand(updateQuery, conn);
                updateCmd.Parameters.AddWithValue("@newBalance", newBalance);
                updateCmd.Parameters.AddWithValue("@customerId", customerId);

                updateCmd.ExecuteNonQuery();

                // Cleanup old transactions if this customer has more than 15 transactions
                // This ensures we keep the table size manageable
                string countQuery = "SELECT COUNT(*) FROM loan_transactions WHERE customer_id = @customerId";
                using MySqlCommand countCmd = new MySqlCommand(countQuery, conn);
                countCmd.Parameters.AddWithValue("@customerId", customerId);
                int transactionCount = Convert.ToInt32(countCmd.ExecuteScalar());

                if (transactionCount > 15)
                {
                    CleanupOldTransactions(customerId);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error adding loan transaction: {ex.Message}");
                throw;
            }
        }

        public List<LoanTransaction> GetLoanTransactions(string customerId)
        {
            var transactions = new List<LoanTransaction>();
            try
            {
                using MySqlConnection conn = new MySqlConnection(connectionString);
                conn.Open();

                // Only get the last 10 transactions for each customer
                string query = @"SELECT * FROM loan_transactions 
                               WHERE customer_id = @customerId 
                               ORDER BY transaction_date DESC 
                               LIMIT 10";
                using MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@customerId", customerId);
                
                using MySqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    var transaction = new LoanTransaction
                    {
                        Id = reader.GetInt32("id"),
                        CustomerId = reader.GetString("customer_id"),
                        TransactionType = Enum.Parse<TransactionType>(reader.GetString("transaction_type")),
                        Amount = reader.GetDecimal("amount"),
                        OldBalance = reader.GetDecimal("old_balance"),
                        NewBalance = reader.GetDecimal("new_balance"),
                        BillNumber = reader.IsDBNull("bill_number") ? "" : reader.GetString("bill_number"),
                        TransactionDate = reader.GetDateTime("transaction_date"),
                        Notes = reader.IsDBNull("notes") ? "" : reader.GetString("notes")
                    };
                    transactions.Add(transaction);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error getting loan transactions: {ex.Message}");
            }
            return transactions;
        }

        public void UpdateLoanBalance(string customerId, decimal newBalance)
        {
            try
            {
                using MySqlConnection conn = new MySqlConnection(connectionString);
                conn.Open();

                string query = "UPDATE loan_customers SET current_balance = @newBalance WHERE customer_id = @customerId";
                using MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@newBalance", newBalance);
                cmd.Parameters.AddWithValue("@customerId", customerId);

                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error updating loan balance: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Cleanup old transactions for a customer, keeping only the last 10 transactions
        /// This method preserves the customer's final balance
        /// </summary>
        public void CleanupOldTransactions(string customerId)
        {
            try
            {
                using MySqlConnection conn = new MySqlConnection(connectionString);
                conn.Open();

                // Get the customer's current balance first
                var customer = GetLoanCustomerById(customerId);
                if (customer == null) return;

                // Delete old transactions, keeping only the last 10
                string deleteQuery = @"
                    DELETE FROM loan_transactions 
                    WHERE customer_id = @customerId 
                    AND id NOT IN (
                        SELECT id FROM (
                            SELECT id FROM loan_transactions 
                            WHERE customer_id = @customerId 
                            ORDER BY transaction_date DESC 
                            LIMIT 10
                        ) AS recent_transactions
                    )";

                using MySqlCommand cmd = new MySqlCommand(deleteQuery, conn);
                cmd.Parameters.AddWithValue("@customerId", customerId);
                int deletedCount = cmd.ExecuteNonQuery();

                if (deletedCount > 0)
                {
                    System.Diagnostics.Debug.WriteLine($"Cleaned up {deletedCount} old transactions for customer {customerId}");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error cleaning up old transactions: {ex.Message}");
            }
        }

        /// <summary>
        /// Cleanup old transactions for all customers, keeping only the last 10 transactions per customer
        /// </summary>
        public void CleanupAllOldTransactions()
        {
            try
            {
                var customers = GetAllLoanCustomers();
                foreach (var customer in customers)
                {
                    CleanupOldTransactions(customer.CustomerId);
                }
                System.Diagnostics.Debug.WriteLine("Completed cleanup of old transactions for all customers");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error cleaning up all old transactions: {ex.Message}");
            }
        }

        public void AddSampleTransactions()
        {
            try
            {
                using MySqlConnection conn = new MySqlConnection(connectionString);
                conn.Open();

                // Add sample transactions for testing
                string query = @"INSERT INTO loan_transactions (customer_id, transaction_type, amount, old_balance, new_balance, description) 
                               VALUES (1, 'loan', 1000.00, 0.00, 1000.00, 'Initial loan'),
                                      (1, 'payment', 500.00, 1000.00, 500.00, 'Partial payment'),
                                      (1, 'loan', 1000.00, 500.00, 1500.00, 'Additional loan')";
                using MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error adding sample transactions: {ex.Message}");
            }
        }

        // System Settings Methods
        public Dictionary<string, string> GetAllBillSettings()
        {
            try
            {
                using MySqlConnection conn = new MySqlConnection(connectionString);
                conn.Open();

                string query = "SELECT * FROM bill_settings LIMIT 1";
                using MySqlCommand cmd = new MySqlCommand(query, conn);
                using var reader = cmd.ExecuteReader();

                var settings = new Dictionary<string, string>();
                if (reader.Read())
                {
                    settings["grocery_name"] = reader["grocery_name"].ToString() ?? "";
                    settings["store_address"] = reader["store_address"].ToString() ?? "";
                    settings["phone_number_1"] = reader["phone_number_1"].ToString() ?? "";
                    settings["phone_number_2"] = reader["phone_number_2"].ToString() ?? "";
                    settings["cashier_name"] = reader["cashier_name"].ToString() ?? "";
                    settings["logo_text"] = reader["logo_text"].ToString() ?? "";
                    settings["receipt_header"] = reader["receipt_header"].ToString() ?? "";
                    settings["receipt_footer"] = reader["receipt_footer"].ToString() ?? "";
                    settings["currency_symbol"] = reader["currency_symbol"].ToString() ?? "";
                    settings["tax_rate"] = reader["tax_rate"].ToString() ?? "";
                }

                return settings;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error getting bill settings: {ex.Message}");
                return new Dictionary<string, string>();
            }
        }

        public string GetBillSetting(string columnName, string defaultValue = "")
        {
            try
            {
                using MySqlConnection conn = new MySqlConnection(connectionString);
                conn.Open();

                string query = $"SELECT {columnName} FROM bill_settings LIMIT 1";
                using MySqlCommand cmd = new MySqlCommand(query, conn);

                var result = cmd.ExecuteScalar();
                return result?.ToString() ?? defaultValue;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error getting bill setting {columnName}: {ex.Message}");
                return defaultValue;
            }
        }

        public void SaveBillSettings(Dictionary<string, string> settings)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("=== SaveBillSettings called ===");
                System.Diagnostics.Debug.WriteLine($"Connection string: {connectionString}");
                System.Diagnostics.Debug.WriteLine($"Settings count: {settings.Count}");
                
                foreach (var setting in settings)
                {
                    System.Diagnostics.Debug.WriteLine($"Setting: {setting.Key} = '{setting.Value}'");
                }
                
                using MySqlConnection conn = new MySqlConnection(connectionString);
                conn.Open();
                System.Diagnostics.Debug.WriteLine("Database connection opened successfully");

                // Start transaction for better data integrity
                using var transaction = conn.BeginTransaction();

                try
                {
                    // First, let's check what's currently in the database
                    string currentQuery = "SELECT * FROM bill_settings WHERE id = 1";
                    using MySqlCommand currentCmd = new MySqlCommand(currentQuery, conn);
                    currentCmd.Transaction = transaction;
                    using var currentReader = currentCmd.ExecuteReader();
                    if (currentReader.Read())
                    {
                        System.Diagnostics.Debug.WriteLine("Current database values:");
                        System.Diagnostics.Debug.WriteLine($"  grocery_name: '{currentReader["grocery_name"]}'");
                        System.Diagnostics.Debug.WriteLine($"  store_address: '{currentReader["store_address"]}'");
                        System.Diagnostics.Debug.WriteLine($"  updated_at: '{currentReader["updated_at"]}'");
                    }
                    currentReader.Close();

                    // Check if default_printer column exists
                    bool hasDefaultPrinterColumn = false;
                    try
                    {
                        string checkColumnQuery = "SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA = 'posdb' AND TABLE_NAME = 'bill_settings' AND COLUMN_NAME = 'default_printer'";
                        using MySqlCommand checkColumnCmd = new MySqlCommand(checkColumnQuery, conn);
                        checkColumnCmd.Transaction = transaction;
                        var columnExists = checkColumnCmd.ExecuteScalar();
                        hasDefaultPrinterColumn = columnExists != null;
                        System.Diagnostics.Debug.WriteLine($"default_printer column exists: {hasDefaultPrinterColumn}");
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"Error checking for default_printer column: {ex.Message}");
                        hasDefaultPrinterColumn = false;
                    }

                    // Build the UPDATE query dynamically, excluding default_printer if column doesn't exist
                    var updateColumns = new List<string>();
                    var parameters = new List<MySqlParameter>();

                    foreach (var setting in settings)
                    {
                        // Skip default_printer if the column doesn't exist
                        if (setting.Key == "default_printer" && !hasDefaultPrinterColumn)
                        {
                            System.Diagnostics.Debug.WriteLine("Skipping default_printer as column doesn't exist");
                            continue;
                        }
                        
                        updateColumns.Add($"{setting.Key} = @{setting.Key}");
                        parameters.Add(new MySqlParameter($"@{setting.Key}", setting.Value));
                    }

                    string updateQuery = $"UPDATE bill_settings SET {string.Join(", ", updateColumns)}, updated_at = CURRENT_TIMESTAMP WHERE id = 1";
                    
                    // If no record exists, create one
                    string checkQuery = "SELECT COUNT(*) FROM bill_settings";
                    using MySqlCommand checkCmd = new MySqlCommand(checkQuery, conn);
                    checkCmd.Transaction = transaction;
                    int count = Convert.ToInt32(checkCmd.ExecuteScalar());
                    System.Diagnostics.Debug.WriteLine($"Records in bill_settings table: {count}");

                    if (count == 0)
                    {
                        System.Diagnostics.Debug.WriteLine("No records found, inserting new record");
                        // Insert new record
                        var insertColumns = settings.Keys.ToList();
                        // Remove default_printer if column doesn't exist
                        if (!hasDefaultPrinterColumn)
                        {
                            insertColumns.Remove("default_printer");
                        }
                        var insertValues = insertColumns.Select(col => $"@{col}").ToList();
                        
                        string insertQuery = $"INSERT INTO bill_settings ({string.Join(", ", insertColumns)}) VALUES ({string.Join(", ", insertValues)})";
                        using MySqlCommand insertCmd = new MySqlCommand(insertQuery, conn);
                        insertCmd.Transaction = transaction;
                        
                        foreach (var param in parameters)
                        {
                            // Only add parameters that are in the insert columns
                            if (insertColumns.Contains(param.ParameterName.Substring(1))) // Remove @ prefix
                            {
                                insertCmd.Parameters.Add(param);
                            }
                        }

                        System.Diagnostics.Debug.WriteLine($"SQL Query: {insertQuery}");
                        int rowsAffected = insertCmd.ExecuteNonQuery();
                        System.Diagnostics.Debug.WriteLine($"Rows affected: {rowsAffected}");
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine("Records found, updating existing record");
                        // Update existing record
                        using MySqlCommand updateCmd = new MySqlCommand(updateQuery, conn);
                        updateCmd.Transaction = transaction;
                        
                        foreach (var param in parameters)
                        {
                            updateCmd.Parameters.Add(param);
                        }

                        System.Diagnostics.Debug.WriteLine($"SQL Query: {updateQuery}");
                        int rowsAffected = updateCmd.ExecuteNonQuery();
                        System.Diagnostics.Debug.WriteLine($"Rows affected: {rowsAffected}");
                        
                        if (rowsAffected == 0)
                        {
                            System.Diagnostics.Debug.WriteLine("WARNING: No rows were updated!");
                            throw new Exception("No rows were updated in bill_settings table");
                        }
                    }

                    // Commit the transaction
                    transaction.Commit();
                    System.Diagnostics.Debug.WriteLine("Transaction committed successfully");
                    
                    // Verify the update immediately after commit
                    string verifyQuery = "SELECT grocery_name FROM bill_settings WHERE id = 1";
                    using MySqlCommand verifyCmd = new MySqlCommand(verifyQuery, conn);
                    var result = verifyCmd.ExecuteScalar();
                    System.Diagnostics.Debug.WriteLine($"Verified grocery_name after commit: '{result}'");
                    
                    // Also verify all other fields
                    string verifyAllQuery = "SELECT * FROM bill_settings WHERE id = 1";
                    using MySqlCommand verifyAllCmd = new MySqlCommand(verifyAllQuery, conn);
                    using var verifyAllReader = verifyAllCmd.ExecuteReader();
                    if (verifyAllReader.Read())
                    {
                        System.Diagnostics.Debug.WriteLine("All values after commit:");
                        System.Diagnostics.Debug.WriteLine($"  grocery_name: '{verifyAllReader["grocery_name"]}'");
                        System.Diagnostics.Debug.WriteLine($"  store_address: '{verifyAllReader["store_address"]}'");
                        System.Diagnostics.Debug.WriteLine($"  phone_number_1: '{verifyAllReader["phone_number_1"]}'");
                        System.Diagnostics.Debug.WriteLine($"  phone_number_2: '{verifyAllReader["phone_number_2"]}'");
                        System.Diagnostics.Debug.WriteLine($"  cashier_name: '{verifyAllReader["cashier_name"]}'");
                        System.Diagnostics.Debug.WriteLine($"  logo_text: '{verifyAllReader["logo_text"]}'");
                        System.Diagnostics.Debug.WriteLine($"  updated_at: '{verifyAllReader["updated_at"]}'");
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"ERROR in transaction: {ex.Message}");
                    transaction.Rollback();
                    throw;
                }
                
                System.Diagnostics.Debug.WriteLine("=== Bill settings saved successfully ===");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"ERROR saving bill settings: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Stack trace: {ex.StackTrace}");
                throw; // Re-throw to let the caller know about the error
            }
        }

        public void InitializeBillSettings()
        {
            try
            {
                using MySqlConnection conn = new MySqlConnection(connectionString);
                conn.Open();

                // Check if bill settings already exist
                string checkQuery = "SELECT COUNT(*) FROM bill_settings";
                using MySqlCommand checkCmd = new MySqlCommand(checkQuery, conn);
                int count = Convert.ToInt32(checkCmd.ExecuteScalar());

                if (count == 0)
                {
                    // Insert default bill settings
                    string insertQuery = @"INSERT INTO bill_settings (grocery_name, store_address, phone_number_1, phone_number_2, cashier_name, logo_text, receipt_header, receipt_footer, currency_symbol, tax_rate) VALUES
                                        ('රසිංහ වෙළඳසැල', 'රන්දෙණිය, පිරිබැද්දර, කාගල්ල', '0352263213', '0763082845', 'Avindra Ranasinghe', 'RS', 'Thank you for shopping with us!', 'Please visit again!', 'Rs.', 0.00)";

                    using MySqlCommand insertCmd = new MySqlCommand(insertQuery, conn);
                    insertCmd.ExecuteNonQuery();
                    System.Diagnostics.Debug.WriteLine("Default bill settings initialized");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error initializing bill settings: {ex.Message}");
            }
        }
    }
}

