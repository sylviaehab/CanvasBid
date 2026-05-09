namespace   CanvasBid.Controllers;
using Microsoft.AspNetCore.Mvc;
using CanvasBid.Services;
using CanvasBid.DTOS.UserDTOS;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;
    public UsersController(IUserService userService){
        _userService=userService;
    
    }
    [HttpGet("{id}")]
    public async Task<IActionResult> GetUserById(int id)
    {
        var user=await _userService.GetUserByIdAsync(id);
        if(user==null)
        return NotFound(new { message = "User not found" });
        return Ok(user);
    }
    [HttpGet("artists")]
    public async Task<IActionResult> GetAllArtists()
    {
        var artists= await  _userService.GetAllArtistsAsync();
        return Ok(artists);
    }
    [HttpGet("buyers")]
     public async Task<IActionResult> GetAllBuyers()
    {
        var buyers= await  _userService.GetAllBuyersAsync();
        return Ok(buyers);
    }
    [HttpGet("MyProfile")]
    public async Task<IActionResult> GetMyProfile()
    {
          var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null)
            return Unauthorized(new { message = "User not authenticated" });

        if (!int.TryParse(userIdClaim.Value, out var userId))
            return BadRequest(new { message = "Invalid user ID" });

        var user = await _userService.GetUserByIdAsync(userId);

        if (user == null)
            return NotFound(new { message = "User not found" });

        return Ok(user);
    }
    [HttpPut("{id}/deactivate")]
public async Task<IActionResult> DeactivateUser(int id)
    {
        var userIdClaim=User.FindFirst(ClaimTypes.NameIdentifier);
        if(userIdClaim==null)
        return Unauthorized(new { message = "User not authenticated" });
        var success =await _userService.DeactivateUserAsync(id);
        if(!success)
        return NotFound(new { message = "User not found" });
        return Ok(new {message = "User deactivated successfully" });
    }
}
