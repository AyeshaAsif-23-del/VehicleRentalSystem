using System;
using MySql.Data.MySqlClient;

namespace VehicleRentalSystem
{
    public class CustomerManager
    {
        private string GetInput(string message)
        {
            Console.Write(message);
            return Console.ReadLine() ?? "";
        }
        private bool GetInt(string message, out int value)
        {
            Console.Write(message);
            return int.TryParse(Console.ReadLine(), out value);
        }

        // 1. REGISTER CUSTOMER
        public void AddCustomer()
        {
            Console.Clear();
            Console.WriteLine("--- Register New Customer ---");

            string name = GetInput("Enter Name: ");
            string cnic = GetInput("Enter CNIC: ");
            string phone = GetInput("Enter Phone: ");
            string license = GetInput("Enter License Number: ");

            using (var conn = DbConfig.GetConnection())
            {
                try
                {
                    conn.Open();

                    string query = @"INSERT INTO Customers 
                                     (Name, CNIC, Phone, LicenseNumber) 
                                     VALUES (@n, @c, @p, @l)";

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

        // 3. SEARCH CUSTOMER
        public void SearchCustomer()
        {
            Console.Clear();
            Console.WriteLine("--- Search Customer ---");

            string search = GetInput("Enter Name or CNIC: ");

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

        // 4. UPDATE CUSTOMER
        public void UpdateCustomer()
        {
            Console.Clear();
            Console.WriteLine("--- Update Customer Details ---");

            if (!GetInt("Enter Customer ID: ", out int cId))
            {
                Console.WriteLine("Invalid ID!");
                Console.ReadKey();
                return;
            }

            string newPhone = GetInput("Enter New Phone Number: ");

            using (var conn = DbConfig.GetConnection())
            {
                conn.Open();

                string query = "UPDATE Customers SET Phone=@p WHERE CustomerID=@id";
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

            if (!GetInt("Enter Customer ID: ", out int cId))
            {
                Console.WriteLine("Invalid ID!");
                Console.ReadKey();
                return;
            }

            string confirm = GetInput("Are you sure? (Y/N): ").ToUpper();

            if (confirm != "Y") return;

            using (var conn = DbConfig.GetConnection())
            {
                try
                {
                    conn.Open();

                    string query = "DELETE FROM Customers WHERE CustomerID=@id";
                    MySqlCommand cmd = new MySqlCommand(query, conn);

                    cmd.Parameters.AddWithValue("@id", cId);

                    int rows = cmd.ExecuteNonQuery();

                    Console.WriteLine(rows > 0 ? "Customer deleted." : "Customer not found.");
                }
                catch
                {
                    Console.WriteLine("\nCannot delete: linked to rental history.");
                }
            }

            Console.ReadKey();
        }


        private void PrintCustomerTable(MySqlDataReader reader)
        {
            Console.WriteLine("--------------------------------------------------------------------------");
            Console.WriteLine(string.Format("{0,-5} | {1,-20} | {2,-15} | {3,-12}",
                "ID", "Name", "CNIC", "Phone"));
            Console.WriteLine("--------------------------------------------------------------------------");

            while (reader.Read())
            {
                Console.WriteLine(string.Format("{0,-5} | {1,-20} | {2,-15} | {3,-12}",
                    reader["CustomerID"],
                    reader["Name"],
                    reader["CNIC"],
                    reader["Phone"]));
            }
        }
    }
}