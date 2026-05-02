using Microsoft.Data.Sqlite;
using System;

namespace VehicleRentalSystem.Services
{
    public class AuthService
    {
        // ---------------- REGISTER ----------------
        public void Register()
        {
            UI.Header("REGISTER NEW ACCOUNT");

            Console.Write(" Name: ");
            string n = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(n))
            {
                UI.Error("Name is required.");
                return;
            }

            Console.Write(" Password: ");
            string p = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(p))
            {
                UI.Error("Password is required.");
                return;
            }

            using var conn = Database.GetConnection();
            conn.Open();

            // 🔍 Check duplicate user
            var check = conn.CreateCommand();
            check.CommandText = "SELECT COUNT(*) FROM Users WHERE Name=@n";
            check.Parameters.AddWithValue("@n", n);

            if ((long)check.ExecuteScalar() > 0)
            {
                UI.Error("User already exists.");
                return;
            }

            // ✅ Insert user
            var cmd = conn.CreateCommand();
            cmd.CommandText = "INSERT INTO Users(Name,Password,Role) VALUES(@n,@p,'Customer')";
            cmd.Parameters.AddWithValue("@n", n);
            cmd.Parameters.AddWithValue("@p", p);
            cmd.ExecuteNonQuery();

            UI.Success("Account Registered Successfully!");
        }

        // ---------------- LOGIN ----------------
        public (int id, string role)? Login()
        {
            UI.Header("USER LOGIN");

            Console.Write(" Name: ");
            string n = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(n))
            {
                UI.Error("Name is required.");
                return null;
            }

            Console.Write(" Password: ");
            string p = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(p))
            {
                UI.Error("Password is required.");
                return null;
            }

            using var conn = Database.GetConnection();
            conn.Open();

            var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT Id, Role FROM Users WHERE Name=@n AND Password=@p";
            cmd.Parameters.AddWithValue("@n", n);
            cmd.Parameters.AddWithValue("@p", p);

            using var r = cmd.ExecuteReader();

            if (r.Read())
            {
                UI.Success("Login Successful!");
                return (r.GetInt32(0), r.GetString(1));
            }

            UI.Error("Invalid Name or Password.");
            return null;
        }
    }
}