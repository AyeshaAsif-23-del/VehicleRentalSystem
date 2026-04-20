using System;
using MySql.Data.MySqlClient;

namespace VehicleRentalSystem
{
    public class VehicleManager
    {
        
        private string GetInput(string message)
        {
            Console.Write(message);
            return Console.ReadLine() ?? "";
        }

      
        private bool GetDecimal(string message, out decimal value)
        {
            Console.Write(message);
            return decimal.TryParse(Console.ReadLine(), out value);
        }

     
        private bool GetInt(string message, out int value)
        {
            Console.Write(message);
            return int.TryParse(Console.ReadLine(), out value);
        }

        // 1. ADD VEHICLE
        public void AddVehicle()
        {
            Console.Clear();
            Console.WriteLine("--- Add New Vehicle ---");

            string name = GetInput("Enter Name: ");
            string type = GetInput("Enter Type (Car/Bike/Van): ");
            string model = GetInput("Enter Model: ");
            string plate = GetInput("Enter Plate Number: ");

            if (!GetDecimal("Rent Per Day: ", out decimal rent))
            {
                Console.WriteLine("Invalid amount. Transaction cancelled.");
                Console.ReadKey();
                return;
            }

            using (var conn = DbConfig.GetConnection())
            {
                conn.Open();

                string query = @"INSERT INTO Vehicles 
                                (Name, Type, Model, PlateNumber, RentPerDay, Status) 
                                VALUES (@n, @t, @m, @p, @r, 'Available')";

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
                    Console.WriteLine(string.Format("{0,-5} | {1,-15} | {2,-12} | {3,-10} | {4,-10}",
                        "ID", "Name", "Plate", "Rent", "Status"));
                    Console.WriteLine("--------------------------------------------------------------------------");

                    while (reader.Read())
                    {
                        Console.WriteLine(string.Format("{0,-5} | {1,-15} | {2,-12} | {3,-10} | {4,-10}",
                            reader["VehicleID"],
                            reader["Name"],
                            reader["PlateNumber"],
                            reader["RentPerDay"],
                            reader["Status"]));
                    }
                }
            }

            Console.WriteLine("\nPress any key to return...");
            Console.ReadKey();
        }

        // 3. UPDATE VEHICLE
        public void UpdateVehicle()
        {
            Console.Clear();
            Console.WriteLine("--- Update Vehicle Details ---");

            if (!GetInt("Enter Vehicle ID: ", out int vId))
            {
                Console.WriteLine("Invalid ID!");
                Console.ReadKey();
                return;
            }

            Console.WriteLine("1. Update Rent");
            Console.WriteLine("2. Update Status");
            string choice = GetInput("Select option: ");

            using (var conn = DbConfig.GetConnection())
            {
                conn.Open();

                string query;
                MySqlCommand cmd = new MySqlCommand();
                cmd.Connection = conn;

                if (choice == "1")
                {
                    if (!GetDecimal("Enter New Rent: ", out decimal newRent))
                    {
                        Console.WriteLine("Invalid amount!");
                        Console.ReadKey();
                        return;
                    }

                    query = "UPDATE Vehicles SET RentPerDay=@val WHERE VehicleID=@id";
                    cmd.Parameters.AddWithValue("@val", newRent);
                }
                else if (choice == "2")
                {
                    string newStatus = GetInput("Enter Status (Available/Rented): ");

                    query = "UPDATE Vehicles SET Status=@val WHERE VehicleID=@id";
                    cmd.Parameters.AddWithValue("@val", newStatus);
                }
                else
                {
                    Console.WriteLine("Invalid option!");
                    Console.ReadKey();
                    return;
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

            string search = GetInput("Enter Vehicle Name or Plate: ");

            using (var conn = DbConfig.GetConnection())
            {
                conn.Open();

                string query = "SELECT * FROM Vehicles WHERE Name LIKE @s OR PlateNumber LIKE @s";
                MySqlCommand cmd = new MySqlCommand(query, conn);

                cmd.Parameters.AddWithValue("@s", "%" + search + "%");

                using (var reader = cmd.ExecuteReader())
                {
                    Console.WriteLine("\nSearch Results:\n");

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

            if (!GetInt("Enter Vehicle ID to DELETE: ", out int vId))
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

                    string query = "DELETE FROM Vehicles WHERE VehicleID=@id";
                    MySqlCommand cmd = new MySqlCommand(query, conn);

                    cmd.Parameters.AddWithValue("@id", vId);

                    int rows = cmd.ExecuteNonQuery();

                    Console.WriteLine(rows > 0 ? "Vehicle deleted." : "Vehicle not found.");
                }
                catch
                {
                    Console.WriteLine("\nCannot delete: linked to rental history.");
                }
            }

            Console.ReadKey();
        }
    }
}