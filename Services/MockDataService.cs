namespace ShipmentMockApi.Services;

using ShipmentMockApi.Models;

public class MockDataService
{
    private readonly List<ShipmentContract> _shipments = new();
    private readonly List<CarrierCdcEvent> _carrierEvents = new();
    private readonly List<EdgeSensorEvent> _edgeEvents = new();
    private readonly List<EmailParserEvent> _emailEvents = new();

    public MockDataService()
    {
        InitializeMockData();
    }  

    private void InitializeMockData()
    {
        _carrierEvents.Add(new CarrierCdcEvent
        {
            Source = "carrier_cdc",
            ShipmentId = "SHP-20250210-0042",
            Status = "in_transit",
            Timestamp = DateTime.Parse("2025-02-08T14:30:00Z"),
            Port = "CNSHA"
        });

        _edgeEvents.Add(new EdgeSensorEvent
        {
            Source = "edge_sensor",
            DeviceId = "EDGE-4491",
            ContainerId = "MSKU1234567",
            Timestamp = DateTime.Parse("2025-02-10T09:15:00Z"),
            Lat = 30.5,
            Lon = 122.1,
            TempC = 4.2
        });

        _emailEvents.Add(new EmailParserEvent
        {
            Source = "email_parser",
            ShipmentRef = "SHP-20250210-0042",
            Status = "delayed",
            Timestamp = null,
            Notes = "Vessel delayed due to weather"
        });

        _shipments.Add(new ShipmentContract
        {
            ShipmentId = "SHP-20250210-0042",
            Status = "in_transit",
            Origin = new Location { Port = "CNSHA", Country = "CN" },
            Destination = new Location { Port = "GBFXT", Country = "GB" },
            Carrier = "MAEU",
            ContainerId = "MSKU1234567",
            Events = new List<ShipmentEvent>
            {
                new ShipmentEvent
                {
                    Timestamp = DateTime.Parse("2025-02-08T14:30:00Z"),
                    Type = "departure",
                    Source = "carrier_api"
                },
                new ShipmentEvent
                {
                    Timestamp = DateTime.Parse("2025-02-10T09:15:00Z"),
                    Type = "location_update",
                    Source = "edge_sensor",
                    Location = new GeoLocation { Lat = 30.5, Lon = 122.1 }
                }
            },
            Eta = DateTime.Parse("2025-03-15T08:00:00Z"),
            LastUpdated = DateTime.Parse("2025-02-10T09:15:00Z")
        });
    }

    public ShipmentContract? GetShipmentById(string shipmentId)
    {
        return _shipments.FirstOrDefault(s => s.ShipmentId == shipmentId);
    }

    public List<CarrierCdcEvent> GetCarrierEvents() => _carrierEvents;
    public List<EdgeSensorEvent> GetEdgeEvents() => _edgeEvents;
    public List<EmailParserEvent> GetEmailEvents() => _emailEvents;
}
