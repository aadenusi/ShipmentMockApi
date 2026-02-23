# Configuration-Based Test Setup

## ✅ What Was Implemented

The base URL and other test settings are now externalized to configuration files instead of being hardcoded in the feature files.

## 📁 Files Created

### 1. Configuration Files

**`ShipmentMockApi.IntegrationTests/appsettings.json`** (Main Config)
```json
{
  "ApiSettings": {
    "BaseUrl": "http://localhost:5024"
  },
  "TestSettings": {
    "DefaultTimeout": 30,
    "RetryAttempts": 3
  }
}
```

**`ShipmentMockApi.IntegrationTests/appsettings.Development.json`**
- Development-specific settings
- Overrides base settings when running locally

**`ShipmentMockApi.IntegrationTests/appsettings.Staging.json`**
- Staging environment settings
- Example: `"BaseUrl": "https://api-staging.example.com"`

### 2. Configuration Helper Class

**`ShipmentMockApi.IntegrationTests/Configuration/TestConfiguration.cs`**
- Singleton pattern for configuration access
- Reads from `appsettings.json`
- Provides strongly-typed properties:
  - `BaseUrl` - API base URL
  - `DefaultTimeout` - HTTP timeout in seconds
  - `RetryAttempts` - Number of retry attempts

## 🔧 How It Works

### Before (Hardcoded):
```gherkin
Given the API is running at base URL "http://localhost:5024"
```

### After (Configuration-based):
```gherkin
Given the API is running at base URL "{configured}"
```

The step definition automatically reads from `appsettings.json`:

```csharp
[Given(@"the API is running at base URL ""(.*)""")]
public void GivenTheAPIIsRunningAtBaseURL(string baseUrl)
{
    // Use configured base URL if placeholder is used
    if (string.IsNullOrWhiteSpace(baseUrl) || baseUrl == "{configured}")
    {
        baseUrl = TestConfiguration.Instance.BaseUrl;
    }
    
    _httpClient.BaseAddress = new Uri(baseUrl);
}
```

## 🎯 Benefits

1. **Environment-specific testing** - Different configs for Dev/Staging/Prod
2. **No code changes** - Change URL without modifying feature files
3. **CI/CD friendly** - Override settings via environment-specific config files
4. **Version control safe** - Sensitive URLs can be kept out of git
5. **Centralized settings** - All test configuration in one place

## 🚀 Usage

### Running Tests Locally (Development)
```powershell
# Uses appsettings.Development.json (http://localhost:5024)
dotnet test ShipmentMockApi.IntegrationTests
```

### Running Tests Against Staging
```powershell
# Create appsettings.Staging.json with staging URL
# Set environment variable
$env:ASPNETCORE_ENVIRONMENT="Staging"
dotnet test ShipmentMockApi.IntegrationTests
```

### Override URL at Runtime (Still Supported)
You can still specify explicit URLs in the feature file:
```gherkin
Given the API is running at base URL "http://custom-server:8080"
```

## 📝 Configuration Reference

### appsettings.json Structure

```json
{
  "ApiSettings": {
    "BaseUrl": "http://localhost:5024"  // API base URL
  },
  "TestSettings": {
    "DefaultTimeout": 30,               // HTTP timeout in seconds
    "RetryAttempts": 3                  // Number of retries on failure
  }
}
```

### Accessing Configuration in Code

```csharp
using ShipmentMockApi.IntegrationTests.Configuration;

// Get base URL
var baseUrl = TestConfiguration.Instance.BaseUrl;

// Get timeout
var timeout = TestConfiguration.Instance.DefaultTimeout;

// Get retry attempts
var retries = TestConfiguration.Instance.RetryAttempts;
```

## 🔐 Security Best Practices

### For Production/Staging URLs:

1. **Don't commit sensitive URLs** - Add to `.gitignore`:
```gitignore
appsettings.Production.json
appsettings.Staging.json
```

2. **Use environment variables** in CI/CD:
```json
{
  "ApiSettings": {
    "BaseUrl": "${API_BASE_URL}"
  }
}
```

3. **Template files** - Commit template, not actual values:
```json
// appsettings.Production.template.json
{
  "ApiSettings": {
    "BaseUrl": "REPLACE_WITH_PRODUCTION_URL"
  }
}
```

## 🧪 Example: Multiple Environments

### Local Development
```json
// appsettings.Development.json
{
  "ApiSettings": {
    "BaseUrl": "http://localhost:5024"
  }
}
```

### Docker Compose
```json
// appsettings.Docker.json
{
  "ApiSettings": {
    "BaseUrl": "http://shipment-api:5024"
  }
}
```

### Staging
```json
// appsettings.Staging.json
{
  "ApiSettings": {
    "BaseUrl": "https://staging-api.company.com"
  }
}
```

### Production
```json
// appsettings.Production.json
{
  "ApiSettings": {
    "BaseUrl": "https://api.company.com"
  }
}
```

## ✅ Updated Files

1. ✅ `ShipmentMockApi.IntegrationTests.csproj` - Added configuration packages
2. ✅ `ShipmentApiStepDefinitions.cs` - Reads from config
3. ✅ `ShipmentApi.feature` - Uses `{configured}` placeholder
4. ✅ Created `TestConfiguration.cs` - Configuration helper
5. ✅ Created `appsettings.json` - Main config file

## 🎉 Result

All feature file scenarios now use the configured base URL from `appsettings.json`:

```gherkin
Feature: Shipment API Integration Tests

  Scenario: Get shipment by valid ID returns correct schema
    Given the API is running at base URL "{configured}"
    # Automatically uses http://localhost:5024 from config
```

No more hardcoded URLs! 🚀
