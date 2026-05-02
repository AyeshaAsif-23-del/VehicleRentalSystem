namespace VehicleRentalSystem.Models
{
    public class Vehicle
    {
        public int Id;
        public string Name;
        public string Brand;
        public string Type; // Example: "Car", "Bike", "SUV"
        public string PlateNumber; // Registration number[cite: 15]
        public string ModelYear;
        public double PricePerDay;
        public int IsAvailable; // 1 for True, 0 for False[cite: 15]
    }
}