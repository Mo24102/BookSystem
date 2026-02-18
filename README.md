# 🚀 BookSystem

A robust and secure Booking Management System backend built with **ASP.NET Core 8**. This project is designed to handle user registrations, client management, and financial expense tracking with high security and precision.

## 🛠️ Tech Stack
* **Framework:** .NET 8 Web API
* **Database:** SQL Server via Entity Framework Core
* **Security:** JWT Authentication & BCrypt Password Hashing
* **Logging & Handling:** Global Exception Middleware

## ✨ Key Features
* **Role-Based Access Control (RBAC):** Admin vs. User permissions.
* **Automatic Admin Initialization:** The first registered user is automatically granted Admin privileges.
* **Financial Precision:** Accurate tracking of payments and expenses using `decimal(18,2)`.
* **Search & Pagination:** Efficient data retrieval for clients and records.

## 🔒 Security Implementation
* **BCrypt Hashing:** Replaced standard SHA256 with BCrypt for superior password protection.
* **JWT Tokens:** Secure communication between client and server.

## 🚀 How to Run
1. Clone the repository.
2. Update the connection string in `appsettings.json`.
3. Run `Update-Database` in Package Manager Console.
4. Start the application and explore via Swagger UI.
