using System;
using MySql.Data.MySqlClient;

namespace VehicleRentalSystem
{
    public class VehicleManager
    {
        // 1. ADD VEHICLE
        public void AddVehicle()
        {
            Console.Clear();
            Console.WriteLine("--- Add New Vehicle ---");
            Console.Write("Enter Name: "); string name = Console.ReadLine();
            Console.Write("Enter Type (Car/Bike/Van): "); string type = Console.ReadLine();
            Console.Write("Enter Model: "); string model = Console.ReadLine();
            Console.Write("Enter Plate Number: "); string plate = Console.ReadLine();

            Console.Write("Rent Per Day: ");
            if (!decimal.TryParse(Console.ReadLine(), out decimal rent))
            {
                Console.WriteLine("Invalid amount. Transaction cancelled.");
                return;
            }

            using (var conn = DbConfig.GetConnection())
            {
                conn.Open();
                string query = "INSERT INTO Vehicles (Name, Type, Model, PlateNumber, RentPerDay) VALUES (@n, @t, @m, @p, @r)";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@n", name);
                cmd.Parameters.AddWithValue("@t", type);
                cmd.Parameters.AddWithValue("@m", model);
                cmd.Parameters.AddWithValue("@p", plate);
                cmd.Parameters.AddWithValue("@r", rent);
                cmd.ExecuteNonQuery();
                Console.WriteLine("\nVehicle added successfully!");
            }
            Console.ReadKey();
        }

        // 2. VIEW ALL VEHICLES
        public void ViewVehicles()
        {
            Console.Clear();
            Console.WriteLine("--- Current Vehicle Fleet ---");
            using (var conn = DbConfig.GetConnection())
            {
                conn.Open();
                string query = "SELECT * FROM Vehicles";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                using (var reader = cmd.ExecuteReader())
                {
                    Console.WriteLine("--------------------------------------------------------------------------");
                    Console.WriteLine(string.Format("{0,-5} | {1,-15} | {2,-10} | {3,-10} | {4,-10}", "ID", "Name", "Plate", "Rent", "Status"));
                    Console.WriteLine("--------------------------------------------------------------------------");
                    while (reader.Read())
                    {
                        Console.WriteLine(string.Format("{0,-5} | {1,-15} | {2,-10} | {3,-10} | {4,-10}",
                            reader["VehicleID"], reader["Name"], reader["PlateNumber"], reader["RentPerDay"], reader["Status"]));
                    }
                }
            }
            Console.WriteLine("\nPress any key to return...");
            Console.ReadKey();
        }

        // 3. UPDATE VEHICLE (RENT OR STATUS)
        public void UpdateVehicle()
        {
            Console.Clear();
            Console.WriteLine("--- Update Vehicle Details ---");
            Console.Write("Enter Vehicle ID to update: ");
            if (!int.TryParse(Console.ReadLine(), out int vId)) return;

            Console.WriteLine("What do you want to update?");
            Console.WriteLine("1. Rent Per Day");
            Console.WriteLine("2. Availability Status");
            string choice = Console.ReadLine();

            using (var conn = DbConfig.GetConnection())
            {
                conn.Open();
                string query = "";
                MySqlCommand cmd = new MySqlCommand();
                cmd.Connection = conn;

                if (choice == "1")
                {
                    Console.Write("Enter New Rent: ");
                    decimal newRent = decimal.Parse(Console.ReadLine());
                    query = "UPDATE Vehicles SET RentPerDay = @val WHERE VehicleID = @id";
                    cmd.Parameters.AddWithValue("@val", newRent);
                }
                else
                {
                    Console.Write("Enter New Status (Available/Rented): ");
                    string newStatus = Console.ReadLine();
                    query = "UPDATE Vehicles SET Status = @val WHERE VehicleID = @id";
                    cmd.Parameters.AddWithValue("@val", newStatus);
                }

                cmd.CommandText = query;
                cmd.Parameters.AddWithValue("@id", vId);
                int rows = cmd.ExecuteNonQuery();
                Console.WriteLine(rows > 0 ? "Update Successful!" : "Vehicle ID not found.");
            }
            Console.ReadKey();
        }

        // 4. SEARCH VEHICLE
        public void SearchVehicle()
        {
            Console.Clear();
            Console.Write("Enter Vehicle Name or Plate to search: ");
            string search = Console.ReadLine();

            using (var conn = DbConfig.GetConnection())
            {
                conn.Open();
                string query = "SELECT * FROM Vehicles WHERE Name LIKE @s OR PlateNumber LIKE @s";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@s", "%" + search + "%");

                using (var reader = cmd.ExecuteReader())
                {
                    Console.WriteLine("\nSearch Results:");
                    while (reader.Read())
                    {
                        Console.WriteLine($"{reader["VehicleID"]} | {reader["Name"]} | {reader["PlateNumber"]} | {reader["Status"]}");
                    }
                }
            }
            Console.ReadKey();
        }

        // 5. DELETE VEHICLE
        public void DeleteVehicle()
        {
            Console.Clear();
            Console.Write("Enter Vehicle ID to DELETE: ");
            if (!int.TryParse(Console.ReadLine(), out int vId)) return;

            Console.Write("Are you sure? This cannot be undone. (Y/N): ");
            if (Console.ReadLine().ToUpper() != "Y") return;

            using (var conn = DbConfig.GetConnection())
            {
                try
                {
                    conn.Open();
                    string query = "DELETE FROM Vehicles WHERE VehicleID = @id";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@id", vId);
                    int rows = cmd.ExecuteNonQuery();
                    Console.WriteLine(rows > 0 ? "Vehicle removed from fleet." : "Vehicle ID not found.");
                }
                catch (Exception)
                {
                    Console.WriteLine("\nError: Cannot delete vehicle because it is linked to rental history.");
                }
            }
            Console.ReadKey();
        }
    }
}