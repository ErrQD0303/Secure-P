using Microsoft.AspNetCore.Mvc;
using SecureP.Service.Abstraction.Entities;
using SecureP.Shared;

namespace Secure_P_Backend.Controllers;

[ApiController]
[Route(AppConstants.AppController.ParkingLocationController.DefaultRoute)]
public class ParkingLocationController : ControllerBase
{
    private readonly ILogger<ParkingLocationController> _logger;

    public ParkingLocationController(ILogger<ParkingLocationController> logger)
    {
        _logger = logger;
    }

    [HttpPost]
    public IActionResult CreateParkingLocation([FromBody] AddNewParkingLocationRequest parkingLocationDto)
    {
        // Logic to create a parking location
        _logger.LogInformation("Creating parking location: {ParkingLocation}", parkingLocationDto);

        if (parkingLocationDto == null)
        {
            return BadRequest(new AddNewParkingLocationResponse
            {
                StatusCode = 400,
                Message = "Invalid parking location data.",
                Success = false,
                Errors = new Dictionary<string, string> { { "Error", "Parking location data is required." } }
            });
        }

        return Ok(new { Message = "Parking location created successfully." });
    }
}