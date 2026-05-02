namespace VehicleRentalSystem.Models
{
    public class Rental
    {
        public int Id;
        public int UserId;
        public int VehicleId;
        public string RentDate; // Format: yyyy-MM-dd[cite: 14]
        public string DueDate;
        public string ReturnDate;

        // Billing Details
        public double BaseRent;
        public double LateFine;
        public double DamageFine;
        public double Total;

        public string Status; // "Rented" or "Completed"[cite: 14]
    }
}