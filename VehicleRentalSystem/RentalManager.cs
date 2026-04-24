using System;
using MySql.Data.MySqlClient;

namespace VehicleRentalSystem
{
    public class RentalManager
    {
       
        private string GetInput(string msg)
        {
            Console.Write(msg);

            string? input = Console.ReadLine();
            return input ?? "";   
        }

        private bool GetInt(string msg, out int value)
        {
            Console.Write(msg);

            string? input = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(input))
            {
                value = 0;
                return false;
            }

            return int.TryParse(input, out value);
        }

        private int SafeInt(object? value)
        {
            return (value == null || value == DBNull.Value) ? 0 : Convert.ToInt32(value);
        }

        private decimal SafeDecimal(object? value)
        {
            return (value == null || value == DBNull.Value) ? 0 : Convert.ToDecimal(value);
        }

        private string SafeString(object? value)
        {
            return (value == null || value == DBNull.Value) ? "" : value.ToString() ?? "";
        }

        
        // 1. RENT VEHICLE
      
        public void RentVehicle()
        {
            Console.Clear();
            Console.WriteLine("--- Process New Rental ---");

            if (!GetInt("Enter Customer ID: ", out int cId)) return;
            if (!GetInt("Enter Vehicle ID: ", out int vId)) return;
            if (!GetInt("Enter Rental Days: ", out int days)) return;

            using (var conn = DbConfig.GetConnection())
            {
                try
                {
                    conn.Open();

                    // Check customer
                    MySqlCommand custCheck = new MySqlCommand(
                        "SELECT COUNT(*) FROM Customers WHERE CustomerID=@cid", conn);

                    custCheck.Parameters.AddWithValue("@cid", cId);

                    int custExists = SafeInt(custCheck.ExecuteScalar());

                    if (custExists == 0)
                    {
                        Console.WriteLine("\nCustomer not found.");
                        Console.ReadKey();
                        return;
                    }

                    // Check vehicle
                    MySqlCommand cmd = new MySqlCommand(
                        "SELECT Status, RentPerDay FROM Vehicles WHERE VehicleID=@vid", conn);

                    cmd.Parameters.AddWithValue("@vid", vId);

                    using (var reader = cmd.ExecuteReader())
                    {
                        if (!reader.Read())
                        {
                            Console.WriteLine("Vehicle not found.");
                            return;
                        }

                        string status = SafeString(reader["Status"]);

                        if (status != "Available")
                        {
                            Console.WriteLine("Vehicle already rented.");
                            return;
                        }

                        decimal rentPerDay = SafeDecimal(reader["RentPerDay"]);
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

                        // update vehicle
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
                    @"SELECT VehicleID, RentDate, TotalDays, TotalAmount 
                      FROM Rentals 
                      WHERE RentalID=@id AND Status='Active'", conn);

                cmd.Parameters.AddWithValue("@id", rId);

                using (var reader = cmd.ExecuteReader())
                {
                    if (!reader.Read())
                    {
                        Console.WriteLine("Rental not found or already completed.");
                        Console.ReadKey();
                        return;
                    }

                    int vId = SafeInt(reader["VehicleID"]);
                    DateTime rentDate = Convert.ToDateTime(reader["RentDate"]);
                    int allowedDays = SafeInt(reader["TotalDays"]);
                    decimal baseAmount = SafeDecimal(reader["TotalAmount"]);

                    reader.Close();

           

                    DateTime returnDate = DateTime.Now;
                    int actualDays = (returnDate - rentDate).Days + 1;

                    int lateDays = actualDays - allowedDays;
                    if (lateDays < 0) lateDays = 0;

                    decimal lateFine = lateDays * 500;

                    // Damage input safe
                    Console.Write("Damage fine (0 if none): ");
                    string? damageInput = Console.ReadLine();
                    decimal damageFine = decimal.TryParse(damageInput, out decimal d) ? d : 0;

                    decimal totalFine = lateFine + damageFine;
                    decimal finalAmount = baseAmount + totalFine;

                    // UPDATE RENTAL

                    MySqlCommand updateRental = new MySqlCommand(
                        @"UPDATE Rentals 
                          SET Status='Completed',
                              ReturnDate=CURDATE(),
                              Fine=@fine,
                              TotalAmount=@total
                          WHERE RentalID=@id", conn);

                    updateRental.Parameters.AddWithValue("@fine", totalFine);
                    updateRental.Parameters.AddWithValue("@total", finalAmount);
                    updateRental.Parameters.AddWithValue("@id", rId);
                    updateRental.ExecuteNonQuery();

                    // update vehicle
                    MySqlCommand updateVehicle = new MySqlCommand(
                        "UPDATE Vehicles SET Status='Available' WHERE VehicleID=@v", conn);

                    updateVehicle.Parameters.AddWithValue("@v", vId);
                    updateVehicle.ExecuteNonQuery();

                    // BILL
                    Console.WriteLine("\n--- FINAL BILL ---");
                    Console.WriteLine($"Base Amount : {baseAmount:0.00}");
                    Console.WriteLine($"Late Days   : {lateDays}");
                    Console.WriteLine($"Late Fine   : {lateFine:0.00}");
                    Console.WriteLine($"Damage Fine : {damageFine:0.00}");
                    Console.WriteLine($"Total Fine  : {totalFine:0.00}");
                    Console.WriteLine($"TOTAL PAY   : {finalAmount:0.00}");

                    Console.WriteLine("\nVehicle returned successfully!");
                }
            }

            Console.ReadKey();
        }

        // 3. VIEW RENTALS
       
        public void ViewRentals()
        {
            Console.Clear();
            Console.WriteLine("--- Rentals ---");

            using (var conn = DbConfig.GetConnection())
            {
                conn.Open();

                MySqlCommand cmd = new MySqlCommand("SELECT * FROM Rentals", conn);

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Console.WriteLine(
                            $"{SafeInt(reader["RentalID"])} | " +
                            $"{SafeInt(reader["CustomerID"])} | " +
                            $"{SafeInt(reader["VehicleID"])} | " +
                            $"{SafeString(reader["Status"])} | " +
                            $"{SafeDecimal(reader["TotalAmount"]):0.00}"
                        );
                    }
                }
            }

            Console.ReadKey();
        }
    }
}