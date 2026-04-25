using System;
using MySql.Data.MySqlClient;

namespace VehicleRentalSystem
{
    public static class DbConfig
    {
        private static string GetConnectionString()
        {
            bool isDocker = Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER") == "true";

            if (isDocker)
            {
                // Docker setup
                return "server=db;port=3306;database=VehicleRental;uid=root;pwd=;";
            }
            else
            {
                // XAMPP ab 3306 par hai, is liye port update kar diya hai
                return "server=localhost;port=3306;database=VehicleRental;uid=root;pwd=;";
            }
        }

        public static MySqlConnection GetConnection()
        {
            return new MySqlConnection(GetConnectionString());
        }
    }
}