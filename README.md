# Vehicle Rental Management System

A professional, console-based management system built with C# and SQLite. This project provides a dual-role system (Admin and Customer) with complete vehicle rental lifecycle management, reporting, and Docker support.

---

## Key Features

### Dual Role System
* Separate dashboards for Admin and Customer for personalized workflows.

### Vehicle Management
* Add Vehicles: Easily add new inventory with details like Brand, Plate Number, and Type.
* View Inventory: Live view of all vehicles or filter specifically for available ones.
* Update Pricing: Modify rental rates dynamically for any vehicle.
* Delete Vehicles: Securely remove vehicles from the system with checks for active rentals.

### Rental Lifecycle
* Rent Vehicle: Real-time booking system with automated rent calculation.
* Return Process: Customers can request returns, and Admins finalize the settlement.
* Billing and Fines: Automated calculation of late fees and handling of damage charges (Minor/Major).

### Advanced Reporting
* Financial Analytics: View Total Revenue and Monthly Revenue.
* Top Performance: Identify the top-earning vehicle in your fleet.

### System Configuration
* Admin-controlled settings to update system-wide fines and fees directly from the UI.

---

## Getting Started

### Prerequisites
* .NET 10.0 SDK or higher
* SQLite 3

### Installation
1. Clone the repository:
 * git clone https://github.com/yourusername/VehicleRentalSystem.git
 * cd VehicleRentalSystem

2. Build the project:
 * dotnet build

3. Run the application:
 * dotnet run

### Default Login
* Username: admin
* Password: 123

---

## Docker Deployment

### 1. Build the Image
docker build -t vehicle-rental-app .

### 2. Run the Container
docker run -it --name rental-container -v rental_data:/app/data vehicle-rental-app

Note: Database is stored persistently using a Docker volume (rental_data).

---

## Project Structure

| File | Description |
| :--- | :--- |
| `Program.cs` | Main entry point and core navigation logic. |
| `Database.cs` | Handles SQLite connections and schema initialization. |
| `UI.cs` | Centralized formatting for tables, headers, and color-coded feedback. |
| `Services/` | Contains `AuthService`, `RentalService`, and `VehicleService` for business logic. |
| `Models/` | Data structures for `User`, `Vehicle`, and `Rental` objects. |

---

## Technologies Used
* Language: C#
* Framework: .NET 10.0
* Database: SQLite
* Containerization: Docker

---
Developed by Ayesha | Computer Science Student 
