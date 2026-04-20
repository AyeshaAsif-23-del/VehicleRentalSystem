using System;
using MySql.Data.MySqlClient;

namespace VehicleRentalSystem
{
    public class LoginManager
    {
       
        private string GetInput(string message)
        {
            Console.Write(message);
            return Console.ReadLine() ?? "";
        }

        public bool Login()
        {
            int attempts = 3; 

            while (attempts > 0)
            {
                Console.Clear();
                Console.WriteLine("==== LOGIN ====");

                string username = GetInput("Username: ");
                string password = GetInput("Password: ");

                using (var conn = DbConfig.GetConnection())
                {
                    try
                    {
                        conn.Open();

                        string query = "SELECT COUNT(*) FROM Users WHERE Username=@u AND Password=@p";
                        MySqlCommand cmd = new MySqlCommand(query, conn);

                        cmd.Parameters.AddWithValue("@u", username);
                        cmd.Parameters.AddWithValue("@p", password);

                        object result = cmd.ExecuteScalar();

                        int count = (result != null && result != DBNull.Value)
                                    ? Convert.ToInt32(result)
                                    : 0;

                        if (count > 0)
                        {
                            Console.WriteLine("\nLogin Successful!");
                            Console.ReadKey();
                            return true;
                        }
                        else
                        {
                            attempts--;
                            Console.WriteLine($"\nInvalid Login! Attempts left: {attempts}");
                            Console.ReadKey();
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Database Error: " + ex.Message);
                        Console.ReadKey();
                        return false;
                    }
                }
            }

            Console.WriteLine("\nToo many failed attempts. System closing...");
            Console.ReadKey();
            return false;
        }
    }
}