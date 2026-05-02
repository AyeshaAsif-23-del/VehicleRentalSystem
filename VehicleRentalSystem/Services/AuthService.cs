using Microsoft.Data.Sqlite;
using System;

namespace VehicleRentalSystem.Services
{
    public class AuthService
    {
        public void Register()
        {
            UI.Header("REGISTER NEW ACCOUNT");

            Console.Write(" Name: ");
            string n = Console.ReadLine();

            Console.Write(" Password: ");
            string p = Console.ReadLine();

            using var conn = Database.GetConnection();
            conn.Open();

            var cmd = conn.CreateCommand();
            cmd.CommandText = "INSERT INTO Users(Name,Password,Role) VALUES(@n,@p,'Customer')";
            cmd.Parameters.AddWithValue("@n", n);
            cmd.Parameters.AddWithValue("@p", p);
            cmd.ExecuteNonQuery();

            
            UI.Success("Account Registered Successfully!");
            Console.ReadLine();
        }

        public (int id, string role)? Login()
        {
            UI.Header("USER LOGIN");

            Console.Write(" Name: ");
            string n = Console.ReadLine();

            Console.Write(" Password: ");
            string p = Console.ReadLine();

            using var conn = Database.GetConnection();
            conn.Open();

            var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT Id,Role FROM Users WHERE Name=@n AND Password=@p";
            cmd.Parameters.AddWithValue("@n", n);
            cmd.Parameters.AddWithValue("@p", p);

            var r = cmd.ExecuteReader();

            if (r.Read())
            {
                UI.Success("Login Successful!");
                return (r.GetInt32(0), r.GetString(1));
            }

            
            UI.Error("Invalid Name or Password.");
            Console.ReadLine();
            return null;
        }
    }
}