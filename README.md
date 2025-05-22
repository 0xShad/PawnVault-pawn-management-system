# Pawn Vault - OOP  ASP.NET Core MVC Project

Follow the instructions below to set up the project on your local machine.

## 🚀 Getting Started

### 1. Clone the Repository

```bash
git clone https://github.com/your-username/your-repo-name.git
cd your-repo-name
```
### 2. Open .sln file
Restore the dependencies
```bash
dotnet restore
```
### 3. Set Up Local Configuration

If the project requires secrets (e.g., database connection strings, API keys), set them up using User Secrets:

```bash
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "YourConnectionStringHere"
```
Or edit appsettings.Development.json if that's being used.

### 4. Apply Database Migrations (Entity Framework Core)

This project uses Entity Framework Core for database access. To set up the database locally, apply the migrations using the following command:

```bash
dotnet ef database update
```

📌 Prerequisite:

You need the EF Core CLI installed globally to use the dotnet ef command.

Install it using:
```bash
dotnet tool install --global dotnet-ef
```

⚠️ You only need to do this once on your machine.

💡 After installing, restart your terminal or IDE if dotnet ef isn’t recognized.
