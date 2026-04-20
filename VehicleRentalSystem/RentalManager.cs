using MySql.Data.MySqlClient;
using System;

namespace VehicleRentalSystem
{
    public class RentalManager
    {
        // 1. PROCESS NEW RENTAL
        public void RentVehicle()
        {
            Console.Clear();
            Console.WriteLine("--- Process New Rental ---");

            if (!int.TryParse(GetInput("Enter Customer ID: "), out int cId)) return;
            if (!int.TryParse(GetInput("Enter Vehicle ID: "), out int vId)) return;
            if (!int.TryParse(GetInput("Enter Rental Days: "), out int days)) return;

            using (var conn = DbConfig.GetConnection())
            {
                try
                {
                    conn.Open();

                    // Check Availability
                    MySqlCommand checkCmd = new MySqlCommand("SELECT Status, RentPerDay FROM Vehicles WHERE VehicleID=@vid", conn);
                    checkCmd.Parameters.AddWithValue("@vid", vId);

                    using (var reader = checkCmd.ExecuteReader())
                    {
                        if (reader.Read() && reader["Status"].ToString() == "Available")
                        {
                            decimal rentPerDay = Convert.ToDecimal(reader["RentPerDay"]);
                            decimal totalAmount = rentPerDay * days;
                            reader.Close();

                            // Insert Rental Record
                            string rentQuery = "INSERT INTO Rentals (CustomerID, VehicleID, RentDate, TotalDays, TotalAmount, Status) " +
                                               "VALUES (@cid, @vid, CURDATE(), @days, @amt, 'Active')";
                            MySqlCommand rentCmd = new MySqlCommand(rentQuery, conn);
                            rentCmd.Parameters.AddWithValue("@cid", cId);
                            rentCmd.Parameters.AddWithValue("@vid", vId);
                            rentCmd.Parameters.AddWithValue("@days", days);
                            rentCmd.Parameters.AddWithValue("@amt", totalAmount);
                            rentCmd.ExecuteNonQuery();

                            // Update Vehicle Status
                            MySqlCommand updateCmd = new MySqlCommand("UPDATE Vehicles SET Status='Rented' WHERE VehicleID=@vid", conn);
                            updateCmd.Parameters.AddWithValue("@vid", vId);
                            updateCmd.ExecuteNonQuery();

                            DisplayReceipt(totalAmount, days);
                        }
                        else
                        {
                            Console.WriteLine("\n[!] Error: Vehicle is already Rented or ID is invalid.");
                        }
                    }
                }
                catch (Exception ex) { Console.WriteLine("Database Error: " + ex.Message); }
            }
            Console.WriteLine("\nPress any key to return...");
            Console.ReadKey();
        }

        // 2. PROCESS RETURN
        public void ReturnVehicle()
        {
            Console.Clear();
            Console.WriteLine("--- Process Vehicle Return ---");
            if (!int.TryParse(GetInput("Enter Rental ID to return: "), out int rId)) return;

            using (var conn = DbConfig.GetConnection())
            {
                try
                {
                    conn.Open();
                    MySqlCommand getVeh = new MySqlCommand("SELECT VehicleID FROM Rentals WHERE RentalID=@rid AND Status='Active'", conn);
                    getVeh.Parameters.AddWithValue("@rid", rId);
                    object result = getVeh.ExecuteScalar();

                    if (result != null)
                    {
                        int vId = Convert.ToInt32(result);

                        // Mark Rental as Completed
                        MySqlCommand closeRental = new MySqlCommand("UPDATE Rentals SET Status='Completed', ReturnDate=CURDATE() WHERE RentalID=@rid", conn);
                        closeRental.Parameters.AddWithValue("@rid", rId);
                        closeRental.ExecuteNonQuery();

                        // Mark Vehicle as Available
                        MySqlCommand openVeh = new MySqlCommand("UPDATE Vehicles SET Status='Available' WHERE VehicleID=@vid", conn);
                        openVeh.Parameters.AddWithValue("@vid", vId);
                        openVeh.ExecuteNonQuery();

                        Console.WriteLine("\n[SUCCESS] Vehicle returned and fleet status updated.");
                    }
                    else { Console.WriteLine("\n[!] Error: Active rental record not found."); }
                }
                catch (Exception ex) { Console.WriteLine("Database Error: " + ex.Message); }
            }
            Console.ReadKey();
        }

        // 3. VIEW RENTAL LOGS (The missing link)
        public void ViewRentals()
        {
            Console.Clear();
            Console.WriteLine("--- Active Rental Records ---");
            using (var conn = DbConfig.GetConnection())
            {
                conn.Open();
                string query = "SELECT RentalID, CustomerID, VehicleID, TotalAmount, Status FROM Rentals";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                using (var reader = cmd.ExecuteReader())
                {
                    Console.WriteLine("------------------------------------------------------------");
                    Console.WriteLine(string.Format("{0,-5} | {1,-5} | {2,-5} | {3,-10} | {4,-10}", "RID", "CID", "VID", "Amount", "Status"));
                    Console.WriteLine("------------------------------------------------------------");
                    while (reader.Read())
                    {
                        Console.WriteLine(string.Format("{0,-5} | {1,-5} | {2,-5} | {3,-10} | {4,-10}",
                            reader["RentalID"], reader["CustomerID"], reader["VehicleID"], reader["TotalAmount"], reader["Status"]));
                    }
                }
            }
            Console.WriteLine("\nPress any key to return...");
            Console.ReadKey();
        }

        private void DisplayReceipt(decimal total, int days)
        {
            Console.WriteLine("\n=================================");
            Console.WriteLine("       RENTAL RECEIPT            ");
            Console.WriteLine("=================================");
            Console.WriteLine($" Date: {DateTime.Now.ToShortDateString()}");
            Console.WriteLine($" Duration: {days} Day(s)");
            Console.WriteLine($" Total Bill: {total:0.00} PKR");
            Console.WriteLine("=================================");
            Console.WriteLine("   Thank you for your business!  ");
        }

        private string GetInput(string prompt)
        {
            Console.Write(prompt);
            return Console.ReadLine();
        }
    }
}