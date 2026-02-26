# 🚀 BookSystem

A robust and secure Booking Management System backend built with **ASP.NET Core 8**. This project is designed to handle user registrations, client management, and financial expense tracking with high security and precision.

## 🛠️ Tech Stack
* **Framework:** ASP.NET Core 8 Web API
* **ORM:** Entity Framework Core (SQL Server)
* **Security:** JWT Authentication & BCrypt Password Hashing
* **Validation:** FluentValidation Auto-Validation
* **Middleware:** Global Exception Handling & Custom Logging

## ✨ Key Features
* **Role-Based Access Control (RBAC):** Admin vs. User permissions.
* **Automatic Admin Initialization:** The first registered user is automatically granted Admin privileges.
* **Financial Precision:** Accurate tracking of payments and expenses using `decimal(18,2)`.
* **Search & Pagination:** Efficient data retrieval for clients and records.
* **Global Exception Handling:** Consistent JSON error responses for all API failures.

## 🔒 Security Implementation
* **BCrypt Hashing:** Replaced standard SHA256 with BCrypt for superior password protection.
* **JWT Tokens:** Secure communication between client and server.

## 🚀 How to Run
1. Clone the repository.
2. Update the connection string in `appsettings.json`.
3. Run `Update-Database` in Package Manager Console.
4. Start the application and explore via Swagger UI.

## 🔒 Security Configuration
This project uses **User Secrets** to keep sensitive keys out of the source code. To run the project locally, you **MUST** configure your secrets:

```powershell
# In Package Manager Console:
dotnet user-secrets set "Jwt:Key" "ThisIsMyVerySecretLongKey1234567890"
dotnet user-secrets set "Jwt:Issuer" "BookingSystem"
dotnet user-secrets set "Jwt:Audience" "BookingSystemUsers"