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
                // Docker uses port 3306 internally
                return "server=db;port=3306;database=VehicleRental;uid=root;pwd=;";
            }
            else
            {
                //  XAMPP setup uses port 3307
                return "server=localhost;port=3307;database=VehicleRental;uid=root;pwd=;";
            }
        }

        public static MySqlConnection GetConnection()
        {
            return new MySqlConnection(GetConnectionString());
        }
    }
}