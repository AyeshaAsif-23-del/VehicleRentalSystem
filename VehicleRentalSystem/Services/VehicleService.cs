using Microsoft.Data.Sqlite;
using System;

namespace VehicleRentalSystem.Services
{
    public class VehicleService
    {
        // ---------------- ADD VEHICLE ----------------
        public void Add()
        {
            UI.Header("ADD NEW VEHICLE");
            try
            {
                Console.Write(" Name: ");
                string n = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(n)) { UI.Error("Name required."); return; }

                Console.Write(" Brand: ");
                string b = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(b)) { UI.Error("Brand required."); return; }

                Console.Write(" Type (Car/Bike/SUV): ");
                string t = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(t)) { UI.Error("Type required."); return; }

                Console.Write(" Plate Number: ");
                string p = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(p)) { UI.Error("Plate required."); return; }

                Console.Write(" Model Year: ");
                string y = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(y)) { UI.Error("Model year required."); return; }

                Console.Write(" Price Per Day (Rs): ");
                if (!double.TryParse(Console.ReadLine(), out double pr) || pr <= 0)
                {
                    UI.Error("Invalid price.");
                    return;
                }

                using var conn = Database.GetConnection();
                conn.Open();

                var cmd = conn.CreateCommand();
                cmd.CommandText = @"INSERT INTO Vehicles 
                    (Name,Brand,Type,PlateNumber,ModelYear,PricePerDay,IsAvailable)
                    VALUES(@n,@b,@t,@p,@y,@pr,1)";

                cmd.Parameters.AddWithValue("@n", n);
                cmd.Parameters.AddWithValue("@b", b);
                cmd.Parameters.AddWithValue("@t", t);
                cmd.Parameters.AddWithValue("@p", p);
                cmd.Parameters.AddWithValue("@y", y);
                cmd.Parameters.AddWithValue("@pr", pr);

                cmd.ExecuteNonQuery();

                UI.Success($"{n} added successfully!");
            }
            catch (Exception ex)
            {
                UI.Error(ex.Message);
            }
        }

        // ---------------- VIEW VEHICLES ----------------
        public void View(bool availableOnly = false)
        {
            UI.Header(availableOnly ? "AVAILABLE VEHICLES" : "ALL VEHICLES");

            using var conn = Database.GetConnection();
            conn.Open();

            var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT Id, Name, Brand, Type, PlateNumber, PricePerDay, IsAvailable FROM Vehicles" +
                              (availableOnly ? " WHERE IsAvailable=1" : "");

            using var r = cmd.ExecuteReader();

            UI.DrawTable(new string[] { "ID", "Name", "Brand", "Type", "Plate", "Price", "Status" });

            if (!r.HasRows)
            {
                Console.WriteLine(" No vehicles found.");
                Console.ReadLine();
                return;
            }

            while (r.Read())
            {
                bool isAvail = r.GetInt32(6) == 1;
                string status = isAvail ? "Available" : "Booked";

                UI.DrawRow(new object[] {
                    r.GetInt32(0),
                    r.GetString(1),
                    r.GetString(2),
                    r.GetString(3),
                    r.GetString(4),
                    r.GetDouble(5),
                    status
                }, isAvail ? ConsoleColor.Green : ConsoleColor.Red);
            }

            Console.WriteLine("\nPress Enter...");
            Console.ReadLine();
        }

        // ---------------- UPDATE PRICE ----------------
        public void Update()
        {
            UI.Header("UPDATE PRICE");

            Console.Write(" Enter Vehicle ID: ");
            if (!int.TryParse(Console.ReadLine(), out int id))
            {
                UI.Error("Invalid ID.");
                return;
            }

            Console.Write(" New Price: ");
            if (!double.TryParse(Console.ReadLine(), out double p) || p <= 0)
            {
                UI.Error("Invalid price.");
                return;
            }

            using var conn = Database.GetConnection();
            conn.Open();

            var cmd = conn.CreateCommand();
            cmd.CommandText = "UPDATE Vehicles SET PricePerDay=@p WHERE Id=@id";
            cmd.Parameters.AddWithValue("@p", p);
            cmd.Parameters.AddWithValue("@id", id);

            if (cmd.ExecuteNonQuery() > 0)
                UI.Success("Price updated.");
            else
                UI.Error("Vehicle not found.");
        }

        // ---------------- DELETE VEHICLE ----------------
        public void Delete()
        {
            UI.Header("REMOVE VEHICLE");

            Console.Write(" Enter Vehicle ID: ");
            if (!int.TryParse(Console.ReadLine(), out int id))
            {
                UI.Error("Invalid ID.");
                return;
            }

            using var conn = Database.GetConnection();
            conn.Open();

            // Check if vehicle in use
            var check = conn.CreateCommand();
            check.CommandText = @"SELECT COUNT(*) FROM Rentals 
                                  WHERE VehicleId=@id 
                                  AND Status IN ('Rented','ReturnPending')";
            check.Parameters.AddWithValue("@id", id);

            if ((long)check.ExecuteScalar() > 0)
            {
                UI.Error("Cannot delete. Vehicle is currently in use.");
                return;
            }

            Console.Write("Are you sure? (y/n): ");
            string confirm = Console.ReadLine()?.ToLower();

            if (confirm != "y") return;

            var cmd = conn.CreateCommand();
            cmd.CommandText = "DELETE FROM Vehicles WHERE Id=@id";
            cmd.Parameters.AddWithValue("@id", id);

            if (cmd.ExecuteNonQuery() > 0)
                UI.Success("Vehicle removed.");
            else
                UI.Error("Vehicle not found.");
        }
    }
}