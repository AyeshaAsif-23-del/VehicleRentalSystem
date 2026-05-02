using Microsoft.Data.Sqlite;
using System;

namespace VehicleRentalSystem
{
    public class Database
    {
        public static SqliteConnection GetConnection() => new SqliteConnection("Data Source=vehicle_rental.db");

        public static void Init()
        {
            using var conn = GetConnection();
            conn.Open();
            var cmd = conn.CreateCommand();
            cmd.CommandText = @"
            CREATE TABLE IF NOT EXISTS Users(Id INTEGER PRIMARY KEY AUTOINCREMENT, Name TEXT, Password TEXT, Role TEXT);
            CREATE TABLE IF NOT EXISTS Vehicles(Id INTEGER PRIMARY KEY AUTOINCREMENT, Name TEXT, Brand TEXT, Type TEXT, PlateNumber TEXT, ModelYear TEXT, PricePerDay REAL, IsAvailable INTEGER);
            CREATE TABLE IF NOT EXISTS Rentals(Id INTEGER PRIMARY KEY AUTOINCREMENT, UserId INTEGER, VehicleId INTEGER, RentDate TEXT, DueDate TEXT, ReturnDate TEXT, BaseRent REAL, LateFine REAL, DamageFine REAL, Total REAL, Status TEXT);
            CREATE TABLE IF NOT EXISTS SystemSettings(Key TEXT PRIMARY KEY, Value REAL);";
            cmd.ExecuteNonQuery();

            
            cmd.CommandText = "INSERT OR IGNORE INTO SystemSettings VALUES('LateFeePerDay', 500.0), ('MinorDamage', 1000.0), ('MajorDamage', 5000.0);";
            cmd.ExecuteNonQuery();
            cmd.CommandText = "INSERT OR IGNORE INTO Users(Name,Password,Role) VALUES('admin','123','Admin')";
            cmd.ExecuteNonQuery();
        }
    }
}
 