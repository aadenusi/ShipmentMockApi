# Quick Start: Configuration-Based Tests

## ✅ What Changed

The base URL `http://localhost:5024` is now stored in `appsettings.json` instead of being hardcoded in feature files.

## 📋 New Files

```
ShipmentMockApi.IntegrationTests/
├── appsettings.json                    ← Main config (localhost)
├── appsettings.Development.json        ← Dev environment
├── appsettings.Staging.json            ← Staging environment
└── Configuration/
    └── TestConfiguration.cs            ← Config helper class
```

## 🎯 How to Use

### Option 1: Use Configured URL (Recommended)
```gherkin
Given the API is running at base URL "{configured}"
```
→ Reads from `appsettings.json` → `http://localhost:5024`

### Option 2: Override with Explicit URL (Still Works)
```gherkin
Given the API is running at base URL "http://custom-server:8080"
```
→ Uses the explicit URL you provide

## 🔧 Change the Base URL

Edit `ShipmentMockApi.IntegrationTests/appsettings.json`:
```json
{
  "ApiSettings": {
    "BaseUrl": "http://your-new-url:port"
  }
}
```

## 🌍 Different Environments

### Development (default)
```bash
dotnet test  # Uses appsettings.Development.json
```

### Staging
```bash
$env:ASPNETCORE_ENVIRONMENT="Staging"
dotnet test  # Uses appsettings.Staging.json
```

### Production
Create `appsettings.Production.json` and run:
```bash
$env:ASPNETCORE_ENVIRONMENT="Production"
dotnet test
```

## 📝 Feature File Example

**Before**:
```gherkin
Scenario: Get shipment by valid ID
  Given the API is running at base URL "http://localhost:5024"
  When I request shipment with ID "SHP-20250210-0042"
```

**After**:
```gherkin
Scenario: Get shipment by valid ID
  Given the API is running at base URL "{configured}"
  When I request shipment with ID "SHP-20250210-0042"
```

## ✅ Status

- ✅ Configuration files created
- ✅ Configuration helper implemented
- ✅ Step definitions updated
- ✅ Feature file updated
- ✅ Build successful
- ✅ All configs copied to output directory

## 🚀 Ready to Use!

Your tests now use the centralized configuration. Change the URL in one place (`appsettings.json`) and all tests will use the new value.
