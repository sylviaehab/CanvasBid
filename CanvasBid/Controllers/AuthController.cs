namespace CanvasBid.Controllers
{
    using CanvasBid.DTOS.UserDTOS;
    using CanvasBid.Services;
    using Microsoft.AspNetCore.Mvc;

    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthServices _authservice;

        public AuthController(IAuthServices authServices)
        {
            _authservice = authServices;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterUserDto registerUserDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var (success, message, user) = await _authservice.RegisterAsync(registerUserDto);

            if (!success)
                return BadRequest(new { message });

            return CreatedAtAction(nameof(Register), new { userId = user?.Id }, user);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginUserDto loginUserDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var (success, message, token, user) = await _authservice.LoginAsync(loginUserDto);

            if (!success)
                return BadRequest(new { message });

            return Ok(new { token, user });
        }
    }
}


