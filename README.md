# Vehicle Rental Management System 

A professional console-based **Management Information System (MIS)** developed in **C# and MySQL**. This system is designed for rental agencies to automate vehicle tracking, customer registration, and complex billing logic including late fees and damage assessments.

---

## Key Features

### 1. Secure Admin Authentication
* **Multi-Attempt Security:** Restricted access with a **3-attempt login limit** to prevent unauthorized access.
* **Database Verification:** Credentials are authenticated against the `Users` table for secure session management.

### 2. Fleet and Customer Management
* **Full CRUD Operations:** Seamlessly Add, View, Update, and Delete vehicles and customers.
* **Real-time Status Tracking:** Monitor vehicle availability (`Available` vs `Rented`) instantly.
* **Data Integrity:** Prevents duplicate entries (Plate Numbers/CNICs) and ensures relational consistency.

### 3. Rental and Return Logic
* **Conflict Prevention:** Smart check to ensure a vehicle isn't double-booked.
* **Automated Fine Calculation:**
    * **Late Fees:** Automatically calculates charges if the vehicle is returned past the due date (Rate: **500 PKR/day**).
    * **Damage Assessment:** Allows admins to manually input damage costs during the return process.
* **Dynamic Billing:** Generates a final bill combining Base Rent + Late Fees + Damage Fines.

### 4. Business Intelligence and Reports
* **Financial Summaries:** Separate tracking of **Base Revenue** and **Collected Fines** for clear auditing.
* **Fleet Analytics:** Grouped reports showing the distribution of available and rented vehicles.
* **Transaction History:** Logs the last 10 rental transactions with detailed cost breakdowns.

---

## Billing Formula

The system ensures financial accuracy using the following logic:

> **Total Payable = (Base Rent per Day × Days Rented) + (Days Late × 500) + Manual Damage Fine**

---

## Tech Stack

| Component | Technology |
| :--- | :--- |
| **Language** | C# (.NET Framework / Core) |
| **Database** | MySQL (Relational) |
| **Server** | XAMPP (Localhost) |
| **Library** | MySql.Data.MySqlClient (Connector) |

---

## Setup and Installation

### 1. XAMPP Configuration
Ensure your XAMPP services are configured to match the project ports:
* **Apache:** Running on Port `8080`.
* **MySQL:** Running on Port **`3307`** (as configured in `DbConfig.cs`).

### 2. Database Setup
1.  Open **phpMyAdmin** (`http://localhost:8080/phpmyadmin`).
2.  Create a new database named: `VehicleRental`.
3.  Import the provided SQL script to generate the following schema:
    * `Users`, `Vehicles`, `Customers`, and `Rentals`.

### 3. Project Configuration
1.  Open the solution in **Visual Studio**.
2.  Verify the connection string in `DbConfig.cs` matches your port settings.
3.  Press **F5** to build and run the application.

---

## Transaction Safety and Null Handling
The system is built to be robust against data errors:
* **Safe Conversion:** Uses `SafeInt`, `SafeDecimal`, and `SafeString` methods to handle `NULL` values from the database.
* **Crash Prevention:** If a field is empty in the database, the system defaults to `0` or an empty string instead of crashing.
* **Input Validation:** Ensures that only valid IDs and amounts are processed during rentals.

---

## 📂 Project Structure
* `Program.cs`: Main navigation and menu logic.
* `RentalManager.cs`: The core engine for Rent/Return and Fine logic.
* `VehicleManager.cs` / `CustomerManager.cs`: CRUD operations for entities.
* `ReportManager.cs`: Financial and operational data visualization.
* `DbConfig.cs`: Centralized MySQL connection string.
