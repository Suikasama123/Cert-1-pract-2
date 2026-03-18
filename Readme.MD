# Citizen Registry API - Web API with Git Flow, CRUD and 12 Factor App

## Author
**Name:** Daniel Ochoa  
**GitHub:** Suikasama123  
**Email:** danielochoa1@upb.edu  

## Overview
This is a .NET Core Web API for managing citizens in a city registry system. The application implements RESTful CRUD operations, integrates with an external API for asset assignment, and follows the 12 Factor App methodology.

---

## Project Features

- **CRUD Operations**: Create, Read, Update, and Delete citizens
- **External API Integration**: Retrieves personal assets from `https://api.restful-api.dev/objects`
- **CSV Persistence**: Stores citizen data in CSV format
- **Swagger UI**: Interactive API documentation and testing
- **Structured Logging**: Comprehensive logging using Serilog
- **Error Handling**: Proper HTTP status codes and error responses
- **Git Flow**: Professional branching model (main, develop, P2-001)

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

## API Endpoints

### Get All Citizens
```http
GET /api/citizens
```

### Get Citizen by CI
```http
GET /api/citizens/{ci}
```

### Create Citizen
```http
POST /api/citizens
Content-Type: application/json

{
  "firstName": "John",
  "lastName": "Doe",
  "ci": "123456"
}
```

Response:
```json
{
  "firstName": "John",
  "lastName": "Doe",
  "ci": "123456",
  "bloodGroup": "O+",
  "personalAsset": "Apple MacBook Pro"
}
```

### Update Citizen
```http
PUT /api/citizens/{ci}
Content-Type: application/json

{
  "firstName": "Jane",
  "lastName": "Doe"
}
```

### Delete Citizen
```http
DELETE /api/citizens/{ci}
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

## Running the Application

### Prerequisites
- .NET 10.0 SDK
- Visual Studio or VS Code

### Build
```bash
dotnet build
```

### Run
```bash
dotnet run
```

### Access Swagger UI
Navigate to: `http://localhost:5000/swagger/ui`

---

## Git Flow Workflow

### Main Branches
- **main**: Production-ready code
- **develop**: Integration branch for features
- **P2-001**: Feature branch for this practice

### Commit Naming Convention
```
P2-001 Add CitizenController
P2-001 Implemented Create Citizen endpoint
P2-001 Adding CSV persistence
P2-001 Integrated external API
```

### Merging Strategy
1. Complete feature on `P2-001`
2. Merge `P2-001` → `develop`
3. Merge `develop` → `main` (release)

---

## Project Structure
```
DogShelter/
├── Controllers/
│   └── CitizensController.cs
├── Models/
│   ├── Citizen.cs
│   └── Response.cs
├── Services/
│   ├── CitizenService.cs
│   ├── ExternalAssetService.cs
├── Utils/
│   └── CSVHelper.cs
├── appsettings.json
├── appsettings.Development.json
├── Program.cs
├── DogShelter.csproj
└── CitizensDatastore.csv
```

---

## Technology Stack
- **.NET Framework**: ASP.NET Core 10.0
- **API Documentation**: Swagger/Swashbuckle
- **Logging**: Serilog
- **Data Persistence**: CSV files
- **Version Control**: Git with Git Flow

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

**Last Updated**: March 17, 2026

## 3. Configuracion
- Use appsettings.json como configuracion general de la aplicacion
- Use appsettings.development.json para la configuracion en ambiente DEVELOPMENT
- Use la variable de entorno....

## 12. Admin Proces
- Que es este documento...

## x. Process
- Async / Await