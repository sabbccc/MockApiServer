# MockApiServer  
*A lightweight .NET-based API mock server for developers and testers.*

[![.NET](https://img.shields.io/badge/.NET-8.0-blueviolet?style=flat-square&logo=dotnet)](https://dotnet.microsoft.com/)  
[![License: MIT](https://img.shields.io/badge/License-MIT-green.svg?style=flat-square)](LICENSE)  
[![Build](https://img.shields.io/github/actions/workflow/status/sabbccc/MockApiServer/dotnet.yml?style=flat-square)](https://github.com/sabbccc/MockApiServer/actions)  
[![Contributions welcome](https://img.shields.io/badge/contributions-welcome-brightgreen.svg?style=flat-square)](../../issues)

---

## Overview  
**MockApiServer** lets you spin up a configurable mock REST API in seconds.  
It's built using **ASP.NET Core Minimal APIs**, designed to help front-end developers, QA engineers, and integration testers simulate backend behavior with ease.

Whether your real backend is still under development or you just need predictable responses for automated tests — MockApiServer’s got you covered.

---

## Features  
✅ Quick to set up and run — no database required  
✅ Supports **GET**, **POST**, **PUT**, **DELETE**, and custom status codes  
✅ Define responses dynamically via **JSON config** or **C# classes**  
✅ Optionally simulate **delays**, **errors**, or **timeouts**  
✅ CORS-enabled out of the box  
✅ Ideal for frontend testing, demos, or CI/CD pipelines  

---

## Getting Started  

### Prerequisites  
- [.NET 8 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/8.0) or later  
- Git  
- Any terminal (PowerShell, Bash, etc.)

---

### Installation  

```bash
# Clone the repository
git clone https://github.com/sabbccc/MockApiServer.git

# Navigate into the project folder
cd MockApiServer

# Restore dependencies
dotnet restore

# Build the project
dotnet build

# Run the application
dotnet run --project MockApiServer/MockApiServer.csproj
