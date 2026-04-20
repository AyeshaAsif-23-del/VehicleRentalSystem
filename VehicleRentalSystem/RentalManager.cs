using MySql.Data.MySqlClient;
using System;

namespace VehicleRentalSystem
{
    public class RentalManager
    {
        private string GetInput(string msg)
        {
            Console.Write(msg);
            return Console.ReadLine() ?? "";
        }

        private bool GetInt(string msg, out int value)
        {
            Console.Write(msg);
            return int.TryParse(Console.ReadLine(), out value);
        }

        // 1. RENT VEHICL
        public void RentVehicle()
        {
            Console.Clear();
            Console.WriteLine("--- Process New Rental ---");

            if (!GetInt("Enter Customer ID: ", out int cId))
                return;

            if (!GetInt("Enter Vehicle ID: ", out int vId))
                return;

            if (!GetInt("Enter Rental Days: ", out int days))
                return;

            using (var conn = DbConfig.GetConnection())
            {
                try
                {
                    conn.Open();

                    // Check Customer exists
                    MySqlCommand custCheck = new MySqlCommand(
                        "SELECT COUNT(*) FROM Customers WHERE CustomerID=@cid", conn);
                    custCheck.Parameters.AddWithValue("@cid", cId);

                    int custExists = Convert.ToInt32(custCheck.ExecuteScalar());

                    if (custExists == 0)
                    {
                        Console.WriteLine("\n[!] Customer does not exist.");
                        Console.ReadKey();
                        return;
                    }

                    // Check Vehicle availability
                    MySqlCommand checkCmd = new MySqlCommand(
                        "SELECT Status, RentPerDay FROM Vehicles WHERE VehicleID=@vid", conn);
                    checkCmd.Parameters.AddWithValue("@vid", vId);

                    using (var reader = checkCmd.ExecuteReader())
                    {
                        if (!reader.Read())
                        {
                            Console.WriteLine("\n[!] Vehicle not found.");
                            return;
                        }

                        string status = reader["Status"]?.ToString() ?? "";

                        if (status != "Available")
                        {
                            Console.WriteLine("\n[!] Vehicle already rented.");
                            return;
                        }

                        decimal rentPerDay =
                            reader["RentPerDay"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["RentPerDay"]);

                        decimal total = rentPerDay * days;

                        reader.Close();

                        // Insert rental
                        MySqlCommand insert = new MySqlCommand(
                            @"INSERT INTO Rentals 
                            (CustomerID, VehicleID, RentDate, TotalDays, TotalAmount, Status)
                            VALUES (@c,@v,CURDATE(),@d,@t,'Active')", conn);

                        insert.Parameters.AddWithValue("@c", cId);
                        insert.Parameters.AddWithValue("@v", vId);
                        insert.Parameters.AddWithValue("@d", days);
                        insert.Parameters.AddWithValue("@t", total);

                        insert.ExecuteNonQuery();

                        // Update vehicle
                        MySqlCommand update = new MySqlCommand(
                            "UPDATE Vehicles SET Status='Rented' WHERE VehicleID=@v", conn);
                        update.Parameters.AddWithValue("@v", vId);
                        update.ExecuteNonQuery();

                        Console.WriteLine("\nRental Successful!");
                        Console.WriteLine($"Total: {total:0.00} PKR");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("DB Error: " + ex.Message);
                }
            }

            Console.ReadKey();
        }

        // 2. RETURN VEHICLE 
        public void ReturnVehicle()
        {
            Console.Clear();
            Console.WriteLine("--- Return Vehicle ---");

            if (!GetInt("Enter Rental ID: ", out int rId))
                return;

            using (var conn = DbConfig.GetConnection())
            {
                conn.Open();

                MySqlCommand cmd = new MySqlCommand(
                    "SELECT VehicleID FROM Rentals WHERE RentalID=@id AND Status='Active'", conn);
                cmd.Parameters.AddWithValue("@id", rId);

                object result = cmd.ExecuteScalar();

                if (result == null || result == DBNull.Value)
                {
                    Console.WriteLine("Rental not found.");
                    Console.ReadKey();
                    return;
                }

                int vId = Convert.ToInt32(result);

                
                Console.Write("Any damages or late fee? (0 if none): ");
                decimal fine = decimal.TryParse(Console.ReadLine(), out decimal f) ? f : 0;

                MySqlCommand updateRental = new MySqlCommand(
                    "UPDATE Rentals SET Status='Completed', ReturnDate=CURDATE(), Fine=@f WHERE RentalID=@id", conn);
                updateRental.Parameters.AddWithValue("@id", rId);
                updateRental.Parameters.AddWithValue("@f", fine);
                updateRental.ExecuteNonQuery();

                MySqlCommand updateVehicle = new MySqlCommand(
                    "UPDATE Vehicles SET Status='Available' WHERE VehicleID=@v", conn);
                updateVehicle.Parameters.AddWithValue("@v", vId);
                updateVehicle.ExecuteNonQuery();

                Console.WriteLine("\nVehicle returned successfully!");
            }

            Console.ReadKey();
        }

        public void ViewRentals()
        {
            Console.Clear();
            Console.WriteLine("--- Rentals ---");

            using (var conn = DbConfig.GetConnection())
            {
                conn.Open();

                MySqlCommand cmd = new MySqlCommand(
                    "SELECT * FROM Rentals", conn);

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Console.WriteLine($"{reader["RentalID"]} | {reader["CustomerID"]} | {reader["VehicleID"]} | {reader["Status"]}");
                    }
                }
            }

            Console.ReadKey();
        }
    }
}