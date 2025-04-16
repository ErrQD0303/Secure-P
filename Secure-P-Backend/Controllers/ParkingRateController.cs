using SecureP.Identity.Models.Dto;
using SecureP.Identity.Models.Dto.SortModels;
using SecureP.Service.ParkingRateService;
using SecureP.Service.ParkingRateService.Mappers;

namespace Secure_P_Backend.Controllers;

[ApiController]
[Route(AppConstants.AppController.ParkingRateController.DefaultRoute)]
[Authorize]
public class ParkingRateController : ControllerBase
{
    public ILogger<ParkingRateController> Logger { get; private set; }
    public ParkingRateService<string> ParkingRateService { get; private set; }

    public ParkingRateController(ILogger<ParkingRateController> logger, ParkingRateService<string> parkingRateService)
    {
        Logger = logger;
        ParkingRateService = parkingRateService;
    }

    [HttpPost]
    [Authorize(Policy = AppPolicy.CreateParkingRate)]
    public async Task<IActionResult> CreateParkingRate([FromBody] CreateParkingRateRequest request)
    {
        // Logic to create a parking location
        Logger.LogInformation("Creating new parking rate");

        if (request == null)
        {
            return BadRequest(new CreateParkingRateResponse
            {
                StatusCode = StatusCodes.Status400BadRequest,
                Message = AppResponses.CreateParkingRateResponses.ParkingRateBodyNotFound,
                Success = false,
                Errors = AppResponseErrors.CreateParkingRateErrors.CreateParkingRateFailed
            });
        }

        var (validationResult, newParkingRate) = await ParkingRateService.CreateParkingRateAsync(request.ToCreateParkingRateDto<string>());

        if (!validationResult.Success)
        {
            return BadRequest(new CreateParkingRateResponse
            {
                StatusCode = StatusCodes.Status400BadRequest,
                Message = validationResult.Message + " " + AppResponses.CreateParkingRateResponses.ParkingRateNotCreated,
                Success = false,
                Errors = validationResult?.Errors ?? []
            });
        }

        return CreatedAtAction(nameof(GetParkingRateById), new { id = newParkingRate?.Id }, new CreateParkingRateResponse
        {
            StatusCode = StatusCodes.Status201Created,
            Message = AppResponses.CreateParkingRateResponses.ParkingRateCreated,
            Success = true,
        });
    }

    [HttpGet("{id}")]
    [Authorize(Policy = AppPolicy.ReadParkingRate)]
    public async Task<IActionResult> GetParkingRateById(string id)
    {
        // Logic to get a parking location by ID
        Logger.LogInformation($"Getting parking location with ID: {id}");

        var parkingRate = await ParkingRateService.GetParkingRateByIdAsync(id);

        if (parkingRate == null)
        {
            return NotFound(new GetParkingRateByIdResponse<string>
            {
                StatusCode = StatusCodes.Status404NotFound,
                Message = AppResponses.GetParkingRateResponses.ParkingRateNotFound,
                Success = false
            });
        }

        return Ok(new GetParkingRateByIdResponse<string>
        {
            StatusCode = StatusCodes.Status200OK,
            Message = AppResponses.GetParkingRateResponses.ParkingRateFound,
            Success = true,
            Data = parkingRate
        });
    }

    [HttpGet]
    [Authorize(Policy = AppPolicy.ReadParkingRate)]
    public async Task<IActionResult> GetAllParkingRates([FromQuery] int page = 1, [FromQuery] int limit = -1, [FromQuery] ParkingRateOrderBy sort = ParkingRateOrderBy.HourlyRate, [FromQuery] bool desc = false, string? search = null)
    {
        Logger.LogInformation($"Getting all parking rates with page: {page}, limit: {limit}, sort: {sort}, desc: {desc}, search: {search}");
        var returnDto = await ParkingRateService.GetParkingRatesAsync(page, limit, sort, desc);

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

    [HttpPut("{id}")]
    [Authorize(Policy = AppPolicy.UpdateParkingRate)]
    public async Task<IActionResult> UpdateParkingRate(string id, [FromBody] UpdateParkingRateRequest<string> request)
    {
        // Logic to update a parking location
        Logger.LogInformation($"Updating parking rate with ID: {id}");

        if (request == null)
        {
            return BadRequest(new UpdateParkingRateResponse
            {
                StatusCode = StatusCodes.Status400BadRequest,
                Message = AppResponses.UpdateParkingRateResponses.ParkingRateBodyNotFound,
                Success = false,
                Errors = AppResponseErrors.UpdateParkingRateErrors.UpdateParkingRateFailed
            });
        }

        var validationResult = await ParkingRateService.UpdateParkingRateAsync(request.ToUpdateParkingRateDto());

        if (!validationResult.Success)
        {
            return BadRequest(new UpdateParkingRateResponse
            {
                StatusCode = StatusCodes.Status400BadRequest,
                Message = validationResult.Message + " " + AppResponses.UpdateParkingRateResponses.ModelValidationFailed,
                Success = false,
                Errors = validationResult?.Errors ?? []
            });
        }

        return Ok(new UpdateParkingRateResponse
        {
            StatusCode = StatusCodes.Status200OK,
            Message = AppResponses.UpdateParkingRateResponses.ParkingRateUpdated,
            Success = true,
        });
    }

    [HttpDelete("{id}")]
    [Authorize(Policy = AppPolicy.DeleteParkingRate)]
    public async Task<IActionResult> DeleteParkingRate(string id)
    {
        // Logic to delete a parking location
        Logger.LogInformation($"Deleting parking rate with ID: {id}");

        if (!await ParkingRateService.DeleteParkingRateAsync(id))
        {
            return NotFound(new DeleteParkingRateResponse
            {
                StatusCode = StatusCodes.Status404NotFound,
                Message = AppResponses.DeleteParkingRateResponses.ParkingRateNotDeleted,
                Success = false
            });
        }

        return Ok(
            new DeleteParkingRateResponse
            {
                StatusCode = StatusCodes.Status200OK,
                Message = AppResponses.DeleteParkingRateResponses.ParkingRateDeleted,
                Success = true
            }
        );
    }
}