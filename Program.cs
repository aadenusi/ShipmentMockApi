using ShipmentMockApi.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<MockDataService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet("/", () => Results.Ok(new
{
    message = "Shipment Mock API",
    endpoints = new[]
    {
        "GET /api/shipments/{shipment_id} - Get shipment details",
        "GET /api/shipments/events/carrier - Get carrier CDC events",
        "GET /api/shipments/events/edge - Get edge sensor events",
        "GET /api/shipments/events/email - Get email parser events"
    },
    example = "/api/shipments/SHP-20250210-0042"
}));

app.MapControllers();

app.Run();
