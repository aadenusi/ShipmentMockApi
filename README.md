# Shipment Mock API

## Overview
This is a C# ASP.NET Core Web API that provides mock shipment tracking data with events from multiple sources.

## Contract
The API returns shipment data following this contract:
```json
{
  "shipment_id": "SHP-20250210-0042",
  "status": "in_transit",
  "origin": {"port": "CNSHA", "country": "CN"},
  "destination": {"port": "GBFXT", "country": "GB"},
  "carrier": "MAEU",
  "container_id": "MSKU1234567",
  "events": [...],
  "eta": "2025-03-15T08:00:00Z",
  "last_updated": "2025-02-10T09:15:00Z"
}
```n
## Step-by-Step Instructions to Run

### 1. Build the Project
```powershell
dotnet build
```n
### 2. Run the API

**Option A: HTTP (default)**
```powershell
dotnet run
```n
The API will start on **http://localhost:5024**

**Option B: HTTPS**
```powershell
dotnet run --launch-profile https
```n
The API will start on **https://localhost:7084** (and http://localhost:5024)

> **Note:** Make sure to use the correct protocol (http:// or https://) based on which profile you're using!

### 3. Test the Endpoints

> **Important:** Replace `http://localhost:5024` with `https://localhost:7084` if you're running with the HTTPS profile.

#### Check API is Running
```powershell
curl http://localhost:5024/
```n
Or open in browser: http://localhost:5024/

This will show all available endpoints.

#### Get Shipment by ID
```powershell
curl http://localhost:5024/api/shipments/SHP-20250210-0042
```n
Or open in browser: http://localhost:5024/api/shipments/SHP-20250210-0042

#### Get Carrier CDC Events
```powershell
curl http://localhost:5024/api/shipments/events/carrier
```n
#### Get Edge Sensor Events
```powershell
curl http://localhost:5024/api/shipments/events/edge
```n
#### Get Email Parser Events
```powershell
curl http://localhost:5024/api/shipments/events/email
```
## Available Endpoints

- GET /api/shipments/{shipment_id} - Get shipment details by ID
- GET /api/shipments/events/carrier - Get carrier CDC events
- GET /api/shipments/events/edge - Get edge sensor events
- GET /api/shipments/events/email - Get email parser events

## Mock Data Sources

The API simulates events from three different sources:

1. **Carrier CDC Event**
   - Source: carrier_cdc
   - Contains: shipment_id, status, timestamp, port

2. **Edge Sensor Event**
   - Source: edge_sensor
   - Contains: device_id, container_id, timestamp, location (lat/lon), temperature

3. **Email Parser Event**
   - Source: email_parser
   - Contains: shipment_ref, status, notes
   - May have null timestamp (malformed data simulation)

## Quick Start Script

Create and run the API in one command:
```powershell
dotnet build && dotnet run
```n