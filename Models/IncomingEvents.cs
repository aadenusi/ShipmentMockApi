using System.Text.Json.Serialization;

namespace ShipmentMockApi.Models;

public class CarrierCdcEvent
{
    [JsonPropertyName("source")]
    public string Source { get; set; } = "carrier_cdc";

    [JsonPropertyName("shipment_id")]
    public string ShipmentId { get; set; } = string.Empty;

    [JsonPropertyName("status")]
    public string Status { get; set; } = string.Empty;

    [JsonPropertyName("timestamp")]
    public DateTime Timestamp { get; set; }

    [JsonPropertyName("port")]
    public string Port { get; set; } = string.Empty;
}

public class EdgeSensorEvent
{
    [JsonPropertyName("source")]
    public string Source { get; set; } = "edge_sensor";

    [JsonPropertyName("device_id")]
    public string DeviceId { get; set; } = string.Empty;

    [JsonPropertyName("container_id")]
    public string ContainerId { get; set; } = string.Empty;

    [JsonPropertyName("timestamp")]
    public DateTime Timestamp { get; set; }

    [JsonPropertyName("lat")]
    public double Lat { get; set; }

    [JsonPropertyName("lon")]
    public double Lon { get; set; }

    [JsonPropertyName("temperature")]
    public double TempC { get; set; }
}

public class EmailParserEvent
{
    [JsonPropertyName("source")]
    public string Source { get; set; } = "email_parser";

    [JsonPropertyName("shipment_ref")]
    public string ShipmentRef { get; set; } = string.Empty;

    [JsonPropertyName("status")]
    public string Status { get; set; } = string.Empty;

    [JsonPropertyName("timestamp")]
    public DateTime? Timestamp { get; set; }

    [JsonPropertyName("notes")]
    public string Notes { get; set; } = string.Empty;
}
