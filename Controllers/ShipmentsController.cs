namespace ShipmentMockApi.Controllers;

using Microsoft.AspNetCore.Mvc;
using ShipmentMockApi.Models;
using ShipmentMockApi.Services;

[ApiController]
[Route("api/shipments")]
public class ShipmentsController : ControllerBase
{
    private readonly MockDataService _mockDataService;

    public ShipmentsController(MockDataService mockDataService)
    {
        _mockDataService = mockDataService;
    }

    [HttpGet("{shipmentId}")]
    public ActionResult<ShipmentContract> GetShipment(string shipmentId)
    {
        var shipment = _mockDataService.GetShipmentById(shipmentId);
        if (shipment == null)
        {
            return NotFound(new { message = $"Shipment {shipmentId} not found" });
        }
        return Ok(shipment);
    }

    [HttpGet("events/carrier")]
    public ActionResult<List<CarrierCdcEvent>> GetCarrierEvents()
    {
        return Ok(_mockDataService.GetCarrierEvents());
    }

    [HttpGet("events/edge")]
    public ActionResult<List<EdgeSensorEvent>> GetEdgeEvents()
    {
        return Ok(_mockDataService.GetEdgeEvents());
    }

    [HttpGet("events/email")]
    public ActionResult<List<EmailParserEvent>> GetEmailEvents()
    {
        return Ok(_mockDataService.GetEmailEvents());
    }
}
