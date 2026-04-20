using System;

namespace VehicleRentalSystem
{
    class Program
    {
        static void Main(string[] args)
        {
            // Set console title for a professional look
            Console.Title = "Vehicle Rental Management System | Admin Portal";

            // Initialize all Managers
            VehicleManager vm = new VehicleManager();
            CustomerManager cm = new CustomerManager();
            RentalManager rm = new RentalManager();
            ReportManager rep = new ReportManager();

            while (true)
            {
                Console.Clear();
                Console.WriteLine("================================================");
                Console.WriteLine("     VEHICLE RENTAL SYSTEM     ");
                Console.WriteLine("================================================");
                Console.WriteLine(" 1.  Add New Vehicle");
                Console.WriteLine(" 2.  View All Vehicles");
                Console.WriteLine(" 3.  Update Vehicle (Rent/Status)");
                Console.WriteLine(" 4.  Delete Vehicle");
                Console.WriteLine(" ---------------------------------------------- ");
                Console.WriteLine(" 5.  Register New Customer");
                Console.WriteLine(" 6.  View All Customers");
                Console.WriteLine(" ---------------------------------------------- ");
                Console.WriteLine(" 7.  Process New Rental (Check Out)");
                Console.WriteLine(" 8.  Process Vehicle Return (Check In)");
                Console.WriteLine(" ---------------------------------------------- ");
                Console.WriteLine(" 9.  View Total Earnings Report");
                Console.WriteLine(" 0.  Exit System");
                Console.WriteLine("================================================");
                Console.Write("Select Option: ");

                string choice = Console.ReadLine();

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
                        cm.AddCustomer();
                        break;
                    case "6":
                        cm.ViewCustomers();
                        break;
                    case "7":
                        rm.RentVehicle();
                        break;
                    case "8":
                        rm.ReturnVehicle();
                        break;
                    case "9":
                        rep.TotalEarnings();
                        break;
                    case "0":
                        Console.WriteLine("\nClosing system... Press any key to exit.");
                        Console.ReadKey();
                        return;
                    default:
                        Console.WriteLine("\n[!] Invalid option! Press any key to try again...");
                        Console.ReadKey();
                        break;
                }
            }
        }
    }
}