using NUnit.Framework;
using Reqnroll;
using ShipmentMockApi.IntegrationTests.Configuration;
using ShipmentMockApi.Models;
using System.Buffers.Text;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;

namespace ShipmentMockApi.IntegrationTests.Steps;

[Binding]
public class ShipmentApiStepDefinitions : IDisposable
{
    private HttpClient _httpClient = null!;
    private HttpResponseMessage? _response;
    private ShipmentContract? _shipmentResponse;
    private JsonElement? _jsonResponse;

    [BeforeScenario]
    public void Setup()
    {
        _httpClient = new HttpClient();
    }

    [Given("the API is running at base URL")]
    public void GivenTheAPIIsRunningAtBaseURL()
    {
        var baseUrl = TestConfiguration.Instance.BaseUrl;
        _httpClient.BaseAddress = new Uri(baseUrl);
    }


    [When(@"I request shipment with ID ""(.*)""")]
    public async Task WhenIRequestShipmentWithID(string shipmentId)
    {
        _response = await _httpClient.GetAsync($"/api/shipments/{shipmentId}");
    }
 
    [When("I request the url {string}")]
    public async Task WhenIRequestTheUrl(string url)
    {
        _response = await _httpClient.GetAsync(url);
    }


    [When(@"I request carrier CDC events")]
    public async Task WhenIRequestCarrierCDCEvents()
    {
        _response = await _httpClient.GetAsync("/api/shipments/events/carrier");
    }
    [When(@"I request edge sensor events")]
    public async Task WhenIRequestEdgeSensorEvents()
    {
        _response = await _httpClient.GetAsync("/api/shipments/events/edge");
    }

    [When(@"I request email parser events")]
    public async Task WhenIRequestEmailParserEvents()
    {
        _response = await _httpClient.GetAsync("/api/shipments/events/email");
    }

    [Then(@"the response status code should be (.*)")]
    public async Task ThenTheResponseStatusCodeShouldBe(int statusCode)
    {
        Assert.That((int)_response!.StatusCode, Is.EqualTo(statusCode));

        // If response is 200, parse JSON for later assertions
        if (statusCode == 200 && _response.Content.Headers.ContentType?.MediaType == "application/json")
        {
            var json = await _response.Content.ReadAsStringAsync();
            _jsonResponse = JsonSerializer.Deserialize<JsonElement>(json);
        }
    }

    [Then(@"the response should match the ShipmentContract schema")]
    public async Task ThenTheResponseShouldMatchTheShipmentContractSchema()
    {
        _shipmentResponse = await _response!.Content.ReadFromJsonAsync<ShipmentContract>();

        Assert.That(_shipmentResponse, Is.Not.Null);
        Assert.That(_shipmentResponse!.ShipmentId, Is.Not.Null);
        Assert.That(_shipmentResponse.Status, Is.Not.Null);
        Assert.That(_shipmentResponse.Origin, Is.Not.Null);
        Assert.That(_shipmentResponse.Destination, Is.Not.Null);
        Assert.That(_shipmentResponse.Carrier, Is.Not.Null);
        Assert.That(_shipmentResponse.ContainerId, Is.Not.Null);
        Assert.That(_shipmentResponse.Events, Is.Not.Null);
        Assert.That(_shipmentResponse.Eta, Is.Not.EqualTo(DateTime.MinValue));
        Assert.That(_shipmentResponse.LastUpdated, Is.Not.EqualTo(DateTime.MinValue));
    }

    [Then(@"the shipment field ""(.*)"" should be ""(.*)""")]
    public void ThenTheShipmentFieldShouldBe(string fieldName, string expectedValue)
    {
        Assert.That(_shipmentResponse, Is.Not.Null);

        var propertyValue = fieldName switch
        {
            "shipment_id" => _shipmentResponse!.ShipmentId,
            "status" => _shipmentResponse!.Status,
            "carrier" => _shipmentResponse!.Carrier,
            "container_id" => _shipmentResponse!.ContainerId,
            _ => throw new ArgumentException($"Unknown field: {fieldName}")
        };

        Assert.That(propertyValue, Is.EqualTo(expectedValue));
    }

    [Then(@"the response should contain carrier event data")]
    public async Task ThenTheResponseShouldContainCarrierEventData()
    {
        var json = await _response!.Content.ReadAsStringAsync();
        _jsonResponse = JsonSerializer.Deserialize<JsonElement>(json);

        Assert.That(_jsonResponse.HasValue, Is.True);
    }

    [Then(@"the carrier event should have a shipment_id")]
    public void ThenTheCarrierEventShouldHaveAShipmentId()
    {
        Assert.That(_jsonResponse!.Value.TryGetProperty("shipment_id", out var shipmentId), Is.True);
        Assert.That(shipmentId.GetString(), Is.Not.Null.And.Not.Empty);
    }

    [Then(@"the carrier event should have a status")]
    public void ThenTheCarrierEventShouldHaveAStatus()
    {
        Assert.That(_jsonResponse!.Value.TryGetProperty("status", out var status), Is.True);
        Assert.That(status.GetString(), Is.Not.Null.And.Not.Empty);
    }

    [Then(@"the carrier event source should be ""(.*)""")]
    public void ThenTheCarrierEventSourceShouldBe(string expectedSource)
    {
        // Handle array response - get first element
        var element = _jsonResponse!.Value.ValueKind == JsonValueKind.Array 
            ? _jsonResponse.Value[0] 
            : _jsonResponse.Value;

        Assert.That(element.TryGetProperty("source", out var source), Is.True);
        Assert.That(source.GetString(), Is.EqualTo(expectedSource));
    }

    [Then(@"the carrier event shipment_id should be ""(.*)""")]
    public void ThenTheCarrierEventShipmentIdShouldBe(string expectedShipmentId)
    {
        // Handle array response - get first element
        var element = _jsonResponse!.Value.ValueKind == JsonValueKind.Array 
            ? _jsonResponse.Value[0] 
            : _jsonResponse.Value;

        Assert.That(element.TryGetProperty("shipment_id", out var shipmentId), Is.True);
        Assert.That(shipmentId.GetString(), Is.EqualTo(expectedShipmentId));
    }

    [Then(@"the carrier event status should be ""(.*)""")]
    public void ThenTheCarrierEventStatusShouldBe(string expectedStatus)
    {
        // Handle array response - get first element
        var element = _jsonResponse!.Value.ValueKind == JsonValueKind.Array 
            ? _jsonResponse.Value[0] 
            : _jsonResponse.Value;

        Assert.That(element.TryGetProperty("status", out var status), Is.True);
        Assert.That(status.GetString(), Is.EqualTo(expectedStatus));
    }

    [Then(@"the carrier event timestamp should be ""(.*)""")]
    public void ThenTheCarrierEventTimestampShouldBe(string expectedTimestamp)
    {
        // Handle array response - get first element
        var element = _jsonResponse!.Value.ValueKind == JsonValueKind.Array 
            ? _jsonResponse.Value[0] 
            : _jsonResponse.Value;

        Assert.That(element.TryGetProperty("timestamp", out var timestamp), Is.True);
        Assert.That(timestamp.GetString(), Is.EqualTo(expectedTimestamp));
    }

    [Then(@"the carrier event port should be ""(.*)""")]
    public void ThenTheCarrierEventPortShouldBe(string expectedPort)
    {
        // Handle array response - get first element
        var element = _jsonResponse!.Value.ValueKind == JsonValueKind.Array 
            ? _jsonResponse.Value[0] 
            : _jsonResponse.Value;

        Assert.That(element.TryGetProperty("port", out var port), Is.True);
        Assert.That(port.GetString(), Is.EqualTo(expectedPort));
    }

    [Then(@"the response should contain edge sensor event data")]
    public async Task ThenTheResponseShouldContainEdgeSensorEventData()
    {
        var json = await _response!.Content.ReadAsStringAsync();
        _jsonResponse = JsonSerializer.Deserialize<JsonElement>(json);

        Assert.That(_jsonResponse.HasValue, Is.True);
    }

    [Then(@"the edge event should have a device_id")]
    public void ThenTheEdgeEventShouldHaveADeviceId()
    {
        // Handle array response - get first element
        var element = _jsonResponse!.Value.ValueKind == JsonValueKind.Array 
            ? _jsonResponse.Value[0] 
            : _jsonResponse.Value;

        Assert.That(element.TryGetProperty("device_id", out var deviceId), Is.True);
        Assert.That(deviceId.GetString(), Is.Not.Null.And.Not.Empty);
    }

    [Then(@"the edge event should have a container_id")]
    public void ThenTheEdgeEventShouldHaveAContainerId()
    {
        // Handle array response - get first element
        var element = _jsonResponse!.Value.ValueKind == JsonValueKind.Array 
            ? _jsonResponse.Value[0] 
            : _jsonResponse.Value;

        Assert.That(element.TryGetProperty("container_id", out var containerId), Is.True);
        Assert.That(containerId.GetString(), Is.Not.Null.And.Not.Empty);
    }

    [Then(@"the edge event should have temperature data")]
    public void ThenTheEdgeEventShouldHaveTemperatureData()
    {
        // Handle array response - get first element
        var element = _jsonResponse!.Value.ValueKind == JsonValueKind.Array 
            ? _jsonResponse.Value[0] 
            : _jsonResponse.Value;

        Assert.That(element.TryGetProperty("temperature", out var temperature), Is.True);
        Assert.That(temperature.ValueKind, Is.EqualTo(JsonValueKind.Number));
    }

    [Then(@"the edge event source should be ""(.*)""")]
    public void ThenTheEdgeEventSourceShouldBe(string expectedSource)
    {
        // Handle array response - get first element
        var element = _jsonResponse!.Value.ValueKind == JsonValueKind.Array 
            ? _jsonResponse.Value[0] 
            : _jsonResponse.Value;

        Assert.That(element.TryGetProperty("source", out var source), Is.True);
        Assert.That(source.GetString(), Is.EqualTo(expectedSource));
    }

    [Then(@"the response should contain email parser event data")]
    public async Task ThenTheResponseShouldContainEmailParserEventData()
    {
        var json = await _response!.Content.ReadAsStringAsync();
        _jsonResponse = JsonSerializer.Deserialize<JsonElement>(json);

        Assert.That(_jsonResponse.HasValue, Is.True);
    }

    [Then(@"the email event should have a shipment_ref")]
    public void ThenTheEmailEventShouldHaveAShipmentRef()
    {
        // Handle array response - get first element
        var element = _jsonResponse!.Value.ValueKind == JsonValueKind.Array 
            ? _jsonResponse.Value[0] 
            : _jsonResponse.Value;

        Assert.That(element.TryGetProperty("shipment_ref", out var shipmentRef), Is.True);
        Assert.That(shipmentRef.GetString(), Is.Not.Null.And.Not.Empty);
    }

    [Then(@"the email event source should be ""(.*)""")]
    public void ThenTheEmailEventSourceShouldBe(string expectedSource)
    {
        // Handle array response - get first element
        var element = _jsonResponse!.Value.ValueKind == JsonValueKind.Array 
            ? _jsonResponse.Value[0] 
            : _jsonResponse.Value;

        Assert.That(element.TryGetProperty("source", out var source), Is.True);
        Assert.That(source.GetString(), Is.EqualTo(expectedSource));
    }

    [AfterScenario]
    public void Dispose()
    {
        _httpClient?.Dispose();
        _response?.Dispose();
    }
}
