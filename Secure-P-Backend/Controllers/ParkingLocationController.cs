

using SecureP.Identity.Models.Dto.SortModels;

namespace Secure_P_Backend.Controllers;

[ApiController]
[Route(AppConstants.AppController.ParkingLocationController.DefaultRoute)]
public class ParkingLocationController : ControllerBase
{
    private readonly ILogger<ParkingLocationController> _logger;
    private readonly IParkingLocationService<string> _parkingLocationService;

    public ParkingLocationController(ILogger<ParkingLocationController> logger, IParkingLocationService<string> parkingLocationService)
    {
        _parkingLocationService = parkingLocationService;
        _logger = logger;
    }

    [HttpPost]
    public async Task<IActionResult> CreateParkingLocation([FromBody] CreateParkingLocationRequest request)
    {
        // Logic to create a parking location
        _logger.LogInformation("Creating new parking location");

        if (request == null)
        {
            return BadRequest(new CreateParkingLocationResponse
            {
                StatusCode = StatusCodes.Status400BadRequest,
                Message = AppResponses.CreateParkingLocationResponses.ParkingLocationNotFound,
                Success = false,
                Errors = AppResponseErrors.CreateParkingLocationErrors.CreateParkingLocationFailed
            });
        }

        var (validationResult, newParkingLocation) = await _parkingLocationService.CreateParkingLocationAsync(request);

        if (!validationResult.Success)
        {
            return BadRequest(new CreateParkingLocationResponse
            {
                StatusCode = StatusCodes.Status400BadRequest,
                Message = validationResult.Message + " " + AppResponses.CreateParkingLocationResponses.ModelValidationFailed,
                Success = false,
                Errors = validationResult?.Errors ?? []
            });
        }

        return CreatedAtAction(nameof(GetParkingLocationById), new { id = newParkingLocation?.Id }, new CreateParkingLocationResponse
        {
            StatusCode = StatusCodes.Status201Created,
            Message = AppResponses.CreateParkingLocationResponses.ParkingLocationCreated,
            Success = true,
        });
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetParkingLocationById(string id)
    {
        // Logic to get a parking location by ID
        _logger.LogInformation($"Getting parking location with ID: {id}");

        var parkingLocation = await _parkingLocationService.GetParkingLocationByIdAsync(id);

        if (parkingLocation == null)
        {
            return NotFound(new GetParkingLocationByIdResponse<string>
            {
                StatusCode = StatusCodes.Status404NotFound,
                Message = AppResponses.GetParkingLocationResponses.ParkingLocationNotFound,
                Success = false
            });
        }

        return Ok(new GetParkingLocationByIdResponse<string>
        {
            StatusCode = StatusCodes.Status200OK,
            Message = AppResponses.GetParkingLocationResponses.ParkingLocationFound,
            Success = true,
            Data = parkingLocation
        });
    }

    [HttpGet]
    public async Task<IActionResult> GetAllParkingLocations([FromQuery] int pageIndex = 1, [FromQuery] int pageSize = 10, [FromQuery] ParkingLocationOrderBy orderBy = ParkingLocationOrderBy.Name, [FromQuery] bool desc = false)
    {
        // Logic to get all parking locations
        _logger.LogInformation("Getting all parking locations from the database");

        var parkingLocations = await _parkingLocationService.GetParkingLocationsAsync(pageIndex, pageSize, orderBy, desc);

        if (parkingLocations == null || !parkingLocations.Any())
        {
            return NotFound(new GetAllParkingLocationResponse<string>
            {
                StatusCode = StatusCodes.Status404NotFound,
                Message = AppResponses.GetAllParkingLocationResponses.NoParkingLocationsFound,
                Success = false
            });
        }

        return Ok(new GetAllParkingLocationResponse<string>
        {
            StatusCode = StatusCodes.Status200OK,
            Message = AppResponses.GetAllParkingLocationResponses.ParkingLocationsFound,
            Success = true,
            Data = parkingLocations
        });
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateParkingLocation(string id, [FromBody] UpdateParkingLocationRequest request)
    {
        // Logic to update a parking location
        _logger.LogInformation($"Updating parking location with ID: {id}");

        if (request == null)
        {
            return BadRequest(new UpdateParkingLocationResponse
            {
                StatusCode = StatusCodes.Status400BadRequest,
                Message = AppResponses.UpdateParkingLocationResponses.ParkingLocationBodyNotFound,
                Success = false,
                Errors = AppResponseErrors.UpdateParkingLocationErrors.UpdateParkingLocationFailed
            });
        }

        var validationResult = await _parkingLocationService.UpdateParkingLocationAsync(id, request);

        if (!validationResult.Success)
        {
            return BadRequest(new UpdateParkingLocationResponse
            {
                StatusCode = StatusCodes.Status400BadRequest,
                Message = validationResult.Message + " " + AppResponses.UpdateParkingLocationResponses.ModelValidationFailed,
                Success = false,
                Errors = validationResult?.Errors ?? []
            });
        }

        return Ok(new UpdateParkingLocationResponse
        {
            StatusCode = StatusCodes.Status200OK,
            Message = AppResponses.UpdateParkingLocationResponses.ParkingLocationUpdated,
            Success = true,
        });
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteParkingLocation(string id)
    {
        // Logic to delete a parking location
        _logger.LogInformation($"Deleting parking location with ID: {id}");

        if (!await _parkingLocationService.DeleteParkingLocationAsync(id))
        {
            return NotFound(new DeleteParkingLocationResponse
            {
                StatusCode = StatusCodes.Status404NotFound,
                Message = AppResponses.DeleteParkingLocationResponses.ParkingLocationNotDeleted,
                Success = false
            });
        }

        return Ok(
            new DeleteParkingLocationResponse
            {
                StatusCode = StatusCodes.Status200OK,
                Message = AppResponses.DeleteParkingLocationResponses.ParkingLocationDeleted,
                Success = true
            }
        );
    }
}