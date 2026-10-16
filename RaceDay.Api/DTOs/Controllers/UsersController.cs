using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.Api.Data;
using RaceDay.Api.DTOs;
using RaceDay.Api.Security;
namespace RaceDay.Api.Controllers;
[ApiController, Route("api/users")]
public class UsersController(RaceDayContext db) : ControllerBase
{
    [HttpGet("me"), SessionAuthorize]
    /// <summary>Returns the authenticated user’s own profile.</summary>
    public async Task<IActionResult> Me()
    {
        var id = HttpContext.Session.GetInt32("UserId")!.Value;
        var user = await db.Users.Where(u => u.Id == id).Select(u => new { u.Id, u.FirstName, u.LastName, u.Email, u.Role, u.Phone, u.CreatedAtUtc }).SingleOrDefaultAsync();
        return user is null ? NotFound() : Ok(user);
    }
    [HttpPut("me"), SessionAuthorize]
    /// <summary>Updates the authenticated user’s own profile fields.</summary>
    public async Task<IActionResult> UpdateMe(UpdateProfileRequest request)
    {
        var id = HttpContext.Session.GetInt32("UserId")!.Value;
        var user = await db.Users.FindAsync(id); if (user is null) return NotFound();
        user.FirstName = request.FirstName.Trim(); user.LastName = request.LastName.Trim(); user.Phone = request.Phone?.Trim();
        await db.SaveChangesAsync(); return Ok(new { user.Id, user.FirstName, user.LastName, user.Email, user.Role, user.Phone });
    }
    [HttpGet("{id:int}"), SessionAuthorize("Organiser")]
    /// <summary>Returns a user profile; Organiser access only.</summary>
    public async Task<IActionResult> GetUser(int id)
    {
        var user = await db.Users.Where(u => u.Id == id).Select(u => new { u.Id, u.FirstName, u.LastName, u.Email, u.Role, u.Phone }).SingleOrDefaultAsync();
        return user is null ? NotFound() : Ok(user);
    }
}
