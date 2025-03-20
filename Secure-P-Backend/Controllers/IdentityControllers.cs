using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SecureP.Identity.Models;
using SecureP.Service.Abstraction.Entities;

namespace Secure_P_Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
// [Authorize]
public class IdentityControllers : ControllerBase
{
    private readonly ILogger<IdentityControllers> _logger;
    private readonly UserManager<AppUser<string>> _userManager;

    public IdentityControllers(ILogger<IdentityControllers> logger, UserManager<AppUser<string>> userManager)
    {
        _logger = logger;
        _userManager = userManager;
    }

    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest registerRequest)
    {
        // Register user
        var user = new AppUser<string>
        {
            Id = Guid.NewGuid().ToString(),
            UserName = registerRequest.Username ?? registerRequest.Email,
            Email = registerRequest.Email,
            EmailConfirmed = false,
            SecurityStamp = Guid.NewGuid().ToString(),
            TwoFactorEnabled = false,
            LockoutEnabled = _userManager.Options.Lockout.AllowedForNewUsers,
            PhoneNumber = registerRequest.PhoneNumber,
        };

        var result = await _userManager.CreateAsync(user, registerRequest.Password);

        if (!result.Succeeded)
        {
            return BadRequest(result.Errors);
        }

        return CreatedAtAction(nameof(GetUser), new { id = user.Id }, user);
    }

    [HttpGet("get-user/{id}")]
    public async Task<IActionResult> GetUser([FromRoute] string id)
    {
        var user = await _userManager.FindByIdAsync(id);

        if (user == null)
        {
            return NotFound();
        }

        return Ok(user);
    }

    [HttpGet("get-users")]
    public async Task<IActionResult> GetUsers()
    {
        var users = await _userManager.Users.ToListAsync();

        return Ok(users);
    }
}