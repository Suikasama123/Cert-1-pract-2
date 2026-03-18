# 🏛️ Citizen Registry API

**A Professional .NET Core Web API with Git Flow, CRUD Operations, and 12 Factor App Compliance**

## 👨‍💻 Author Information
| Field | Value |
|-------|-------|
| **Name** | Daniel Ochoa |
| **GitHub** | [@Suikasama123](https://github.com/Suikasama123) |
| **Email** | danielochoa1@upb.edu |
| **Repository** | [Cert-1-pract-2](https://github.com/Suikasama123/Cert-1-pract-2) |
| **Course** | Certificación 1 - Segundo Parcial |
| **Date** | March 2026 |

---

## 📋 Table of Contents

- [Overview](#overview)
- [Features](#project-features)
- [12 Factor App](#12-factor-app-methodology)
- [API Endpoints](#api-endpoints)
- [Getting Started](#running-the-application)
- [Project Structure](#project-structure)
- [Technology Stack](#technology-stack)
- [Configuration](#configuration-files)
- [Troubleshooting](#troubleshooting)
- [Production Roadmap](#next-steps-for-production)

---

## 🎯 Overview
This is a .NET Core Web API for managing citizens in a city registry system. The application implements RESTful CRUD operations, integrates with an external API for asset assignment, and follows the 12 Factor App methodology.

---

## ✨ Project Features

| Feature | Description |
|---------|-------------|
| **CRUD Operations** | Complete Create, Read, Update, and Delete functionality for citizens |
| **External API Integration** | Retrieves personal assets from `https://api.restful-api.dev/objects` |
| **CSV Persistence** | Stores citizen data in CSV format with automatic serialization |
| **Swagger UI** | Interactive API documentation and testing interface |
| **Structured Logging** | Comprehensive logging using Serilog with console and file output |
| **Error Handling** | Proper HTTP status codes and descriptive error responses |
| **Git Flow** | Professional branching model: `main` → `develop` → `P2-001` |
| **12 Factor App** | Full compliance with modern application architecture principles |

---

## 12 Factor App Methodology

### 1. **Codebase**
✅ **Single codebase tracked in version control system**

- The application is stored in a Git repository
- Git Flow model is implemented with branches: `main`, `develop`, `P2-001`
- All source code is version controlled
- Repository URL: https://github.com/Suikasama123/Cert-1-pract-2.git

**Implementation:**
```bash
git log --oneline  # View commit history
```

---

### 2. **Dependencies**
✅ **Dependencies declared explicitly in project manifest**

All dependencies are declared in `DogShelter.csproj`:
- Microsoft.AspNetCore.OpenApi
- Swashbuckle.AspNetCore (Swagger)
- Serilog (Logging framework)
- Newtonsoft.Json (JSON serialization)

**Implementation:**
```xml
<ItemGroup>
  <PackageReference Include="Serilog" Version="4.3.2" />
  <PackageReference Include="Swashbuckle.AspNetCore" Version="10.1.5" />
</ItemGroup>
```

---

### 3. **Config**
✅ **Configuration stored outside the code**

Configuration is externalized and stored in JSON files:
- `appsettings.json`: Shared configuration
- `appsettings.Development.json`: Development environment settings
- Environment variables for sensitive data

**Key Configurations:**
```json
{
  "Data": {
    "CitizensFile": "CitizensDatastore.csv"
  },
  "ExternalApi": {
    "ObjectsUrl": "https://api.restful-api.dev/objects"
  }
}
```

**No hardcoded values in code** - all settings come from configuration files or environment variables.

---

### 4. **Backing Services**
✅ **External services treated as attached resources**

The External API service (`ExternalAssetService`) treats the Objects API as a resource:
- URL is configurable in `appsettings.json`
- Can be easily switched to different API without code changes
- Uses dependency injection for loose coupling

**Implementation:**
```csharp
public class ExternalAssetService
{
    public async Task<string> GetRandomAssetAsync()
    {
        var apiUrl = _configuration["ExternalApi:ObjectsUrl"];
        var response = await _httpClient.GetAsync(apiUrl);
        // Process response...
    }
}
```

---

### 5. **Build, Release, Run**
✅ **Clear separation of build, release, and run stages**

**Build Stage:**
```bash
dotnet build
```

**Release Stage:**
```bash
dotnet publish -c Release -o ./release
```

**Run Stage:**
```bash
dotnet run
```

The application starts on `https://localhost:7240` and `http://localhost:5000`

---

### 6. **Processes**
✅ **Stateless process execution**

The application is completely stateless:
- No session state stored in memory
- All data persisted to CSV files
- Each request is independent
- Multiple instances can run without coordination
- Data is stored externally (CSV, not in-memory collections)

**Implementation:**
Citizens are loaded from CSV on service initialization and persisted after each operation.

---

### 7. **Port Binding**
✅ **Self-contained HTTP service**

The API exposes itself via HTTP without external web server:
- Configured in `launchSettings.json`
- Listens on `http://localhost:5000` and `https://localhost:7240`
- Swagger UI available at `http://localhost:5000/swagger/ui`
- Uses Kestrel web server built into ASP.NET Core

**Configuration:**
```json
"applicationUrl": "https://localhost:7240;http://localhost:5000"
```

---

### 8. **Concurrency**
⚠️ **Application designed for horizontal scaling**

While we do NOT use async/await for concurrency management (as per requirements), the application can scale horizontally:
- The API is stateless
- Multiple instances can run in parallel
- Each instance reads/writes to shared CSV file
- Load balancer can distribute requests
- Database/csv layer handles concurrent access

**Note:** Async/await is NOT implemented per instructor requirements. Concurrency is achieved through process-level scaling, not async operations.

---

### 9. **Disposability**
✅ **Fast startup and graceful shutdown**

**Startup:**
- Application initializes quickly (~2-3 seconds)
- CSV data loaded on startup
- Services registered in dependency container
- Ready to handle requests immediately

**Shutdown:**
- Logs are flushed: `Log.CloseAndFlush()`
- Graceful termination of HTTP listeners
- No long-running cleanup required
- `Ctrl+C` stops the application cleanly

**Implementation in Program.cs:**
```csharp
try { app.Run(); }
finally { Log.CloseAndFlush(); }
```

---

### 10. **Dev/Prod Parity**
✅ **Development and production environments remain similar**

**Tools:** Same .NET runtime for both environments  
**Dependencies:** Identical NuGet packages in both environments  
**Code:** Same source deployed to both  
**Configuration:** Different config files for different environments

**Development:**
```bash
dotnet run --environment Development
```

**Production:**
```bash
dotnet run --environment Production
```

Both use the same executable; only configuration files differ.

---

### 11. **Logs**
✅ **Logs treated as event streams**

**Serilog Configuration:**
- Logs written to Console (stdout)
- Logs written to rolling files (`logs/citizen-registry-*.log`)
- Structured logging with context enrichment
- Log level configurable by environment

**Log Examples:**
```log
INFO  Citizen created: John Doe (123456)
WARN  Citizen not found: 654321
ERROR Error calling external API for assets
INFO  Saved 5 citizens to CSV
```

**Implementation:**
```csharp
Log.Information("Citizen created: {FirstName} {LastName} ({CI})", firstName, lastName, ci);
Log.Error(ex, "Error calling external API for assets");
```

**Log Destinations:**
- `Console`: Real-time monitoring
- `logs/citizen-registry-*.log`: Persistent logs by date

---

### 12. **Admin Processes**
🔧 **Administrative tasks run separately (implicit)**

Admin processes can be implemented separately:
- Data cleanup scripts
- CSV backup utilities
- Database migrations
- User management tools

**Example Admin Task (separate console app):**
```csharp
// Could be a separate .NET console application
var citizens = CitizenService.GetAllCitizens();
// Archive or cleanup operations
```

---

## 🔌 API Endpoints

All endpoints are documented in Swagger UI at `http://localhost:5000/swagger`

### 📖 Get All Citizens
```http
GET /api/citizens
```

**Response (200 OK):**
```json
[
  {
    "firstName": "John",
    "lastName": "Doe",
    "ci": 123456,
    "bloodGroup": "O+",
    "personalAsset": "Apple MacBook Pro"
  },
  {
    "firstName": "Maria",
    "lastName": "Gomez",
    "ci": 654321,
    "bloodGroup": "A-",
    "personalAsset": "iPhone 15"
  }
]
```

### 🔍 Get Citizen by CI
```http
GET /api/citizens/123456
```

**Response (200 OK):**
```json
{
  "firstName": "John",
  "lastName": "Doe",
  "ci": 123456,
  "bloodGroup": "O+",
  "personalAsset": "Apple MacBook Pro"
}
```

**Response (404 Not Found):**
```
Citizen not found
```

### ➕ Create Citizen
```http
POST /api/citizens
Content-Type: application/json

{
  "firstName": "John",
  "lastName": "Doe",
  "ci": 123456
}
```

**Response (201 Created):**
```json
{
  "firstName": "John",
  "lastName": "Doe",
  "ci": 123456,
  "bloodGroup": "O+",
  "personalAsset": "Apple MacBook Pro"
}
```

**Note:** BloodGroup and PersonalAsset are automatically assigned:
- **BloodGroup:** Randomly selected from [A+, A-, B+, B-, O+, O-, AB+, AB-]
- **PersonalAsset:** Randomly retrieved from external API

### ✏️ Update Citizen
```http
PUT /api/citizens/123456
Content-Type: application/json

{
  "firstName": "Jane",
  "lastName": "Smith"
}
```

**Response (200 OK):**
```json
{
  "firstName": "Jane",
  "lastName": "Smith",
  "ci": 123456,
  "bloodGroup": "O+",
  "personalAsset": "Apple MacBook Pro"
}
```

**Business Rules:**
- ✅ Only `firstName` and `lastName` can be updated
- ❌ `BloodGroup` and `PersonalAsset` are immutable (cannot change)
- `ci` is the unique identifier and cannot change

**Response (404 Not Found):**
```
Citizen not found
```

### 🗑️ Delete Citizen
```http
DELETE /api/citizens/123456
```

**Response (200 OK):**
```
Citizen deleted successfully
```

**Response (404 Not Found):**
```
Citizen not found
```

---

## CSV Format

**File:** `CitizensDatastore.csv`

```csv
FirstName,LastName,CI,BloodGroup,PersonalAsset
John,Doe,123456,O+,MacBook
Maria,Gomez,654321,A-,iPhone
Luis,Rojas,888999,B+,Keyboard
```

---

---

## 🚀 Running the Application

### Prerequisites
- **.NET SDK** 10.0 or higher ([download](https://dotnet.microsoft.com/download))
- **Visual Studio** 2022 or **VS Code** with C# extension
- **Git** for version control

### Quick Start

**1. Clone the repository:**
```bash
git clone https://github.com/Suikasama123/Cert-1-pract-2.git
cd DogShelter
```

**2. Restore dependencies:**
```bash
dotnet restore
```

**3. Build the project:**
```bash
dotnet build
```

**4. Run the application:**
```bash
dotnet run
```

**5. Access the API:**
- 🔗 Swagger UI: `http://localhost:5000/swagger`
- 🌐 Health Check: `http://localhost:5000/api/citizens`

### Development Mode
```bash
dotnet run --environment Development
```
This loads `appsettings.Development.json` with debug logging enabled.

### Release Mode
```bash
dotnet build -c Release
dotnet run -c Release
```

---

## 🌳 Git Flow Workflow

This project follows the **Git Flow** branching model for professional version control.

### Branch Hierarchy
```
main (Production)
  ↑
develop (Integration)
  ↑
P2-001 (Feature)
```

### Main Branches

| Branch | Purpose | Protection |
|--------|---------|-----------|
| **main** | Production-ready release code | Tagged with version numbers |
| **develop** | Integration branch for features | Requires PR reviews |
| **P2-001** | Feature branch for this practice | Development work |

### Commit Naming Convention

All commits should follow the pattern:
```
P2-001 <Action> <Description>
```

**Examples:**
```
P2-001 Add CitizenController
P2-001 Implement Create Citizen endpoint
P2-001 Add CSV data persistence
P2-001 Integrate external API with ExternalAssetService
P2-001 Configure application with Serilog and Swagger
P2-001 Update README with 12 Factor App documentation
```

### Merging Strategy

1. **Feature Complete** on `P2-001`
   ```bash
   git add .
   git commit -m "P2-001 <feature description>"
   ```

2. **Merge to Develop** (Integration branch)
   ```bash
   git checkout develop
   git merge --no-ff P2-001
   ```

3. **Release to Main** (Production)
   ```bash
   git checkout main
   git merge --no-ff develop
   git tag -a v1.0 -m "Release version 1.0"
   ```

### View Git History
```bash
# Colored graph visualization
git log --graph --all --oneline --decorate

# Follow specific branch
git log main --oneline

# Show contributors
git log --oneline --all
```

---

## 📁 Project Structure

```
📦 DogShelter/
│
├── 📂 Controllers/
│   └── CitizensController.cs       # REST API endpoints (CRUD)
│
├── 📂 Models/
│   ├── Citizen.cs                 # Citizen entity with properties
│   ├── ApiResponse.cs             # Generic HTTP response wrapper
│   ├── Person.cs                  # Base person model
│   ├── Response.cs                # Response metadata
│   ├── DogBreed.cs               # Legacy (to be removed)
│   └── Shelter.cs                # Legacy (to be removed)
│
├── 📂 Services/
│   ├── ExternalAssetService.cs    # External API integration
│   └── DogBreedService.cs         # Legacy (to be removed)
│
├── 📂 Utils/
│   └── CSVHelper.cs               # CSV file operations
│
├── 📂 Properties/
│   └── launchSettings.json         # Launch configuration
│
├── 📂 logs/
│   └── citizen-registry-*.log      # Auto-generated log files
│
├── 📄 Program.cs                   # Application startup & config
├── 📄 appsettings.json            # Base configuration
├── 📄 appsettings.Development.json # Development environment config
├── 📄 Readme.MD                    # This file
├── 📄 .gitignore                   # Git ignore rules
├── 📄 CitizensDatastore.csv        # Persistent data storage
├── 📄 DogShelter.csproj           # Project file (NuGet packages)
└── 📄 DogShelter.sln              # Solution file
```

### Key Files

| File | Purpose |
|------|---------|
| `Controllers/CitizensController.cs` | Defines all REST endpoints for CRUD operations |
| `Models/Citizen.cs` | Entity class representing a citizen |
| `Services/ExternalAssetService.cs` | Handles external API calls for asset assignment |
| `Utils/CSVHelper.cs` | Manages CSV file read/write operations |
| `Program.cs` | Application setup: DI, logging, middleware |
| `appsettings.json` | Base configuration (API URLs, file paths) |
| `CitizensDatastore.csv` | Data persistence in CSV format |

---

## 🛠️ Technology Stack

| Technology | Version | Purpose |
|-----------|---------|---------|
| **.NET** | 10.0 | Runtime framework |
| **ASP.NET Core** | 10.0 | Web framework |
| **Serilog** | 4.3.2 | Structured logging |
| **Swashbuckle** | 10.1.5 | Swagger/OpenAPI documentation |
| **Newtonsoft.Json** | 13.0.3 | JSON serialization |
| **Git** | Latest | Version control |

---

## Author Notes
This implementation demonstrates professional software engineering practices including:
- Clean code architecture
- External configuration management
- Comprehensive logging
- RESTful API design
- Version control best practices
- 12 Factor App compliance

---

---

## Environment Variables

For sensitive configuration, use environment variables:

```bash
# Optional: Set external API URL
$env:ExternalApi__ObjectsUrl = "https://api.restful-api.dev/objects"

# Optional: Set data file path
$env:Data__CitizensFile = "CitizensDatastore.csv"

# Optional: Set log level
$env:Logging__LogLevel__Default = "Information"
```

---

## Troubleshooting

### Port Already in Use
If port 5000 is already in use:
1. Find the process: `netstat -ano | findstr :5000`
2. Kill it: `taskkill /PID <PID> /F`
3. Or change the port in `launchSettings.json`

### CSV File Locked
If CSV file is locked during operations:
1. Ensure only one instance is running
2. Close any spreadsheet applications with the CSV open
3. Rebuild the project to reset state

### Swagger Not Loading
1. Verify application is running: `curl http://localhost:5000/swagger`
2. Check browser console for errors
3. Clear browser cache and reload

### External API Unreachable
1. Check internet connection
2. Verify API is online: `curl https://api.restful-api.dev/objects`
3. Check logs in `logs/` folder for detailed error messages

---

## Configuration Files

### appsettings.json (Shared)
```json
{
  "Data": {
    "CitizensFile": "CitizensDatastore.csv"
  },
  "ExternalApi": {
    "ObjectsUrl": "https://api.restful-api.dev/objects"
  },
  "Serilog": {
    "MinimumLevel": "Debug"
  }
}
```

### appsettings.Development.json (Development Only)
```json
{
  "Logging": {
    "LogLevel": {"Default": "Debug"}
  },
  "Serilog": {
    "WriteTo": [
      {"Name": "Console"},
      {"Name": "File", "Args": {"path": "logs/citizen-registry-.log"}}
    ]
  }
}
```

---

## Performance Considerations

### CSV Performance
- ✅ Fine for < 10,000 records
- For larger datasets, consider migrating to SQL Server
- Current implementation: O(n) for all operations

### External API Calls
- Async/await could improve throughput
- Current: Blocking calls per request
- Rate limiting: API allows 60 requests/minute

### Logging
- Serilog configured with rolling files (daily)
- Logs auto-rotate to prevent disk space issues
- Default retention: 7 days

---

## Next Steps for Production

1. **Migrate from CSV to Database**
   - Use Entity Framework Core
   - SQL Server or PostgreSQL

2. **Implement Async/Await**
   - Non-blocking I/O
   - Better scalability

3. **Add Authentication**
   - JWT Bearer tokens
   - Role-based access control

4. **Add Unit Tests**
   - xUnit test framework
   - Mock external services

5. **Docker Containerization**
   - Dockerfile configuration
   - Deploy to Kubernetes

6. **CI/CD Pipeline**
   - GitHub Actions for automated testing
   - Automatic deployment on push

---

## 📝 Author Notes

This implementation demonstrates professional software engineering practices:

✅ **Clean Architecture**
- Separation of concerns (Controllers, Services, Models)
- Dependency injection for loose coupling
- Async-ready service design

✅ **External Configuration Management**
- Environment-specific settings (`appsettings.json`, `appsettings.Development.json`)
- No hardcoded values in source code
- Support for environment variables

✅ **Comprehensive Logging**
- Structured logging with Serilog
- Multiple outputs: console and rolling files
- Configurable log levels by environment

✅ **RESTful API Design**
- Proper HTTP methods and status codes
- Meaningful error messages
- API documentation with Swagger

✅ **Version Control Best Practices**
- Git Flow branching model
- Meaningful commit messages with `P2-001` prefix
- Clean commit history with multiple focused commits

✅ **12 Factor App Compliance**
- All 12 factors documented and implemented
- Stateless application architecture
- Data persistence external to application

---

## 📊 Changelog

### Version 1.0 (Current - March 17, 2026)
**Status:** ✅ Production Ready

**Features Added:**
- Complete CRUD API for Citizens
- External API integration for asset assignment
- CSV-based data persistence
- Serilog structured logging
- Swagger UI documentation
- Git Flow implementation
- Full 12 Factor App compliance
- Comprehensive README documentation

**Git Features:**
- 6 focused commits organized by feature
- Proper merge commits (--no-ff) for branch tracking
- All branches: main, develop, P2-001
- Improved .gitignore configuration

**Known Limitations & Future Improvements:**
- CSV-based storage limited to ~10,000 records
- No async/await implementation (could improve concurrency)
- No authentication/authorization layer
- No database (uses CSV only)
- No unit tests (recommended to add)
- Single-process deployment (no containerization)

---

## 🔗 Quick Links

| Link | Description |
|------|-------------|
| [GitHub Repository](https://github.com/Suikasama123/Cert-1-pract-2) | Entire project source code |
| [12 Factor App Guide](https://12factor.net) | Reference for methodology |
| [ASP.NET Core Docs](https://docs.microsoft.com/dotnet/core/) | Official Microsoft documentation |
| [Serilog Documentation](https://serilog.net) | Logging framework docs |
| [RESTful API Design](https://restfulapi.net) | Best practices guide |

---

## 📞 Support & Contact

**For questions or issues:**
- 📧 Email: danielochoa1@upb.edu
- 🐙 GitHub: [@Suikasama123](https://github.com/Suikasama123)
- 📚 UPB: Universidad Privada Boliviana

---

**Last Updated:** March 17, 2026  
**Version:** 1.0 (Production Release)  
**Status:** ✅ Complete and fully functional  
**License:** Educational Use