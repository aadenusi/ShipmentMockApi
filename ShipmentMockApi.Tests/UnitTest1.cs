using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;
using ShipmentMockApi.Models;

namespace ShipmentMockApi.Tests;

public class ShipmentApiTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public ShipmentApiTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetShipment_ValidId_ReturnsOkWithCorrectSchema()
    {
        // Act
        var response = await _client.GetAsync("/api/shipments/SHP-20250210-0042");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var shipment = await response.Content.ReadFromJsonAsync<ShipmentContract>();
        Assert.NotNull(shipment);
        Assert.Equal("SHP-20250210-0042", shipment.ShipmentId);
        Assert.Equal("in_transit", shipment.Status);
        Assert.Equal("MAEU", shipment.Carrier);
        Assert.Equal("MSKU1234567", shipment.ContainerId);
    }

    [Fact]
    public async Task GetShipment_InvalidId_Returns404()
    {
        // Act
        var response = await _client.GetAsync("/api/shipments/INVALID-ID");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetCarrierEvents_ReturnsOk()
    {
        // Act
        var response = await _client.GetAsync("/api/shipments/events/carrier");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var content = await response.Content.ReadAsStringAsync();
        Assert.Contains("carrier_cdc", content);
    }

    [Fact]
    public async Task GetEdgeEvents_ReturnsOk()
    {
        // Act
        var response = await _client.GetAsync("/api/shipments/events/edge");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var content = await response.Content.ReadAsStringAsync();
        Assert.Contains("edge_sensor", content);
    }

    [Fact]
    public async Task GetEmailEvents_ReturnsOk()
    {
        // Act
        var response = await _client.GetAsync("/api/shipments/events/email");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var content = await response.Content.ReadAsStringAsync();
        Assert.Contains("email_parser", content);
    }
}
