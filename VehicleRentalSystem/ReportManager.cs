using System;
using MySql.Data.MySqlClient;

namespace VehicleRentalSystem
{
    public class ReportManager
    {
       
        private string GetInput(string message)
        {
            Console.Write(message);
            return Console.ReadLine() ?? "";
        }

        public void DisplayReportsMenu()
        {
            Console.Clear();
            Console.WriteLine("--- Business Intelligence & Reports ---");
            Console.WriteLine("1. Total Earnings Summary");
            Console.WriteLine("2. Fleet Status Report (Available vs Rented)");
            Console.WriteLine("3. Recent Rental History");

            string choice = GetInput("\nSelect Report Type: ");

            switch (choice)
            {
                case "1":
                    TotalEarnings();
                    break;
                case "2":
                    FleetStatus();
                    break;
                case "3":
                    RentalHistory();
                    break;
                default:
                    Console.WriteLine("Invalid selection.");
                    Console.ReadKey();
                    break;
            }
        }

        // 1. TOTAL EARNINGS
        public void TotalEarnings()
        {
            Console.Clear();

            using (var conn = DbConfig.GetConnection())
            {
                conn.Open();

                string query = "SELECT SUM(TotalAmount) FROM Rentals WHERE Status='Completed'";
                MySqlCommand cmd = new MySqlCommand(query, conn);

                object result = cmd.ExecuteScalar();

                // ✔ Safe conversion (handles NULL from DB)
                decimal total = (result != DBNull.Value && result != null)
                                ? Convert.ToDecimal(result)
                                : 0;

                Console.WriteLine("\n==================================");
                Console.WriteLine("        FINANCIAL SUMMARY        ");
                Console.WriteLine("==================================");
                Console.WriteLine($"TOTAL REVENUE: {total:0.00} PKR");
                Console.WriteLine("==================================\n");
            }

            Console.WriteLine("Press any key to return...");
            Console.ReadKey();
        }

        // 2. FLEET STATUS
        private void FleetStatus()
        {
            Console.Clear();

            using (var conn = DbConfig.GetConnection())
            {
                conn.Open();

                string query = "SELECT Status, COUNT(*) AS Count FROM Vehicles GROUP BY Status";
                MySqlCommand cmd = new MySqlCommand(query, conn);

                using (var reader = cmd.ExecuteReader())
                {
                    Console.WriteLine("\n--- Fleet Availability ---\n");

                    while (reader.Read())
                    {
                        Console.WriteLine($"{reader["Status"]}: {reader["Count"]} vehicle(s)");
                    }
                }
            }

            Console.WriteLine("\nPress any key to return...");
            Console.ReadKey();
        }

        // 3. RENTAL HISTORY
        private void RentalHistory()
        {
            Console.Clear();

            using (var conn = DbConfig.GetConnection())
            {
                conn.Open();

                string query = @"SELECT r.RentalID, c.Name AS Customer, v.Name AS Vehicle, r.TotalAmount, r.Status 
                                 FROM Rentals r
                                 JOIN Customers c ON r.CustomerID = c.CustomerID
                                 JOIN Vehicles v ON r.VehicleID = v.VehicleID
                                 ORDER BY r.RentalID DESC LIMIT 10";

                MySqlCommand cmd = new MySqlCommand(query, conn);

                using (var reader = cmd.ExecuteReader())
                {
                    Console.WriteLine("\n--- Last 10 Transactions ---\n");

                    Console.WriteLine("----------------------------------------------------------");
                    Console.WriteLine(string.Format("{0,-5} | {1,-15} | {2,-15} | {3,-10}",
                        "ID", "Customer", "Vehicle", "Amount"));
                    Console.WriteLine("----------------------------------------------------------");

                    while (reader.Read())
                    {
                        Console.WriteLine(string.Format("{0,-5} | {1,-15} | {2,-15} | {3,-10}",
                            reader["RentalID"],
                            reader["Customer"],
                            reader["Vehicle"],
                            reader["TotalAmount"]));
                    }
                }
            }

            Console.WriteLine("\nPress any key to return...");
            Console.ReadKey();
        }
    }
}