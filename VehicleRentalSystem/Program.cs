using System;

namespace VehicleRentalSystem
{
    class Program
    {
        // SAFE INPUT METHOD (fixes all null warnings)
        static string GetInput(string message)
        {
            Console.Write(message);
            return Console.ReadLine() ?? "";
        }

        static void Main(string[] args)
        {
            Console.Title = "Vehicle Rental Management System | Admin Portal";

            // LOGIN
            LoginManager login = new LoginManager();

            if (!login.Login())
            {
                Console.WriteLine("\nAccess Denied...");
                Console.ReadKey();
                return;
            }

            // Managers
            VehicleManager vm = new VehicleManager();
            CustomerManager cm = new CustomerManager();
            RentalManager rm = new RentalManager();
            ReportManager rep = new ReportManager();

            while (true)
            {
                Console.Clear();

                Console.WriteLine("======================================");
                Console.WriteLine("     VEHICLE RENTAL SYSTEM");
                Console.WriteLine("======================================");

                Console.WriteLine("1.  Add Vehicle");
                Console.WriteLine("2.  View Vehicles");
                Console.WriteLine("3.  Update Vehicle");
                Console.WriteLine("4.  Delete Vehicle");
                Console.WriteLine("5.  Search Vehicle");

                Console.WriteLine("--------------------------------------");

                Console.WriteLine("6.  Add Customer");
                Console.WriteLine("7.  View Customers");
                Console.WriteLine("8.  Search Customer");

                Console.WriteLine("--------------------------------------");

                Console.WriteLine("9.  Rent / Return Vehicle");

                Console.WriteLine("--------------------------------------");

                Console.WriteLine("10. Reports Menu");

                Console.WriteLine("--------------------------------------");

                Console.WriteLine("0.  Exit System");

                Console.WriteLine("======================================");

                string choice = GetInput("Select Option: ");

                switch (choice)
                {
                    case "1":
                        vm.AddVehicle();
                        break;

                    case "2":
                        vm.ViewVehicles();
                        break;

                    case "3":
                        vm.UpdateVehicle();
                        break;

                    case "4":
                        vm.DeleteVehicle();
                        break;

                    case "5":
                        vm.SearchVehicle();
                        break;

                    case "6":
                        cm.AddCustomer();
                        break;

                    case "7":
                        cm.ViewCustomers();
                        break;

                    case "8":
                        cm.SearchCustomer();
                        break;

                    case "9":
                        Console.Clear();
                        Console.WriteLine("1. Rent Vehicle");
                        Console.WriteLine("2. Return Vehicle");

                        string rChoice = GetInput("Select Option: ");

                        if (rChoice == "1")
                            rm.RentVehicle();
                        else if (rChoice == "2")
                            rm.ReturnVehicle();
                        else
                        {
                            Console.WriteLine("Invalid option!");
                            Console.ReadKey();
                        }
                        break;

                    case "10":
                        rep.DisplayReportsMenu();
                        break;

                    case "0":
                        Console.WriteLine("\nExiting system...");
                        return;

                    default:
                        Console.WriteLine("\nInvalid option!");
                        Console.ReadKey();
                        break;
                }
            }
        }
    }
}