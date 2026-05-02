using System;
using VehicleRentalSystem.Services;

namespace VehicleRentalSystem
{
    class Program
    {
        static AuthService auth = new AuthService();
        static VehicleService v = new VehicleService();
        static RentalService r = new RentalService();

        static void Main()
        {
            Database.Init();

            while (true)
            {
                UI.Header("VEHICLE RENTAL SYSTEM");
                Console.WriteLine(" 1. Login\n 2. Register\n 3. Exit");
                Console.Write("\n Choose Option: ");
                string c = Console.ReadLine();

                if (c == "1")
                {
                    var user = auth.Login();
                    if (user != null)
                    {
                        if (user.Value.role == "Admin")
                            Admin(user.Value.id);
                        else
                            Customer(user.Value.id);
                    }
                }
                else if (c == "2") auth.Register();
                else if (c == "3") break;
            }
        }

        // ---------------- ADMIN ----------------
        static void Admin(int id)
        {
            while (true)
            {
                UI.Header("ADMIN CONTROL PANEL");

                Console.WriteLine(
                    " 1. Add New Vehicle\n" +
                    " 2. View Inventory\n" +
                    " 3. Update Pricing\n" +
                    " 4. Remove Vehicle\n" +
                    " 5. Settle Returns\n" +
                    " 6. Set System Fines\n" +
                    " 7. View Total Revenue\n" +
                    " 8. Monthly Revenue\n" +
                    " 9. Top Earning Vehicle\n" +
                    " 10. Logout"
                );

                Console.Write("\n Admin Action: ");
                string c = Console.ReadLine();

                if (c == "1") v.Add();
                else if (c == "2") v.View();
                else if (c == "3") v.Update();
                else if (c == "4") v.Delete();
                else if (c == "5") r.ProcessReturn(id, true);
                else if (c == "6") SetSystemCharges();

                // 🔥 NEW REPORTING FEATURES
                else if (c == "7") ViewTotalRevenue();
                else if (c == "8") ViewMonthlyRevenue();
                else if (c == "9") TopEarningVehicle();

                else if (c == "10") break;
            }
        }

        // ---------------- CUSTOMER ----------------
        static void Customer(int id)
        {
            while (true)
            {
                UI.Header("CUSTOMER DASHBOARD");

                Console.WriteLine(
                    " 1. View Available Vehicles\n" +
                    " 2. Rent a Vehicle\n" +
                    " 3. Return a Vehicle\n" +
                    " 4. View My History\n" +
                    " 5. Logout"
                );

                Console.Write("\n Customer Action: ");
                string c = Console.ReadLine();

                if (c == "1") v.View(true);
                else if (c == "2") r.Rent(id);
                else if (c == "3") r.ProcessReturn(id, false);
                else if (c == "4") r.ViewUserHistory(id);
                else if (c == "5") break;
            }
        }

        // ---------------- SYSTEM CHARGES ----------------
        static void SetSystemCharges()
        {
            UI.Header("SYSTEM CONFIGURATION");

            using var conn = Database.GetConnection();
            conn.Open();

            Console.Write(" Late Fee Per Day: ");
            if (!double.TryParse(Console.ReadLine(), out double late)) return;

            Console.Write(" Minor Damage Fee: ");
            if (!double.TryParse(Console.ReadLine(), out double minor)) return;

            Console.Write(" Major Damage Fee: ");
            if (!double.TryParse(Console.ReadLine(), out double major)) return;

            var cmd = conn.CreateCommand();

            cmd.CommandText = "UPDATE SystemSettings SET Value=@v WHERE Key='LateFeePerDay'";
            cmd.Parameters.AddWithValue("@v", late);
            cmd.ExecuteNonQuery();

            cmd.CommandText = "UPDATE SystemSettings SET Value=@v WHERE Key='MinorDamage'";
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@v", minor);
            cmd.ExecuteNonQuery();

            cmd.CommandText = "UPDATE SystemSettings SET Value=@v WHERE Key='MajorDamage'";
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@v", major);
            cmd.ExecuteNonQuery();

            UI.Success("System charges updated!");
        }

        // ---------------- REVENUE FUNCTIONS ----------------

        static void ViewTotalRevenue()
        {
            UI.Header("TOTAL REVENUE");

            using var conn = Database.GetConnection();
            conn.Open();

            var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT SUM(Total) FROM Rentals WHERE Status='Completed'";

            object result = cmd.ExecuteScalar();
            double total = result == DBNull.Value || result == null ? 0 : Convert.ToDouble(result);

            Console.WriteLine($"\n Total Revenue: Rs {total}");
            Console.ReadLine();
        }

        static void ViewMonthlyRevenue()
        {
            UI.Header("MONTHLY REVENUE");

            using var conn = Database.GetConnection();
            conn.Open();

            var cmd = conn.CreateCommand();
            cmd.CommandText = @"
                SELECT SUM(Total)
                FROM Rentals
                WHERE Status='Completed'
                AND strftime('%Y-%m', ReturnDate) = strftime('%Y-%m','now')";

            object result = cmd.ExecuteScalar();
            double total = result == DBNull.Value || result == null ? 0 : Convert.ToDouble(result);

            Console.WriteLine($"\n This Month Revenue: Rs {total}");
            Console.ReadLine();
        }

        static void TopEarningVehicle()
        {
            UI.Header("TOP EARNING VEHICLE");

            using var conn = Database.GetConnection();
            conn.Open();

            var cmd = conn.CreateCommand();
            cmd.CommandText = @"
                SELECT v.Name, SUM(r.Total) as Revenue
                FROM Rentals r
                JOIN Vehicles v ON r.VehicleId = v.Id
                WHERE r.Status='Completed'
                GROUP BY v.Name
                ORDER BY Revenue DESC
                LIMIT 1";

            using var r = cmd.ExecuteReader();

            if (r.Read())
            {
                Console.WriteLine($"\n Vehicle: {r.GetString(0)}");
                Console.WriteLine($" Earnings: Rs {r.GetDouble(1)}");
            }
            else
            {
                Console.WriteLine("No data found.");
            }

            Console.ReadLine();
        }
    }
}