using System;
using MySql.Data.MySqlClient;

namespace VehicleRentalSystem
{
    public class ReportManager
    {
        public void DisplayReportsMenu()
        {
            Console.Clear();
            Console.WriteLine("--- Business Intelligence & Reports ---");
            Console.WriteLine("1. Total Earnings Summary");
            Console.WriteLine("2. Fleet Status Report (Available vs Rented)");
            Console.WriteLine("3. Recent Rental History");
            Console.Write("\nSelect Report Type: ");
            string choice = Console.ReadLine();

            switch (choice)
            {
                case "1": TotalEarnings(); break;
                case "2": FleetStatus(); break;
                case "3": RentalHistory(); break;
                default: Console.WriteLine("Invalid selection."); break;
            }
        }

        public void TotalEarnings()
        {
            using (var conn = DbConfig.GetConnection())
            {
                conn.Open();
                // We sum all payments from completed rentals
                string query = "SELECT SUM(TotalAmount) FROM Rentals WHERE Status='Completed'";
                MySqlCommand cmd = new MySqlCommand(query, conn);

                object result = cmd.ExecuteScalar();
                decimal total = (result != DBNull.Value) ? Convert.ToDecimal(result) : 0;

                Console.WriteLine("\n==================================");
                Console.WriteLine($"   FINANCIAL SUMMARY             ");
                Console.WriteLine("==================================");
                Console.WriteLine($" TOTAL REVENUE: {total:C}"); // :C formats as Currency
                Console.WriteLine("==================================\n");
            }
            Console.WriteLine("Press any key to return...");
            Console.ReadKey();
        }

        private void FleetStatus()
        {
            using (var conn = DbConfig.GetConnection())
            {
                conn.Open();
                string query = "SELECT Status, COUNT(*) as Count FROM Vehicles GROUP BY Status";
                MySqlCommand cmd = new MySqlCommand(query, conn);

                using (var reader = cmd.ExecuteReader())
                {
                    Console.WriteLine("\n--- Fleet Availability ---");
                    while (reader.Read())
                    {
                        Console.WriteLine($"{reader["Status"]}: {reader["Count"]} vehicles");
                    }
                }
            }
            Console.ReadKey();
        }

        private void RentalHistory()
        {
            using (var conn = DbConfig.GetConnection())
            {
                conn.Open();
                // Join query to show Names instead of just IDs
                string query = @"SELECT r.RentalID, c.Name as Customer, v.Name as Vehicle, r.TotalAmount, r.Status 
                                 FROM Rentals r
                                 JOIN Customers c ON r.CustomerID = c.CustomerID
                                 JOIN Vehicles v ON r.VehicleID = v.VehicleID
                                 ORDER BY r.RentalID DESC LIMIT 10";

                MySqlCommand cmd = new MySqlCommand(query, conn);
                using (var reader = cmd.ExecuteReader())
                {
                    Console.WriteLine("\n--- Last 10 Transactions ---");
                    Console.WriteLine(string.Format("{0,-5} | {1,-15} | {2,-15} | {3,-10}", "ID", "Customer", "Vehicle", "Amount"));
                    while (reader.Read())
                    {
                        Console.WriteLine(string.Format("{0,-5} | {1,-15} | {2,-15} | {3,-10}",
                            reader["RentalID"], reader["Customer"], reader["Vehicle"], reader["TotalAmount"]));
                    }
                }
            }
            Console.ReadKey();
        }
    }
}