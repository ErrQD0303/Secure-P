using SecureP.Identity.Models.Dto.SortModels;
using SecureP.Service.ParkingZoneService.Mappers;

namespace Secure_P_Backend.Controllers;

[ApiController]
[Route(AppConstants.AppController.ParkingZoneController.DefaultRoute)]
[Authorize]
public class ParkingZoneController : ControllerBase
{
    public ILogger<ParkingZoneController> Logger { get; private set; }
    public IParkingZoneService<string> ParkingZoneService { get; private set; }

    public ParkingZoneController(ILogger<ParkingZoneController> logger, IParkingZoneService<string> parkingRateService)
    {
        Logger = logger;
        ParkingZoneService = parkingRateService;
    }

    [HttpPost]
    [Authorize(Policy = AppPolicy.CreateParkingZone)]
    public async Task<IActionResult> CreateParkingZone([FromBody] CreateParkingZoneRequest<string> request)
    {
        // Logic to create a parking location
        Logger.LogInformation("Creating new parking rate");

        if (request == null)
        {
            return BadRequest(new CreateParkingZoneResponse
            {
                StatusCode = StatusCodes.Status400BadRequest,
                Message = AppResponses.CreateParkingZoneResponses.ParkingZoneBodyNotFound,
                Success = false,
                Errors = AppResponseErrors.CreateParkingZoneErrors.CreateParkingZoneFailed
            });
        }

        var (validationResult, newParkingZone) = await ParkingZoneService.CreateParkingZoneAsync(request.ToCreateParkingZoneDto());

        if (!validationResult.Success)
        {
            return BadRequest(new CreateParkingZoneResponse
            {
                StatusCode = StatusCodes.Status400BadRequest,
                Message = validationResult.Message + " " + AppResponses.CreateParkingZoneResponses.ParkingZoneNotCreated,
                Success = false,
                Errors = validationResult?.Errors ?? []
            });
        }

        return CreatedAtAction(nameof(GetParkingZoneById), new { id = newParkingZone?.Id }, new CreateParkingZoneResponse
        {
            StatusCode = StatusCodes.Status201Created,
            Message = AppResponses.CreateParkingZoneResponses.ParkingZoneCreated,
            Success = true,
        });
    }

    [HttpGet("{id}")]
    [Authorize(Policy = AppPolicy.ReadParkingZone)]
    public async Task<IActionResult> GetParkingZoneById(string id)
    {
        // Logic to get a parking zone by ID
        Logger.LogInformation($"Getting parking zone with ID: {id}");

        var parkingZone = await ParkingZoneService.GetParkingZoneByIdAsync(id);

        if (parkingZone == null)
        {
            return NotFound(new GetParkingZoneByIdResponse<string>
            {
                StatusCode = StatusCodes.Status404NotFound,
                Message = AppResponses.GetParkingZoneResponses.ParkingZoneNotFound,
                Success = false
            });
        }

        return Ok(new GetParkingZoneByIdResponse<string>
        {
            StatusCode = StatusCodes.Status200OK,
            Message = AppResponses.GetParkingZoneResponses.ParkingZoneFound,
            Success = true,
            Data = parkingZone
        });
    }

    [HttpGet]
    [Authorize(Policy = AppPolicy.ReadParkingZone)]
    public async Task<IActionResult> GetAllParkingZones([FromQuery] int page = 1, [FromQuery] int limit = -1, [FromQuery] ParkingZoneOrderBy sort = ParkingZoneOrderBy.Name, [FromQuery] bool desc = false, string? search = null)
    {
        Logger.LogInformation($"Getting all parking zones with page: {page}, limit: {limit}, sort: {sort}, desc: {desc}, search: {search}");
        var returnDto = await ParkingZoneService.GetParkingZonesAsync(page, limit, sort, desc);

        if (returnDto == null)
        {
            return NotFound(new GetAllParkingZoneResponse<string>
            {
                StatusCode = StatusCodes.Status404NotFound,
                Message = AppResponses.GetAllParkingZoneResponses.NoParkingZonesFound,
                Success = false
            });
        }

        return Ok(new GetAllParkingZoneResponse<string>
        {
            StatusCode = StatusCodes.Status200OK,
            Message = AppResponses.GetAllParkingZoneResponses.ParkingZonesFound,
            Success = true,
            Data = returnDto
        });
    }

    [HttpPut("{id}")]
    [Authorize(Policy = AppPolicy.UpdateParkingZone)]
    public async Task<IActionResult> UpdateParkingZone(string id, [FromBody] UpdateParkingZoneRequest<string> request)
    {
        // Logic to update a parking zone
        Logger.LogInformation($"Updating parking zone with ID: {id}");

        if (request == null)
        {
            return BadRequest(new UpdateParkingZoneResponse
            {
                StatusCode = StatusCodes.Status400BadRequest,
                Message = AppResponses.UpdateParkingZoneResponses.ParkingZoneBodyNotFound,
                Success = false,
                Errors = AppResponseErrors.UpdateParkingZoneErrors.UpdateParkingZoneFailed
            });
        }

        var validationResult = await ParkingZoneService.UpdateParkingZoneAsync(request.ToUpdateParkingZoneDto());

        if (!validationResult.Success)
        {
            return BadRequest(new UpdateParkingZoneResponse
            {
                StatusCode = StatusCodes.Status400BadRequest,
                Message = validationResult.Message + " " + AppResponses.UpdateParkingZoneResponses.ModelValidationFailed,
                Success = false,
                Errors = validationResult?.Errors ?? []
            });
        }

        return Ok(new UpdateParkingZoneResponse
        {
            StatusCode = StatusCodes.Status200OK,
            Message = AppResponses.UpdateParkingZoneResponses.ParkingZoneUpdated,
            Success = true,
        });
    }

    [HttpDelete("{id}")]
    [Authorize(Policy = AppPolicy.DeleteParkingZone)]
    public async Task<IActionResult> DeleteParkingZone(string id)
    {
        // Logic to delete a parking zone
        Logger.LogInformation($"Deleting parking zone with ID: {id}");

        if (!await ParkingZoneService.DeleteParkingZoneAsync(id))
        {
            return NotFound(new DeleteParkingZoneResponse
            {
                StatusCode = StatusCodes.Status404NotFound,
                Message = AppResponses.DeleteParkingZoneResponses.ParkingZoneNotDeleted,
                Success = false
            });
        }

        return Ok(
            new DeleteParkingZoneResponse
            {
                StatusCode = StatusCodes.Status204NoContent,
                Message = AppResponses.DeleteParkingZoneResponses.ParkingZoneDeleted,
                Success = true
            }
        );
    }
}