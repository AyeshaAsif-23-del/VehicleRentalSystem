Vehicle Rental Management System

A professional, console-based management system built with C# and SQLite.
This project provides a dual-role system (Admin and Customer) with complete vehicle rental lifecycle management, reporting, and Docker support.

Key Features
Dual Role System
Separate dashboards for Admin and Customer for personalized workflows
Vehicle Management
Add Vehicles: Add new inventory with details like brand, plate number, and type
View Inventory: View all vehicles or filter available ones
Update Pricing: Modify rental rates dynamically
Delete Vehicles: Remove vehicles with checks for active rentals
Rental Lifecycle
Rent Vehicle: Booking system with automated rent calculation
Return Process: Customer return request and admin settlement
Billing and Fines: Automated late fee and damage charge calculation
Advanced Reporting
Financial analytics: total revenue and monthly revenue
Top performance: identify top earning vehicle
System Configuration
Admin-controlled system settings for fines and charges
Getting Started
Prerequisites
.NET 10.0 SDK or higher
SQLite 3
Installation
Clone Repository
git clone https://github.com/yourusername/VehicleRentalSystem.git
cd VehicleRentalSystem
Build Project
dotnet build
Run Application
dotnet run
Default Login

Username: admin
Password: 123

Docker Deployment
Build Image
docker build -t vehicle-rental-app .
Run Container
docker run -it --name rental-container -v rental_data:/app/data vehicle-rental-app

Note: Database is stored persistently using a Docker volume (rental_data).

Project Structure
VehicleRentalSystem
│
├── Program.cs              Main application flow and dashboards
├── Database.cs             SQLite connection and initialization
├── UI.cs                   Console UI formatting and styling
│
├── Services/
│   ├── AuthService.cs      Authentication and registration logic
│   ├── VehicleService.cs   Vehicle CRUD operations
│   └── RentalService.cs    Rental, billing, and settlement logic
│
└── Models/                 User, Vehicle, and Rental data structures
Technologies Used
C#
.NET 10.0
SQLite
Docker
Author

Developed by Ayesha
Computer Science Student at KICSIT
