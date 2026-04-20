using System;
using System.Collections.Generic;
using System.Text;
using MySql.Data.MySqlClient;

namespace VehicleRentalSystem
{
    public static class DbConfig
    {
        // Using Port 3307 as configured in your XAMPP my.ini
        private static string connString = "server=localhost;port=3307;database=VehicleRentalDB;uid=root;pwd=;";

        public static MySqlConnection GetConnection()
        {
            return new MySqlConnection(connString);
        }
    }
}