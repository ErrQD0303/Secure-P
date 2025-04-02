namespace Secure_P_Backend.Controllers;

[ApiController]
[Route(AppConstants.AppController.UploadController.DefaultRoute)]
[Authorize]
public class UploadController : ControllerBase
{
    private readonly IUploadService<string> _uploadService;

    public UploadController(IUploadService<string> uploadService)
    {
        _uploadService = uploadService;
    }

    [HttpPost(AppConstants.AppController.UploadController.UploadAvatar)]
    public async Task<IActionResult> UploadAvatar([FromForm] UploadAvatarRequest request)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userId == null)
        {
            return Unauthorized(
                new UploadAvatarResponse<string>
                {
                    StatusCode = 401,
                    Success = "false",
                    Message = "Unauthorized",
                    Errors = new Dictionary<string, string>
                    {
                        { "summary", "Unauthorized" }
                    }
                }
            );
        }
        var avatarUrl = await _uploadService.UploadAvatarAsync(request.Avatar, userId);

        if (string.IsNullOrEmpty(avatarUrl))
        {
            return BadRequest(
                new UploadAvatarResponse<string>
                {
                    StatusCode = 500,
                    Success = "false",
                    Message = "Upload failed",
                    Errors = new Dictionary<string, string>
                    {
                        { "summary", "Upload Failed" }
                    }
                }
            );
        }

        return Ok(
            new UploadAvatarResponse<string>
            {
                StatusCode = StatusCodes.Status200OK,
                Success = "true",
                Message = "Upload successful",
                AvatarUrl = avatarUrl
            }
        );
    }
}