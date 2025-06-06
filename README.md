# Pawn Vault - OOP  ASP.NET Core MVC Project

Follow the instructions below to set up the project on your local machine.

## 🚀 Getting Started

### 1. Clone the Repository
open git bash mkdir in your chosen location 
```bash
git clone https://github.com/0xShad/PawnVault-pawn-management-system.git
cd PawnVault-pawn-management-system
```
### 2. Restore the dependencies
On your git bash, cd where the clone folder located and run
```bash
dotnet restore
```
### 3. Set Up Local Configuration
You need SQL Server and SQL Server Management Studio to get your connection strings.
-You may need to change the connection string on the appsettings.json 
```bash
  "DefaultConnection": "Server=localhost\\SQLEXPRESS;Database=PawnVaultDB;Trusted_Connection=True;Encrypt=True;TrustServerCertificate=True;Multi
```

📌 Prerequisite:

SQL Server , SQL Server Management Studio 

⚠️ You only need to do this once on your machine.

💡 After installing, restart your terminal or IDE if dotnet ef isn’t recognized.
