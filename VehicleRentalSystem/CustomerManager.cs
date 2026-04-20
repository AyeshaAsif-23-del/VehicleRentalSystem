using System;
using MySql.Data.MySqlClient;

namespace VehicleRentalSystem
{
    public class CustomerManager
    {
        // 1. REGISTER CUSTOMER
        public void AddCustomer()
        {
            Console.Clear();
            Console.WriteLine("--- Register New Customer ---");
            Console.Write("Enter Name: "); string name = Console.ReadLine();
            Console.Write("Enter CNIC: "); string cnic = Console.ReadLine();
            Console.Write("Enter Phone: "); string phone = Console.ReadLine();
            Console.Write("Enter License Number: "); string license = Console.ReadLine();

            using (var conn = DbConfig.GetConnection())
            {
                try
                {
                    conn.Open();
                    string query = "INSERT INTO Customers (Name, CNIC, Phone, LicenseNumber) VALUES (@n, @c, @p, @l)";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@n", name);
                    cmd.Parameters.AddWithValue("@c", cnic);
                    cmd.Parameters.AddWithValue("@p", phone);
                    cmd.Parameters.AddWithValue("@l", license);
                    cmd.ExecuteNonQuery();
                    Console.WriteLine("\nCustomer registered successfully!");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("\nError: " + ex.Message);
                }
            }
            Console.ReadKey();
        }

        // 2. VIEW ALL CUSTOMERS
        public void ViewCustomers()
        {
            Console.Clear();
            Console.WriteLine("--- Registered Customers List ---");
            using (var conn = DbConfig.GetConnection())
            {
                conn.Open();
                string query = "SELECT * FROM Customers";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                using (var reader = cmd.ExecuteReader())
                {
                    PrintCustomerTable(reader);
                }
            }
            Console.WriteLine("\nPress any key to return...");
            Console.ReadKey();
        }

        // 3. SEARCH CUSTOMER (The "Full-Fledge" Addition)
        public void SearchCustomer()
        {
            Console.Clear();
            Console.WriteLine("--- Search Customer ---");
            Console.Write("Enter Name or CNIC to search: ");
            string search = Console.ReadLine();

            using (var conn = DbConfig.GetConnection())
            {
                conn.Open();
                string query = "SELECT * FROM Customers WHERE Name LIKE @s OR CNIC LIKE @s";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@s", "%" + search + "%");

                using (var reader = cmd.ExecuteReader())
                {
                    PrintCustomerTable(reader);
                }
            }
            Console.WriteLine("\nPress any key to return...");
            Console.ReadKey();
        }

        // 4. UPDATE CUSTOMER INFO
        public void UpdateCustomer()
        {
            Console.Clear();
            Console.WriteLine("--- Update Customer Details ---");
            Console.Write("Enter Customer ID to update: ");
            if (!int.TryParse(Console.ReadLine(), out int cId)) return;

            Console.Write("Enter New Phone Number: ");
            string newPhone = Console.ReadLine();

            using (var conn = DbConfig.GetConnection())
            {
                conn.Open();
                string query = "UPDATE Customers SET Phone = @p WHERE CustomerID = @id";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@p", newPhone);
                cmd.Parameters.AddWithValue("@id", cId);

                int rows = cmd.ExecuteNonQuery();
                Console.WriteLine(rows > 0 ? "Update Successful!" : "Customer ID not found.");
            }
            Console.ReadKey();
        }

        // 5. DELETE CUSTOMER
        public void DeleteCustomer()
        {
            Console.Clear();
            Console.WriteLine("--- Delete Customer Account ---");
            Console.Write("Enter Customer ID to DELETE: ");
            if (!int.TryParse(Console.ReadLine(), out int cId)) return;

            Console.Write("Are you sure? (Y/N): ");
            if (Console.ReadLine().ToUpper() != "Y") return;

            using (var conn = DbConfig.GetConnection())
            {
                try
                {
                    conn.Open();
                    string query = "DELETE FROM Customers WHERE CustomerID = @id";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@id", cId);
                    int rows = cmd.ExecuteNonQuery();
                    Console.WriteLine(rows > 0 ? "Customer deleted." : "Customer ID not found.");
                }
                catch (Exception)
                {
                    Console.WriteLine("\nError: Cannot delete customer who has active or past rental history.");
                }
            }
            Console.ReadKey();
        }

        // Helper Method to keep code clean
        private void PrintCustomerTable(MySqlDataReader reader)
        {
            Console.WriteLine("--------------------------------------------------------------------------");
            Console.WriteLine(string.Format("{0,-5} | {1,-20} | {2,-15} | {3,-12}", "ID", "Name", "CNIC", "Phone"));
            Console.WriteLine("--------------------------------------------------------------------------");
            while (reader.Read())
            {
                Console.WriteLine(string.Format("{0,-5} | {1,-20} | {2,-15} | {3,-12}",
                    reader["CustomerID"], reader["Name"], reader["CNIC"], reader["Phone"]));
            }
        }
    }
}