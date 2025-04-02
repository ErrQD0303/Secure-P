namespace Secure_P_Backend.Controllers;

[ApiController]
[Route(AppConstants.AppController.GDPRController.DefaultRoute)]
public class GDPRController : ControllerBase
{
    [HttpGet]
    public IActionResult AcceptGeneralDataProtectionRegulation([FromServices] IOptions<CookiePolicyOptions> cookiePolicyOptions)
    {
        var consentFeature = HttpContext.Features.Get<ITrackingConsentFeature>();
        if (consentFeature?.CanTrack == true)
        {
            return Ok("Consent already granted");
        }

        consentFeature?.GrantConsent();

        return Ok("Consent granted");
    }
}