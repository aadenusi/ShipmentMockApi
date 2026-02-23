using System.Text.Json.Serialization;

namespace ShipmentMockApi.Models;

public class ShipmentContract
{
    [JsonPropertyName("shipment_id")]
    public string ShipmentId { get; set; } = string.Empty;

    [JsonPropertyName("status")]
    public string Status { get; set; } = string.Empty;

    [JsonPropertyName("origin")]
    public Location Origin { get; set; } = new();

    [JsonPropertyName("destination")]
    public Location Destination { get; set; } = new();

    [JsonPropertyName("carrier")]
    public string Carrier { get; set; } = string.Empty;

    [JsonPropertyName("container_id")]
    public string ContainerId { get; set; } = string.Empty;

    [JsonPropertyName("events")]
    public List<ShipmentEvent> Events { get; set; } = new();

    [JsonPropertyName("eta")]
    public DateTime Eta { get; set; }

    [JsonPropertyName("last_updated")]
    public DateTime LastUpdated { get; set; }
}

public class Location
{
    [JsonPropertyName("port")]
    public string Port { get; set; } = string.Empty;

    [JsonPropertyName("country")]
    public string Country { get; set; } = string.Empty;
}

public class ShipmentEvent
{
    [JsonPropertyName("timestamp")]
    public DateTime Timestamp { get; set; }

    [JsonPropertyName("type")]
    public string Type { get; set; } = string.Empty;

    [JsonPropertyName("source")]
    public string Source { get; set; } = string.Empty;

    [JsonPropertyName("location")]
    public GeoLocation? Location { get; set; }
}

public class GeoLocation
{
    [JsonPropertyName("lat")]
    public double Lat { get; set; }

    [JsonPropertyName("lon")]
    public double Lon { get; set; }
}
