Feature: Shipment API Integration Tests
  As a client of the Shipment Mock API
  I want to test all API endpoints
  So that I can ensure the API works correctly
  
  Background: 
  Given the API is running at base URL

  Scenario: Get shipment by valid ID returns correct schema
    When I request shipment with ID "SHP-20250210-0042"
    Then the response status code should be 200
    And the response should match the ShipmentContract schema
    And the shipment field "shipment_id" should be "SHP-20250210-0042"
    And the shipment field "status" should be "in_transit"
    And the shipment field "carrier" should be "MAEU"
    And the shipment field "container_id" should be "MSKU1234567"

  Scenario: Get shipment with invalid ID returns 404
    When I request shipment with ID "INVALID-ID"
    Then the response status code should be 404

  Scenario Outline: Get carrier CDC events returns results
    When I request the url "<Url>"
    Then the response status code should be 200
    And the carrier event source should be "<Source>"
    And the carrier event shipment_id should be "<ShipmentId>"
    And the carrier event status should be "<Status>"
    And the carrier event timestamp should be "<Timestamp>"
    And the carrier event port should be "<Port>"

    Examples:
    | Url                           | Source      | ShipmentId        | Status     | Timestamp                | Port  |
    | /api/shipments/events/carrier | carrier_cdc | SHP-20250210-0042 | in_transit | 2025-02-08T14:30:00+00:00 | CNSHA |

  Scenario: Get edge sensor events returns results
    When I request the url "/api/shipments/events/edge"
    Then the response status code should be 200
    And the response should contain edge sensor event data
    And the edge event should have a device_id
    And the edge event should have a container_id
    And the edge event should have temperature data
    And the edge event source should be "edge_sensor"

  Scenario: Get email parser events returns results
    When I request the url "/api/shipments/events/email"
    Then the response status code should be 200
    And the response should contain email parser event data
    And the email event should have a shipment_ref
    And the email event source should be "email_parser"
