# MockApiServer  
*A powerful, configurable .NET 8 API Mocking Engine with a built-in Management Dashboard.*

[![.NET](https://img.shields.io/badge/.NET-8.0-blueviolet?style=flat-square&logo=dotnet)](https://dotnet.microsoft.com/)
[![Build](https://img.shields.io/github/actions/workflow/status/sabbccc/MockApiServer/dotnet.yml?style=flat-square)](https://github.com/sabbccc/MockApiServer/actions)  
[![Contributions welcome](https://img.shields.io/badge/contributions-welcome-brightgreen.svg?style=flat-square)](../../issues)

---

## 🚀 Overview  
**MockApiServer** is a centralized tool designed to simulate complex backend behaviors. Unlike static mock servers, it provides a full **Web Management Dashboard** to define multiple response scenarios for the same endpoint, making it ideal for:
- **Frontend Development:** Work independently of backend progress.
- **QA Automation:** Easily trigger "Edge Cases" (404, 500, timeouts) via headers.
- **Integration Testing:** Simulate third-party API responses without real network calls.

---

## ✨ Features  
- **Multi-Tenant Support:** Organize mocks by "Applications".
- **Dynamic Scenarios:** Define multiple responses (Success, Validation Error, Server Error) for a single URL and switch between them using the `X-Mock-Scenario` header.
- **Web Dashboard:** Full CRUD interface for managing Users, Applications, and Mocks.
- **Custom Headers:** Define specific HTTP headers for every mock response.
- **Advanced Logging:** Integrated Serilog request/response logging to track incoming mock hits.
- **Security:** Built-in Cookie Authentication and User Management.
- **Persistence:** Powered by MySQL/MariaDB for reliable data storage.

---

## 🛠️ Tech Stack  
- **Framework:** ASP.NET Core 8.0 (MVC + Web API)
- **ORM:** Entity Framework Core
- **Database:** MySQL
- **Logging:** Serilog (with Correlation ID support)
- **UI:** Bootstrap 5 & Razor Views
- **API Documentation:** Swagger/OpenAPI

---

## 🚦 Getting Started  

### Prerequisites  
- [.NET 8 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)
- [MySQL Server](https://dev.mysql.com/downloads/installer/) (or MariaDB)
- A tool to run SQL scripts (MySQL Workbench, DBeaver, etc.)

---

### 1. Database Setup  
Before running the application, you must initialize the database schema.
1. Locate the SQL script in `Files/mock_api.sql`.
2. Run the script on your MySQL instance. This will create the `mock_api_server_db` and initial tables.
3. Configure your connection string in `MockApiServer/appsettings.json`:
   ```json
   "ConnectionStrings": {
     "MySqlConnection": "Server=localhost;Database=mock_api_server_db;User=db_user;Password=yourpassword;"
   }
   ```

### 2. Installation & Run  
```bash
# Clone the repository
git clone https://github.com/sabbccc/MockApiServer.git

# Navigate into the project folder
cd MockApiServer

# Build and Run
dotnet run --project MockApiServer/MockApiServer.csproj
```
The application will be available at `http://localhost:5283` (or your configured Kestrel port).

---

## 📖 How to Use  

### 1. The Management Dashboard  
Log in to the web interface to:
1. **Create an Application:** e.g., "Mobile App".
2. **Define a Mock:** e.g., Path: `/api/v1/profile`, Method: `GET`.
3. **Add Scenarios:** 
   - `default`: Status 200, Body: `{"name": "John"}`
   - `unauthorized`: Status 401, Body: `{"error": "Token expired"}`

### 2. Calling the Mock API  
To hit your mock endpoint, simply call the URL. The server uses a regex-based controller to intercept any route not reserved by the system.

**To trigger a specific scenario:**
Add the `X-Mock-Scenario` header to your request.

```http
GET /api/v1/profile
X-Mock-Scenario: unauthorized
Host: localhost:5283
```
---

## 📂 Project Structure  
- `MockApiServer/Controllers`: Contains both the Dashboard (MVC) and the `MockRequestsController` (the engine).
- `MockApiServer/Services`: Business logic for scenario matching and response building.
- `MockApiServer/Repositories`: Data access layer using EF Core.
- `Files/`: Database scripts.

---

## 🤝 Contributing  
Contributions are what make the open-source community such an amazing place to learn, inspire, and create.
1. Fork the Project
2. Create your Feature Branch (`git checkout -b feature/AmazingFeature`)
3. Commit your Changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the Branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request
