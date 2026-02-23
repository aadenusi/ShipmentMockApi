# Comprehensive Integration Tests for Shipment Mock API
# Run this script while the API is running on http://localhost:5024

$baseUrl = "http://localhost:5024"
$testsPassed = 0
$testsFailed = 0
$testsTotal = 0

function Test-Scenario {
    param(
        [string]$Name,
        [scriptblock]$TestBlock
    )
    
    $script:testsTotal++
    Write-Host "`n[$script:testsTotal] $Name" -ForegroundColor Yellow
    
    try {
        & $TestBlock
        $script:testsPassed++
        Write-Host "  ✓ PASSED" -ForegroundColor Green
    }
    catch {
        $script:testsFailed++
        Write-Host "  ✗ FAILED: $($_.Exception.Message)" -ForegroundColor Red
    }
}

function Assert-Equal {
    param($Actual, $Expected, $Message)
    if ($Actual -ne $Expected) {
        throw "$Message - Expected: $Expected, Actual: $Actual"
    }
    Write-Host "  ✓ $Message" -ForegroundColor Green
}

function Assert-NotNull {
    param($Value, $Message)
    if ($null -eq $Value) {
        throw "$Message - Value is null"
    }
    Write-Host "  ✓ $Message" -ForegroundColor Green
}

Write-Host "`n╔═══════════════════════════════════════════════════════════╗" -ForegroundColor Cyan
Write-Host "║   Shipment Mock API - Integration Tests                 ║" -ForegroundColor Cyan
Write-Host "╚═══════════════════════════════════════════════════════════╝" -ForegroundColor Cyan
Write-Host "Base URL: $baseUrl`n" -ForegroundColor Gray

# ═══════════════════════════════════════════════════════════════════
# Scenario 1: Get shipment by valid ID returns correct schema
# ═══════════════════════════════════════════════════════════════════
Test-Scenario "Get shipment by valid ID returns correct schema" {
    $shipmentId = "SHP-20250210-0042"
    $response = Invoke-WebRequest -Uri "$baseUrl/api/shipments/$shipmentId" -Method GET -ErrorAction Stop
    $shipment = $response.Content | ConvertFrom-Json
    
    Assert-Equal $response.StatusCode 200 "Status code is 200"
    Assert-NotNull $shipment "Response contains data"
    Assert-NotNull $shipment.shipment_id "shipment_id exists"
    Assert-NotNull $shipment.status "status exists"
    Assert-NotNull $shipment.origin "origin exists"
    Assert-NotNull $shipment.destination "destination exists"
    Assert-NotNull $shipment.carrier "carrier exists"
    Assert-NotNull $shipment.container_id "container_id exists"
    Assert-NotNull $shipment.events "events exists"
    Assert-NotNull $shipment.eta "eta exists"
    Assert-NotNull $shipment.last_updated "last_updated exists"
    
    Assert-Equal $shipment.shipment_id "SHP-20250210-0042" "shipment_id matches"
    Assert-Equal $shipment.status "in_transit" "status is in_transit"
    Assert-Equal $shipment.carrier "MAEU" "carrier is MAEU"
    Assert-Equal $shipment.container_id "MSKU1234567" "container_id matches"
}

# ═══════════════════════════════════════════════════════════════════
# Scenario 2: Get shipment with invalid ID returns 404
# ═══════════════════════════════════════════════════════════════════
Test-Scenario "Get shipment with invalid ID returns 404" {
    try {
        $response = Invoke-WebRequest -Uri "$baseUrl/api/shipments/INVALID-ID" -Method GET -ErrorAction Stop
        throw "Expected 404 but got $($response.StatusCode)"
    }
    catch {
        if ($_.Exception.Response.StatusCode -eq 404) {
            Write-Host "  ✓ Status code is 404" -ForegroundColor Green
        }
        else {
            throw "Expected 404, got $($_.Exception.Response.StatusCode)"
        }
    }
}

# ═══════════════════════════════════════════════════════════════════
# Scenario 3: Get carrier CDC events returns results (Data-Driven)
# ═══════════════════════════════════════════════════════════════════
Test-Scenario "Get carrier CDC events - Example 1" {
    $url = "/api/shipments/events/carrier"
    $expected = @{
        Source = "carrier_cdc"
        ShipmentId = "SHP-20250210-0042"
        Status = "in_transit"
        Timestamp = "2025-02-08T14:30:00Z"
        Port = "CNSHA"
    }
    
    $response = Invoke-WebRequest -Uri "$baseUrl$url" -Method GET -ErrorAction Stop
    $event = $response.Content | ConvertFrom-Json
    
    Assert-Equal $response.StatusCode 200 "Status code is 200"
    Assert-Equal $event.source $expected.Source "source is $($expected.Source)"
    Assert-Equal $event.shipment_id $expected.ShipmentId "shipment_id is $($expected.ShipmentId)"
    Assert-Equal $event.status $expected.Status "status is $($expected.Status)"
    Assert-Equal $event.timestamp $expected.Timestamp "timestamp is $($expected.Timestamp)"
    Assert-Equal $event.port $expected.Port "port is $($expected.Port)"
}

# ═══════════════════════════════════════════════════════════════════
# Scenario 4: Get edge sensor events returns results
# ═══════════════════════════════════════════════════════════════════
Test-Scenario "Get edge sensor events returns results" {
    $response = Invoke-WebRequest -Uri "$baseUrl/api/shipments/events/edge" -Method GET -ErrorAction Stop
    $event = $response.Content | ConvertFrom-Json
    
    Assert-Equal $response.StatusCode 200 "Status code is 200"
    Assert-NotNull $event "Response contains data"
    Assert-NotNull $event.device_id "device_id exists"
    Assert-NotNull $event.container_id "container_id exists"
    Assert-NotNull $event.temperature "temperature exists"
    Assert-Equal $event.source "edge_sensor" "source is edge_sensor"
    
    # Verify temperature is a number
    if ($event.temperature -is [double] -or $event.temperature -is [int]) {
        Write-Host "  ✓ temperature is numeric" -ForegroundColor Green
    } else {
        throw "temperature is not a number"
    }
}

# ═══════════════════════════════════════════════════════════════════
# Scenario 5: Get email parser events returns results
# ═══════════════════════════════════════════════════════════════════
Test-Scenario "Get email parser events returns results" {
    $response = Invoke-WebRequest -Uri "$baseUrl/api/shipments/events/email" -Method GET -ErrorAction Stop
    $event = $response.Content | ConvertFrom-Json
    
    Assert-Equal $response.StatusCode 200 "Status code is 200"
    Assert-NotNull $event "Response contains data"
    Assert-NotNull $event.shipment_ref "shipment_ref exists"
    Assert-Equal $event.source "email_parser" "source is email_parser"
}

# ═══════════════════════════════════════════════════════════════════
# Test Summary
# ═══════════════════════════════════════════════════════════════════
Write-Host "`n╔═══════════════════════════════════════════════════════════╗" -ForegroundColor Cyan
Write-Host "║                    Test Summary                          ║" -ForegroundColor Cyan
Write-Host "╚═══════════════════════════════════════════════════════════╝" -ForegroundColor Cyan
Write-Host "Total:  $testsTotal" -ForegroundColor Gray
Write-Host "Passed: $testsPassed" -ForegroundColor Green
Write-Host "Failed: $testsFailed" -ForegroundColor $(if ($testsFailed -eq 0) { "Green" } else { "Red" })
Write-Host ""

if ($testsFailed -eq 0) {
    Write-Host "✓ All tests passed!" -ForegroundColor Green
    Write-Host "Coverage: 5/5 scenarios from ShipmentApi.feature" -ForegroundColor Gray
    exit 0
} else {
    Write-Host "✗ Some tests failed" -ForegroundColor Red
    exit 1
}
