using SecureP.Identity.Models.Dto;
using SecureP.Identity.Models.Dto.SortModels;

namespace Secure_P_Backend.Controllers;

[ApiController]
[Route(AppConstants.AppController.ParkingRateController.DefaultRoute)]
[Authorize]
public class ParkingRateController : ControllerBase
{
    private readonly ILogger<ParkingRateController> _logger;
    private readonly AppDbContext<string> _context;

    public ParkingRateController(ILogger<ParkingRateController> logger, AppDbContext<string> context)
    {
        _logger = logger;
        _context = context;
    }

    /* [HttpPost]
    [Authorize(Policy = AppPolicy.CreateParkingLocation)]
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
    [Authorize(Policy = AppPolicy.ReadParkingLocation)]
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
    } */

    [HttpGet]
    [Authorize(Policy = AppPolicy.ReadParkingLocation)]
    public async Task<IActionResult> GetAllParkingLocations([FromQuery] int page = 1, [FromQuery] int limit = -1, [FromQuery] ParkingRateOrderBy sort = ParkingRateOrderBy.HourlyRate, [FromQuery] bool desc = false)
    {
        // Logic to get all parking locations
        _logger.LogInformation("Getting all parking rates from the database");
        var parkingLocationsQuery = _context.ParkingRates.AsNoTracking().AsQueryable();
        if (limit > 0)
        {
            parkingLocationsQuery = parkingLocationsQuery.Skip((page - 1) * limit).Take(limit);
        }
        parkingLocationsQuery = sort switch
        {
            ParkingRateOrderBy.HourlyRate => desc ? parkingLocationsQuery.OrderByDescending(x => x.HourlyRate) : parkingLocationsQuery.OrderBy(x => x.HourlyRate),
            ParkingRateOrderBy.DailyRate => desc ? parkingLocationsQuery.OrderByDescending(x => x.DailyRate) : parkingLocationsQuery.OrderBy(x => x.DailyRate),
            ParkingRateOrderBy.MonthlyRate => desc ? parkingLocationsQuery.OrderByDescending(x => x.MonthlyRate) : parkingLocationsQuery.OrderBy(x => x.MonthlyRate),
            _ => throw new InvalidOperationException("Invalid sort option"),
        };

        var parkingLocations = await parkingLocationsQuery.ToListAsync();
        var totalItems = await _context.ParkingRates.CountAsync();

        var returnDto = new GetAllParkingRateDto<string>
        {
            Items = [.. parkingLocations.Select(x => new GetParkingRateDto<string>
            {
                Id = x.Id,
                HourlyRate = x.HourlyRate,
                DailyRate = x.DailyRate,
                MonthlyRate = x.MonthlyRate,
            })],
            TotalItems = totalItems,
            TotalPages = (int)Math.Ceiling((double)totalItems / (limit > 0 ? limit : totalItems)),
        };

        if (returnDto == null || returnDto.Items.Count == 0)
        {
            return NotFound(new GetAllParkingRateResponse<string>
            {
                StatusCode = StatusCodes.Status404NotFound,
                Message = AppResponses.GetAllParkingRateResponses.NoParkingRatesFound,
                Success = false
            });
        }

        return Ok(new GetAllParkingRateResponse<string>
        {
            StatusCode = StatusCodes.Status200OK,
            Message = AppResponses.GetAllParkingRateResponses.ParkingRatesFound,
            Success = true,
            Data = returnDto
        });
    }

    /* [HttpPut("{id}")]
    [Authorize(Policy = AppPolicy.UpdateParkingLocation)]
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
    [Authorize(Policy = AppPolicy.DeleteParkingLocation)]
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
    } */
}