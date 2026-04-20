# Vehicle Rental Management System 

A console-based **Management Information System (MIS)** developed in **C# and MySQL** to manage vehicle rental operations efficiently.  
This system helps rental agencies handle vehicles, customers, rentals, returns, and billing in a structured and automated way.

---

## Project Overview

The Vehicle Rental Management System is designed to simplify rental business operations by automating:

- Vehicle management  
- Rental processing  
- Return handling  
- Billing calculations  
- Database record keeping  

It ensures accuracy, reduces manual errors, and improves system efficiency using a relational database structure.

---

## Features

### Vehicle Management
- Add new vehicles to the system  
- View all available and rented vehicles  
- Update vehicle status (Available / Rented)  
- Store vehicle details like model, type, and rent per day  

---

### Customer Management
- Register customers in the system (admin-controlled)  
- Store customer details for rental tracking  

---

### Rental System
- Rent a vehicle to a customer  
- Prevent double booking of the same vehicle  
- Store rental and return dates  
- Maintain rental history  

---

### Automatic Billing
- Calculates total rent based on number of days  
- Formula:
  - `Total Rent = Daily Rate × Number of Days`
- Ensures accurate and automatic billing  

---

### Return System
- Update vehicle status on return  
- Close rental record after completion  
- Make vehicle available for next customer  

---

### Database Integration
- MySQL database using XAMPP  
- Stores:
  - Vehicles  
  - Customers  
  - Rentals  
- Ensures structured relational data  

---

### Transaction Safety
- Ensures data consistency using atomic operations  
- Prevents partial updates in case of errors  
- Maintains database integrity  

---

## Tech Stack

| Technology | Description |
|------------|-------------|
| C# | Programming Language |
| .NET Framework / Core | Application Framework |
| MySQL | Database |
| XAMPP | Local Server |
| MySql.Data | Database Connector |

---



