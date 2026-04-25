# Vehicle Rental Management System 

A professional console-based **Management Information System (MIS)** developed in **C# and MySQL**. This system is designed for rental agencies to automate vehicle tracking, customer registration, and complex billing logic including late fees and damage assessments. It features **Smart Environment Detection**, allowing it to run seamlessly on local servers (XAMPP) or within **Docker containers**.

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
    * **Late Fees:** Automatically calculates charges if the vehicle is returned past the due date.
    * **Damage Assessment:** Allows admins to manually input damage costs during the return process.
* **Dynamic Billing:** Generates a final bill combining Base Rent + Late Fees + Damage Fines.

### 4. Business Intelligence and Reports
* **Financial Summaries:** Separate tracking of **Base Revenue** and **Collected Fines** for clear auditing.
* **Fleet Analytics:** Grouped reports showing the distribution of available and rented vehicles.
* **Transaction History:** Logs the last 10 rental transactions with detailed cost breakdowns.

### 5. Containerization and DevOps 
* **Dockerized: Fully configured with Dockerfile and docker-compose.
* **Smart DbConfig: Auto-switches between localhost:3306 (XAMPP) and db:3306 (Docker) using environment variables.

---

## Billing Formula

The system ensures financial accuracy using the following logic:

> **Total Payable = (Base Rent per Day × Days Rented) + (Days Late × 500) + Manual Damage Fine**

---

## Tech Stack

| Component | Technology |
| :--- | :--- |
| **Language** | C# (.NET 6.0 / Core) |
| **Database** | MySQL (Relational) |
| **Containerization** | **Docker and Docker Compose** |
| **Server (Local)** | XAMPP (MySQL Port 3306) |

---

## Setup and Installation

### Option 1: Docker Setup 
If you have Docker installed, you don't need to manually create a database:
1. Open a terminal in the project root folder.
2. Run the command: `docker-compose up --build`
3. The system will automatically set up MySQL, create the `VehicleRental` database, and launch the application.

### Option 2: Local XAMPP Setup 
1. **XAMPP Configuration:** Ensure MySQL is running on port **3306**.
2. Run XAMPP Control Panel as Administrator to avoid permission issues.
3. **Database Setup:** * Open phpMyAdmin (`http://localhost/phpmyadmin`).
    * Create a database named `VehicleRental`.
    * Import the provided `db_init.sql` script to generate the schema.
4. **Run Application:** Open the solution in **Visual Studio** and press **F5**.

---

## Transaction Safety and Null Handling
The system is built to be robust against data errors:
* **Safe Conversion:** Uses `SafeInt`, `SafeDecimal`, and `SafeString` methods to handle `NULL` values from the database.
* **Crash Prevention:** If a field is empty in the database, the system defaults to `0` or an empty string instead of crashing.
* **Input Validation:** Ensures that only valid IDs and amounts are processed during rentals.

---

## Project Structure
* `Program.cs`: Main navigation and menu logic.
* `RentalManager.cs`: The core engine for Rent/Return and Fine logic.
* `VehicleManager.cs` / `CustomerManager.cs`: CRUD operations for entities.
* `ReportManager.cs`: Financial and operational data visualization.
* `DbConfig.cs`: Intelligent MySQL connection management (Auto-detects environment).
* `Dockerfile` and `docker-compose.yml`: Containerization configuration.
