using Microsoft.Data.Sqlite;
using System;

namespace VehicleRentalSystem.Services
{
    public class RentalService
    {
        // ---------------- BOOK VEHICLE ----------------
        public void Rent(int uid)
        {
            UI.Header("BOOK A VEHICLE");

            Console.Write(" Enter Vehicle ID: ");
            if (!int.TryParse(Console.ReadLine(), out int vid))
            {
                UI.Error("Invalid ID.");
                return;
            }

            Console.Write(" Rental Days: ");
            if (!int.TryParse(Console.ReadLine(), out int days) || days <= 0)
            {
                UI.Error("Invalid days.");
                return;
            }

            using var conn = Database.GetConnection();
            conn.Open();

            var check = conn.CreateCommand();
            check.CommandText = "SELECT IsAvailable, PricePerDay FROM Vehicles WHERE Id=@id";
            check.Parameters.AddWithValue("@id", vid);

            using var reader = check.ExecuteReader();

            if (!reader.Read() || reader.GetInt32(0) != 1)
            {
                UI.Error("Vehicle not available.");
                return;
            }

            double price = reader.GetDouble(1);
            double total = price * days;
            reader.Close();

            // Insert into Rentals
            var cmd = conn.CreateCommand();
            cmd.CommandText = @"INSERT INTO Rentals
                (UserId,VehicleId,RentDate,DueDate,BaseRent,Status)
                VALUES(@u,@v,@rd,@dd,@b,'Rented')";

            cmd.Parameters.AddWithValue("@u", uid);
            cmd.Parameters.AddWithValue("@v", vid);
            cmd.Parameters.AddWithValue("@rd", DateTime.Now.ToString("yyyy-MM-dd"));
            cmd.Parameters.AddWithValue("@dd", DateTime.Now.AddDays(days).ToString("yyyy-MM-dd"));
            cmd.Parameters.AddWithValue("@b", total);

            cmd.ExecuteNonQuery();

            // Update Vehicle Status
            var upV = conn.CreateCommand();
            upV.CommandText = "UPDATE Vehicles SET IsAvailable=0 WHERE Id=@v";
            upV.Parameters.AddWithValue("@v", vid);
            upV.ExecuteNonQuery();

            UI.Success($"Booked! Total Rent: Rs {total:F2}");
        }

        // ---------------- RETURN & SETTLEMENT ----------------
        public void ProcessReturn(int uid, bool isAdmin = false)
        {
            UI.Header(isAdmin ? "ADMIN SETTLEMENT" : "RETURN VEHICLE");

            using var conn = Database.GetConnection();
            conn.Open();

            var listCmd = conn.CreateCommand();
            listCmd.CommandText = isAdmin
                ? "SELECT r.Id, v.Name, r.Status FROM Rentals r JOIN Vehicles v ON r.VehicleId=v.Id WHERE r.Status='ReturnPending'"
                : "SELECT r.Id, v.Name, r.Status FROM Rentals r JOIN Vehicles v ON r.VehicleId=v.Id WHERE r.UserId=@uid AND r.Status='Rented'";
            listCmd.Parameters.AddWithValue("@uid", uid);

            using (var reader = listCmd.ExecuteReader())
            {
                if (!reader.HasRows)
                {
                    UI.Error("No active rentals found.");
                    return;
                }

                Console.WriteLine("{0,-5} | {1,-15} | {2,-15}", "ID", "Vehicle", "Status");
                UI.DrawTableLine();

                while (reader.Read())
                {
                    Console.WriteLine("{0,-5} | {1,-15} | {2,-15}",
                        reader.GetInt32(0),
                        reader.GetString(1),
                        reader.GetString(2));
                }
            }

            Console.Write("\n Enter Rental ID: ");
            if (!int.TryParse(Console.ReadLine(), out int rid))
            {
                UI.Error("Invalid ID.");
                return;
            }

            // Fetch rental info
            var cmd = conn.CreateCommand();
            cmd.CommandText = @"SELECT BaseRent, DueDate, VehicleId,
                (SELECT Value FROM SystemSettings WHERE Key='LateFeePerDay')
                FROM Rentals WHERE Id=@rid";
            cmd.Parameters.AddWithValue("@rid", rid);

            using var data = cmd.ExecuteReader();

            if (!data.Read())
            {
                UI.Error("Rental not found.");
                return;
            }

            double baseRent = data.GetDouble(0);
            DateTime dueDate = DateTime.Parse(data.GetString(1));
            int vid = data.GetInt32(2);
            double lateRate = data.GetDouble(3);
            data.Close();

            int lateDays = Math.Max(0, (DateTime.Now.Date - dueDate.Date).Days);
            double lateFine = lateDays * lateRate;
            double subTotal = baseRent + lateFine;

            // ---------------- CUSTOMER FLOW ----------------
            if (!isAdmin)
            {
                var up = conn.CreateCommand();
                up.CommandText = @"UPDATE Rentals 
                    SET Status='ReturnPending', LateFine=@lf, Total=@t 
                    WHERE Id=@rid";

                up.Parameters.AddWithValue("@lf", lateFine);
                up.Parameters.AddWithValue("@t", subTotal);
                up.Parameters.AddWithValue("@rid", rid);

                up.ExecuteNonQuery();

                UI.Success("Request sent! Please pay at counter.");
            }

            // ---------------- ADMIN FLOW ----------------
            else
            {
                Console.WriteLine($"\n[BILL] Base: {baseRent:F2} | Late: {lateFine:F2}");

                Console.Write(" Damage (0:None, 1:Minor, 2:Major): ");
                string d = Console.ReadLine();

                double damage = 0;

                if (d == "1" || d == "2")
                {
                    var dCmd = conn.CreateCommand();
                    dCmd.CommandText = d == "1"
                        ? "SELECT Value FROM SystemSettings WHERE Key='MinorDamage'"
                        : "SELECT Value FROM SystemSettings WHERE Key='MajorDamage'";

                    damage = Convert.ToDouble(dCmd.ExecuteScalar());
                }

                double finalTotal = subTotal + damage;

                var up = conn.CreateCommand();
                up.CommandText = @"UPDATE Rentals 
                    SET Status='Completed', DamageFine=@df, Total=@t, ReturnDate=@rd 
                    WHERE Id=@rid";

                up.Parameters.AddWithValue("@df", damage);
                up.Parameters.AddWithValue("@t", finalTotal);
                up.Parameters.AddWithValue("@rd", DateTime.Now.ToString("yyyy-MM-dd"));
                up.Parameters.AddWithValue("@rid", rid);

                up.ExecuteNonQuery();

                var upV = conn.CreateCommand();
                upV.CommandText = "UPDATE Vehicles SET IsAvailable=1 WHERE Id=@vid";
                upV.Parameters.AddWithValue("@vid", vid);
                upV.ExecuteNonQuery();

                UI.Success($"Settle! Total Paid: Rs {finalTotal:F2}");
            }
        }

        // ---------------- USER HISTORY ----------------
        public void ViewUserHistory(int uid)
        {
            UI.Header("MY RENTAL HISTORY");

            using var conn = Database.GetConnection();
            conn.Open();

            var cmd = conn.CreateCommand();
            cmd.CommandText = @"SELECT v.Name, r.RentDate, r.Total, r.Status 
                                FROM Rentals r 
                                JOIN Vehicles v ON r.VehicleId=v.Id 
                                WHERE r.UserId=@u";

            cmd.Parameters.AddWithValue("@u", uid);

            using var r = cmd.ExecuteReader();

            string fmt = "{0,-15} | {1,-12} | {2,-10} | {3,-12}";

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine(fmt, "Vehicle", "Date", "Total", "Status");
            UI.DrawTableLine();

            while (r.Read())
            {
                double total = r.IsDBNull(2) ? 0 : r.GetDouble(2);

                Console.WriteLine(fmt,
                    r.GetString(0),
                    r.GetString(1),
                    "Rs " + total.ToString("F2"),
                    r.GetString(3));
            }

            Console.ResetColor();
            Console.ReadLine();
        }
    }
}